using System;
using System.Net;
using Catalyst.Core.Lib.Extensions;
using Catalyst.Core.Modules.Cryptography.BulletProofs;
using Catalyst.Modules.Repository.CosmosDb;
using DocumentStamp;
using DocumentStamp.Model;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using TheDotNetLeague.MultiFormats.MultiBase;

[assembly: FunctionsStartup(typeof(Startup))]

namespace DocumentStamp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var cryptoContext = new FfiWrapper();
            var privateKey = cryptoContext.GetPrivateKeyFromBytes(Environment.GetEnvironmentVariable("FunctionPrivateKey").FromBase32());
            var publicKey = privateKey.GetPublicKey();
            var publicKeyBase32 = publicKey.Bytes.ToBase32();

            var peerId = publicKeyBase32.BuildPeerIdFromBase32Key(IPAddress.Loopback, 42076);

            var recptIp = IPAddress.Parse(Environment.GetEnvironmentVariable("NodeIpAddress"));
            var recptPort = int.Parse(Environment.GetEnvironmentVariable("NodePort"));
            var recptPublicKey = Environment.GetEnvironmentVariable("NodePublicKey");

            var recipientPeer =
                recptPublicKey.BuildPeerIdFromBase32Key(recptIp,
                    recptPort);

            var documentStampMetaDataRepository = new CosmosDbRepository<DocumentStampMetaData>(Environment.GetEnvironmentVariable("CosmosDBServer"), Environment.GetEnvironmentVariable("AuthorizationKey"), Environment.GetEnvironmentVariable("DatabaseId"), true);

            var restClient = new RestClient(Environment.GetEnvironmentVariable("NodeWebAddress"));
            builder.Services.AddSingleton(restClient);
            builder.Services.AddSingleton(peerId);
            builder.Services.AddSingleton(cryptoContext);
            builder.Services.AddSingleton(privateKey);
            builder.Services.AddSingleton(recipientPeer);
            builder.Services.AddSingleton(documentStampMetaDataRepository);
        }
    }
}