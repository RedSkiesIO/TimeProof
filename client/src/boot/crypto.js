import Vue from 'vue';

const mode = 'AES-GCM';
const length = 256;
const ivLength = 12;

const crypto = {

  async genEncryptionKey(password) {
    const algo = {
      name: 'PBKDF2',
      hash: 'SHA-256',
      salt: new TextEncoder().encode('a-unique-salt'),
      iterations: 1000,
    };
    const derived = { name: mode, length };
    const encoded = new TextEncoder().encode(password);
    const key = await window.crypto.subtle.importKey('raw', encoded, { name: 'PBKDF2' }, false, ['deriveKey']);

    return window.crypto.subtle.deriveKey(algo, key, derived, false, ['encrypt', 'decrypt']);
  },

  async encrypt(text, password) {
    const algo = {
      name: mode,
      length,
      iv: window.crypto.getRandomValues(new Uint8Array(ivLength)),
    };
    console.log(algo.iv);
    const key = await this.genEncryptionKey(password, mode, length);
    const encoded = new TextEncoder().encode(text);

    return {
      cipherText: await window.crypto.subtle.encrypt(algo, key, encoded),
      iv: algo.iv,
    };
  },

  // encrypt('Secret text', 'password', 'AES-GCM', 256, 12).then((encrypted) => {
  //   console.log(encrypted); // { cipherText: ArrayBuffer, iv: Uint8Array }
  // });

  async decrypt(encrypted, password) {
    const algo = {
      name: mode,
      length,
      iv: encrypted.iv,
    };
    const key = await this.genEncryptionKey(password, mode, length);
    const decrypted = await window.crypto.subtle.decrypt(algo, key, encrypted.cipherText);

    return new TextDecoder().decode(decrypted);
  },

  //   (async () => {
  //     var mode = 'AES-GCM',
  //     length = 256,
  //     ivLength = 12;

  //     var encrypted = await encrypt('Secret text', 'password', mode, length, ivLength);
  //     console.log(encrypted); // { cipherText: ArrayBuffer, iv: Uint8Array }

  //     var decrypted = await decrypt(encrypted, 'password', mode, length);
  //     console.log(decrypted); // Secret text
  //     })();

};

Vue.prototype.$crypto = crypto;
