import axios from 'axios';
import auth from '../auth';

let axiosInterceptor;
export default class Server {
  constructor() {
    this.axios = axios;
    this.auth = auth;
    // remove interceptor if exists
    // I found axiosInterceptor starts with 0, then +1.
    if (!!axiosInterceptor || axiosInterceptor === 0) {
      axios.interceptors.request.eject(axiosInterceptor);
    }
    // use for remove an interceptor later
    axiosInterceptor = axios.interceptors.request.use(
      async (request) => {
        await this.setToken(request);
        return request;
      },
      error => Promise.reject(error),
    );
  }

  setToken = async (request) => {
    const token = await this.auth.getToken();
    // This fails if MSAL requested a new token
    request.headers.common.Authorization = `Bearer ${token.idToken.rawIdToken}`;
  };

  getUser() {
    return this.auth.user(false, true, 'address');
  }

  getAccount() {
    return this.auth.account();
  }
}
