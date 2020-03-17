import Vue from 'vue';
import * as Msal from 'msal';
import { sendEmail } from '../util';
import config from '../i18n/en-gb';


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
    if (!account || account.idToken.tfp !== 'B2C_1_TimestampSignUpSignIn') {
      return null;
    }
    return account;
  },

  membership() {
    return (this.account()).idToken.extension_membershipTier || 'free';
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
    msalConfig.auth.authority = 'https://timestamper.b2clogin.com/timestamper.onmicrosoft.com/B2C_1_PasswordReset';
    const passwordReset = new Msal.UserAgentApplication(msalConfig);
    passwordReset.handleRedirectCallback((error, response) => {
      console.log(error, response);
      if (response) {
        this.logout();
      }
    });
    passwordReset.loginRedirect();
    msalConfig.auth.authority = 'https://timestamper.b2clogin.com/timestamper.onmicrosoft.com/B2C_1_TimestampSignUpSignIn';
  },

  editProfile() {
    msalConfig.auth.authority = 'https://timestamper.b2clogin.com/timestamper.onmicrosoft.com/B2C_1_EditProfile';
    const editProfile = new Msal.UserAgentApplication(msalConfig);
    editProfile.handleRedirectCallback((error, response) => {
      console.log(error, response);
    });
    editProfile.loginRedirect();
  },
};

function authCallback(error, response) {
  try {
    if (error) {
      console.log(error);
      if (error.message && error.message.indexOf('AADB2C90118') > -1) {
        auth.forgotPassword();
      }
    } else if (response.account.idToken.tfp !== 'B2C_1_TimestampSignUpSignIn') {
      msalConfig.auth.authority = 'https://timestamper.b2clogin.com/timestamper.onmicrosoft.com/B2C_1_TimestampSignUpSignIn';
      auth.signIn();
    }

    console.log(response);

    if (response.account.idToken.newUser) {
      const content = {
        templateId: config.welcomeEmailTemplate,
        detail: {
          to_name: response.account.idToken.given_name.concat(` ${response.account.idToken.family_name}`),
          to_email: response.account.idToken.emails[0],
        },
      };
      sendEmail(content);
    }
  } catch (err) {
    console.log(err);
  }
}

myMSALObj.handleRedirectCallback(authCallback);


Vue.prototype.$auth = auth;

export default auth;
