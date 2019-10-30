using Autofac;
using Catalyst.Abstractions.Keystore;
using Catalyst.Abstractions.Types;
using Catalyst.Protocol.Network;
using DocumentStamp;
using DocumentStamp.Helper;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

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
                keyStore.KeyStoreGenerate(NetworkType.Devnet, KeyRegistryTypes.DefaultKey).Wait();
            }
        }
    }
}