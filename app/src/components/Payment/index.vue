<template>
  <q-page class="flex flex-center price-plans">
    <div
      class="row justify-center"
      style="width: 100%"
    >
      <div class="col-6">
        <q-card
          class="q-mt-md"
          flat
          bordered
        >
          <q-card-section
            horizontal
          >
            <div class="row col-12 justify-between">
              <q-card-section class="col-4">
                <div
                  class="text-overline"
                  :style="{backgroundColor: sellingProduct.color}"
                >
                  <div class="text-weight-bold text-white plan-title">
                    {{ sellingProduct.package }}
                  </div>
                </div>
              </q-card-section>

              <q-card-section class="col-4">
                <div class="price text-weight-bold">
                  {{ sellingProduct.price !== 'Free' ?
                    `£${sellingProduct.price}` : sellingProduct.price }}
                </div>
                <div class="price-subtitle">
                  {{ sellingProduct.freq }}
                </div>
              </q-card-section>
            </div>
          </q-card-section>
          <q-card-section>
            <form id="payment-form">
              <div
                v-if="anotherPaymentVisible"
                id="payment-request-button"
                ref="anotherpayment"
              >
              <!-- A Stripe Element will be inserted here. -->
              </div>
              <q-card-section>
                <div class="row">
                  <div class="col-3 payment-card-section">
                    <label for="name">
                      Name
                    </label>
                  </div>
                  <input
                    id="name"
                    v-model="cardName"
                    name="name"
                    class="col-6 payment-card-section combo-inputs-row"
                    placeholder="Name and Surname"
                    :class="{ 'uk-form-danger': cardNameError }"
                    required
                  >
                </div>
                <div class="row">
                  <label
                    v-if="cardNameError"
                    id="name-errors"
                    class="field-error offset-4"
                    role="alert"
                  >
                    {{ cardNameError }}
                  </label>
                </div>
              </q-card-section>
              <q-card-section>
                <div class="row">
                  <div class="col-3 payment-card-section">
                    <label for="card">
                      Card Number
                    </label>
                  </div>
                  <div
                    id="card"
                    ref="card"
                    class="col-6 payment-card-section combo-inputs-row"
                    :class="{ 'uk-form-danger': cardNumberError }"
                  />
                </div>
                <div class="row">
                  <label
                    v-if="cardNumberError"
                    id="card-errors"
                    class="field-error offset-4"
                    role="alert"
                  >
                    {{ cardNumberError }}
                  </label>
                </div>
              </q-card-section>
              <q-card-section>
                <div class="row">
                  <div class="col-3 payment-card-section">
                    <label for="cvv">
                      Card CVC
                    </label>
                  </div>
                  <div
                    id="cvv"
                    ref="cvv"
                    class="col-6 payment-card-section combo-inputs-row"
                    :class="{ 'uk-form-danger': cardCvcError }"
                  />
                </div>
                <div class="row">
                  <label
                    v-if="cardCvcError"
                    id="cvv-errors"
                    class="field-error offset-4"
                    role="alert"
                  >
                    {{ cardCvcError }}
                  </label>
                </div>
              </q-card-section>
              <q-card-section>
                <div class="row">
                  <div class="col-3 payment-card-section">
                    <label for="expiry">
                      Expiry
                    </label>
                  </div>
                  <div
                    id="expiry"
                    ref="expiry"
                    class="col-6 payment-card-section combo-inputs-row"
                    :class="{ 'uk-form-danger': cardExpiryError }"
                  />
                </div>
                <div class="row">
                  <label
                    v-if="cardExpiryError"
                    id="expiry-errors"
                    class="field-error offset-4"
                    role="alert"
                  >
                    {{ cardExpiryError }}
                  </label>
                </div>
              </q-card-section>
              <q-separator />
              <q-card-actions class="justify-center q-mt-lg q-mb-lg">
                <q-btn
                  class="self-center payment-button"
                  label="SUBMIT PAYMENT"
                  style="color: #ffffff; background: #32325d;"
                  @click.prevent="submitFormToCreateToken"
                />
              </q-card-actions>
            </form>
          </q-card-section>
        </q-card>
      </div>
    </div>

    <q-dialog
      v-model="errorDialog"
      :position="position"
    >
      <q-card style="width: 350px">
        <q-card-section class="row items-center no-wrap stripeError">
          <div>
            <div class="text-weight-bold">
              Payment
            </div>
            <div class="text-grey">
              {{ stripeError }}
            </div>
          </div>

          <q-space />

          <q-btn
            flat
            round
            icon="close"
            @click="closeErrorDialog"
          />
        </q-card-section>
      </q-card>
    </q-dialog>
  </q-page>
