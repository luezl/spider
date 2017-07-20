using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spider
{
   [Serializable]
    public class Config
    {
        public string url { get; set; }

        public string name { get; set; }

        public string selector1 { get; set; }

        public string selector2 { get; set; }

        public string selector3 { get; set; }

        public string selector4 { get; set; }

        public string selector5 { get; set; }
        
    }

}
