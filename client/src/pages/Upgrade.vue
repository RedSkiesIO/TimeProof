<template>
  <q-page class="flex flex-center q-gutter-x-lg price-plans">
    <div
      class="row"
      style="width: 75%"
    >
      <div class="col-4">
        <q-card
          flat
        >
          <div class="top-section bg-primary text-white text-center">
            <div class="price-title">
              Starter
            </div>
            <div class="price text-weight-bold">
              Free
            </div>
            <div class="price-subtitle">
              per month
            </div>
          </div>
          <q-card-section class="text-center">
            <div class="price-timestamps">
              5 timestamps
            </div>
            <div class="price-button-container">
              <StripeButton
                label="Choose Plan"
                :item="items[0]"
              />
            </div>
          </q-card-section>
        </q-card>
      </div>
      <div class="col-4">
        <q-card
          flat
        >
          <div class="top-section bg-primary text-white text-center">
            <div class="price-title">
              Basic
            </div>
            <div class="price text-weight-bold">
              £5.99
            </div>
            <div class="price-subtitle">
              per month
            </div>
          </div>
          <q-card-section class="text-center">
            <div class="price-timestamps">
              30 timestamps
            </div>
            <div class="price-button-container">
              <StripeButton
                label="Choose Plan"
                :item="items[0]"
              />
            </div>
          </q-card-section>
        </q-card>
      </div>
      <div class="col-4">
        <q-card
          flat
        >
          <div class="top-section bg-primary text-white text-center">
            <div class="price-title">
              Premium
            </div>
            <div class="price text-weight-bold">
              £25.99
            </div>
            <div class="price-subtitle">
              per month
            </div>
          </div>
          <q-card-section class="text-center">
            <div class="price-timestamps">
              200 timestamps
            </div>
            <div class="price-button-container">
              <StripeButton
                label="Choose Plan"
                :item="items[0]"
              />
            </div>
          </q-card-section>
        </q-card>
      </div>
    </div>
  </q-page>
</template>
<script>
import StripeButton from '../components/StripeCheckoutButton';

export default {
  components: {
    StripeButton,
  },
  data: () => ({
    loading: false,
    publishableKey: 'pk_test_IRxZZJoqfYY2SSVj8arguq9k00mg8SQT5R',
    successUrl: 'http://localhost:6420/',
    cancelUrl: 'http://localhost:6420/upgrade',
    items: [
      {
        plan: 'plan_GeJGwbwuTvjjTi',
        quantity: 1,
      },
    ],
  }),

  methods: {
    checkout() {
      this.$refs.checkoutRef.redirectToCheckout();
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
