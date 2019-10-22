using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentStamp.Model
{
    public class UserProof
    {
        public string PublicKey { set; get; }
        public string Hash { set; get; }
        public string Signature { set; get; }
    }
}
