/* eslint-disable import/no-cycle */
import { Model } from '@vuex-orm/core';
import axios from 'axios';
import moment from 'moment';
import Timestamp from '../Timestamp';
import Address from '../Address';
import Tier from '../../util/tier';

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
      tier: this.attr(Tier.Free),
      timestamps: this.hasMany(Timestamp, 'accountIdentifier'),
      address: this.hasOne(Address, 'accountIdentifier'),
      tokenExpires: this.attr(''),
      upgrade: this.attr(false),
      firstTimeDialog: this.attr(true),
      subscriptionStart: this.attr(moment().toISOString()),
      subscriptionEnd: this.attr(moment().add(1, 'months').toISOString()),
      selectedCardNumber: this.attr(''),
      cardExpirationDate: this.attr(''),
      userId: this.attr(''),
      clientSecret: this.attr(''),
      customerId: this.attr(''),
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
    const timestamps = this.timestamps.filter(stamp => moment(new Date(stamp.date))
      .isBetween(moment(this.subscriptionStart),
        moment(this.subscriptionEnd), null, '[]'));
    return timestamps.length;
  }

  get orderedTimestamps() {
    const ts = this.timestamps.slice(0);
    ts.sort((a, b) => new Date(b.date) - new Date(a.date));
    return ts.slice(0);
  }

  async fetchTimestamps() {
    const re = /(?:\.([^.]+))?$/;

    const { data, status } = await axios.get(`${process.env.API}/getTimestamps/${this.accountIdentifier}`);
    if (status === 200 && data) {
      const stamps = data.map(file => ({
        txId: file.id,
        hash: file.fileHash,
        signature: file.signature,
        pubKey: file.publicKey.toLowerCase(),
        accountIdentifier: this.accountIdentifier,
        name: file.fileName,
        date: Number(file.timestamp),
        type: re.exec(file.fileName)[1],
        blockNumber: Number(file.blockNumber),
      }));

      await Timestamp.create({
        data: stamps,
      });
    }
  }

  async verifyUserDetails() {
    return axios.get(`${process.env.API}/user/${this.email}`)
      .then(async (result) => {
        if (!result || !result.data) {
          console.log('DATATATTATATTATATA');

          let address;
          if (this.address) {
            address = {
              line1: this.address.line1,
              line2: this.address.line2,
              city: this.address.city,
              state: this.address.state,
              postcode: this.address.postcode,
              country: this.address.country,
            };
          }

          const data = {
            email: this.email,
            firstName: this.givenName,
            lastName: this.familyName,
            ...address,
          };
          console.log(data);
          const newUserResult = await axios.post(`${process.env.API}/user`, data);
          console.log('New User created on Timeproof API');
          console.log(newUserResult);
          return newUserResult;
        }
        console.log('User is already exist in timeproof api');
        console.log(result.data);
        return result;
      })
      .then(async (userResult) => {
        if (userResult && userResult.data) {
          const intentResult = await axios.get(`${process.env.API}/user/intent/${userResult.data.id}`);
          console.log('USER INTENT RESULT');
          console.log(intentResult);
          if (intentResult && intentResult.status === 200 && intentResult.data) {
            intentResult.data.userId = userResult.data.id;
            return intentResult;
          }
        }
        return {
          error: 'client secret key not found',
        };
      })
      .catch((err) => {
        console.log(err);
      });
  }
}
