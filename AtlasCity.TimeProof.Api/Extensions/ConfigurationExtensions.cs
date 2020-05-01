using System;
using System.Collections.Generic;
using System.Linq;
using Dawn;
using Microsoft.Extensions.Configuration;

namespace AtlasCity.TimeProof.Api.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string GetValue(this IConfiguration configuration, string key)
        {
            Guard.Argument(configuration, nameof(configuration)).NotNull();
            Guard.Argument(key, nameof(key)).NotNull().NotEmpty().NotWhiteSpace();

            var configValue = configuration.GetSection(key).Value;

            return configValue;
        }

        public static int GetValueAsInt(this IConfiguration configuration, string key)
        {
            var configValue = GetValue(configuration, key);
            int.TryParse(configValue, out int intValue);
            return intValue;
        }

        public static List<string> GetValueAsList(this IConfiguration configuration, string key, string separator)
        {
            Guard.Argument(separator, nameof(separator)).NotNull().NotEmpty().NotWhiteSpace();

            var configValue = GetValue(configuration, key);

            if (string.IsNullOrWhiteSpace(configValue))
                return new List<string>();

            var configValueList = configValue.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            return configValueList.ToList();
        }

        public static string[] GetValueAsArray(this IConfiguration configuration, string key, string separator)
        {
            Guard.Argument(separator, nameof(separator)).NotNull().NotEmpty().NotWhiteSpace();

            var configValues = GetValueAsList(configuration, key, separator);

            return configValues.ToArray();
        }
    }
}
