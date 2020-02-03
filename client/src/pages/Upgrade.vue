<template>
  <q-page class="flex flex-center q-gutter-x-lg price-plans">
    <q-card
      flat
    >
      <div class="top-section bg-primary text-white text-center">
        <div>
          Starter
        </div>
        <div>
          Free
        </div>
      </div>
      <q-card-section class="text-center">
        5 timestamps per month
        <StripeButton
          label="Choose Plan"
          :item="items[0]"
        />
      </q-card-section>
    </q-card>
    <q-card
      flat
    >
      <q-card-section class="text-center">
        30 timestamps per month
        <StripeButton
          label="Choose Plan"
          :item="items[0]"
        />
      </q-card-section>
    </q-card>
    <q-card
      flat
    >
      <q-card-section class="text-center">
        200 timestamps per month
        <StripeButton
          label="Choose Plan"
          :item="items[0]"
        />
      </q-card-section>
    </q-card>
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
    items: [
      { plan: 'plan_GeJGwbwuTvjjTi', quantity: 1 },
    ],
    successUrl: 'http://localhost:6420/',
    cancelUrl: 'http://localhost:6420/upgrade',
  }),
  methods: {
    checkout() {
      this.$refs.checkoutRef.redirectToCheckout();
    },
  },
};
</script>
<style>

</style>
