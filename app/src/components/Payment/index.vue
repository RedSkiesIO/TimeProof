<template>
  <div>
    <div
      v-if="getSellingProduct"
      class="payment-page"
    >
      <main
        id="main"
        :class="{
          loading: mainClassLoading,
          success: mainClassSuccess,
          processing: mainClassProcessing,
          receiver: mainClassReceiver,
          error: mainClassError,
          checkout: mainClassCheckout}"
      >
        <div id="checkout">
          <div
            v-show="paymentRequestVisible"
            id="payment-request"
            :class="{visible: paymentRequestVisible}"
          >
            <div id="payment-request-button" />
          </div>
          <form
            id="payment-form"
            @submit.prevent="formSubmit"
          >
            <section>
              <PaymentBilling
                ref="paymentBilling"
                :show-relevant-payment-methods="showRelevantPaymentMethods"
                :email="user.email"
              />
            </section>
            <section v-show="!user.paymentIntentId">
              <div class="row">
                <h2 class="col-5">
                  Payment Information
                </h2>
                <div
                  class="col-7"
                  style="padding-top:0.5rem"
                >
                  <div class="row">
                    <div class="col-3 visa" />
                    <div class="col-3 master-card" />
                    <div class="col-3 american-express" />
                    <div class="col-3 maestro" />
                  </div>
                </div>
              </div>
              <nav
                id="payment-methods"
              >
                <ul>
                  <li
                    v-for="item in uiPaymentTypeList"
                    :key="item.id"
                    :class="{visible: item.visible}"
                  >
                    <input
                      :id="item.id"
                      type="radio"
                      name="payment"
                      :value="item.value"
                      checked
                      @change.prevent="paymentTypeChange"
                    >
                    <label :for="item.id">{{ item.label }}</label>
                  </li>
                </ul>
              </nav>
              <div
                class="payment-info card"
                :class="{visible: cardPaymentVisible}"
              >
                <fieldset>
                  <label>
                    <span>Card</span>
                    <div
                      id="card-element"
                      class="field"
                    />
                  </label>
                </fieldset>
              </div>
              <div
                class="payment-info sepa_debit"
                :class="{visible: sepaDebitPaymentVisible}"
              >
                <fieldset>
                  <label>
                    <span>IBAN</span>
                    <div
                      id="iban-element"
                      class="field"
                    />
                  </label>
                </fieldset>
                <p class="notice">
                  By providing your IBAN and confirming this payment,
                  you’re authorizing Payments Demo, our payment
                  provider, to send instructions to your bank to debit your account.
                  You’re entitled to a refund under the terms
                  and conditions of your agreement with your bank.
                </p>
              </div>
              <div
                class="payment-info ideal"
                :class="{visible: idealPaymentVisible}"
              >
                <fieldset>
                  <label>
                    <span>iDEAL Bank</span>
                    <div
                      id="ideal-bank-element"
                      class="field"
                    />
                  </label>
                </fieldset>
              </div>
              <div
                class="payment-info redirect"
                :class="{visible: redirectPaymentVisible}"
              >
                <p class="notice">
                  You’ll be redirected to the banking site to complete your payment.
                </p>
              </div>
              <div
                class="payment-info receiver"
                :class="{visible: receiverPaymentVisible}"
              >
                <p class="notice">
                  Payment information will be provided after you place the order.
                </p>
              </div>
              <div
                class="payment-info wechat"
                :class="{visible: wechatPaymentVisible}"
              >
                <div id="wechat-qrcode" />
                <p
                  class="notice"
                  :style="{display: wechatPaymentInfoNoticeDisplay}"
                >
                  Click the button below to generate a QR code for WeChat.
                </p>
              </div>
            </section>
            <button
              class="payment-button"
              data-test-key="paymentButton"
              type="submit"
              :disabled="submitButtonDisable || hasPaymentBillingEmptyProperty"
            >
              {{ submitButtonText }}
            </button>
            <div
              id="card-errors"
              class="element-errors"
              :class="{ visible: cardErrorVisible}"
            >
              {{ cardErrorContent }}
            </div>
            <div
              id="iban-errors"
              class="element-errors"
              :class="{ visible: ibanErrorVisible}"
            >
              {{ ibanErrorContent }}
            </div>
          </form>
          <div style="margin-top:0.7rem">
            <img src="./images/powered_by_stripe.png">
          </div>
        </div>

        <div id="confirmation">
          <div class="status processing">
            <q-linear-progress
              dark
              indeterminate
              size="md"
              color="#4cbbc2"
            />
            <template v-if="!isFreePlan">
              <h1>Completing your order...</h1>
              <p>
                We’re just waiting for the confirmation
                from your bank… This might take a moment but feel free to close this page.
              </p>
              <p>We’ll send your receipt via email shortly.</p>
            </template>
            <template v-else>
              <h1>Completing your downgrading...</h1>
              <p>
                Processing… This might take a moment but feel free to close this page.
              </p>
            </template>
          </div>
          <div class="status success">
            <template v-if="!isFreePlan">
              <h1>Thanks for your order!</h1>
              <p>
                Woot! You successfully made a payment.
              </p>
              <p class="note">
                {{ confirmationElementNote }}
              </p>
            </template>
            <template v-else>
              <h1>You have successfully changed your plan!</h1>
            </template>
            <button
              style="width: 20vh; height: 5vh"
              @click="$router.push('/dashboard')"
            >
              Go to Dashboard
            </button>
          </div>
          <div class="status receiver">
            <h1>Thanks! One last step!</h1>
            <p>Please make a payment using the details below to complete your order.</p>
            <div class="info">
              {{ receiverInfo }}
            </div>
          </div>
          <div class="status error">
            <template v-if="!isFreePlan">
              <h1>Oops, payment failed.</h1>
              <p>
                It looks like your upgrading could not
                be paid at this time. Please try again or select a different payment option.
              </p>
            </template>
            <template v-else>
              <h1>Oops, downgrade failed.</h1>
              <p>
                It looks like your downgrading could not
                be completed at this time. Please try again.
              </p>
            </template>

            <p
              v-if="false"
              class="error-message"
            >
              {{ confirmationElementErrorMessage }}
            </p>
            <div class="col-md-12 col-sm-12 row">
              <button
                class="col-md-5 col-sm-5 col-xs-5"
                style="width: 18vh; height: 5vh"
                @click="tryAgain"
              >
                Try again
              </button>

              <button
                class="col-md-5 col-sm-5 col-xs-5"
                style="width: 20vh; height: 5vh; margin-left:2rem"
                @click="$router.push('/dashboard')"
              >
                Go to Dashboard
              </button>
            </div>
          </div>
        </div>
      </main>
      <aside
        v-once
        id="summary"
      >
        <div class="header">
          <h1>Order Summary</h1>
        </div>
        <div id="order-items" />
        <div id="order-total">
          <!-- <div class="line-item demo">
            <div id="demo">
              <p class="label">
                Demo in test mode
              </p>
              <p class="note">
                You can copy and paste the following test cards to trigger different scenarios:
              </p>
              <table class="note">
                <tr>
                  <td>Default UK card:</td>
                  <td class="card-number">
                    4000<span />0082<span />6000<span />0000
                  </td>
                </tr>
                <tr>
                  <td>
                    <a
                      href="https://stripe.com/guides/strong-customer-authentication"
                      target="_blank"
                    >Authentication</a> required:
                  </td>
                  <td class="card-number">
                    4000<span />0027<span />6000<span />3184
                  </td>
                </tr>
              </table>
              <p class="note">
                See the <a
                  href="https://stripe.com/docs/testing#cards"
                  target="_blank"
                >docs</a> for a full list of test cards.
                Non-card payments will redirect to test pages.
              </p>
            </div>
          </div> -->
          <div class="line-item">
            <img
              class="image"
              :src="productImage"
              :alt="getSellingProduct.title"
            >
            <div class="label">
              <span class="product">
                {{ getSellingProduct.title }}
              </span>
              <p class="sku">
                <strong>
                  {{ Object.values([getSellingProduct.noOfStamps,'timestamps']).join(' ') }}
                </strong>
              </p>
              <p class="sku">
                {{ getSellingProduct.freqDesc }}
              </p>
              <p class="sku text-italic">
                {{ getSellingProduct.description }}
              </p>
            </div>
            <p class="price">
              {{ amount }}
            </p>
          </div>
          <div class="line-item total">
            <p class="label">
              Total
            </p>
            <p
              class="price"
            >
              {{ amount }}
            </p>
          </div>
        </div>
      </aside>
    </div>
  </div>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';
