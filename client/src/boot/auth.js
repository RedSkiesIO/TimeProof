import Vue from 'vue';
import * as Msal from 'msal';
import axios from 'axios';

// configuration to initialize msal
const msalConfig = {
  auth: {
    clientId: '413664b7-41ea-4096-ae23-3abcb7f8c359', // This is your client ID
    authority: 'https://login.microsoftonline.com/e76987d2-a8b1-490c-89bb-08e77b63d40b', // This is your tenant info
    redirectURI: 'http://localhost:6420/',
  },
  cache: {
    cacheLocation: 'localStorage',
    storeAuthStateInCookie: true,
  },
};

function authCallback(error, response) {
  if (error) {
    console.log(error);
  }
  console.log(response);
  // handle redirect response
}

// instantiate MSAL
const myMSALObj = new Msal.UserAgentApplication(msalConfig);
myMSALObj.handleRedirectCallback(authCallback);
// request to signin - returns an idToken
const requestObj = {
  scopes: ['https://easyauthtest3.azurewebsites.net/.default'],
};

// signin and acquire a token silently with POPUP flow.
//  Fall back in case of failure with silent acquisition to popup
const auth = {
  signIn() {
    return myMSALObj.loginRedirect(requestObj);
  },

  account() {
    return myMSALObj.getAccount();
  },

  // acquire a token silently
  getToken() {
    return myMSALObj.acquireTokenSilent(requestObj).catch((error) => {
      console.log('aquire token popup', error);
      // fallback to interaction when silent call fails
      return myMSALObj.acquireTokenPopup(requestObj).then((tokenResponse) => {
        console.log('popup: ', tokenResponse);
        return tokenResponse;
      }).catch((error2) => {
        console.log('Failed token acquisition', error2);
      });
    });
  },

  // get token for api access
  async getSessionToken() {
    const token = await this.getToken();
    console.log(token);
    try {
      const session = await axios.post('https://easyauthtest3.azurewebsites.net/.auth/login/microsoftaccount',
        {
          access_token: token.accessToken,
        });
      console.log(session);
    } catch (e) {
      console.log(e);
    }
  },

  logout() {
    // Removes all sessions, need to call AAD endpoint to do full logout
    myMSALObj.logout();
  },

};

Vue.prototype.$auth = auth;
