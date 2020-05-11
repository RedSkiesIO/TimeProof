<template>
  <q-card
    flat
    class="bg-grey-2"
  >
    <div class="row justify-center q-mx-sm q-mb-md">
      <label class="text-h6 text-weight-bold">My Subscription</label>
    </div>
    <div
      class="row justify-start q-gutter-lg items-center
            q-mb-md q-mt-md q-ml-sm rounded-borders"
    >
      <div class="col-md-2 col-sm-2 col-xs-2">
        <label>Plan</label>
      </div>
      <div class="col-md-3 col-sm-3 col-xs-3">
        {{ currentPlan.title }} / Per Month
      </div>
      <div class="col-md-4 col-sm-4 col-xs-4 text-right">
        <q-space />
      </div>
    </div>
    <div
      class="row justify-start q-gutter-lg items-center
            q-mb-md q-mt-md q-ml-sm rounded-borders"
    >
      <div class="col-md-2 col-sm-2 col-xs-2">
        <label>Next Billng Date</label>
      </div>
      <div class="col-md-2 col-sm-2 col-xs-2">
        {{ moment(user.membershipRenewDate).format('DD-MM-YYYY') }}
      </div>
      <div class="col-md-5 col-sm-5 col-xs-5 text-right">
        <q-space />
      </div>
    </div>
    <div
      class="row justify-start q-gutter-lg items-center
            q-mb-md q-mt-md q-ml-sm rounded-borders"
    >
      <div class="col-md-2 col-sm-2 col-xs-2">
        <label>Billing Method</label>
      </div>
      <div
        :class="saveMode ? 'col-md-6 col-sm-8 col-xs-8' : 'col-md-7 col-sm-8 col-xs-8'"
      >
        <div
          v-show="saveMode"
          class="row"
        >
          <label class="col-md-1 col-sm-1 col-xs-1">
            <span>Card:</span>
          </label>
          <div
            id="card-element"
            class="field col-md-9 col-sm-9 col-xs-9"
          />
        </div>
        <div v-show="!saveMode">
          <div class="row">
            <label class="col-md-4 col-sm-4 col-xs-4">
              <span>Card Number:</span>
            </label>

            <div class="field col-md-6 col-sm-6 col-xs-6">
              {{ user.selectedCardNumber ? user.selectedCardNumber : '---- ---- ---- ----' }}
            </div>
          </div>
          <div class="row">
            <label class="col-md-4 col-sm-4 col-xs-4">
              <span>Expiration Date:</span>
            </label>

            <div class="field col-md-6 col-sm-6 col-xs-6">
              {{ user.cardExpirationDate ? user.cardExpirationDate : '-- / ----' }}
            </div>
          </div>
        </div>
      </div>
      <div
        v-if="user.selectedCardNumber && !saveMode"
        class="col-md-1 col-sm-1 col-xs-1"
      >
        <q-btn
          flat
          no-caps
          color="blue"
          label="Change"
          @click="changeCard"
        />
      </div>
      <div
        v-else-if="saveMode"
        class="col-md-2 col-sm-3 col-xs-3"
      >
        <div class="row">
          <q-btn
            flat
            class="col-md-6 col-sm-6 col-xs-6"
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
          <q-btn
            flat
            class="col-md-6 col-sm-6 col-xs-6"
            no-caps
            :disable="loading"
            color="blue"
            label="Cancel"
            @click="saveMode = false"
          />
        </div>
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
    this.refreshCard();
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
      this.card = this.elements.getElement('card');
      if (!this.card) {
        this.card = this.elements.create('card', { style: this.style });
      }

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

        const { status, error } = await
        this.$userServer.saveCard(this.user.name, stripe, this.card);

        this.cardMessageVisible = true;
        if (error) {
          this.cardMessageSuccessful = false;
          this.cardMessageContent = error.message;
        } else if (status === 'succeeded') {
          this.cardMessageSuccessful = true;
          this.cardMessageContent = 'Your card is successfully saved';
          this.refreshCard();
        } else if (status === 'processing') {
          this.cardMessageSuccessful = true;
          this.cardMessageContent = 'Your card is successfully processing';
        } else {
          this.cardMessageSuccessful = false;
          this.cardMessageContent = error.message;
        }
      } catch (err) {
        console.log('SAVE CARD FRONT ERROR');
        console.log(err);
      }

      setTimeout(() => {
        this.cardMessageVisible = false;
      }, 10000);

      this.loading = false;
      this.saveMode = false;
    },
    changeCard() {
      this.saveMode = true;
      this.setupCard();
    },
    async refreshCard() {
      this.$userServer.refreshCard(this.user);
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
