using System.Numerics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Repository;
using AtlasCity.TimeProof.Abstractions.Services;
using AtlasCity.TimeProof.Common.Lib.Extensions;
using AtlasCity.TimeProof.Abstractions;
using AtlasCity.TimeProof.Abstractions.Helpers;
using Nethereum.Web3;
using Nethereum.Web3.Accounts.Managed;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using Newtonsoft.Json;

namespace AtlasCity.TimeProof.Common.Lib.Services
{
    public class TimestampService : ITimestampService
    {
        private readonly ITimestampRepository _timestampRepository;

        private readonly ISignatureHelper _signatureHelper;

        public TimestampService(ITimestampRepository timeStampRepository, ISignatureHelper signatureHelper)
        {
            AtlasGuard.IsNotNull(timeStampRepository);
            AtlasGuard.IsNotNull(signatureHelper);

            _timestampRepository = timeStampRepository;
            _signatureHelper = signatureHelper;
        }

        public async Task<IEnumerable<TimestampDao>> GetUesrTimestamps(string userId, CancellationToken cancellationToken)
        {
            AtlasGuard.IsNotNullOrWhiteSpace(userId);

            //TODO: Sudhir Paging
            return await _timestampRepository.GetTimestampByUser(userId, cancellationToken);
        }

        public async Task<TimestampDao> GenerateTimestamp(string userId, TimestampDao timestamp, CancellationToken cancellationToken)
        { 
            AtlasGuard.IsNotNullOrWhiteSpace(timestamp.UserId);
            AtlasGuard.IsNotNull(timestamp);

            // TODO: Sudhir Check if user have stamp remaining

             bool proofVerified = _signatureHelper.VerifyStamp(timestamp);

            if (!proofVerified)
            {
                // TODO: return meaningful response
                return null;//"Could not verify signature";
            }

            var resultStamp = await sendTransaction(timestamp);

            if (resultStamp.TxId != null)
            {
                return await _timestampRepository.CreateTimestamp(userId, timestamp, cancellationToken);
            }

            return null;// TODO: return meaningful response
            // TODO: Sudhir Decrease the remaining timestamp
        }

         private async Task<TimestampDao> sendTransaction(TimestampDao timestamp)
        {

            ManagedAccount account = new ManagedAccount(Constants.ACCOUNT_ADDRESS, "");

            var web3 = new Web3(account, Constants.NODE_ENDPOINT);

            var user = "";//(jwtDecode(req.headers.authorization)).sub;

            string proofStr = JsonConvert.SerializeObject(
                new {
                    file = timestamp.FileName,
                    hash = timestamp.Hash,
                    publicKey = timestamp.PublicKey,
                    signature = timestamp.Signature
                });
       
            var txData = HexStringUTF8ConvertorExtensions.ToHexUTF8(proofStr);

            var currentNonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(account.Address,
                            Nethereum.RPC.Eth.DTOs.BlockParameter.CreatePending());
           
            var res = (
                privateKey: Constants.SECRET_KEY,
                chainId: Nethereum.Signer.Chain.Kovan,
                to:Constants.TO_ADDRESS,
                amount: Web3.Convert.ToWei(0, UnitConversion.EthUnit.Gwei),
                nonce: currentNonce,
                gasPrice: Web3.Convert.ToWei(Constants.GAS_PRICE, UnitConversion.EthUnit.Gwei),
                gasLimit: new BigInteger(100000),
                data: txData
            ); 

            var encoded = Web3.OfflineTransactionSigner.SignTransaction(res.privateKey,
                res.chainId, res.to, res.amount, res.nonce, res.gasPrice, res.gasLimit, res.data);

            var verified = Web3.OfflineTransactionSigner.VerifyTransaction(encoded);

            var txId = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync("0x" + encoded);
            var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txId);
            while (receipt == null)
            {
                Thread.Sleep(1000);
                receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txId);
            }     

            timestamp.BlockNumber = -1;
            timestamp.TxId = txId;
            timestamp.Nonce = (long) currentNonce.Value;
            timestamp.Network = Nethereum.Signer.Chain.Kovan.ToString();

            // Assert.Equal(txId, receipt.TransactionHash);

            return timestamp;
        }
    }
}