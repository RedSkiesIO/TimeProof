<template>
  <q-page class="q-mt-md justify-start price-plans">
    <div
      class="row"
      style="width: 100%"
    >
      <div
        v-for="item in planItems"
        :key="item.plan"
        :class="planClass"
      >
        <q-card
          flat
        >
          <div
            class="top-section text-white text-center"
            :style="{backgroundColor: item.color}"
          >
            <div class="price-title">
              {{ item.package }}
            </div>
            <div class="price text-weight-bold">
              {{ item.price }}
            </div>
            <div class="price-subtitle">
              {{ item.freq }}
            </div>
          </div>
          <q-card-section class="text-center">
            <div class="price-timestamps">
              {{ item.timestamps }} timestamps
            </div>
            <div class="price-button-container">
              <!-- <StripeButton
                label="Choose Plan"
                :item="items[0]"
                :token="token"
              /> -->
              <q-btn
                unelevated
                style="color: #0047cc; background: #e5ecfa;"
                label="Choose Plan"
                @click="choosePlan(item)"
              />
            </div>
          </q-card-section>
        </q-card>
      </div>
    </div>
  </q-page>
</template>
<script>
// import StripeButton from '../components/StripeCheckoutButton';
import { mapActions } from 'vuex';

const planItems = [
  {
    plan: 'plan_GeJGwbwuTvjjTi',
    quantity: 1,
    package: 'Starter',
    price: 'Free',
    freq: 'Per Month',
    timestamps: 5,
    color: 'green',
  },
  {
    plan: 'plan_Gga2y8jPcryhWJ',
    quantity: 1,
    package: 'Basic',
    price: 30.56,
    freq: 'Per Month',
    timestamps: 15,
    color: 'blue',
  },
  {
    plan: 'plan_ASDıuwbendnoquhw',
    quantity: 1,
    package: 'Premium',
    price: 97.45,
    freq: 'Per Year',
    timestamps: 115,
    color: 'orange',
  },
  {
    plan: 'plan_Aojaosdjoıpj',
    quantity: 1,
    package: 'Gold',
    price: 129.87,
    freq: 'Per Year',
    timestamps: 225,
    color: 'purple',
  },
];

export default {
  components: {
    // StripeButton,
  },
  data() {
    return {
      token: null,
      loading: false,
      publishableKey: 'pk_test_IRxZZJoqfYY2SSVj8arguq9k00mg8SQT5R',
      successUrl: 'http://localhost:6420/',
      cancelUrl: 'http://localhost:6420/upgrade',
      planItems,
    };
  },
  computed: {

    planClass: () => `col-${12 / planItems.length}`,
  },
  mounted() {
    this.getToken();
  },
  beforeRouteUpdate(to, from, next) {
    console.log('LLLLLLLLLLLL');
    console.log(from);
    console.log(to);
    next();
  },
  methods: {
    ...mapActions('settings', [
      'setSellingProduct',
    ]),
    async getToken() {
      const accessToken = await this.$auth.getToken();
      this.token = accessToken.idToken.rawIdToken;
    },
    checkout() {
      this.$refs.checkoutRef.redirectToCheckout();
    },
    choosePlan(item) {
      this.setSellingProduct(item);
      this.$router.push('/payment');
    },
  },
};
</script>
<style lang="scss" scoped>
.price-plans {
  .q-card{
    padding: 0;
    margin: 0 16px;
    border-radius: 20px;
  }

  .price-title {
    font-size: 18px;
  }
  .price {
    font-size: 36px;
  }
  .price-timestamps {
    padding: 16px 20px;
    margin: 0 25px;
    border-bottom: 1px solid lightgrey;
    color: $secondary;
    font-weight: bold;
  }
  .price-button-container {
    padding-top: 32px;
    padding-bottom: 24px;
  }
  .top-section {
    padding: 40px 20px;
  }

  .q-card__section {
  }
}
</style>
