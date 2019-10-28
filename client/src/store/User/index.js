import { Model } from '@vuex-orm/core';

export default class User extends Model {
  static entity = 'users';

  static primaryKey = 'email';

  static fields() {
    return {
      pubKey: this.attr(''),
      secretKey: this.attr(''),
      name: this.attr('Satoshi Nakamoto'),
      email: this.attr('user@email.com'),
    };
  }
}
