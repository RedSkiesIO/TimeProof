import Vue from 'vue';
import * as Msal from 'msal';

const appConfig = {
  b2cScopes: ['https://fabrikamb2c.onmicrosoft.com/helloapi/demo.read'],
  webApi: 'https://fabrikamb2chello.azurewebsites.net/hello',
};

// configuration to initialize msal
const msalConfig = {
  auth: {
    clientId: 'e760cab2-b9a1-4c0d-86fb-ff7084abd902', // This is your client ID
    authority: 'https://fabrikamb2c.b2clogin.com/fabrikamb2c.onmicrosoft.com/b2c_1_susi', // This is your tenant info
    validateAuthority: false,
  },
  cache: {
    cacheLocation: 'localStorage',
    storeAuthStateInCookie: true,
  },
};
// instantiate MSAL
const myMSALObj = new Msal.UserAgentApplication(msalConfig);
// request to signin - returns an idToken
const loginRequest = {
  scopes: appConfig.b2cScopes,
};
// request to acquire a token for resource access
const tokenRequest = {
  scopes: appConfig.b2cScopes,
};
// signin and acquire a token silently with POPUP flow.
//  Fall back in case of failure with silent acquisition to popup
const auth = {
  signIn() {
    myMSALObj.loginPopup(loginRequest).then((loginResponse) => {
      console.log(loginResponse);
      this.getToken(tokenRequest).then(this.updateUI);
    }).catch((error) => {
      this.logMessage(error);
    });
  },
  // acquire a token silently
  getToken(tokenrequest) {
    return myMSALObj.acquireTokenSilent(tokenRequest).catch((error) => {
      console.log('aquire token popup', error);
      // fallback to interaction when silent call fails
      return myMSALObj.acquireTokenPopup(tokenrequest).then((tokenResponse) => {
        console.log(tokenResponse);
      }).catch((error2) => {
        this.logMessage('Failed token acquisition', error2);
      });
    });
  },
  // updates the UI post login/token acqusition
  updateUI() {
    const userName = myMSALObj.getAccount().name;
    console.log(myMSALObj.getAccount());
    this.logMessage(`User '${userName}' logged-in`);
    // add the logout button
    const authButton = document.getElementById('auth');
    authButton.innerHTML = 'logout';
    authButton.setAttribute('onclick', 'logout();');
    // greet the user - specifying login
    const label = document.getElementById('label');
    label.innerText = `Hello ${userName}`;
    // add the callWebApi button
    const callWebApiButton = document.getElementById('callApiButton');
    callWebApiButton.setAttribute('class', 'visible');
  },
  // calls the resource API with the token
  callApi() {
    this.getToken(tokenRequest).then((tokenResponse) => {
      console.log(tokenResponse);
      // this.callApiWithAccessToken(tokenResponse.accessToken);
    });
  },
  // helper function to access the resource with the token
  // callApiWithAccessToken(accessToken) {
  //   // Call the Web API with the AccessToken
  //   $.ajax({
  //     type: 'GET',
  //     url: appConfig.webApi,
  //     headers: {
  //       Authorization: `Bearer ${accessToken}`,
  //     },
  //   }).done((data) => {
  //     this.logMessage(`Web APi returned:\n${JSON.stringify(data)}`);
  //   })
  //     .fail((jqXHR, textStatus) => {
  //       this.logMessage(`Error calling the Web api:\n${textStatus}`);
  //     });
  // },
  // signout the user
  logout() {
    // Removes all sessions, need to call AAD endpoint to do full logout
    myMSALObj.logout();
  },
  // debug helper
  logMessage(s) {
    console.log('LOG: ', s);
    // document.body.querySelector('.response').appendChild(document.createTextNode(`\n${s}`));
  },

};

Vue.prototype.$auth = auth;
