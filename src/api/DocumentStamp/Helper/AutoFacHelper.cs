using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using Autofac;
using Autofac.Configuration;
using Catalyst.Abstractions.Cli;
using Catalyst.Abstractions.Cryptography;
using Catalyst.Abstractions.IO.Observers;
using Catalyst.Abstractions.Keystore;
using Catalyst.Abstractions.P2P;
using Catalyst.Abstractions.Rpc;
using Catalyst.Abstractions.Types;
using Catalyst.Core.Lib;
using Catalyst.Core.Lib.Cli;
using Catalyst.Core.Lib.Cryptography;
using Catalyst.Core.Lib.P2P;
using Catalyst.Core.Modules.Cryptography.BulletProofs;
using Catalyst.Core.Modules.Hashing;
using Catalyst.Core.Modules.KeySigner;
using Catalyst.Core.Modules.Keystore;
using Catalyst.Core.Modules.Rpc.Client;
using Catalyst.Core.Modules.Rpc.Client.IO.Observers;
using Catalyst.Protocol.Network;
using DocumentStamp.Model;
using Microsoft.Extensions.Configuration;
using Serilog;
using TheDotNetLeague.MultiFormats.MultiBase;

namespace DocumentStamp.Helper
{
    public static class AutoFacHelper
    {
        public static ContainerBuilder GenerateRpcClientContainerBuilder()
        {
            var configRoot = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "config.json")).Build();
            var configModule = new ConfigurationModule(configRoot);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(configModule);
            containerBuilder.RegisterInstance(configRoot).As<IConfigurationRoot>();

            containerBuilder.RegisterType<Config>();
            containerBuilder.RegisterType<ConsoleUserOutput>().As<IUserOutput>();
            containerBuilder.RegisterType<ConsoleUserInput>().As<IUserInput>();
            containerBuilder.RegisterInstance(new LoggerConfiguration().WriteTo.Console().CreateLogger())
                .As<ILogger>();
            containerBuilder.RegisterModule<CoreLibProvider>();
            containerBuilder.RegisterModule<RpcClientModule>();
            containerBuilder.RegisterModule<KeySignerModule>();
            containerBuilder.RegisterModule<KeystoreModule>();
            containerBuilder.RegisterModule<BulletProofsModule>();
            containerBuilder.RegisterModule<HashingModule>();

            RegisterPasswordRegistry(containerBuilder);
            RegisterCertificate(containerBuilder);
            RegisterPeerSettings(containerBuilder);
            RegisterRpcClientSettings(containerBuilder);
            RegisterRpcResponseObservers(containerBuilder);

            return containerBuilder;
        }

        private static void RegisterPasswordRegistry(ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(x =>
            {
                var config = x.Resolve<Config>();
                var passwordRegistry = new PasswordRegistry();
                var secureString = new SecureString();
                config.NodeConfig.NodePassword.ToCharArray().ToList().ForEach(secureString.AppendChar);
                passwordRegistry.AddItemToRegistry(PasswordRegistryTypes.DefaultNodePassword, secureString);
                return passwordRegistry;
            }).As<IPasswordRegistry>();
        }

        private static void RegisterRpcResponseObservers(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<BroadcastRawTransactionResponseObserver>().As<IRpcResponseObserver>();
        }

        private static void RegisterCertificate(ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(x =>
            {
                var config = x.Resolve<Config>();
                return new X509Certificate2(File.ReadAllBytes(config.NodeConfig.PfxFileName),
                    config.NodeConfig.SslCertPassword);
            }).As<X509Certificate2>();
        }

        private static void RegisterPeerSettings(ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(x =>
            {
                var keyStore = x.Resolve<IKeyStore>();
                var privateKey = keyStore.KeyStoreDecrypt(KeyRegistryTypes.DefaultKey);
                var publicKey = privateKey.GetPublicKey();

                var publicKeyBase32 = publicKey.Bytes.ToBase32();

                var peerConfig = new Dictionary<string, string>
                {
                    {"CatalystNodeConfiguration:Peer:Network", "Devnet"},
                    {"CatalystNodeConfiguration:Peer:PublicKey", publicKeyBase32},
                    {"CatalystNodeConfiguration:Peer:Port", "42076"},
                    {"CatalystNodeConfiguration:Peer:PublicIpAddress", IPAddress.Loopback.ToString()},
                    {"CatalystNodeConfiguration:Peer:BindAddress", IPAddress.Loopback.ToString()}
                };

                var configRoot = new ConfigurationBuilder().AddInMemoryCollection(peerConfig).Build();

                var peerSettingsObj = new PeerSettings(configRoot);
                containerBuilder.RegisterInstance(peerSettingsObj).As<IPeerSettings>();
                return peerSettingsObj;
            }).As<IPeerSettings>();
        }

        private static void RegisterRpcClientSettings(ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(x =>
            {
                var config = x.Resolve<Config>();
                var rpcClientSettings = new RpcClientSettings
                {
                    HostAddress = config.NodeConfig.IpAddress,
                    Port = config.NodeConfig.Port,
                    PfxFileName = config.NodeConfig.PfxFileName,
                    SslCertPassword = config.NodeConfig.SslCertPassword,
                    PublicKey = config.NodeConfig.PublicKey
                };
                return rpcClientSettings;
            }).As<IRpcClientConfig>();
        }
    }
}