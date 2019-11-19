using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using SharpRepository.Repository;

namespace DocumentStamp.Model
{
    public class DocumentStampMetaData
    {
        [RepositoryPrimaryKey(Order = 1)]
        [Key]
        [JsonProperty(PropertyName = "id")]
        public string Id { set; get; }
        public string FileName { set; get; }
        public string PublicKey { set; get; }
        public StampDocumentProof StampDocumentProof { set; get; }
        public string User { set; get; }
    }
}
