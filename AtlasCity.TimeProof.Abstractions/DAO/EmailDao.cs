using System;
using System.Collections.Generic;
using System.Text;

namespace AtlasCity.TimeProof.Abstractions.DAO
{
    public class EmailDao
    {
        public string ToAddress { get; set; }
        public string ToName { get; set; }
        
        public string FromAddress { get; set; }

        public string Subject{ get; set; }
        public string HtmlBody { get; set; }
    }
}
