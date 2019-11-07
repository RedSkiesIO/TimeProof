import Vue from 'vue';
import * as Msal from 'msal';
// import axios from 'axios';

const appConfig = {
  b2cScopes: ['https://timestamper.onmicrosoft.com/api/demo.read'],
  webApi: '',
};

// configuration to initialize msal
const msalConfig = {
  auth: {
    clientId: 'e4ab5c13-6a76-4784-a0f3-da7d50135fa3', // This is your client ID
    authority: 'https://timestamper.b2clogin.com/timestamper.onmicrosoft.com/B2C_1_TimestampSignUpSignIn', // This is your tenant info
    validateAuthority: false,
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

const loginRequest = {
  scopes: appConfig.b2cScopes,
};
// request to acquire a token for resource access
const tokenRequest = {
  scopes: appConfig.b2cScopes,
};

// instantiate MSAL
const myMSALObj = new Msal.UserAgentApplication(msalConfig);
myMSALObj.handleRedirectCallback(authCallback);

const auth = {
  signIn() {
    return myMSALObj.loginRedirect(loginRequest);
  },

  account() {
    return myMSALObj.getAccount();
  },

  // acquire a token silently
  getToken() {
    return myMSALObj.acquireTokenSilent(tokenRequest).catch((error) => {
      console.log('aquire token popup', error);
      // fallback to interaction when silent call fails
      return myMSALObj.acquireTokenPopup(tokenRequest).then((tokenResponse) => {
        console.log('popup: ', tokenResponse);
        return tokenResponse;
      }).catch((error2) => {
        console.log('Failed token acquisition', error2);
      });
    });
  },

  logout() {
    // Removes all sessions, need to call AAD endpoint to do full logout
    myMSALObj.logout();
  },

};

Vue.prototype.$auth = auth;
