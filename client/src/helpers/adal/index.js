const isBrowser = () => typeof window !== 'undefined';
const adal = !isBrowser() ? null : require('./adal');


export const auth = {
  login: adal ? adal.handleUserLogin : () => {},
  isLoggedIn: adal ? adal.isUserLoggedIn : () => false,
  getUserName: adal ? adal.userName : () => '',
  logout: adal ? adal.handleUserLogout : () => {},
  handleLoginCallback: adal ? adal.handleAdalCallback : () => {},
  acquireTokenForAPI: adal ? adal.handleAuthBearerRequest : () => {},
};
