<template>
  <div>
    <q-card
      flat
      class="left-box"
    >
      <div
        class="column justify-center"
        style="height: 100%;"
      >
        <div class="row justify-between q-px-sm">
          <div class="col-auto column flex-center q-mr-md">
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
            <div
              class="column"
            >
              <div>{{ $t('Allowance') }}:</div>
              <div class="text-green">
                {{ tiers[user.tier] }} timestamps p/m
              </div>
            </div>
          </div>
          <div class="col-auto column q-gutter-y-md">
            <div
              class="column"
            >
              <div>{{ $t('moreTimestamps') }}</div>
              <q-btn
                outline
                color="secondary"
                :label="$t('upgrade')"
                @click.prevent="upgradeUser"
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
        free: 10,
        basic: 30,
        premium: 200,
      },
    };
  },

  computed: {
    account() {
      const account = this.$auth.account();
      if (!account || account.idToken.tfp !== 'B2C_1_SignUpSignIn') {
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
      const used = this.user.monthlyAllowanceUsage;
      const { tier } = this.user;

      return `${used}/${this.tiers[tier]}`;
    },
    usedPercentage() {
      return (this.user.monthlyAllowanceUsage / this.tiers[this.user.tier]) * 100;
    },
  },

  methods: {
    upgradeUser() {
      this.$router.push('/upgrade');
    },

  },
};
</script>
<style lang="scss">
.usage-summary {
    border: 2px solid rgba(0, 0, 0, 0.12);
    max-width: 400px;
}
</style>
