const AuthenticationContext = require('adal-node').AuthenticationContext;

const authorityHostUrl = 'https://login.windows.net';
const tenant = 'myTenant.onmicrosoft.com'; // AAD Tenant name.
const authorityUrl = authorityHostUrl + '/' + tenant;
const applicationId = 'yourApplicationIdHere'; // Application Id of app registered under AAD.
const clientSecret = 'yourAADIssuedClientSecretHere'; // Secret generated for app. Read this environment variable.
const resource = '00000002-0000-0000-c000-000000000000'; // URI that identifies the resource for which the token is valid.

const context = new AuthenticationContext(authorityUrl);

context.acquireTokenWithClientCredentials(resource, applicationId, clientSecret, function(err, tokenResponse) {
  if (err) {
    console.log('well that didn\'t work: ' + err.stack);
  } else {
    console.log(tokenResponse);
  }
});