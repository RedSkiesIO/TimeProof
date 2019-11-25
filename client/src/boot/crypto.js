import Vue from 'vue';
import encode from 'base32-encode';
import decode from 'base32-decode';

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
    const key = await this.genEncryptionKey(password, mode, length);
    const encoded = new TextEncoder().encode(text);
    return {
      cipherText: encode(await window.crypto.subtle.encrypt(algo, key, encoded), 'RFC4648', { padding: false }),
      iv: encode(algo.iv, 'RFC4648', { padding: false }),
    };
  },

  async decrypt(encrypted, password) {
    try {
      const algo = {
        name: mode,
        length,
        iv: new Uint8Array(decode(encrypted.iv, 'RFC4648')),
      };

      const key = await this.genEncryptionKey(password, mode, length);
      const decrypted = await window.crypto.subtle.decrypt(algo, key, decode(encrypted.cipherText, 'RFC4648'));
      return new TextDecoder().decode(decrypted);
    } catch (e) {
      console.log(e);
      return null;
    }
  },

  async createKeystore(user) {
    const obj = {
      publicKey: user.pubKey,
      cipher: user.secretKey,
    };
    this.downloadObjectAsJson(obj, 'test');
  },

  downloadObjectAsJson(exportObj, exportName) {
    const dataStr = `data:text/json;charset=utf-8,${encodeURIComponent(JSON.stringify(exportObj))}`;
    const downloadAnchorNode = document.createElement('a');
    downloadAnchorNode.setAttribute('href', dataStr);
    downloadAnchorNode.setAttribute('download', `${exportName}.json`);
    document.body.appendChild(downloadAnchorNode); // required for firefox
    downloadAnchorNode.click();
    downloadAnchorNode.remove();
  },
};

Vue.prototype.$crypto = crypto;
