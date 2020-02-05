const nacl = require('tweetnacl');
const decode = require('base32-decode');

module.exports = function verifyProof(proof) {
  if (proof.hash && proof.signature && proof.publicKey) {
    const h = new Uint8Array(decode(proof.hash.toUpperCase(), 'RFC4648'));
    const sig = new Uint8Array(decode(proof.signature.toUpperCase(), 'RFC4648'));
    const pub = new Uint8Array(decode(proof.publicKey.toUpperCase(), 'RFC4648'));

    const verify = nacl.sign.detached.verify(h, sig, pub);

    return verify ? JSON.stringify({
      file: proof.fileName,
      hash: proof.hash,
      publicKey: proof.publicKey,
      signature: proof.signature,
    }) : false;
  }

  return null;
};
