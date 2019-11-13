using Autofac;
using Catalyst.Abstractions.Cryptography;
using Catalyst.Abstractions.Keystore;
using Catalyst.Abstractions.P2P;
using Catalyst.Abstractions.Rpc;
using Catalyst.Abstractions.Types;
using Catalyst.Core.Lib.Extensions;
using Catalyst.Protocol.Network;
using DocumentStamp;
using DocumentStamp.Helper;
using DocumentStamp.Model;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace DocumentStamp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var containerBuilder = AutoFacHelper.GenerateRpcClientContainerBuilder();
            var container = containerBuilder.Build();
            var keyStore = container.Resolve<IKeyStore>();

            if (keyStore.KeyStoreDecrypt(KeyRegistryTypes.DefaultKey) == null)
            {
                keyStore.KeyStoreGenerateAsync(NetworkType.Devnet, KeyRegistryTypes.DefaultKey).Wait();
            }

            var rpcClientConfig = container.Resolve<IRpcClientConfig>();

            builder.Services.AddSingleton(container.Resolve<Config>());
            builder.Services.AddSingleton(container.Resolve<IPeerSettings>());
            builder.Services.AddSingleton(container.Resolve<ICryptoContext>());
            builder.Services.AddSingleton(keyStore.KeyStoreDecrypt(KeyRegistryTypes.DefaultKey));
            builder.Services.AddSingleton(container.Resolve<IRpcClient>());
            builder.Services.AddSingleton(rpcClientConfig.PublicKey.BuildPeerIdFromBase32Key(rpcClientConfig.HostAddress, rpcClientConfig.Port));
        }
    }
}