using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apzon.PosmanErp.Entities
{
    public class ObjectDefinedModel
    {
        /// <summary>
        /// header table of object
        /// </summary>
        public string HeaderTable { get; set; }

        /// <summary>
        /// children table of object
        /// </summary>
        public List<string> LineTables { get; set; }

        /// <summary>
        /// danh sách column key của object
        /// </summary>
        public List<string> ObjectColumnKeys { get; set; }

    }
}
