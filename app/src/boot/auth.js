import Vue from 'vue';
import * as Msal from 'msal';
import User from '../store/User';

const appConfig = {
  b2cScopes: ['https://timeproof.onmicrosoft.com/api/read'],
  webApi: '',
};

// configuration to initialize msal
const msalConfig = {
  auth: {
    clientId: 'caead9d0-3263-42b9-b25e-2ca36d0ff535', // This is your client ID
    authority: 'https://timeproof.b2clogin.com/timeproof.onmicrosoft.com/B2C_1_SignUpSignIn', // This is your tenant info
    validateAuthority: false,
    redirectURI: 'http://localhost:6420/',
  },
  cache: {
    cacheLocation: 'localStorage',
    storeAuthStateInCookie: true,
  },
};


const loginRequest = {
  scopes: appConfig.b2cScopes,
};
const tokenRequest = {
  scopes: appConfig.b2cScopes,
};


// instantiate MSAL
const myMSALObj = new Msal.UserAgentApplication(msalConfig);

const auth = {
  signIn() {
    return myMSALObj.loginRedirect(loginRequest);
  },

  account() {
    const account = myMSALObj.getAccount();
    if (!account || account.idToken.tfp !== 'B2C_1_SignUpSignIn') {
      return null;
    }
    return account;
  },

  user(withAll, withDependency, specifiedDependency) {
    let user;
    const account = this.account();
    if (account) {
      if (withAll) {
        user = User.query().whereId(account.accountIdentifier).withAll().get();
      } else if (withDependency) {
        user = User.query().whereId(account.accountIdentifier).with(specifiedDependency).get();
      } else {
        user = User.query().whereId(account.accountIdentifier).get();
      }

      if (user) {
        return user[0];
      }
    }

    return null;
  },

  membership() {
    return (this.account()).idToken.extension_membershipTier || this.user().tier;
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

  getTokenRedirect() {
    return myMSALObj.acquireTokenRedirect(tokenRequest).catch((error) => {
      console.log(error);
    });
  },

  logout() {
    myMSALObj.logout();
  },

  forgotPassword() {
    msalConfig.auth.authority = 'https://timeproof.b2clogin.com/timeproof.onmicrosoft.com/B2C_1_PasswordReset';
    const passwordReset = new Msal.UserAgentApplication(msalConfig);
    passwordReset.handleRedirectCallback((error, response) => {
      console.log(error, response);
      if (response) {
        this.logout();
      }
    });
    passwordReset.loginRedirect();
    msalConfig.auth.authority = 'https://timeproof.b2clogin.com/timeproof.onmicrosoft.com/B2C_1_SignUpSignIn';
  },

  editProfile() {
    msalConfig.auth.authority = 'https://timeproof.b2clogin.com/timeproof.onmicrosoft.com/B2C_1_EditProfile';
    const editProfile = new Msal.UserAgentApplication(msalConfig);
    editProfile.handleRedirectCallback((error, response) => {
      console.log(error, response);
    });
    editProfile.loginRedirect();
  },
};

function authCallback(error, response) {
  if (error) {
    console.log(error);
    if (error.message && error.message.indexOf('AADB2C90118') > -1) {
      auth.forgotPassword();
    }
  } else if (response.account.idToken.tfp !== 'B2C_1_SignUpSignIn') {
    msalConfig.auth.authority = 'https://timeproof.b2clogin.com/timeproof.onmicrosoft.com/B2C_1_SignUpSignIn';
    auth.signIn();
  }

  console.log(response);
}

myMSALObj.handleRedirectCallback(authCallback);


Vue.prototype.$auth = auth;

export default auth;
