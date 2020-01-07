import { Model } from '@vuex-orm/core';

export default class Timestamp extends Model {
  static entity = 'timestamps';

  static primaryKey = 'txId';

  static fields() {
    return {
      txId: this.attr(''),
      hash: this.attr(''),
      signature: this.attr(''),
      pubkey: this.attr(''),
      accountIdentifier: this.attr(''),
      name: this.attr(''),
      date: this.attr(''),
      type: this.attr(''),
      size: this.attr(''),
      blockNumber: this.attr(''),
    };
  }
}
