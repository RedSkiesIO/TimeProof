using System;

namespace DocumentStamp.Model
{
    public class StampDocumentProof
    {
        public string TransactionId { set; get; }
        public DateTime TimeStamp { set; get; }
        public UserProof UserProof { set; get; }
        public NodeProof NodeProof { set; get; }
    }
}
