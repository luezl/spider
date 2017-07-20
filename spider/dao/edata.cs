using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spider
{
    public class edata
    {
        public edata() {
            create_at = DateTime.Now.ToLongDateString();
        }

        public Int64 id { get; set; }

        public Int64 type { get; set; }

        public string name { get; set; }

        public string url { get; set; }

        public string value1 { get; set; }

        public string value2 { get; set; }

        public string value3 { get; set; }

        public string value4 { get; set; }

        public string value5 { get; set; }

        public string create_at { get; set; }
        
    }
}
