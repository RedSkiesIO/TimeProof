<template>
  <div>
    <h2>Billing Information</h2>
    <fieldset :class="{'with-state': country === 'GB'}">
      <label>
        <span>Name</span>
        <input
          v-model="name"
          data-test-key="paymentBillingName"
          name="name"
          class="field"
          placeholder="Name Surname"
          required
        >
      </label>
      <label>
        <span>Email</span>
        <input
          name="email"
          type="email"
          data-test-key="paymentBillingEmail"
          class="field"
          placeholder="contact@atlascity.io"
          :value="email"
          disabled="true"
          required
        >
      </label>
      <label>
        <span>Address</span>
        <input
          v-model="address"
          data-test-key="paymentBillingAddress"
          name="address"
          class="field"
          placeholder="5 Merchant Square"
        >
      </label>
      <label>
        <span>City</span>
        <input
          v-model="city"
          name="city"
          data-test-key="paymentBillingCity"
          class="field"
          placeholder="London"
        >
      </label>
      <label class="state">
        <span>State</span>
        <input
          v-model="state"
          data-test-key="paymentBillingState"
          name="state"
          class="field"
          placeholder="London"
        >
      </label>
      <label class="zip">
        <span>{{ zipSpanText }}</span>
        <input
          v-model="postalCode"
          data-test-key="paymentBillingPostCode"
          name="postal_code"
          class="field"
          placeholder="W2 1AY"
        >
      </label>
      <label class="select">
        <span>Country</span>
        <div
          id="country"
          :class="countryClassName"
        >
          <select
            v-model="country"
            name="country"
            data-test-key="paymentBillingCountry"
            @change.prevent="countryChange"
          >
            <option value="AU">Australia</option>
            <option value="AT">Austria</option>
            <option value="BE">Belgium</option>
            <option value="BR">Brazil</option>
            <option value="CA">Canada</option>
            <option value="CN">China</option>
            <option value="DK">Denmark</option>
            <option value="FI">Finland</option>
            <option value="FR">France</option>
            <option value="DE">Germany</option>
            <option value="HK">Hong Kong</option>
            <option value="IE">Ireland</option>
            <option value="IT">Italy</option>
            <option value="JP">Japan</option>
            <option value="LU">Luxembourg</option>
            <option value="MY">Malaysia</option>
            <option value="MX">Mexico</option>
            <option value="NL">Netherlands</option>
            <option value="NZ">New Zealand</option>
            <option value="NO">Norway</option>
            <option value="PT">Portugal</option>
            <option value="SG">Singapore</option>
            <option value="ES">Spain</option>
            <option value="SE">Sweden</option>
            <option value="CH">Switzerland</option>
            <option
              value="GB"
              selected="selected"
            >United Kingdom</option>
            <option value="US">United States</option>
          </select>
        </div>
      </label>
    </fieldset>
    <p class="tip">
      Select another country to see different payment options.
    </p>
  </div>
</template>

<script>
import config from '../Payment/config';

export default {
  props: {
    showRelevantPaymentMethods: {
      type: Function,
      required: true,
    },
    email: {
      type: String,
      required: true,
    },
  },

  data() {
    return {
      name: '',
      address: '',
      city: '',
      state: '',
      postalCode: '',
      country: config.country,
      countryClassName: 'field gb',
      zipSpanText: 'Postcode',
    };
  },
  created() {
    this.selectCountry();
  },
  mounted() {
    this.changeThePaymentButtonContent();
  },
  methods: {
    countryChange(event) {
      this.country = event.target.value;
      this.selectCountry();
    },
    selectCountry() {
      this.countryClassName = `field ${this.country.toLowerCase()}`;
      this.showRelevantFormFields();
    },
    // eslint-disable-next-line vue/no-dupe-keys
    showRelevantFormFields() {
      if (this.country === 'US') {
        this.zipSpanText = 'ZIP';
      } else if (this.country === 'GB') {
        this.zipSpanText = 'Postcode';
      } else {
        this.zipSpanText = 'Postal Code';
      }
    },

    changeThePaymentButtonContent() {
      this.showRelevantPaymentMethods(this.country);
    },

  },

};
</script>
