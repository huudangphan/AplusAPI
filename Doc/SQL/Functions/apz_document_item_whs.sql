CREATE or replace function aplus_document_item_whs(_obj_type character varying(50), _trans_type character varying(1), _obj_id character varying(254), _cancellation character varying(1))
	returns table (msg_code integer, message character varying(254))
	language 'plpgsql'
as
declare
	_main_currency character varying(3);
	_neg_whs character varying(1);
	_card_code character varying(50);
	_is_ins character varying(1);
BEGIN

	_neg_whs = coalesce((select neg_whs from APZ_CINF), 'N');
	_main_currency = coalesce((SELECT main_currency FROM APZ_CINF),'');

	create local temporary table _tbl_base_types (obj_type character varying(20), id INTEGER IDENTITY (1,1) NOT NULL);
	
	_card_code = coalesce((SELECT card_code FROM _temp_doc),'')

	------------------------------ Lấy dữ liệu base object
	INSERT INTO _tbl_base_types(obj_type)
		SELECT DISTINCT T0.base_type FROM _temp_lines  T0 WHERE isnull(T0.base_type,-1)<>-1;

	_is_ins = coalesce((select is_ins from _temp_doc),'N');

	--------------------------------- Lấy dữ liệu base object ------------------------------------------
	create local temporary table _base_data(obj_type character varying(20), doc_entry int);
	if @countBaseType > 0 and @cancellation = 'N'
	BEGIN

		WHILE(@indicator<@countBaseType)
		BEGIN
			SET @basetype=ISNULL((SELECT ObjType FROM @tblBaseTypes WHERE Id=@indicator+1),'')
			
			IF	@basetype NOT IN ('0','-1','')
			BEGIN
				SET @basetable=(SELECT dbo.apz_document_get_table_doc(@basetype))
				SET @base_tblline=(SELECT dbo.apz_document_get_table_docline(@basetype))

				set @SQL = 'insert into #baseData select ObjType, DocEntry, IsIns from ' + @basetable + ' a where exists(select 1 from #tempLines x where x.baseType = a.ObjType and x.BaseEntry = a.DocEntry)';
				exec(@SQL)
				SET @indicator=@indicator+1
			END
		END
	END

	set @indicator = 0
	set @basetable = ''
	set @base_tblline = ''
	-------------------------------- Kết thúc lấy dữ liệu base object -------------------------------

	if @negWhsSetting = 'N' and (@objtype IN (SELECT ObjType FROM dbo.apz_document_DecOnhandObject() x) AND @transtype='A' AND @cancellation='N' and @isIns = 'Y'
		or @cancellation = 'Y' and @transtype = 'L' and @isIns = 'Y' and @objtype in (SELECT ObjType FROM dbo.apz_document_IncOnhandObject() x))
	BEGIN

		declare @itemCode nvarchar(50) = ''
		declare @whsCode nvarchar(50) = ''
		select top 1 @itemCode = a.ItemCode, @whsCode = a.WhsCode 
			from (select sum(InvQty) Qty, ItemCode, WhsCode from #tempLines x where not exists(select 1 from #baseData y where y.ObjectType = x.BaseType and y.DocEntry = x.BaseEntry and y.IsIns = 'Y') group by ItemCode, WhsCode) a 
							inner join APZ_OITM c ON a.ItemCode = c.ItemCode
							Inner JOin APZ_OITW b On a.ItemCode = b.ItemCode and a.WhsCode = b.WhsCode where c.InvntItem = 'Y' and isnull(b.OnHand, 0) - a.Qty < 0

		if @itemCode <> ''
		BEGIN

			select @objtype + 201, N'Mặt hàng "' + @itemCode + N'" không đủ tồn trong kho. Vui lòng thử lại sau'
			return

		END

	END

	IF(@objtype IN (SELECT ObjType FROM #IsCommitedObject) AND @cancellation='N')
	BEGIN  -- update committed in oitw (Items - warehouse)
		
		--------------------------------------------- giảm số lượng commit của document trong dữ liệu
		SET @SQL='
		UPDATE a
		SET a.IsCommited =
			isnull(a.IsCommited,0) -
			isnull((SELECT SUM(ISNULL(T0.OpenInvQty,0))
						FROM '+@tblline+' T0
						WHERE  T0.ItemCode=a.ItemCode AND T0.LineStatus=''O'' and T0.DocEntry='+@obj_id+'),0)
		FROM APZ_OITM a 
		INNER JOIN '+@tblline+' b ON a.ItemCode=b.ItemCode
		INNER JOIN #IsCommitedObject e on (''' + @objtype +'''=e.ObjType and e.IsIns = ''' + @isIns +''') where a.InvntItem = ''Y'' and b.DocEntry = '+@obj_id + '

		UPDATE a
		SET a.IsCommited =
			isnull(a.IsCommited,0) -
			isnull((SELECT SUM(ISNULL(T0.OpenInvQty,0))
						FROM '+@tblline+' T0
						WHERE T0.ItemCode=a.ItemCode AND T0.LineStatus=''O'' AND T0.WhsCode=a.WhsCode  and T0.DocEntry='+@obj_id+'),0)
		FROM APZ_OITW a 
		INNER JOIN '+@tblline+' b ON a.ItemCode=b.ItemCode AND a.WhsCode=b.WhsCode
		INNER JOIN #IsCommitedObject e on (''' + @objtype +'''=e.ObjType and e.IsIns = ''' + @isIns +''')
		INNER JOIN APZ_OITM x ON x.ItemCode = a.ItemCode where x.InvntItem = ''Y'' and  b.DocEntry='+@obj_id;

		-------------------------------------------------- tăng số lượng commit theo số lượng update document
		SET @SQL= @SQL + '
		UPDATE a
		SET IsCommited=
			isnull(a.IsCommited,0) 
			+ isnull((SELECT SUM(ISNULL(T0.OpenInvQty,0)) 
						FROM #tempLines  T0
						WHERE T0.LineStatus=''O'' AND T0.ItemCode=a.ItemCode AND T0.WhsCode=a.WhsCode),0)
		FROM APZ_OITW a 
		INNER JOIN #tempLines  b ON a.ItemCode=b.ItemCode AND a.WhsCode=b.WhsCode and b.LineStatus=''O''
		INNER JOIN #IsCommitedObject e on (''' + @objtype +'''=e.ObjType and e.IsIns = ''' + @isIns +''')
		INNER JOIN APZ_OITM x on x.ItemCode = a.ItemCode and x.InvntItem = ''Y'' 

		UPDATE a
		SET IsCommited=
			isnull(a.IsCommited,0) 
			+ isnull((SELECT SUM(ISNULL(T0.OpenInvQty,0))
						FROM #tempLines  T0
						WHERE T0.LineStatus=''O'' AND T0.ItemCode=a.ItemCode),0)
		
		FROM APZ_OITM a 
		INNER JOIN #tempLines  b ON a.ItemCode=b.ItemCode and b.LineStatus=''O''
		INNER JOIN #IsCommitedObject e on (''' + @objtype +'''=e.ObjType and e.IsIns = ''' + @isIns +''') where a.InvntItem = ''Y'''
		EXEC(@SQL)
		SET @SQL=''
    END
    IF(@objtype IN (SELECT ObjType FROM #IsOrderObject) AND @cancellation='N')
	BEGIN  -- update order in oitw (Items - warehouse)
		
		---------------------------------------------- update lại số lượng của các mặt hàng theo số lượng của phiếu trong dữ liệu
		SET @SQL='
		UPDATE a
		SET a.OnOrder =
			isnull(a.OnOrder,0) -
			isnull((SELECT SUM(ISNULL(T0.OpenInvQty,0))
						FROM '+@tblline+' T0
						WHERE T0.ItemCode=a.ItemCode AND T0.WhsCode=a.WhsCode and T0.LineStatus=''O''  and T0.DocEntry='+@obj_id+'),0)
		FROM APZ_OITW a 
		INNER JOIN '+@tblline+' b ON a.ItemCode=b.ItemCode AND a.WhsCode=b.WhsCode
		INNER JOIN #IsOrderObject e on (''' + @objtype +'''=e.ObjType and e.IsIns = ''' + @isIns +''')
		inner join APZ_OITM x ON x.ItemCode = a.ItemCode where x.InvntItem = ''Y'' and b.DocEntry='+@obj_id+'
		
		UPDATE a
		SET a.OnOrder =
			isnull(a.OnOrder,0) -
			isnull((SELECT SUM(ISNULL(T0.OpenInvQty,0))
						FROM '+@tblline+' T0
						WHERE T0.ItemCode=a.ItemCode AND T0.LineStatus=''O'' and T0.DocEntry='+@obj_id+'),0)
		FROM APZ_OITM a 
		INNER JOIN '+@tblline+' b ON a.ItemCode=b.ItemCode
		INNER JOIN #IsOrderObject e on (''' + @objtype +'''=e.ObjType and e.IsIns = ''' + @isIns +''') where a.InvntItem = ''Y'' and b.DocEntry='+@obj_id;

		------------------------------------------------------- tăng số lượng order của các mặt hàng theo phiếu update hoặc addnew 
		SET @SQL= @SQL + '
		UPDATE a
		SET OnOrder=
			isnull(a.OnOrder,0) 
			+ isnull((SELECT SUM(ISNULL(T0.OpenInvQty,0)) 
						FROM #tempLines  T0
						WHERE T0.LineStatus=''O'' AND T0.ItemCode=a.ItemCode AND T0.WhsCode=a.WhsCode),0)
		
		FROM APZ_OITW a 
		INNER JOIN #tempLines  b ON a.ItemCode=b.ItemCode AND a.WhsCode=b.WhsCode and b.LineStatus=''O''
		INNER JOIN #IsOrderObject e on (''' + @objtype +'''=e.ObjType and e.IsIns = ''' + @isIns +''')
		INNER JOIN APZ_OITM x On a.ItemCode = x.ItemCode and x.InvntItem = ''Y''

		UPDATE a
		SET OnOrder=
			isnull(a.OnOrder,0)
			+ isnull((SELECT SUM(ISNULL(T0.OpenInvQty,0)) 
						FROM #tempLines  T0
						WHERE T0.LineStatus=''O'' AND T0.ItemCode=a.ItemCode),0)
		
		FROM APZ_OITM a 
		INNER JOIN #tempLines  b ON a.ItemCode=b.ItemCode and b.LineStatus=''O''
		INNER JOIN #IsOrderObject e on (''' + @objtype +'''=e.ObjType and e.IsIns = ''' + @isIns +''') where a.InvntItem = ''Y'''
		
		EXEC(@SQL)
		SET @SQL=''
    END

	IF (@objtype IN (SELECT ObjType FROM #DecOnhandObject UNION ALL SELECT ObjType FROM #IncOnhandObject)) AND (@transtype='A' or @transtype = 'L')
	BEGIN -- update quantity in oitw (Items - warehouse)
		IF (@objtype IN (SELECT ObjType FROM #DecOnhandObject)) SET @onhand_operator= case when @cancellation='N' then '-' else '+' end
		IF (@objtype IN (SELECT ObjType FROM #IncOnhandObject)) SET @onhand_operator= case when @cancellation='N' then '+' else '-' end

		IF @onhand_operator='' 
		BEGIN
			SET @error_prefix=@error_prefix+1
			SELECT @objtype + @error_prefix, 'Không thể kiểm tra dữ liệu tồn kho'
			RETURN
		END	
	
		SET @SQL='UPDATE a
			SET OnHand=isnull(a.OnHand,0) '+@onhand_operator+' isnull(b.Qty,0)
		FROM APZ_OITW a INNER JOIN (select SUM(ISNULL(x.OpenInvQty,0)) Qty, ItemCode, WhsCode from #tempLines x where
			((x.BaseType not in (select x1.ObjType from #DecOnhandObject x1 inner join #baseData x2 on x1.ObjType = x2.ObjectType and x2.IsIns = ''Y'' and x2.DocEntry = x.BaseEntry) and '''+@onhand_operator+''' =''-'')
		or (x.BaseType not in (select ObjType from #IncOnhandObject x1 inner join #baseData x2 on x1.ObjType = x2.ObjectType and x2.IsIns = ''Y'' and x2.DocEntry = x.BaseEntry) and '''+@onhand_operator+''' =''+'')) and ''' + @isIns + ''' = ''Y'' group by x.ItemCode, x.WhsCode) b ON a.ItemCode=b.ItemCode AND a.WhsCode=b.WhsCode
		inner join APZ_OITM xx on a.ItemCode = xx.ItemCode and xx.InvntItem = ''Y''

		UPDATE a 
			SET OnHand= isnull(a.OnHand,0) '+@onhand_operator+' isnull(b.Qty,0) 
		FROM APZ_OITM a INNER JOIN (select SUM(ISNULL(x.OpenInvQty,0)) Qty, ItemCode from #tempLines x where
		 ((x.BaseType not in (select x1.ObjType from #DecOnhandObject x1 inner join #baseData x2 on x1.ObjType = x2.ObjectType and x2.IsIns = ''Y'' and x2.DocEntry = x.BaseEntry) and '''+@onhand_operator+''' =''-'')
		or (x.BaseType not in (select ObjType from #IncOnhandObject x1 inner join #baseData x2 on x1.ObjType = x2.ObjectType and x2.IsIns = ''Y'' and x2.DocEntry = x.BaseEntry) and '''+@onhand_operator+''' =''+'')) and ''' + @isIns + ''' = ''Y'' group by x.ItemCode) b ON a.ItemCode=b.ItemCode
		INNER JOIN APZ_OITM xx on xx.ItemCode = a.ItemCode and xx.InvntItem = ''Y''
		'
		EXEC(@SQL)
		------------------------- Update close document khi tất cả các line close
		if @cancellation = 'N'
		BEGIN
			SET @SQL='
			UPDATE T0 
			SET T0.DocStatus=''C''
			FROM #tempDoc  T0 WHERE NOT EXISTS (SELECT 1 FROM #tempLines  T1 WHERE T1.LineStatus=''O'')'
		
			EXEC(@SQL)
		END
		SET @SQL=''
    END	
	-- *********** BASE AND TARGET DOCUMENT **********--
	
	IF @countBaseType>0 AND ((@objtype IN (SELECT ObjType FROM #DecOnhandObject UNION ALL SELECT ObjType FROM #IncOnhandObject) AND @transtype='A') OR (@objtype NOT IN (SELECT ObjType FROM #DecOnhandObject UNION ALL SELECT ObjType FROM #IncOnhandObject)))  AND @cancellation='N'
	BEGIN
		WHILE(@indicator<@countBaseType)
		BEGIN
			SET @basetype=ISNULL((SELECT ObjType FROM @tblBaseTypes WHERE Id=@indicator+1),'')
			
			IF	@basetype NOT IN ('0','-1','')
			BEGIN
				SET @basetable=(SELECT dbo.apz_document_get_table_doc(@basetype))
				SET @base_tblline=(SELECT dbo.apz_document_get_table_docline(@basetype))

				IF	(@objtype=@basetype) CONTINUE
				IF @basetype IN (SELECT ObjType FROM #IsCommitedObject)
				BEGIN
					SET @SQL='
					UPDATE a
						SET a.IsCommited= isnull(a.IsCommited,0) -ISNULL((SELECT SUM(CASE WHEN T2.OpenInvQty-T1.InvQty>=0 THEN T1.InvQty ELSE T2.OpenInvQty end) 
						FROM  #tempLines  T1 INNER JOIN '+@base_tblline+' T2 ON T1.BaseType='''+@basetype+''' AND T2.DocEntry=T1.BaseEntry AND T2.LineNum=T1.BaseLine AND  t2.ItemCode=a.ItemCode),0)
					FROM APZ_OITW a 
					INNER JOIN #tempLines  b ON a.ItemCode=b.ItemCode AND b.LineStatus=''O'' and b.BaseType='''+@basetype+'''
					INNER JOIN '+@base_tblline+' c ON b.BaseEntry=c.DocEntry and b.BaseLine=c.LineNum and a.WhsCode=c.WhsCode
					INNER JOIN APZ_OITM x On x.ItemCode = a.ItemCode and X.InvntItem = ''Y''

					UPDATE a
						SET a.IsCommited= isnull(a.IsCommited,0) -ISNULL((SELECT SUM(CASE WHEN T2.OpenInvQty-T1.InvQty>=0 THEN T1.InvQty ELSE T2.OpenInvQty end) 
						FROM  #tempLines  T1 INNER JOIN '+@base_tblline+' T2 ON T2.DocEntry=T1.BaseEntry AND T2.LineNum=T1.BaseLine AND T1.BaseType='''+@basetype+'''  AND t2.ItemCode=a.ItemCode ),0)
					FROM APZ_OITM a INNER JOIN #tempLines  b ON a.ItemCode=b.ItemCode and b.LineStatus=''O'' and b.BaseType='''+@basetype+''' where a.InvntItem = ''Y'''
					
					EXEC(@sql)
					SET @SQL=''
				END
				IF @basetype IN (SELECT ObjType FROM #IsOrderObject)
				BEGIN
					SET @SQL='
					UPDATE a
						SET a.OnOrder= isnull(a.OnOrder,0) -ISNULL((SELECT SUM(CASE WHEN T2.OpenInvQty-T1.InvQty>=0 THEN T1.InvQty ELSE T2.OpenInvQty end) 
						FROM  #tempLines  T1 INNER JOIN '+@base_tblline+' T2 ON T1.BaseType='''+@basetype+''' AND T2.DocEntry=T1.BaseEntry AND T2.LineNum=T1.BaseLine AND  t2.ItemCode=a.ItemCode),0)
					FROM APZ_OITW a 
					INNER JOIN #tempLines  b ON a.ItemCode=b.ItemCode AND b.LineStatus=''O'' 
					INNER JOIN '+@base_tblline+' c ON b.BaseEntry=c.DocEntry and b.BaseLine=c.LineNum and a.WhsCode=c.WhsCode
					INNER JOIN #IsOrderObject e on (b.BaseType='''+@basetype+''' OR (b.BaseType=e.ObjType and e.IsIns = ''' + @isIns +'''))
					INNER JOIN APZ_OITM xx ON xx.ItemCode = a.ItemCode and xx.InvntItem = ''Y''

					UPDATE a
						SET a.OnOrder= isnull(a.OnOrder,0) -ISNULL((SELECT SUM(CASE WHEN T2.OpenInvQty-T1.InvQty>=0 THEN T1.InvQty ELSE T2.OpenInvQty end) 
						FROM  #tempLines  T1 INNER JOIN '+@base_tblline+' T2 ON T2.DocEntry=T1.BaseEntry AND T2.LineNum=T1.BaseLine AND T1.BaseType='''+@basetype+'''  AND t2.ItemCode=a.ItemCode ),0)
					FROM APZ_OITM a 
					INNER JOIN #tempLines  b ON a.ItemCode=b.ItemCode and b.LineStatus=''O''
					INNER JOIN #IsOrderObject e on (b.BaseType='''+@basetype+''' OR (b.BaseType=e.ObjType and e.IsIns = ''' + @isIns +''')) where a.InvntItem = ''Y'''
					
					EXEC(@sql)
					SET @SQL=''
				END

				BEGIN
                SET @SQL='UPDATE T0
						SET 
						 T0.OpenQty=CASE WHEN T0.OpenQty +isnull(T3.Quantity,0) -T2.Quantity <0 THEN 0 ELSE T0.OpenQty +isnull(T3.Quantity,0)-T2.Quantity end
						,T0.OpenInvQty=CASE WHEN T0.OpenInvQty +isnull(T3.InvQty,0)-T2.OpenInvQty <0 THEN 0 ELSE T0.OpenInvQty +isnull(T3.InvQty,0)-T2.OpenInvQty END
						,T0.LineStatus=CASE WHEN T0.OpenQty +isnull(T3.Quantity,0) -T2.Quantity <=0 THEN ''C'' ELSE T0.LineStatus END
						,T0.TargetType='''+@objtype+'''
						,T0.TrgetEntry=T2.DocEntry
					FROM '+@base_tblline+' T0
					INNER JOIN '+@basetable+' T1 ON T0.DocEntry=T1.DocEntry
					INNER JOIN #tempLines  T2 
												LEFT JOIN '+@tblline+' T3 ON T2.DocEntry=T3.DocEntry and T2.LineNum=T3.LineNum and T2.ItemCode=T3.ItemCode and T3.DocEntry='+@obj_id+'                                                                                                                                                                                                                                                                                                                                                                                                                                          
					ON T2.BaseType=T1.ObjType and T0.DocEntry=T2.BaseEntry AND T0.LineNum=T2.BaseLine
					WHERE T0.LineStatus=''O''
					'

					EXEC (@SQL)
					SET @SQL=''

					SET @SQL='UPDATE T0 
					SET T0.DocStatus=''C''
					FROM '+@basetable+' T0 WHERE T0.DocEntry IN (SELECT BaseEntry FROM #tempLines  WHERE BaseType=T0.ObjType)
					AND NOT EXISTS (SELECT 1 FROM '+@base_tblline+' T1 WHERE T1.DocEntry=T0.DocEntry AND T1.LineStatus=''O'')'
					EXEC (@SQL)
					SET @SQL=''
				END
            END
            
			SET @indicator=@indicator+1
		END 
	END
	
	SELECT 0, ''
END
