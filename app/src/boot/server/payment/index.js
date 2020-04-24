/* eslint-disable prefer-destructuring */
/* eslint-disable class-methods-use-this */
import moment from 'moment';
import axios from 'axios';
import Vue from 'vue';
import User from '../../../store/User';
import Address from '../../../store/Address';
import auth from '../../auth';

class PaymentServer {
  constructor() {
    this.lineItems = [];
    this.products = {};
    this.productsFetchPromise = null;
  }

  // Retrieve the configuration from the API.
  async getConfig() {
    try {
      return null;
    // eslint-disable-next-line no-unreachable
    } catch (err) {
      return { error: err.message };
    }
  }

  async listAllPriceplans() {
    const { data, status } = await axios.get(`${process.env.API}/priceplans`);
    console.log('PRICE PLANSS');
    console.log(data);
    const productsData = {};
    const colorList = ['orange', 'green', 'blue'];
    if (status === 200 && data) {
      data.forEach((plan, index) => {
        productsData[plan.id] = plan;
        productsData[plan.id].color = colorList[index];
      });
    }
    return productsData;
  }

  async updatePaymentAddress(user, data) {
    let addressFound = false;

    if (user.accountIdentifier) {
      const addressResult = Address.query()
        .where('accountIdentifier', user.accountIdentifier).last();

      console.log('UPDATE PAYMENT ADDRESS');
      console.log(addressResult);

      let userResult;
      if (data) {
        userResult = await User.insertOrUpdate({
          data: {
            accountIdentifier: user.accountIdentifier,
            address: {
              addressId: user.accountIdentifier,
              accountIdentifier: user.accountIdentifier,
              line1: data.line,
              line2: '',
              city: data.city,
              state: data.state,
              postcode: data.postalCode,
              country: data.country,
            },
          },
        });
      }

      console.log('AFTER UPDATE PAYMENT ADDRESS');
      console.log(userResult);

      userResult = User.query().whereId(user.accountIdentifier).with('address').get();

      if (Array.isArray(userResult) && userResult.length && userResult[0].address) {
        user = userResult[0];
        console.log('Address added to user');
        addressFound = true;
      }
    }

    return addressFound;
  }

  async getPaymentIntent(pricePlanId, userId) {
    let paymentIntentResult;
    console.log('BEFORE GET PAYMENT INTENT');
    console.log({
      pricePlanId, userId,
    });
    try {
      paymentIntentResult = await axios.get(`${process.env.API}/user/paymentintent/${pricePlanId}/${userId}`);
      console.log('AFTER GET PAYMENT INTENT');
      console.log(paymentIntentResult);
    } catch (err) {
      console.log('AFTER GET PAYMENT INTENT ERROR');
      console.error(err);
    }

    return paymentIntentResult;
  }

  async upgradePackage(pricePlanId, userId) {
    let upgradeResult;
    console.log('BEFORE PAYMENT UPGRADE');
    console.log({
      pricePlanId, userId,
    });
    try {
      upgradeResult = await axios.put(`${process.env.API}/user/upgrade/${pricePlanId}/${userId}`);
      console.log('AFTER PAYMENT UPGRADE');
      console.log(upgradeResult);
    } catch (err) {
      console.log('AFTER PAYMENT UPGRADE ERROR:');
      console.error(err);
    }

    return upgradeResult;
  }

  async makePayment(paymentIntentId, pricePlanId, userId) {
    let paymentResult;
    console.log('BEFORE MAKE PAYMENT');
    console.log({
      paymentIntentId, userId,
    });
    try {
      paymentResult = await
      axios.put(`${process.env.API}/user/payment/${paymentIntentId}/${pricePlanId}/${userId}`);
      console.log('AFTER MAKE PAYMENT RESULT');
      console.log(paymentResult);
    } catch (err) {
      console.log('AFTER MAKE PAYMENT RESULT ERROR:');
      console.error(err);
    }

    return paymentResult;
  }

  async updateUserSubscription(tier) {
    console.log('UPDATE USER SUBSCRIPTION');
    try {
      User.update({
        data: {
          accountIdentifier: auth.account().accountIdentifier,
          tier,
          subscriptionStart: moment().toISOString(),
          subscriptionEnd: moment().add(1, 'months').toISOString(),
        },
      });
    } catch (err) {
      console.log('HATA updateUserSubscription');
      console.log(err);
    }
  }

  async subscribeToPackage(stripe, user, billingDetails, card, pricePlanId) {
    const response = {};

    try {
      console.log('BEFORE USER SUBSCRIPTION');
      console.log({
        user, billingDetails, card, pricePlanId,
      });
      if (!user.paymentIntentId) {
        const {
          data: paymentIntentData, status,
          error: paymentIntentError,
        } = await
        this.getPaymentIntent(pricePlanId, user.userId);

        if (status === 200 && paymentIntentData && !paymentIntentError) {
          const { paymentIntent, error: confirmPaymentIntentError } = await
          stripe.confirmCardPayment(paymentIntentData.clientSecret,
            {
              payment_method: {
                card,
                billing_details: billingDetails,
              },
            });

          if (paymentIntent && paymentIntent.status === 'succeeded' && !confirmPaymentIntentError) {
            await this.makePayment(paymentIntent.id, pricePlanId, user.userId);
            User.update({
              data: {
                accountIdentifier: user.accountIdentifier,
                paymentIntentId: paymentIntent.id,
              },
            });
            response.status = paymentIntent.status;
          } else {
            response.error = confirmPaymentIntentError;
          }
        } else {
          response.error = paymentIntentError;
        }
      } else { // upgrade
        const { status, error } = await
        this.upgradePackage(pricePlanId, user.userId);
        if (status === 200 && !error) {
          response.status = 'succeeded';
        } else {
          response.error = error;
        }
      }
      console.log('AFTER USER SUBSCRIPTION');
      console.log(response);
    } catch (err) {
      console.log('AFTER USER SUBSCRIPTON ERROR');
      console.error(err);
    }
    return response;
  }
}

const paymentServer = new PaymentServer();

Vue.prototype.$paymentServer = paymentServer;

export default paymentServer;