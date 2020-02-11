const jwtDecode = require('jwt-decode');

module.exports = function jwt(accessToken) {
  const token = jwtDecode(accessToken.replace('Bearer ', ''));
  return token;
};
