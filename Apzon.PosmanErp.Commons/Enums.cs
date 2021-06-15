using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apzon.PosmanErp.Commons
{
    public enum TransactionType
    {
        A = 0, //[A]dd
        U = 1, //[U]pdate
        D = 2, //[D]elete
        C = 3, //[C]ancel
        L = 4, //C[L]ose
        R = 5,  //[R]un
        M = 6  //comments
    }

    public enum DatabaseSystemType
    {
        SQL = 0,
        HANA = 1,
        PostgresSql = 2
    }
}
