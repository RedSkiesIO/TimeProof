using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.Requests
{
    public class UpdateUserRequest
    {
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }
    }
}