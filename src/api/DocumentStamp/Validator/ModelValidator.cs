using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace DocumentStamp.Validator
{
    public static class ModelValidator
    {
        public static T ValidateAndConvert<T>(string content)
        {
            var request = JsonConvert.DeserializeObject<T>(content);
            Validate(request);
            return request;
        }

        public static void Validate<T>(T model)
        {
            foreach (var propertyInfo in typeof(T).GetProperties()
                .Where(x => x.PropertyType == typeof(string)))
            {
                if (string.IsNullOrEmpty((string) propertyInfo.GetValue(model)))
                {
                    throw new InvalidDataException($"Request must contain a valid {propertyInfo.Name}");
                }
            }
        }
    }
}
