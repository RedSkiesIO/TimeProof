const { AuthenticationContext } = require('adal-node');
const axios = require('axios');

const authorityHostUrl = 'https://login.microsoftonline.com/';
const tenant = 'timestamper.onmicrosoft.com'; // AAD Tenant name.
const authorityUrl = authorityHostUrl + tenant;
const applicationId = process.env.AZURE_AD_APP_ID; // Application Id of app registered under AAD.
const clientSecret = process.env.AZURE_AD_SECRET; // Secret generated for app. Read this environment variable.
const resource = 'https://graph.windows.net'; // URI that identifies the resource for which the token is valid.

const context = new AuthenticationContext(authorityUrl);

const auth = {

  async getToken() {
    return new Promise((res, rej) => {
      context.acquireTokenWithClientCredentials(resource, applicationId, clientSecret, async (err, tokenResponse) => {
        if (err) {
          rej(`well that didn't work: ${err.stack}`);
        } else {
          res(tokenResponse);
        }
      });
    });
  },

  async updateMembership(userId, tier) {
    const { accessToken } = await this.getToken();
    axios.defaults.headers.common.Authorization = `Bearer ${accessToken}`;
    await axios.patch(`https://graph.windows.net/timestamper.onmicrosoft.com/users/${userId}?api-version=1.6`, {
      extension_f04cd6b40d034d308e9beca3341bd09d_membershipTier: tier,
    }).catch((e) => console.error('Error: ', e.message));
  },

  async getUser(userId) {
    const { accessToken } = await this.getToken();
    axios.defaults.headers.common.Authorization = `Bearer ${accessToken}`;
    return axios.get(`https://graph.windows.net/timestamper.onmicrosoft.com/users/${userId}?api-version=1.6`)
      .catch((e) => {
        console.log(e.message);
      });
  },
};

module.exports = auth;