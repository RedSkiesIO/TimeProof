using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentStamp.Model
{
    public class StampedUserProof
    {
        public DateTime TimeStamp { set; get; }
        public UserProof UserProof { set; get; }
    }
}
