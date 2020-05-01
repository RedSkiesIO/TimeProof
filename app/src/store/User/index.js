/* eslint-disable import/no-cycle */
import { Model } from '@vuex-orm/core';
import Timestamp from '../Timestamp';
import Address from '../Address';

export default class User extends Model {
  static entity = 'users';

  static primaryKey = 'accountIdentifier';

  static fields() {
    return {
      accountIdentifier: this.attr(''),
      pubKey: this.attr(''),
      secretKey: this.attr(''),
      givenName: this.attr(''),
      familyName: this.attr(''),
      email: this.attr(''),
      tier: this.attr(''),
      timestamps: this.hasMany(Timestamp, 'accountIdentifier'),
      address: this.hasOne(Address, 'accountIdentifier'),
      tokenExpires: this.attr(''),
      upgrade: this.attr(false),
      firstTimeDialog: this.attr(true),
      membershipRenewDate: this.attr(''),
      remainingTimeStamps: this.attr(''),
      selectedCardNumber: this.attr(''),
      cardExpirationDate: this.attr(''),
      userId: this.attr(''),
      clientSecret: this.attr(''),
      paymentIntentId: this.attr(''),
    };
  }

  get pendingTimestamps() {
    return this.timestamps.filter(({ status }) => status === 0);
  }

  get name() {
    return `${this.givenName} ${this.familyName}`;
  }

  get totalTimestamps() {
    return this.timestamps.length;
  }

  get orderedTimestamps() {
    const ts = this.timestamps.slice(0);
    ts.sort((a, b) => new Date(b.date) - new Date(a.date));
    return ts.slice(0);
  }
}
