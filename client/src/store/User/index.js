import { Model } from '@vuex-orm/core';

export default class User extends Model {
  static entity = 'users';

  static primaryKey = 'accountIdentifier';

  static fields() {
    return {
      accountIdentifier: this.attr(''),
      pubKey: this.attr(''),
      secretKey: this.attr(''),
      name: this.attr('Satoshi Nakamoto'),
      email: this.attr('user@email.com'),
      tier: this.attr('free'),
      timestampsUsed: this.attr(0),
    };
  }
}
