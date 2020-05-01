import axios from 'axios';
import auth from '../auth';

export default class Server {
  constructor() {
    this.axios = axios;
    this.auth = auth;
  }

  getUser() {
    return this.auth.user(false, true, 'address');
  }

  getAccount() {
    return this.auth.account();
  }

  async updateAxiosToken() {
    const token = await this.auth.getToken();
    if (token && !token.fromCache) {
      console.log('TOKENXXX');
      console.log(token);
      this.axios.defaults.headers.common.Authorization = `Bearer ${token.idToken.rawIdToken}`;
    }
  }

  async axiosGet(url) {
    await this.updateAxiosToken();
    return this.axios.get(url);
  }

  async axiosPost(url, data) {
    await this.updateAxiosToken();
    return this.axios.post(url, data);
  }

  async axiosPut(url) {
    await this.updateAxiosToken();
    return this.axios.put(url);
  }
}
