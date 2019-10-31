using System;
using Catalyst.Abstractions.Cryptography;
using Catalyst.Core.Lib.Extensions;
using Catalyst.Core.Lib.Extensions.Protocol.Wire;
using Catalyst.Core.Modules.Cryptography.BulletProofs;
using Catalyst.Protocol.Cryptography;
using Catalyst.Protocol.Network;
using Catalyst.Protocol.Rpc.Node;
using Catalyst.Protocol.Transaction;
using Catalyst.Protocol.Wire;
using Google.Protobuf.WellKnownTypes;
using Nethermind.Dirichlet.Numerics;

namespace DocumentStamp.Helper
{
    public static class StampTransactionHelper
    {
        private static readonly SigningContext DevNetPublicTransactionContext = new SigningContext
        {
            NetworkType = NetworkType.Devnet,
            SignatureType = SignatureType.TransactionPublic
        };

        public static BroadcastRawTransactionRequest GenerateStampTransaction(IPrivateKey senderPrivateKey,
            IPublicKey receiverPublicKey, byte[] data, uint amount, int fee,
            int nonce = 0)
        {
            var cryptoWrapper = new FfiWrapper();
            var senderPublicKey = senderPrivateKey.GetPublicKey().Bytes;

            var transaction = new TransactionBroadcast
            {
                PublicEntries =
                {
                    new PublicEntry
                    {
                        Amount = ((UInt256) amount).ToUint256ByteString(),
                        Base = new BaseEntry
                        {
                            Nonce = (ulong) nonce,
                            SenderPublicKey = senderPublicKey.ToByteString(),
                            ReceiverPublicKey = receiverPublicKey.Bytes.ToByteString(),
                            TransactionFees = ((UInt256) fee).ToUint256ByteString()
                        }
                    }
                },
                ContractEntries =
                {
                    new ContractEntry
                    {
                        Amount = ((UInt256) amount).ToUint256ByteString(),
                        Base = new BaseEntry
                        {
                            Nonce = (ulong) nonce,
                            SenderPublicKey = senderPublicKey.ToByteString(),
                            ReceiverPublicKey = receiverPublicKey.Bytes.ToByteString(),
                            TransactionFees = ((UInt256) fee).ToUint256ByteString()
                        },
                        Data = data.ToByteString()
                    }
                },
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
            };

            var signedTransaction = transaction.Sign(cryptoWrapper, senderPrivateKey, DevNetPublicTransactionContext);
            var broadcastRawTransactionRequest = new BroadcastRawTransactionRequest
            {
                Transaction = signedTransaction
            };

            return broadcastRawTransactionRequest;
        }
    }
}