import Identicon from 'identicon.js';
import PaymentBilling from '../PaymentBilling';
import config from './config';
import stripe from './stripeModule';
import { paymentMethods, uiPaymentTypeList } from './paymentMethods';
import { formatPrice } from '../../util';

export default {

  components: {
    PaymentBilling,
  },
  data() {
    return {
      elements: null,
      config,
      stripe,
      errorDialog: false,
      position: 'top',
      cardErrorContent: null,
      cardErrorVisible: false,
      ibanErrorContent: null,
      ibanErrorVisible: false,
      submitButtonDisable: true,
      paymentRequestVisible: false,
      mainClassSuccess: false,
      mainClassProcessing: false,
      mainClassReceiver: false,
      mainClassError: false,
      mainClassCheckout: false,
      mainClassLoading: true,
      cardPaymentVisible: true,
      idealPaymentVisible: false,
      sepaDebitPaymentVisible: false,
      wechatPaymentVisible: false,
      redirectPaymentVisible: false,
      receiverPaymentVisible: false,
      submitButtonText: 'Pay',
      confirmationElementNote: 'We just sent your receipt to your email address',
      confirmationElementErrorMessage: null,
      wechatPaymentInfoNoticeDisplay: 'none',
      paymentIntent: null,
      card: null,
      iban: null,
      idealBank: null,
      paymentType: 'card',
      receiverInfo: null,
      uiPaymentTypeList,
      style: config.paymentButtonStyle,
    };
  },

  computed: {
    ...mapGetters({
      getSellingProduct: 'settings/getSellingProduct',
    }),
    isFreePlan() {
      return this.getSellingProduct && this.getSellingProduct.price === 0;
    },
    user() {
      return this.$auth.user(true);
    },
    amount() {
      return formatPrice(this.getSellingProduct.price, config.currency);
    },
    productImage() {
      let imageData;
      if (this.getSellingProduct) {
        imageData = new Identicon(this.getSellingProduct.id, 420).toString();
      } else {
        imageData = new Identicon(Math.random().toString(15), 420).toString();
      }
      return `data:image/png;base64,${imageData}`;
    },
    hasPaymentBillingEmptyProperty() {
      return !this.$refs.paymentBilling.name
      || !this.$refs.paymentBilling.email
      || !this.$refs.paymentBilling.address
      || !this.$refs.paymentBilling.city
      || !this.$refs.paymentBilling.state
      || !this.$refs.paymentBilling.postalCode
      || !this.$refs.paymentBilling.country;
    },

  },
  created() {
    if (!this.getSellingProduct) {
      this.$router.push('/upgrade');
    }
  },
  mounted() {
    if (this.getSellingProduct) {
      if (this.isFreePlan) {
        this.downgradeToFreePlan();
      } else {
        this.setUpStripe();
      }
    }
  },

  methods: {
    ...mapActions('settings', [
      'setSellingProduct',
    ]),

    async setUpStripe() {
      if (stripe) {
        this.elements = stripe.elements();
        this.setupCard();
        this.setupIban();
        this.setupIdealBank();
        this.setupPaymentRequest();
        this.setupRest();
      }
    },

    async setupCard() {
      // Create a Card Element and pass some custom styles to it.
      if (this.user.paymentIntentId) { // upgrade the package
        this.submitButtonDisable = false;
      } else {
        this.card = this.elements.create('card', { style: this.style, iconStyle: 'solid' });

        // Mount the Card Element on the page.
        this.card.mount('#card-element');
        // Monitor change events on the Card Element to display any errors.
        this.card.on('change', ({ error, complete }) => {
          if (error) {
            this.cardErrorContent = error.message;
            this.cardErrorVisible = true;
          } else {
            this.cardErrorVisible = false;
            this.submitButtonDisable = !complete;
          }
        });
      }
    },

    async setupIban() {
      // Create a IBAN Element and pass the right options for styles and supported countries.
      const ibanOptions = {
        style: this.style,
        supportedCountries: ['SEPA'],
      };
      this.iban = this.elements.create('iban', ibanOptions);

      // Mount the IBAN Element on the page.
      this.iban.mount('#iban-element');

      // Monitor change events on the IBAN Element to display any errors.
      this.iban.on('change', ({ error, bankName, complete }) => {
        if (error) {
          this.ibanErrorContent = error.message;
          this.ibanErrorVisible = true;
        } else {
          this.ibanErrorVisible = false;
          if (bankName) {
            this.updateButtonLabel('sepa_debit', bankName);
          }
        }
        this.submitButtonDisable = !complete;
      });
    },

    async setupIdealBank() {
      // Create a iDEAL Bank Element and pass the style options,
      // along with an extra `padding` property.
      this.idealBank = this.elements.create('idealBank', {
        style: { base: Object.assign({ padding: '10px 15px' }, this.style.base) },
      });

      // Mount the iDEAL Bank Element on the page.
      this.idealBank.mount('#ideal-bank-element');
    },

    // Update the main button to reflect the payment method being selected.
    updateButtonLabel(paymentMethod, bankName) {
      // get payment total
      const { name } = paymentMethods[paymentMethod];
      let label = `Pay ${this.amount}`;
      if (paymentMethod !== 'card') {
        label = `Pay ${this.amount} with ${name}`;
      }
      if (paymentMethod === 'wechat') {
        label = `Generate QR code to pay ${this.amount} with ${name}`;
      }
      if (paymentMethod === 'sepa_debit' && bankName) {
        label = `Debit ${this.amount} from ${bankName}`;
      }
      this.submitButtonText = label;
    },

    async setupPaymentRequest() {
      let paymentIntent;// define it

      // Create the payment request.
      const paymentRequest = stripe.paymentRequest({
        country: config.country,
        currency: config.currency,
        total: {
          label: 'Total',
          amount: 1231,
        },
        requestShipping: true,
        requestPayerEmail: true,
        shippingOptions: config.shippingOptions,
      });

      // Callback when a payment method is created.
      paymentRequest.on('paymentmethod', async (event) => {
        // Confirm the PaymentIntent with the payment method returned from the payment request.
        const { error } = await stripe.confirmCardPayment(
          paymentIntent.client_secret,
          {
            payment_method: event.paymentMethod.id,
            shipping: {
              name: event.shippingAddress.recipient,
              phone: event.shippingAddress.phone,
              address: {
                line1: event.shippingAddress.addressLine[0],
                city: event.shippingAddress.city,
                postal_code: event.shippingAddress.postalCode,
                state: event.shippingAddress.region,
                country: event.shippingAddress.country,
              },
            },
          },
          { handleActions: false },
        );
        if (error) {
          // Report to the browser that the payment failed.
          event.complete('fail');
          this.completePayment({ error });
        } else {
          // Report to the browser that the confirmation was successful, prompting
          // it to close the browser payment method collection interface.
          event.complete('success');
          // Let Stripe.js handle the rest of the payment flow, including 3D Secure if needed.
          const response = await stripe.confirmCardPayment(
            paymentIntent.client_secret,
          );
          this.completePayment(response);
        }
      });

      // Callback when the shipping address is updated.
      paymentRequest.on('shippingaddresschange', (event) => {
        event.updateWith({ status: 'success' });
      });

      // Callback when the shipping option is changed.
      paymentRequest.on('shippingoptionchange', async (event) => {
        // Update the PaymentIntent to reflect the shipping cost.
        const response = await this.$paymentServer.updatePaymentIntentWithShippingCost(
          paymentIntent.id,
          this.$paymentServer.getLineItems(),
          event.shippingOption,
        );
        event.updateWith({
          total: {
            label: 'Total',
            amount: response.paymentIntent.amount,
          },
          status: 'success',
        });
        const amount = formatPrice(
          response.paymentIntent.amount,
          config.currency,
        );
        this.submitButtonText = `Pay ${amount}`;
      });

      // Create the Payment Request Button.
      const paymentRequestButton = this.elements.create('paymentRequestButton', {
        paymentRequest,
        style: {
          paymentRequestButton: {
            type: 'default',
            // One of 'default', 'book', 'buy', or 'donate'
            // Defaults to 'default'

            theme: 'dark',
            // One of 'dark', 'light', or 'light-outline'
            // Defaults to 'dark'

            height: '64px',
            // Defaults to '40px'. The width is always '100%'.
          },
        },
      });

      // Check if the Payment Request is available (or Apple Pay on the Web).
      const paymentRequestSupport = await paymentRequest.canMakePayment();
      if (paymentRequestSupport) {
        // Display the Pay button by mounting the Element in the DOM.
        paymentRequestButton.mount('#payment-request-button');
        // Replace the instruction.
        document.querySelector('.instruction span').innerText = 'Or enter'; // TODO change it
        // Show the payment request section.
        this.paymentRequestVisible = true;
      }
    },

    async completePayment(paymentResponse) {
      // Handle new PaymentIntent result
      try {
        const { status, error } = paymentResponse;
        console.log('HANDLE PAYMENT');
        console.log(paymentResponse);

        if (error) {
          this.confirmationElementErrorMessage = error.message;
          this.paymentResultUpdate(false, false, false, true, false, false, false);
        } else if (status === 'succeeded') {
          this.confirmationElementNote = 'We just sent your receipt to your email address';
          this.paymentResultUpdate(true, false, false, false, true, false, false);
        } else {
          this.confirmationElementErrorMessage = 'Unsupported operation!';
          this.paymentResultUpdate(false, false, false, true, false, false, false);
        }
      } catch (err) {
        console.error(err);
        this.paymentResultUpdate(false, false, false, true, false, false, false);
      }
    },

    async paymentResultUpdate(isSuccesss, isProcessing, isReceiver,
      isError, isSubmitButtonDisable, isLoading, isCheckOut) {
      this.mainClassSuccess = isSuccesss;
      this.mainClassProcessing = isProcessing;
      this.mainClassReceiver = isReceiver;
      this.mainClassError = isError;
      this.submitButtonDisable = isSubmitButtonDisable;
      this.mainClassLoading = isLoading;
      this.mainClassCheckout = isCheckOut;
    },

    showRelevantPaymentMethods(country) {
      const form = document.getElementById('payment-form');
      if (form) {
        const paymentInputs = form.querySelectorAll('input[name=payment]');
        for (let i = 0; i < paymentInputs.length; i += 1) {
          const input = paymentInputs[i];
          input.parentElement.classList.toggle(
            'visible',
            input.value === 'card'
              || (config.paymentMethods.includes(input.value)
                && paymentMethods[input.value].countries.includes(country)
                && paymentMethods[input.value].currencies.includes(config.currency)),
          );
        }

        // Hide the tabs if card is the only available option.
        const paymentMethodsTabs = document.getElementById('payment-methods');
        paymentMethodsTabs.classList.toggle(
          'visible',
          paymentMethodsTabs.querySelectorAll('li.visible').length > 1,
        );

        // Check the first payment option again.
        paymentInputs[0].checked = 'checked';
        this.cardPaymentVisible = true;
        this.idealPaymentVisible = false;
        this.sepaDebitPaymentVisible = false;
        this.wechatPaymentVisible = false;
        this.redirectPaymentVisible = false;
        this.updateButtonLabel(paymentInputs[0].value);
      }
    },

    async formSubmit() {
      // Disable the Pay button to prevent multiple click events.
      this.submitButtonText = 'Processing…';
      this.paymentResultUpdate(false, true, false, false, false, true, false);

      console.log('FORM SUBMIT');
      console.log(this.$refs.paymentBilling);

      const addressData = {
        name: this.$refs.paymentBilling.name,
        email: this.$refs.paymentBilling.email,
        line: this.$refs.paymentBilling.address,
        city: this.$refs.paymentBilling.city,
        state: this.$refs.paymentBilling.state,
        postalCode: this.$refs.paymentBilling.postalCode,
        country: this.$refs.paymentBilling.country,
      };
      console.log('GGGGGGGGGGGGGGGG');
      console.log(addressData);

      const addressFound = await this.$paymentServer.updatePaymentAddress(this.user, addressData);

      if (addressFound) {
        const {
          name, email, line, city, state, postalCode, country,
        } = addressData;
        const billingDetails = {
          name,
          email,
          address: {
            line1: line,
            city,
            postal_code: postalCode,
            state,
            country,
          },
        };

        if (this.user.userId) {
          if (this.paymentType === 'card') {
            const response = await this.$paymentServer
              .subscribeToPackage(stripe, this.user,
                billingDetails, this.card, this.getSellingProduct.id);

            this.completePayment(response);
          } else if (this.paymentType === 'sepa_debit') {
            // Confirm the PaymentIntent with the IBAN Element.
            const response = await stripe.confirmSepaDebitPayment(
              this.user.clientSecret,
              {
                payment_method: {
                  sepa_debit: this.iban,
                  billing_details: {
                    name: addressData.name,
                    email: addressData.email,
                  },
                },
              },
            );
            this.completePayment(response);
          } else {
            // Prepare all the Stripe source common data.
            const sourceData = {
              type: this.paymentType,
              amount: this.paymentIntent.amount,
              currency: this.paymentIntent.currency,
              owner: {
                name: addressData.name,
                email: addressData.email,
              },
              redirect: {
                return_url: window.location.href,
              },
              statement_descriptor: 'Stripe Payments Demo',
              metadata: {
                paymentIntent: this.paymentIntent.id,
              },
            };

            // Add extra source information which are specific to a payment method.
            switch (this.paymentType) {
              case 'ideal':
                // Confirm the PaymentIntent with the iDEAL bank Element.
                // This will redirect to the banking site.
                stripe.confirmIdealPayment(this.paymentIntent.client_secret, {
                  payment_method: {
                    ideal: this.idealBank,
                  },
                  return_url: window.location.href,
                });
                return;
              case 'sofort':
                // SOFORT: The country is required before redirecting to the bank.
                sourceData.sofort = {
                  country: addressData.country,
                };
                break;
              case 'ach_credit_transfer':
                // ACH Bank Transfer: Only supports USD payments,
                // edit the default config to try it.
                // In test mode, we can set the funds to be received via the owner email.
                sourceData.owner.email = `amount_${this.paymentIntent.amount}@example.com`;
                break;
              default: break;
            }

            // Create a Stripe source with the common data and extra information.
            const { source } = await stripe.createSource(sourceData);
            this.handleSourceActiviation(source);
          }
        } else {
          this.completePayment({ error: { message: 'User not found' } });
        }
      } else {
        this.activateCheckoutPage();
      }
    },

    // Handle activation of payment sources not yet supported by PaymentIntents
    handleSourceActiviation(source) {
      switch (source.flow) {
        case 'none': {
        // Normally, sources with a `flow` value of `none` are chargeable right away,
        // but there are exceptions, for instance for WeChat QR codes just below.
          if (source.type === 'wechat') {
            // Display the QR code.
            // const qrCode = new QRCode('wechat-qrcode', {
            //   text: source.wechat.qr_code_url,
            //   width: 128,
            //   height: 128,
            //   colorDark: '#424770',
            //   colorLight: '#f8fbfd',
            //   correctLevel: QRCode.CorrectLevel.H,
            // });
            // Hide the previous text and update the call to action.
            this.wechatPaymentInfoNoticeDisplay = 'none';
            const amount = formatPrice(
              source.amount, // get payment total
              config.currency,
            );
            this.submitButtonText = `Scan this QR code on WeChat to pay ${amount}`;
            // Start polling the PaymentIntent status.
            this.pollPaymentIntentStatus(this.paymentIntent.id, 300000);
          } else {
            console.log('Unhandled none flow.', source);
          }
          break;
        } case 'redirect': {
          // Immediately redirect the customer.
          this.submitButtonText = 'Redirecting…';
          window.location.replace(source.redirect.url);
          break;
        } case 'code_verification': {
          // Display a code verification input to verify the source.
          break;
        } case 'receiver': {
          // Display the receiver address to send the funds to.
          this.mainClassSuccess = true;
          this.mainClassReceiver = true;
          const amount = formatPrice(source.amount, config.currency);
          switch (source.type) {
            case 'ach_credit_transfer': {
              // Display the ACH Bank Transfer information to the user.
              const ach = source.ach_credit_transfer;
              this.receiverInfo = `
                <ul>
                  <li>
                    Amount:
                    <strong>${amount}</strong>
                  </li>
                  <li>
                    Bank Name:
                    <strong>${ach.bank_name}</strong>
                  </li>
                  <li>
                    Account Number:
                    <strong>${ach.account_number}</strong>
                  </li>
                  <li>
                    Routing Number:
                    <strong>${ach.routing_number}</strong>
                  </li>
                </ul>`;
              break;
            } case 'multibanco': {
              // Display the Multibanco payment information to the user.
              const { multibanco } = source;
              this.receiverInfo = `
                <ul>
                  <li>
                    Amount (Montante):
                    <strong>${amount}</strong>
                  </li>
                  <li>
                    Entity (Entidade):
                    <strong>${multibanco.entity}</strong>
                  </li>
                  <li>
                    Reference (Referencia):
                    <strong>${multibanco.reference}</strong>
                  </li>
                </ul>`;
              break;
            } default: {
              console.log('Unhandled receiver flow.', source);
            }
          }
          // Poll the PaymentIntent status.
          this.pollPaymentIntentStatus(this.paymentIntent.id);
          break;
        } default: {
          // Customer's PaymentIntent is received, pending payment confirmation.
          break;
        }
      }
    },

    async setupRest() {
      const url = new URL(window.location.href);
      if (url.searchParams.get('source') && url.searchParams.get('client_secret')) {
        // Update the interface to display the processing screen.
        this.mainClassSuccess = true;
        this.mainClassProcessing = true;
        this.mainClassCheckout = true;

        const { source } = await stripe.retrieveSource({
          id: url.searchParams.get('source'),
          client_secret: url.searchParams.get('client_secret'),
        });

        // Poll the PaymentIntent status.
        this.pollPaymentIntentStatus(source.metadata.paymentIntent);
      } else if (url.searchParams.get('payment_intent')) {
        // Poll the PaymentIntent status.
        this.pollPaymentIntentStatus(url.searchParams.get('payment_intent'));
      } else {
        // Update the interface to display the checkout form.
        this.mainClassCheckout = true;
      }
      this.mainClassLoading = false;
    },

    paymentTypeChange(event) {
      this.paymentType = event.target.value;
      const { flow } = paymentMethods[this.paymentType];
      // Update button label.
      this.updateButtonLabel(this.paymentType);

      // Show the relevant details, whether it's
      // an extra element or extra information for the user.
      this.cardPaymentVisible = this.paymentType === 'card';
      this.idealPaymentVisible = this.paymentType === 'ideal';
      this.sepaDebitPaymentVisible = this.paymentType === 'sepa_debit';
      this.wechatPaymentVisible = this.paymentType === 'wechat';
      this.cardErrorVisible = this.paymentType !== 'card';
      this.redirectPaymentVisible = flow === 'redirect';
      this.receiverPaymentVisible = flow === 'receiver';
    },

    tryAgain() {
      if (this.isFreePlan) {
        this.$router.push('/upgrade');
      } else {
        this.activateCheckoutPage();
      }
    },

    activateCheckoutPage() {
      this.paymentResultUpdate(false, false, false, false, false, true);
      this.updateButtonLabel(this.paymentType);
    },

    async downgradeToFreePlan() {
      this.paymentResultUpdate(false, true, false, false, false, false);
      const response = await this.$paymentServer
        .subscribeToPackage(null, this.user,
          null, null, this.getSellingProduct.id);
      this.completePayment(response);
    },

  },
};
</script>
<style lang="scss">
  @import './index.scss';
</style>
