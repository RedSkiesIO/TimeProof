using DocumentStamp.Model;

namespace DocumentStamp.Http.Response
{
    public class StampDocumentResponse
    {
        public string FileName { set; get; }
        public StampDocumentProof StampDocumentProof { set; get; }
    }
}
