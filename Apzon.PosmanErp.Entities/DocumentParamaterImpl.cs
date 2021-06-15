using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apzon.PosmanErp.Entities
{
    public class DocumentParamaterImpl
    {
        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public List<string> Suppliers { get; set; }
        
        public List<int> Users { get; set; }
        
        public string TextSearch { get; set; }
        
        public string Filter { get; set; }  

        public int PageSize { get; set; }

        public int PageIndex { get; set; }
    }
}
