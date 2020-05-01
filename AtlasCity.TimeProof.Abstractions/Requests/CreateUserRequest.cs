using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Abstractions.Requests
{
    public class CreateUserRequest
    {
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "address")]
        public AddressRequest Address { get; set; }       
    }
}