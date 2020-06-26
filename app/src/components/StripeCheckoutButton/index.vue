<template>
  <div>
    <stripe-checkout
      ref="checkoutRef"
      :pk="publishableKey"
      :items="[item]"
      :success-url="successUrl"
      :cancel-url="cancelUrl"
      :client-reference-id="accountId"
    >
      <template slot="checkout-button">
        <q-btn
          unelevated
          class="shade-color"
          :label="label"
          @click="checkout"
        />
      </template>
    </stripe-checkout>
  </div>
</template>
<script>
import { StripeCheckout } from 'vue-stripe-checkout';

export default {
  components: {
    StripeCheckout,
  },

  props: {
    item: {
      type: Object,
      required: true,
    },
    label: {
      type: String,
      required: true,
    },
    token: {
      type: String,
      required: true,
    },
  },

  data: () => ({
    loading: false,
    publishableKey: 'pk_test_IRxZZJoqfYY2SSVj8arguq9k00mg8SQT5R',
    successUrl: 'http://localhost:6420/#/upgrade-success',
    cancelUrl: 'http://localhost:6420/#/upgrade',
  }),
  computed: {
    accountId() {
      const account = this.$auth.account();
      return account.accountIdentifier;
    },
  },
  methods: {
    checkout() {
      this.$refs.checkoutRef.redirectToCheckout();
    },
  },
};
</script>
