using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apzon.PosmanErp.Commons;

namespace Apzon.PosmanErp.Entities
{
    public class ExchangeOrderImpl
    {
        public APlusObjectType Type { get; set; }

        public int OrderEntry { get; set; }

        public DataSet ReturnOrder { get; set; }

        public DataSet OrderNew { get; set; }

        public DataTable Payments { get; set; }
    }
}
