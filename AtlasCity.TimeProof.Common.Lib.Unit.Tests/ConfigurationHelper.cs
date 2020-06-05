using Microsoft.Extensions.Configuration;
using System;

namespace AtlasCity.TimeProof.Common.Lib.Unit.Tests
{
    public static class ConfigurationHelper
    {
        private static IConfiguration Configuration;

        public static IConfiguration GetIConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json", optional: true)
                .Build();
        }

        public static string GetConfigValue(string configKey)
        {
            if (Configuration == null)
                Configuration = GetIConfiguration();

            return Configuration.GetSection(configKey).Value;
        }


        public static int GetConfigValueAsInt(string configKey)
        {
            return int.Parse(GetConfigValue(configKey));
        }
    }
}
