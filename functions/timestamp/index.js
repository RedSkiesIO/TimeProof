/* eslint-disable no-param-reassign */
const Web3 = require('web3');
const Tx = require('ethereumjs-tx');
const { jwtDecode, verifyProof } = require('../utils');


module.exports = async function timestamp(context, req, lastNonce) {
  if (req.method !== 'POST') {
    context.log('not verified');
    return {
      status: 400,
      body: 'request failed',
    };
  }

  const user = (jwtDecode(req.headers.authorization)).sub;
  const proof = verifyProof(req.body);

  if (proof === false) {
    return {
      status: 400,
      body: 'Could not verify signature',
    };
  }

  if (proof === null) {
    return {
      status: 400,
      body: 'request failed',
    };
  }

  const web3js = new Web3(new Web3.providers.HttpProvider(process.env.NODE_API));
  const txData = web3js.utils.asciiToHex(proof);
  console.log(lastNonce);
  let localNonce;
  if (lastNonce.latestNonce) {
   localNonce = lastNonce[0].latestNonce + 1;
  } else {
    localNonce = await web3js.eth.getTransactionCount(process.env.ACCOUNT_ADDRESS, 'pending');
    context.log('fetchNonce: ', localNonce);
  }
  context.log('LocalNonce: ', localNonce);
  context.log('DatabaseNonce: ', lastNonce[0]);

  

  function sendTransaction(accountNonce = localNonce) {
    const rawTransaction = {
      from: process.env.ACCOUNT_ADDRESS,
      nonce: accountNonce,
      gasPrice: web3js.utils.toHex(process.env.GAS_PRICE * 1e9),
      gasLimit: web3js.utils.toHex(100000),
      to: process.env.TO_ADDRESS,
      value: web3js.utils.numberToHex(web3js.utils.toWei('0', 'ether')),
      data: txData,
    };

    const privateKey = Buffer.from(process.env.SECRET_KEY, 'hex');
    const tx = new Tx.Transaction(rawTransaction, { chain: process.env.CHAIN });
    tx.sign(privateKey);
    const serializedTx = tx.serialize();
    const hash = `0x${tx.hash().toString('hex')}`;

    const value = {
      id: hash,
      fileName: req.body.fileName,
      publicKey: req.body.publicKey,
      txHash: hash,
      timestamp: Date.now(),
      blockNumber: -1,
      fileHash: req.body.hash,
      signature: req.body.signature,
      nonce: accountNonce,
      network: process.env.CHAIN,
      user,
    };
    context.bindings.timestampDatabase = JSON.stringify(value);

    context.log('Hash: ',`0x${tx.hash().toString('hex')}`);
    return new Promise((resolve, reject) => {
      let timestampTx;
      web3js.eth.sendSignedTransaction(`0x${serializedTx.toString('hex')}`, (error, txHash) => {
        if (error) {
          context.log(error);
          if (error.message.includes('same nonce')) {
            return resolve({
              body: 'UPDATE_NONCE',
            });
          }
          timestampTx = {
            status: 400,
            body: 'Could not verify signature',
          };
          return resolve(timestampTx);
        }

        context.log(txHash);
        timestampTx = {
          body: {
            success: true,
            value,
          },
        };
        return resolve(timestampTx);
      });
    });
  }
  let result = await sendTransaction();
  
  while (result.body === 'UPDATE_NONCE') {

    localNonce += 1;
    result = await sendTransaction(localNonce);
  }
  return result;
};
