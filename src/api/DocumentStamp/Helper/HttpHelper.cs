using System;
using System.IO;
using System.Linq;
using System.Text;
using Catalyst.Core.Lib.DAO;
using DocumentStamp.Http.Response;
using DocumentStamp.Model;
using Newtonsoft.Json;
using RestSharp;

namespace DocumentStamp.Helper
{
    public static class HttpHelper
    {
        public static StampDocumentResponse GetStampDocument(string webAddress, string txId)
        {
            var client = new RestClient($"{webAddress}");
            var request = new RestRequest("/api/Mempool/Get/{id}", Method.GET);
            request.AddUrlSegment("id", txId);

            var response = client.Execute<TransactionBroadcastDao>(request);
            var transactionBroadcastDao = response.Data;
            if (response.Data == null)
            {
                throw new InvalidDataException("DocumentStamp does not exist under txId");
            }

            var smartContract = transactionBroadcastDao.ContractEntries.First();
            var smartContractData = Encoding.UTF8.GetString(Convert.FromBase64String(smartContract.Data));
            var userProof = JsonConvert.DeserializeObject<UserProof>(smartContractData);

            //Verify the signature of the stamp document request
            var verifyResult = SignatureHelper.VerifyStampDocumentRequest(userProof);
            if (!verifyResult)
            {
                throw new InvalidDataException("Could not verify signature of document stamp request");
            }

            var stampDocumentResponse = new StampDocumentResponse
            {
                TransactionId = transactionBroadcastDao.Id.ToUpper(),
                TimeStamp = transactionBroadcastDao.TimeStamp,
                UserProof = userProof,
                NodeProof = new NodeProof
                {
                    PublicKey = smartContract.Base.SenderPublicKey.ToUpper(),
                    Signature = transactionBroadcastDao.Signature.RawBytes.ToUpper()
                }
            };
            return stampDocumentResponse;
        }
    }
}