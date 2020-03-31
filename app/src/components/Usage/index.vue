<template>
  <div>
    <q-card
      flat
      class="left-box"
    >
      <div
        class="justify-center q-gutter-md"
        style="height: 100%;"
      >
        <div class="col-auto row justify-between q-px-sm">
          <div class="col-3 column flex-center q-mr-md q-ml-md">
            <!-- <q-knob
              v-model="usedPercentage"
              readonly
              size="70px"
              :thickness="0.22"
              color="blue"
              track-color="blue-3"
              class="text-blue q-ma-md"
            /> -->
            <div
              class="row text-h4 text-weight-bold q-mb-md"
              :class="usedClass"
            >
              {{ timestampsUsed }}
            </div>
            <div class="row text-h8 text-weight-bold q-mb-md">
              Stamps Used
            </div>
            <div class="row text-h12 q-mb-md">
              This Month
            </div>
          </div>
          <q-separator
            vertical
            inset
          />

          <div class="col-3 column flex-center q-mr-md q-ml-md">
            <!-- <div class="column">
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
            </div> -->
            <div
              class="text-brown row text-h4 text-weight-bold q-mb-md"
            >
              {{ tiers[user.tier] }}
            </div>
            <div class="row text-h8 text-weight-bold q-mb-md">
              Stamps Allowed
            </div>
            <div class="row text-h12 q-mb-md">
              This Month
            </div>
          </div>
          <q-separator
            vertical
            inset
          />
          <div class="col-3 column flex-center q-mr-md q-ml-md">
            <div class="text-blue row text-h4 text-weight-bold q-mb-md">
              {{ totalUsage }}
            </div>
            <div class="row text-h8 text-weight-bold q-mb-md">
              Stamps Used
            </div>
            <div class="row text-h12 q-mb-md">
              All Time
            </div>
          </div>
        </div>
        <q-separator
          class="q-mt-md"
          horizontal
          inset
        />
        <div class="row justify-start items-center">
          <div class="col-md-5 q-ml-md self-center">
            <!-- <div>
              {{ $t('subscription') }}:
              <span class="text-green">
                {{ $t(user.tier) }} {{ $t('tier') }}
              </span>
            </div> -->
          </div>
          <div class="col-md-4 q-mr-md self-center">
            <!-- <div>{{ $t('moreTimestamps') }}</div> -->
            <q-btn
              outline
              no-caps
              color="secondary"
              :label="$t('needAnUpgrade')"
              @click.prevent="upgradeUser"
            />
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
      // const { tier } = this.user;

      // return `${used}/${this.tiers[tier]}`;
      return used;
    },
    totalUsage() {
      return this.user.totalTimestamps;
    },
    usedPercentage() {
      return (this.user.monthlyAllowanceUsage / this.tiers[this.user.tier]) * 100;
    },
    usedClass() {
      return this.usedPercentage > 100 ? 'text-red' : 'text-green';
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
