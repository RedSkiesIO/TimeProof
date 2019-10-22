using System;
using DocumentStamp.Model;

namespace DocumentStamp.Response
{
    public class StampDocumentResponse
    {
        public string TransactionId { set; get; }
        public DateTime TimeStamp { set; get; }
        public UserProof UserProof { set; get; }
        public NodeProof NodeProof { set; get; }
    }
}