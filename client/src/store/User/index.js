/* eslint-disable import/no-cycle */
import { Model } from '@vuex-orm/core';
import Timestamp from '../Timestamp';

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
      tier: this.attr('free'),
      timestamps: this.hasMany(Timestamp, 'accountIdentifier'),
      tokenExpires: this.attr(''),
    };
  }

  get pendingTimestamps() {
    return this.timestamps.filter(({ blockNumber }) => blockNumber === -1);
  }

  get name() {
    return `${this.givenName} ${this.familyName}`;
  }

  get totalTimestamps() {
    return this.timestamps.length;
  }

  get monthlyAllowanceUsage() {
    const d = new Date();
    const thisMonth = `${d.getMonth()}${d.getFullYear()}`;
    const timestamps = this.timestamps.filter((stamp) => {
      const date = new Date(stamp.date);
      const month = `${date.getMonth()}${date.getFullYear()}`;
      return month === thisMonth;
    });
    return timestamps.length;
  }

  get orderedTimestamps() {
    const ts = this.timestamps.slice(0);
    ts.sort((a, b) => new Date(b.date) - new Date(a.date));
    return ts.slice(0);
  }
}
