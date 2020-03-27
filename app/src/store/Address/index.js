import { Model } from '@vuex-orm/core';

export default class Address extends Model {
  static entity = 'addresses'

  static primaryKey = 'addressId';

  static fields() {
    return {
      addressId: this.increment(),
      accountIdentifier: this.attr(''),
      line1: this.attr(''),
      line2: this.attr(''),
      city: this.attr(''),
      state: this.attr(''),
      postcode: this.attr(''),
      country: this.attr(''),
    };
  }
}
