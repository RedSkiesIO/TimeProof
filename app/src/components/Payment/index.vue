<template>
  <div>
    <h1>Please give us your payment details:</h1>
    <card
      class="stripe-card"
      :class="{ complete }"
      stripe="stripeKey"
      :options="stripeOptions"
      @change="complete = $event.complete"
    />
    <button
      class="pay-with-stripe"
      :disabled="!complete"
      @click="pay"
    >
      Pay with credit card
    </button>
  </div>
</template>

<script>
import { Card, createToken } from 'vue-stripe-elements-plus';
import { stripeKey, stripeOptions } from '../../../stripeConfig.json';

export default {

  components: { Card },
  data() {
    return {
      complete: false,
      stripeKey,
      stripeOptions,

      // see https://stripe.com/docs/stripe.js#element-options for details

    };
  },

  methods: {
    pay() {
      // createToken returns a Promise which resolves in a result object with
      // either a token or an error key.
      // See https://stripe.com/docs/api#tokens for the token object.
      // See https://stripe.com/docs/api#errors for the error object.
      // More general https://stripe.com/docs/stripe.js#stripe-create-token.
      createToken().then(data => console.log(data.token));
    },
  },
};
</script>

<style>
.stripe-card {
  width: 300px;
  border: 1px solid grey;
}
.stripe-card.complete {
  border-color: green;
}
</style>
