/* eslint-disable prefer-destructuring */
/* eslint-disable class-methods-use-this */
import Vue from 'vue';
import User from '../../../store/User';
import Address from '../../../store/Address';
import userServer from '../user';
import Server from '../index';
import { compare } from '../../../util';

class PaymentServer extends Server {
  async listAllPriceplans() {
    const { data, status } = await this.axios.get(`${process.env.API}/priceplans`);
    console.log('PRICE PLANSS');
    console.log(data);
    const productsData = {};
    if (status === 200 && data) {
      data.forEach((plan) => {
        productsData[plan.id] = plan;
      });
    }
    return productsData;
  }

  async updatePaymentAddress(user, data) {
    let addressFound = false;

    if (user.accountIdentifier) {
      const addressResult = Address.query()
        .where('accountIdentifier', user.accountIdentifier).last();

      console.log('UPDATE PAYMENT LAST ADDRESS');
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

  async getPaymentIntent(pricePlanId) {
    let paymentIntentResult;
    console.log('BEFORE GET PAYMENT INTENT');
    console.log({
      pricePlanId,
    });
    try {
      paymentIntentResult = await this.axios.get(`${process.env.API}/user/paymentintent/${pricePlanId}`);
      console.log('AFTER GET PAYMENT INTENT');
      console.log(paymentIntentResult);
    } catch (err) {
      console.log('AFTER GET PAYMENT INTENT ERROR');
      console.error(err);
    }

    return paymentIntentResult;
  }


  async getPaymentDetails() {
    let paymentResult = {};
    try {
      console.log('BEFORE GET PAYMENT DETAILS');

      paymentResult = await this.axios.get(`${process.env.API}/user/paymentmethod`);
      console.log('AFTER PAYMENT DETAILS');
      console.log(paymentResult);
    } catch (err) {
      console.log('AFTER PAYMENT DETAILS ERROR');
      console.error(err);
      paymentResult.error = err;
    }

    return paymentResult;
  }

  compateBillingAddresses(prevBillingAddress, billingDetails) {
    if (!billingDetails) {
      return false;
    }

    if (prevBillingAddress) {
      const address = prevBillingAddress.line2
        ? prevBillingAddress.line1 + prevBillingAddress.line2 : prevBillingAddress.line1;
      if (!compare(billingDetails.line1, address)
          || !compare(billingDetails.city, prevBillingAddress.city)
          || !compare(billingDetails.postcode, prevBillingAddress.postcode)
          || !compare(billingDetails.state, prevBillingAddress.state)
          || !compare(billingDetails.country, prevBillingAddress.country)) {
        return true;
      }
      return false;
    }

    return true;
  }

  async upgradePackage(pricePlanId, pmMethodId, billingDetails, prevBillingAddress) {
    let upgradeResult = {};
    try {
      console.log('BEFORE PAYMENT METHOD UPDATE');
      const compResult = this.compateBillingAddresses(prevBillingAddress, billingDetails);
      if (compResult) {
        const {
          status: pmUpdateStatus,
          error: pmUpdateError,
        } = await this.axios.put(`${process.env.API}/user/paymentmethod/${pmMethodId}`, billingDetails);
        console.log('PAYMENT METHOD UPDATE');
        if (pmUpdateStatus === 200 && !pmUpdateError) {
          upgradeResult = await this.axios.put(`${process.env.API}/user/upgrade/${pricePlanId}`);
        } else {
          upgradeResult.error = pmUpdateError;
        }
      } else {
        upgradeResult = await this.axios.put(`${process.env.API}/user/upgrade/${pricePlanId}`);
      }

      console.log('AFTER PAYMENT METHOD UPDATE');
      console.log(upgradeResult);
    } catch (err) {
      console.log('PAYMENT METHOD UPDATE ERROR');
      console.error(err);
      upgradeResult.error = err;
    }

    return upgradeResult;
  }

  async makePayment(paymentIntentId, pricePlanId) {
    let paymentResult = {};
    console.log('BEFORE MAKE PAYMENT');
    console.log({
      paymentIntentId,
    });
    try {
      paymentResult = await
      this.axios.put(`${process.env.API}/user/payment/${paymentIntentId}/${pricePlanId}`);
      console.log('AFTER MAKE PAYMENT RESULT');
      console.log(paymentResult);
    } catch (err) {
      console.log('AFTER MAKE PAYMENT RESULT ERROR');
      console.error(err);
      paymentResult.error = err;
    }

    return paymentResult;
  }

  async updateUserSubscription() {
    console.log('UPDATE USER SUBSCRIPTION');
    try {
      const verifyResult = await userServer.verifyUserDetails();
      if (verifyResult) {
        User.update({
          data: {
            accountIdentifier: this.getAccount().accountIdentifier,
            tier: verifyResult.pricePlanId,
            clientSecret: verifyResult.clientSecret,
            paymentIntentId: verifyResult.paymentIntentId,
            membershipRenewDate: verifyResult.membershipRenewDate,
            remainingTimeStamps: verifyResult.remainingTimeStamps,
            pendingPricePlanId: verifyResult.pendingPricePlanId,
          },
        });
      }
    } catch (err) {
      console.log('AFTER UPDATE USER SUBSCRIPTION ERROR');
      console.log(err);
    }
  }

  async subscribeToPackage(stripe, user, pmMethodId,
    billingDetails, prevBillingAddress, card, pricePlanId) {
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
        } = await this.getPaymentIntent(pricePlanId);

        if (status === 200 && paymentIntentData && !paymentIntentError) {
          const { paymentIntent, error: confirmPaymentIntentError } = await
          stripe.confirmCardPayment(paymentIntentData.clientSecret,
            {
              payment_method: {
                card,
                billing_details: { address: billingDetails },
              },
            });

          if (paymentIntent && paymentIntent.status === 'succeeded' && !confirmPaymentIntentError) {
            await this.makePayment(paymentIntent.id, pricePlanId);
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
      } else { // upgrade and downgrade
        const { status, error } = await
        this.upgradePackage(pricePlanId, pmMethodId, billingDetails, prevBillingAddress);
        if (status === 200 && !error) {
          response.status = 'succeeded';
        } else if (status === 409) {
          response.error.message = 'You have already confirmed that you will switch to this plan. Cancel your future plan and try again.';
        } else if (error) {
          response.error = error;
        }
      }
      console.log('AFTER USER SUBSCRIPTION');
      console.log(response);
      this.updateUserSubscription();
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
