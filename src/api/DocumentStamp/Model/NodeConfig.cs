using System.Net;
using Catalyst.Core.Lib.Extensions;
using Catalyst.Protocol.Peer;
using Microsoft.Extensions.Configuration;

namespace DocumentStamp.Model
{
    public class NodeConfig
    {
        public string PublicKey { get; }
        public int Port { get; }
        public IPAddress IpAddress { get; }
        public PeerId PeerId { get; }
        public string PfxFileName { get; }
        public string SslCertPassword { get; }
        public string NodePassword { get; }
        public string WebAddress { get; }

        public NodeConfig(IConfiguration configuration)
        {
            PublicKey = configuration.GetSection("PublicKey").Value;
            Port = int.Parse(configuration.GetSection("Port").Value);
            IpAddress = IPAddress.Parse(configuration.GetSection("IpAddress").Value);
            PeerId = PublicKey.BuildPeerIdFromBase32Key(IpAddress, Port);
            PfxFileName = configuration.GetSection("PfxFileName").Value;
            SslCertPassword = configuration.GetSection("SslCertPassword").Value;
            NodePassword = configuration.GetSection("NodePassword").Value;
            WebAddress = configuration.GetSection("WebAddress").Value;
        }
    }
}