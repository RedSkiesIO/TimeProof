import AuthenticationContext from 'adal-angular';
import { getAdalConfig } from './config';

const logErrorMessage = (msg) => {
  // TODO send to AppInsights
  console.log(msg);
};

const authContext = new AuthenticationContext(getAdalConfig());

const getUser = () => authContext.getCachedUser();

export const isUserLoggedIn = () => (!!getUser());
export const userName = () => getUser().userName;
export const handleUserLogin = () => authContext.login();
export const handleUserLogout = () => authContext.logOut();
export const handleAdalCallback = () => {
  const isCallback = authContext.isCallback(window.location.hash);
  authContext.handleWindowCallback();

  if (isCallback && !authContext.getLoginError()) {
    // eslint-disable-next-line no-underscore-dangle
    window.location = authContext._getItem(authContext.CONSTANTS.STORAGE.LOGIN_REQUEST);
  }
};
export const handleAuthBearerRequest = (callback) => {
  authContext.acquireToken(authContext.config.apiId, (error, token) => {
    if (error || !token) {
      logErrorMessage(`error occurred: ${error}`);
    }

    callback(error, token);
  });
};
