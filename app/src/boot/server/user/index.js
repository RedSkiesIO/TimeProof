/* eslint-disable class-methods-use-this */
import axios from 'axios';
import Vue from 'vue';
import Timestamp from '../../../store/Timestamp';
import auth from '../../auth';
import User from '../../../store/User';

class UserServer {
  async fetchTimestamps(accountIdentifier) {
    const re = /(?:\.([^.]+))?$/;

    const { data, status } = await axios.get(`${process.env.API}/getTimestamps/${accountIdentifier}`);
    if (status === 200 && data) {
      const stamps = data.map(file => ({
        id: file.id,
        txId: file.transactionId,
        status: file.status,
        hash: file.fileHash,
        signature: file.signature,
        pubKey: file.publicKey.toLowerCase(),
        accountIdentifier,
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
    const user = auth.user(false, true, 'address');
    const account = auth.account();

    if (account) {
      return axios.get(`${process.env.API}/user?email=${encodeURIComponent(account.idToken.emails[0])}`)
        .then(async (result) => {
          if (!result || !result.data) {
            console.log('DATATATTATATTATATA');

            let address;
            let data;
            if (user) {
              if (user.address) {
                address = {
                  line1: user.address.line1,
                  line2: user.address.line2,
                  city: user.address.city,
                  state: user.address.state,
                  postcode: user.address.postcode,
                  country: user.address.country,
                };
              }

              data = {
                email: user.email,
                firstName: user.givenName,
                lastName: user.familyName,
                address,
              };
            } else {
              data = {
                email: account.idToken.emails[0],
                firstName: account.idToken.given_name,
                lastName: account.idToken.family_name,
              };
            }
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
            return userResult.data;
          }

          throw new Error('user data not found');
        })
        .catch((err) => {
          console.log(err);
        });
    }
    return Promise.resolve();
  }

  async getSetupIntent(userId) {
    let setupIntentResult;
    console.log('BEFORE GET SETUP INTENT');
    console.log({
      userId,
    });
    try {
      setupIntentResult = await axios.get(`${process.env.API}/user/setupintent/${userId}`);
      console.log('AFTER GET SETUP INTENT');
      console.log(setupIntentResult);
    } catch (err) {
      console.log('AFTER GET SETUP INTENT ERROR');
      console.error(err);
    }

    return setupIntentResult;
  }

  async refreshCard(user) {
    try {
      const { data, status } = await axios.get(`${process.env.API}/user/paymentmethod/${user.userId}`);
      console.log('PAYMENT METHOD RESULT');
      console.log(data);
      if (status === 200 && data && data.card) {
        User.update({
          data: {
            accountIdentifier: user.accountIdentifier,
            selectedCardNumber: `**** **** **** ${data.card.last4}`,
            cardExpirationDate: `${data.card.expMonth} / ${data.card.expYear}`,
          },
        });
      }
    } catch (err) {
      console.log('PAYMENT METHOD RESULT ERROR');
      console.log(err);
    }
  }

  async saveCard(user, stripe, card) {
    const response = {};
    console.log('BEFORE SAVE THE CARD');

    try {
      const { data: setupIntentData, status, error } = await this.getSetupIntent(user.userId);

      if (setupIntentData && status === 200 && !error) {
        const { setupIntent: confirmSetupIntent, error: confirmError } = await
        stripe.confirmCardSetup(setupIntentData.clientSecret,
          {
            payment_method: {
              card,
              billing_details: {
                name: user.name,
              },
            },
          });

        console.log('IN SAVE THE CARD');
        console.log(confirmSetupIntent);

        if (confirmSetupIntent && confirmSetupIntent.status === 'succeeded' && !confirmError) {
          response.status = confirmSetupIntent.status;
        } else if (confirmError) {
          response.eror = confirmError;
        } else {
          response.eror.message = 'Unexpected error';
        }
      } else if (error) {
        response.eror = error;
      } else {
        response.eror.message = 'Unexpected error';
      }
      console.log('AFTER SAVE THE CARD');
      console.log(response);
    } catch (err) {
      console.log('AFTER SAVE THE CARD ERROR');
      console.log(err);
      response.eror.message = 'Unexpected error';
    }

    return response;
  }
}

const userServer = new UserServer();

Vue.prototype.$userServer = userServer;

export default userServer;
