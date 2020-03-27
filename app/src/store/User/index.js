/* eslint-disable import/no-cycle */
import { Model } from '@vuex-orm/core';
import axios from 'axios';
// import auth from '../../boot/auth';
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
      tier: this.attr('free'),
      timestamps: this.hasMany(Timestamp, 'accountIdentifier'),
      address: this.hasOne(Address, 'accountIdentifier'),
      tokenExpires: this.attr(''),
      upgrade: this.attr(false),
      firstTimeDialog: this.attr(true),
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

  // static account() {
  //   const account = auth.account();
  //   if (!account || account.idToken.tfp !== 'B2C_1_SignUpSignIn') {
  //     return null;
  //   }
  //   return account;
  // }

  // static membership() {
  //   return (User.account()).idToken.extension_membershipTier || 'free';
  // }

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

  async fetchTimestamps() {
    const re = /(?:\.([^.]+))?$/;

    const { timestamps } = (await axios.get(`${process.env.API}/getTimestamps/${this.accountIdentifier}`)).data;
    const stamps = timestamps.map(file => ({
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

  async verifyUserDetails() {
    return axios.get(`${process.env.API}/user/${this.email}`)
      .then(async (result) => {
        if (!result || !result.data) {
          console.log('DATATATTATATTATATA');
          const data = {
            email: this.email,
            firstName: this.givenName,
            lastName: this.familyName,
            address: {
              line1: this.address.line1,
              line2: this.address.line2,
              city: this.address.city,
              state: this.address.state,
              postcode: this.address.postcode,
              country: this.address.country,
            },
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
