<template>
  <div>
    <q-card
      flat
      class="dash-top-box left-box"
    >
      <div
        class="column justify-center"
        style="height: 100%;"
      >
        <div class="row justify-between">
          <div class="col-auto column flex-center">
            <q-knob
              v-model="usedPercentage"
              readonly
              size="70px"
              :thickness="0.22"
              color="blue"
              track-color="blue-3"
              class="text-blue q-ma-md"
            />
            <div>
              {{ $t('timestampsUsed') }}: {{ timestampsUsed }}
            </div>
          </div>
          <div class="col-auto column q-gutter-y-md">
            <div class="column">
              <div>{{ $t('subscription') }}:</div>
              <div class="text-green">
                {{ $t(user.tier) }} {{ $t('tier') }}
              </div>
            </div>
            <div class="column q-gutter-y-sm">
              <div>{{ $t('moreTimestamps') }}</div>
              <q-btn
                outline
                color="primary"
                :label="$t('upgrade')"
              />
            </div>
          </div>
        </div>
      </div>
    </q-card>
  </div>
</template>
<script>
import User from '../../store/User';

export default {
  name: 'UsageSummary',
  data() {
    return {
      tiers: {
        free: 50,
        basic: 1000,
        standard: 10000,
        premium: 100000,
      },
    };
  },

  computed: {
    account() {
      const account = this.$auth.account();
      if (!account || account.idToken.tfp !== 'B2C_1_TimestampSignUpSignIn') {
        return null;
      }
      return account;
    },

    user() {
      if (this.account) {
        const user = User.query().whereId(this.account.accountIdentifier).with('timestamps').get();
        if (user) {
          return user[0];
        }
      }
      return null;
    },
    timestampsUsed() {
      const used = this.user.timestampsUsed;
      const { tier } = this.user;

      return `${used}/${this.tiers[tier]}`;
    },
    usedPercentage() {
      return (this.user.timestampsUsed / this.tiers[this.user.tier]) * 100;
    },

  },
};
</script>
<style lang="scss">
.usage-summary {
    border: 2px solid rgba(0, 0, 0, 0.12);
    max-width: 350px;
}
</style>
