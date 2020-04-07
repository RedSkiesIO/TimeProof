<template>
  <q-card
    flat
    class="bg-grey-2"
    style="width:65%"
  >
    <div class="row justify-center q-mx-sm q-mb-md">
      <label class="text-h5 text-weight-bold">My Subscription</label>
    </div>
    <div
      class="row justify-start q-gutter-lg items-center
            q-mb-md q-mt-md q-ml-sm rounded-borders"
    >
      <div class="col-md-3">
        <label>Plan</label>
      </div>
      <div class="col-md-2">
        {{ currentPlan.title }} Monthly
      </div>
      <div class="col-md-5 text-right">
        <q-space />
      </div>
    </div>
    <div
      class="row justify-start q-gutter-lg items-center
            q-mb-md q-mt-md q-ml-sm rounded-borders"
    >
      <div class="col-md-3">
        <label>Next Billng Date</label>
      </div>
      <div class="col-md-2">
        {{ moment(user.subscriptionEnd).format('DD-MM-YYYY') }}
      </div>
      <div class="col-md-5 text-right">
        <q-space />
      </div>
    </div>
    <div
      class="row justify-start q-gutter-lg items-center
            q-mb-md q-mt-md q-ml-sm rounded-borders"
    >
      <div class="col-md-3">
        <label>Billing Method</label>
      </div>
      <div
        class="col-md-5"
      >
        <div
          v-show="saveMode"
          class="row"
        >
          <label class="col-md-1">
            <span>Card:</span>
          </label>
          <div
            id="card-element"
            class="field col-md-7"
          />
        </div>
        <div v-show="!saveMode">
          <div class="row">
            <label class="col-md-4">
              <span>Card Number:</span>
            </label>

            <div class="field col-md-6">
              {{ user.selectedCardNumber }}
            </div>
          </div>
          <div class="row">
            <label class="col-md-4">
              <span>Expiration Date:</span>
            </label>

            <div class="field col-md-6">
              {{ user.cardExpirationDate }}
            </div>
          </div>
        </div>
      </div>
      <div
        class="col-md-2"
      >
        <q-btn
          v-if="!user.selectedCardNumber && !saveMode"
          flat
          no-caps
          color="blue"
          label="Add"
          @click="addCard"
        />
        <q-btn
          v-else-if="user.selectedCardNumber && !saveMode"
          flat
          no-caps
          color="blue"
          label="Change"
          @click="changeCard"
        />
        <q-btn
          v-else
          flat
          no-caps
          :disable="cardSaveDisable"
          color="blue"
          :label="loading ? '' : 'Save'"
          @click="saveCard"
        >
          <q-spinner
            v-show="loading"
            color="primary"
            size="1.5em"
            :thickness="2"
          />
        </q-btn>
      </div>
    </div>
    <div
      v-if="cardMessageVisible"
      class="row justify-start q-gutter-lg q-mx-sm items-center rounded-borders"
      :style="{ color: cardMessageSuccessful ? 'green' : '#e25950'}"
    >
      <div
        class="card-message"
        :class="{error: !cardMessageSuccessful, success: cardMessageSuccessful}"
      >
        {{ cardMessageContent }}
      </div>
    </div>

    <div
      class="row justify-start q-gutter-lg q-mx-sm items-center
            q-mb-md q-mt-md rounded-borders"
    >
      <q-btn
        flat
        no-caps
        size="sm"
        color="white bg-black"
        label="Change My Subscription"
        @click="$router.push('/upgrade')"
      />
    </div>
  </q-card>
</template>
<script>
import { mapGetters } from 'vuex';
import moment from 'moment';
import stripe from '../Payment/stripeModule';
import config from '../Payment/config';
import User from '../../store/User';

