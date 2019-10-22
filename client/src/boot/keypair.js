import Vue from 'vue';
import tweetnacl from 'tweetnacl';
import {
  encodeBase64,
  decodeBase64,
} from 'tweetnacl-util';

const keypair = {
  new() {
    const key = tweetnacl.sign.keyPair();
    return {
      publicKey: encodeBase64(key.publicKey),
      secretKey: encodeBase64(key.secretKey),
    };
  },

  newFromSeed(seed) {
    const key = tweetnacl.sign.keyPair.fromSeed(seed);
    return {
      publicKey: encodeBase64(key.publicKey),
      secretKey: encodeBase64(key.secretKey),
    };
  },

  signMessage(message, secretKey) {
    return tweetnacl.sign.detached(message, decodeBase64(secretKey));
  },

  verifyMessage(signedMessage, publicKey) {
    return tweetnacl.sign.detached.verify(signedMessage, decodeBase64(publicKey));
  },

};

Vue.prototype.$keypair = keypair;
