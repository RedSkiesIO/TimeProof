using DocumentStamp.Model;

namespace DocumentStamp.Http.Request
{
    public class StampDocumentRequest : UserProof
    {
        public string FileName { set; get; }
    }
}
