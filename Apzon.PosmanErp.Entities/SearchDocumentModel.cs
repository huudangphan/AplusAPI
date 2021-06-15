// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using Apzon.PosmanErp.Commons;

namespace Apzon.PosmanErp.Entities
{
    public class SearchDocumentModel
    {
        public int doc_entry { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public int? page_index { get; set; }
        public int? page_size { get; set; }
        public string order_by { get; set; }
        public DateTime? from_date { get; set; }
        public DateTime? to_date { get; set; }

        public DateTime? valid_from { get; set; }
        public DateTime? valid_to { get; set; }
        public bool is_ascending { get; set; }
        public APlusObjectType object_type { get; set; }
        public List<string> cpn_codes { get; set; } 
        public int? user_sign { get; set; }
        public int? branch { get; set; }
        public List<string> company { get; set; }
        public List<string> groups { get; set; }
        public List<string> trademarks { get; set; }
        public List<string> tags { get; set; }
        public List<string> types { get; set; }
        public string type { get; set; }
        public string tran_type { get; set; }
        public string whs_code { get; set; }

        public int? price_list { get; set; }
        public string search_name { get; set; }

        #region choose from list

        public int setting_id { get; set; }
        public AplusSubObjectType sub_type { get; set; }

        public string form_id { get; set; }

        public string item_id { get; set; }

        #region UDF

        public int? user_id { get; set; }
        public string language_code { get; set; }

        #endregion

        public string table_name { get; set; }
        public string column_id { get; set; }
        public string column_name { get; set; }
        public string data_type { get; set; }

        #endregion
    }
}