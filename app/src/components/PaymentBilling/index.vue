<template>
  <div>
    <h2>Billing Information</h2>
    <fieldset :class="{'with-state': country === 'GB'}">
      <label>
        <span>Name</span>
        <input
          v-model="name"
          name="name"
          class="field"
          placeholder="Jenny Rosen"
          required
        >
      </label>
      <label>
        <span>Email</span>
        <input
          name="email"
          type="email"
          class="field"
          placeholder="jenny@example.com"
          :value="email"
          disabled="true"
          required
        >
      </label>
      <label>
        <span>Address</span>
        <input
          v-model="address"
          name="address"
          class="field"
          placeholder="185 Berry Street Suite 550"
        >
      </label>
      <label>
        <span>City</span>
        <input
          v-model="city"
          name="city"
          class="field"
          placeholder="San Francisco"
        >
      </label>
      <label class="state">
        <span>State</span>
        <input
          v-model="state"
          name="state"
          class="field"
          placeholder="CA"
        >
      </label>
      <label class="zip">
        <span>{{ zipSpanText }}</span>
        <input
          v-model="postalCode"
          name="postal_code"
          class="field"
          placeholder="94107"
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
      country: '',
      countryClassName: 'field gb',
      zipSpanText: 'ZIP',
    };
  },
  mounted() {
    this.country = config.country;
    this.selectCountry(this.country);
  },
  methods: {
    countryChange(event) {
      this.selectCountry(event.target.value);
    },
    selectCountry(country) {
      // const selector = document.getElementById('country');
      // selector.querySelector(`option[value=${country}]`).selected = 'selected';
      this.countryClassName = `field ${country.toLowerCase()}`;
      // Trigger the methods to show relevant fields and payment methods on page load.
      this.showRelevantFormFields(country);
      this.showRelevantPaymentMethods(country);
    },
    // Show only form fields that are relevant to the selected country.
    // eslint-disable-next-line vue/no-dupe-keys
    showRelevantFormFields(country) {
      if (country === 'US') {
        this.zipSpanText = 'ZIP';
      } else if (country === 'GB') {
        this.zipSpanText = 'Postcode';
      } else {
        this.zipSpanText = 'Postal Code';
      }
    },

  },

};
</script>
