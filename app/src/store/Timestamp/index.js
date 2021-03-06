/* eslint-disable import/no-cycle */
import { Model } from '@vuex-orm/core';
import User from '../User';

export default class Timestamp extends Model {
  static entity = 'timestamps';

  static primaryKey = 'id';

  static fields() {
    return {
      id: this.attr(''),
      txId: this.attr(''),
      hash: this.attr(''),
      signature: this.attr(''),
      pubkey: this.attr(''),
      status: this.attr(''),
      accountIdentifier: this.attr(''),
      name: this.attr(''),
      date: this.attr(''),
      type: this.attr(''),
      size: this.attr(''),
      blockNumber: this.attr(''),
    };
  }

  get timestampDate() {
    const stampDate = new Date(this.date ? this.date.split('Z')[0] : '');
    return `${stampDate.toLocaleDateString()} ${stampDate.toLocaleTimeString('en-GB')}`;
  }

  get certificate() {
    const splitString = (string, index) => ({
      one: string.substr(0, index),
      two: string.substr(index),
    });
    const user = User.find(this.accountIdentifier);
    const hash = splitString(this.hash.toLowerCase(), 65);
    const proofId = splitString(this.txId.toLowerCase(), 65);
    const signature = splitString(this.signature.toLowerCase(), 65);
    return {
      file: this.name,
      hash,
      proofId,
      signature,
      user: user.name,
      timestamp: this.timestampDate,
    };
  }
}
