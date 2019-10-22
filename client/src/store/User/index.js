import { Model } from '@vuex-orm/core';

export default class User extends Model {
  static entity = 'users';

  static primaryKey = 'pubKey';

  static fields() {
    return {
      pubKey: this.attr(''),
      secretKey: this.attr(''),
    };
  }
}
