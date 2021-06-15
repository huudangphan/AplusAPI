﻿using Apzon.HDBConnection.Interfaces;
using Sap.Data.Hana;

namespace Apzon.HDBConnection
{
    public class ApzHanaTransaction : ITransaction
    {
        public HanaTransaction Transaction;

        public ApzHanaTransaction(HanaTransaction transaction)
        {
            Transaction = transaction;
        }

        public void Commit()
        {
            Transaction.Commit();
            Transaction.Dispose();
        }

        public void Rollback()
        {
            Transaction.Rollback();
            Transaction.Dispose();
        }

        public void Dispose()
        {
            Transaction.Dispose();
        }
    }
}