</template>
<script>
import { mapGetters } from 'vuex';

export default {

  data() {
    return {
      card: {
        cvc: '',
        number: '',
        expiry: '',
      },

      errorDialog: false,
      position: 'top',

      // Custom styling can be passed to options when creating an Element.
      // (Note that this demo uses a wider set of styles than the guide below.)
      style: {
        base: {
          color: '#32325d',
          fontFamily: '"Helvetica Neue", Helvetica, sans-serif',
          fontSmoothing: 'antialiased',
          fontSize: '16px',
          '::placeholder': {
            color: '#aab7c4',
          },
        },
        invalid: {
          color: '#fa755a',
          iconColor: '#fa755a',
        },
      },
      // elements
      cardNumber: '',
      cardExpiry: '',
      cardCvc: '',
      cardName: '',
      stripe: null,

      // errors
      stripeError: '',
      cardCvcError: '',
      cardExpiryError: '',
      cardNumberError: '',
      cardNameError: '',

      anotherPaymentVisible: true,
      loading: false,
    };
  },
  computed: {
    ...mapGetters({
      sellingProduct: 'settings/getSellingProduct',
    }),
  },
  watch: {
    $route(to, from) {
      console.log('PPPPPPPPPP');
      console.log(to, from);
    },
  },
  mounted() {
    this.setUpStripe();
  },

  methods: {
    async setUpStripe() {
      if (window.Stripe === undefined) {
        alert('Stripe V3 library not loaded!');
      } else {
        this.stripe = window.Stripe('pk_test_GZYfG6M52r52tEHYj4G5mUkz');

        const elements = this.stripe.elements();

        const paymentRequest = this.stripe.paymentRequest({
          country: 'GB',
          currency: 'gbp',
          total: {
            label: 'Demo total',
            amount: 10,
          },
          requestPayerName: true,
          requestPayerEmail: true,
        });

        const prButton = elements.create('paymentRequestButton', {
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

        (async () => {
          // Check the availability of the Payment Request API first.
          console.log('KKKKKKKKKKKKKKKKK');
          const result = await paymentRequest.canMakePayment();
          console.log(result);
          console.log(prButton);
          if (result) {
            prButton.mount(this.$refs.anotherpayment);
          } else {
            this.anotherPaymentVisible = false;
          }
        })();

        paymentRequest.on('paymentmethod', async (ev) => {
          // Confirm the PaymentIntent without handling potential next actions (yet).
          const clientSecret = '';
          const { error: confirmError } = await this.stripe.confirmCardPayment(
            clientSecret,
            { payment_method: ev.paymentMethod.id },
            { handleActions: false },
          );

          if (confirmError) {
            // Report to the browser that the payment failed, prompting it to
            // re-show the payment interface, or show an error message and close
            // the payment interface.
            ev.complete('fail');
          } else {
            // Report to the browser that the confirmation was successful, prompting
            // it to close the browser payment method collection interface.
            ev.complete('success');
            // Let Stripe.js handle the rest of the payment flow.
            const { error, paymentIntent } = await this.stripe.confirmCardPayment(clientSecret);
            if (error) {
              console.log(error);
              // The payment failed -- ask your customer for a new payment method.
            } else {
              console.log(paymentIntent);
              // The payment has succeeded.
            }
          }
        });

        this.cardNumber = elements.create('cardNumber', { style: this.style, showIcon: true });
        this.cardCvc = elements.create('cardCvc', { style: this.style });
        this.cardExpiry = elements.create('cardExpiry', { style: this.style });

        this.cardNumber.mount(this.$refs.card);
        this.cardCvc.mount(this.$refs.cvv);
        this.cardExpiry.mount(this.$refs.expiry);

        this.listenForErrors();
      }
    },

    listenForErrors() {
      const vm = this;

      this.cardNumber.addEventListener('change', (event) => {
        vm.toggleError(event);
        vm.cardNumberError = '';
        vm.card.number = !!event.complete;
      });

      this.cardExpiry.addEventListener('change', (event) => {
        vm.toggleError(event);
        vm.cardExpiryError = '';
        vm.card.expiry = !!event.complete;
      });

      this.cardCvc.addEventListener('change', (event) => {
        vm.toggleError(event);
        vm.cardCvcError = '';
        vm.card.cvc = !!event.complete;
      });
    },

    toggleError(event) {
      if (event.error) {
        this.stripeError = event.error.message;
        this.openErrorDialog('top');
      } else {
        this.stripeError = '';
        this.closeErrorDialog();
      }
    },

    submitFormToCreateToken() {
      this.clearCardErrors();
      let valid = true;

      if (!this.cardName) {
        valid = false;
        this.cardNameError = 'Card Name is Required';
      }
      if (!this.card.number) {
        valid = false;
        this.cardNumberError = 'Card Number is Required';
      }
      if (!this.card.cvc) {
        valid = false;
        this.cardCvcError = 'CVC is Required';
      }
      if (!this.card.expiry) {
        valid = false;
        this.cardExpiryError = 'Month is Required';
      }
      if (this.stripeError) {
        valid = false;
      }
      if (valid) {
        this.createToken();
      }
    },

    async createToken() {
      const { token, error } = await this.stripe.createToken(this.cardNumber);

      // this.stripe.createToken(this.cardNumber).then((result) => {
      if (error) {
        this.stripeError = error.message;
      } else {
        alert('Thanks for donating.', token.id);
        // send the token to your server
        // clear the inputs
      }
      // });
    },

    clearElementsInputs() {
      this.cardCvc.clear();
      this.cardExpiry.clear();
      this.cardNumber.clear();
    },

    clearCardErrors() {
      this.stripeError = '';
      this.cardCvcError = '';
      this.cardExpiryError = '';
      this.cardNumberError = '';
      this.cardNameError = '';
    },

    reset() {
      this.clearElementsInputs();
      this.clearCardErrors();
    },

    openErrorDialog(position) {
      this.position = position;
      this.errorDialog = true;
    },

    closeErrorDialog() {
      this.errorDialog = false;
    },
  },
};
</script>
<style lang="scss" scoped>
.price-plans {
  .q-card{
    padding: 0;
    margin: 0 16px;
    border-radius: 10px;
  }

  .price-title {
    font-size: 18px;
  }
  .price {
    font-size: 36px;
  }
  .payment-card-section {
    padding: 16px 20px;
    margin: 0 15px;
    border-bottom: 1px solid lightgrey;
    color: $secondary;
    font-weight: bold;
  }
  .payment-button {
    width: 200px;
  }
  .top-section {
    padding: 30px 15px;
  }

}
</style>
<style>
:root {
   --font-color: rgb(105, 115, 134);
}
.plan-title{
  font-size: 24px;
}
/**
* The CSS shown here will not be introduced in the Quickstart guide, but
* shows how you can use CSS to style your Element's container.
*/
input,
.StripeElement {
  height: 40px;

  color: #32325d;
  background-color: white;
  border: 1px solid transparent;
  border-radius: 4px;

  box-shadow: 0 1px 3px 0 gray;
  -webkit-transition: box-shadow 150ms ease;
  transition: box-shadow 150ms ease;
}

input {
  padding: 10px 12px;
}

input:focus,
.StripeElement--focus {
  box-shadow: 0 1px 3px 0 #cfd7df;
}

.StripeElement--invalid {
  border-color: #fa755a;
}

.StripeElement--webkit-autofill {
  background-color: #fefde5 !important;
}

.field-error {
  color: var(--font-color);
  font-size: 13px;
  line-height: 17px;
  margin-top: 12px;
}
.combo-inputs-row {
  box-shadow: 0px 0px 0px 0.5px rgba(50, 50, 93, 0.1),
    0px 2px 5px 0px rgba(50, 50, 93, 0.1), 0px 1px 1.5px 0px rgba(0, 0, 0, 0.07);
  border-radius: 7px;
}
</style>
