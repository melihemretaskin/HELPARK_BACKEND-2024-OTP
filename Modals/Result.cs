using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modals
{
    public class Result
    {
        public Result() { }

        public string message { get; set; }
        public string result { get; set; }
        public int statusCode { get; set; } // HTTP status kodu için int türü daha uygun
        public string timeStamp { get; set; }
    }
}

