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
          class="q-mt-md"
          flat
          bordered
        >
          <q-card-section horizontal>
            <div class="row col-12 justify-around">
              <q-card-section class="col-6">
                <div
                  class="text-overline"
                  :style="{backgroundColor: item.color}"
                >
                  <div class="text-weight-bold text-white plan-title">
                    {{ item.package }}
                  </div>
                </div>
              </q-card-section>

              <q-card-section class="col-5">
                <div class="price text-weight-bold">
                  {{ item.price !== 'Free' ? `£${item.price}` : item.price }}
                </div>
                <div class="price-subtitle">
                  {{ item.freq }}
                </div>
              </q-card-section>
            </div>
          </q-card-section>
          <q-card-section>
            <div class="text-h5 q-mt-sm q-mb-md">
              Product Detail
            </div>
            <div class="text-caption text-grey">
              Lorem ipsum dolor sit amet,
              consectetur adipiscing elit,
              sed do eiusmod tempor incididunt
              ut labore et dolore magna aliqua.
            </div>
          </q-card-section>
          <q-separator />

          <q-card-actions class="row justify-between">
            <q-btn
              :disable="currentMemberShip === item.package"
              flat
              color="primary"
              @click="choosePlan(item)"
            >
              Choose Plan
            </q-btn>
            <q-badge
              v-if="currentMemberShip === item.package"
              outline
              class="q-ml-md"
              color="orange"
              label="Active"
            />
          </q-card-actions>
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
    currentMemberShip() {
      return this.$auth.account().idToken.extension_membershipTier || 'Starter';
    },
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
  .plan-title{
    font-size: 20px;
  }
  .price-title {
    font-size: 18px;
  }
  .price {
    font-size: 24px;
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
