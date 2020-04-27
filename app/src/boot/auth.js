import Vue from 'vue';
import * as Msal from 'msal';
import User from '../store/User';
import authConfig from '../../auth.config';

const appConfig = {
  b2cScopes: [authConfig.B2C_SCOPES],
  webApi: '',
};

// configuration to initialize msal
const msalConfig = {
  auth: {
    clientId: authConfig.CLIENT_ID,
    authority: authConfig.AUTHORITY_SIGNUP_SIGNIN,
    validateAuthority: false,
    redirectURI: authConfig.REDIRECT_URI,
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
    if (!account || account.idToken.tfp !== authConfig.B2C_1_SIGNUP_SIGNIN) {
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
    msalConfig.auth.authority = authConfig.AUTHORITY_FORGOT_PASSWORD;
    const passwordReset = new Msal.UserAgentApplication(msalConfig);
    passwordReset.handleRedirectCallback((error, response) => {
      console.log(error, response);
      if (response) {
        this.logout();
      }
    });
    passwordReset.loginRedirect();
    msalConfig.auth.authority = authConfig.AUTHORITY_SIGNUP_SIGNIN;
  },

  editProfile() {
    msalConfig.auth.authority = authConfig.AUTHORITY_EDIT_PROFILE;
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
    if (error.message) {
      if (error.message.indexOf(authConfig.FORGOT_PASSWORD_ERROR_CODE) > -1) {
        auth.forgotPassword();
      } else if (error.message.indexOf(authConfig.CANCEL_BUTTON_ERROR_CODE) > -1) {
        msalConfig.auth.authority = authConfig.AUTHORITY_SIGNUP_SIGNIN;
        auth.signIn();
      }
    }
  } else if (response.account.idToken.tfp !== authConfig.B2C_1_SIGNUP_SIGNIN) {
    msalConfig.auth.authority = authConfig.AUTHORITY_SIGNUP_SIGNIN;
    auth.signIn();
  }

  console.log(response);
}

myMSALObj.handleRedirectCallback(authCallback);


Vue.prototype.$auth = auth;

export default auth;
