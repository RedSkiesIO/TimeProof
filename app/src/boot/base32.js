import base32 from 'base32-encode';

function encode(text) {
  return base32(text, 'RFC4648', { padding: false });
}

export default ({ Vue }) => {
  Vue.prototype.$base32 = encode;
};
