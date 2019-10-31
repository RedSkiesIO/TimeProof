using Dawn;
using Microsoft.Extensions.Configuration;

namespace DocumentStamp.Model
{
    public class Config
    {
        public NodeConfig NodeConfig { set; get; }

        public Config(IConfigurationRoot rootSection)
        {
            Guard.Argument(rootSection, nameof(rootSection)).NotNull();

            var documentStampSection = rootSection.GetSection("DocumentStamp");
            var nodeConfigSection = documentStampSection.GetSection("Node");

            NodeConfig = new NodeConfig(nodeConfigSection);
        }
    }
}