export default {
  name: 'AccountSubscription',

  data() {
    return {
      isPwd: true,
      moment,
      stripe,
      elements: null,
      card: null,
      loading: false,
      cardInfo: null,
      saveMode: false,
      style: config.paymentButtonStyle,
      cardSaveDisable: true,
      cardMessageVisible: false,
      cardMessageContent: null,
      cardMessageSuccessful: false,
    };
  },

  computed: {
    ...mapGetters({
      products: 'settings/getProducts',
    }),
    user() {
      return this.$auth.user();
    },
    currentPlan() {
      return this.products[this.user.tier];
    },
  },
  created() {
    this.getPaymentMethod();
  },
  mounted() {
    this.setUpStripe();
  },
  methods: {
    async setUpStripe() {
      if (window.Stripe === undefined) {
        console.log('Stripe V3 library not loaded!');
      } else {
        this.elements = this.stripe.elements();
      }
    },

    async setupCard() {
      // Create a Card Element and pass some custom styles to it.
      // this.card = this.elements.getElement('card');

      this.card = this.elements.create('card', { style: this.style });


      // Mount the Card Element on the page.
      this.card.mount('#card-element');

      // Monitor change events on the Card Element to display any errors.
      this.card.on('change', ({ error, complete }) => {
        if (error) {
          this.cardMessageContent = error.message;
          this.cardMessageSuccessful = false;
          this.cardMessageVisible = true;
        } else {
          this.cardMessageVisible = false;
          this.cardMessageContent = null;
          this.cardSaveDisable = !complete;
        }
      });
    },

    async saveCard() {
      try {
        this.loading = true;
        // const { paymentMethod } = await stripe.createPaymentMethod({
        //   type: 'card',
        //   card: this.card,
        //   billing_details: {
        //     name: this.user.name,
        //     email: this.user.email,
        //   },
        // });
        // console.log('PATMENT METHOD CARD');
        // console.log(paymentMethod);

        let { setupIntent, error } = await stripe.confirmCardSetup(
          this.user.clientSecret,
          {
            payment_method: {
              card: this.card,
              billing_details: {
                name: this.user.name,
              },
            },
          },
        );
        // let { setupIntent, error } = response;
        console.log('SAVE CARD');
        // console.log(setupIntent);

        if (error && error.setup_intent) {
          setupIntent = error.setup_intent;
          error = null;
        }

        console.log(setupIntent);
        this.cardMessageVisible = true;
        if (error) {
          this.cardMessageSuccessful = false;
          this.cardMessageContent = error.message;
        } else if (setupIntent.status === 'succeeded') {
          this.cardMessageSuccessful = true;
          this.cardMessageContent = 'Your card is successfully saved';
        } else if (setupIntent.status === 'processing') {
          this.cardMessageSuccessful = true;
          this.cardMessageContent = 'Your card is successfully processing';
        } else {
          this.cardMessageSuccessful = false;
          this.cardMessageContent = error.message;
        }
      } catch (err) {
        console.log(err);
      }

      setTimeout(() => {
        this.cardMessageVisible = false;
      }, 2000);

      this.loading = false;
      this.saveMode = false;
    },
    addCard() {
      this.saveMode = true;
      this.setupCard();
    },
    changeCard() {
      this.saveMode = true;
      this.setupCard();
    },
    async getPaymentMethod() {
      const { data, status } = await this.$axios.get(`${process.env.API}/user/paymentmethod/${this.user.userId}`);
      console.log('PAYMENT METHOD RESULT');
      console.log(data);
      if (status === 200 && data && data.card) {
        User.update({
          data: {
            accountIdentifier: this.user.accountIdentifier,
            selectedCardNumber: `**** **** **** ${data.card.last4}`,
            cardExpirationDate: `${data.card.expMonth} / ${data.card.expYear}`,
          },
        });
      }
    },
  },
};
</script>
<style lang="scss" scoped>
.payment-info.card{
    text-align: center;
}
.signing-key {
    width: 100%;
}
.field {
    flex: 1;
    padding: 0 15px;
    background: transparent;
    font-weight: 400;
    color: #31325f;
    outline: none;
    cursor: text;
}
.card-message {
    display: inline-flex;
    height: 20px;
    margin: 15px auto 0;
    padding-left: 20px;
    transform: translateY(10px);
    transition-property: opacity, transform;
    transition-duration: 0.35s;
    transition-timing-function: cubic-bezier(0.165, 0.84, 0.44, 1);
    &.error{
        background: url(../Payment/images/error.svg) center left no-repeat;
        color: #e25950
    }
    &.success{
        &::before{
           content: '\02705';
        }
        color: green;
    }
    background-size: 15px 15px;
}
.card-message.visible {
    transform: none;
}
</style>
