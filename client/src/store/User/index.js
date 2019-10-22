import { Model } from '@vuex-orm/core';

export default class User extends Model {
  static entity = 'users';

  static fields() {
    return {
      id: this.increment(),
      pubKey: this.attr(''),
      secretKey: this.attr(''),
    };
  }
}
