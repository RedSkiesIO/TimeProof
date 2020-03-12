import Vue from 'vue';
import tweetnacl from 'tweetnacl';
import encode from 'base32-encode';
import decode from 'base32-decode';


const keypair = {
  new() {
    const key = tweetnacl.sign.keyPair();

    return {
      publicKey: encode(key.publicKey, 'RFC4648', { padding: false }),
      secretKey: encode(key.secretKey, 'RFC4648', { padding: false }),
    };
  },

  newFromSeed(seed) {
    const key = tweetnacl.sign.keyPair.fromSeed(seed);
    return {
      publicKey: encode(key.publicKey, 'RFC4648', { padding: false }),
      secretKey: encode(key.secretKey, 'RFC4648', { padding: false }),
    };
  },

  keypairFromSecretKey(secretKey) {
    const key = tweetnacl.sign.keyPair.fromSecretKey(new Uint8Array(decode(secretKey, 'RFC4648')));
    return {
      publicKey: encode(key.publicKey, 'RFC4648', { padding: false }),
      secretKey: encode(key.secretKey, 'RFC4648', { padding: false }),
    };
  },

  signMessage(message, secretKey) {
    return tweetnacl.sign.detached(message, new Uint8Array(decode(secretKey, 'RFC4648')));
  },

  verifyMessage(signedMessage, publicKey) {
    return tweetnacl.sign.detached.verify(signedMessage, new Uint8Array(decode(publicKey, 'RFC4648')));
  },

};

Vue.prototype.$keypair = keypair;
