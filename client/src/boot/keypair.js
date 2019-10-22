import Vue from 'vue';
import tweetnacl from 'tweetnacl';

const keypair = {
  new() {
    return tweetnacl.sign.keyPair();
  },

  newFromSeed(seed) {
    return tweetnacl.sign.keyPair.fromSeed(seed);
  },

  signMessage(message, secretKey) {
    return tweetnacl.sign(message, secretKey);
  },

  verifyMessage(signedMessage, publicKey) {
    return tweetnacl.sign.open(signedMessage, publicKey);
  },

};

Vue.prototype.$keypair = keypair;
