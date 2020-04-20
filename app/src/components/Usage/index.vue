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
        <div class="col-auto row justify-between">
          <div class="col-md-3 col-sm-2 col-xs-2 column flex-center q-mr-md q-ml-md">
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

          <div class="col-md-3 col-sm-2 col-xs-2 column flex-center q-mr-md q-ml-md">
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
              {{ allowedTimestamps }}
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
          <div class="col-md-3 col-sm-2 col-xs-2 column flex-center q-mr-md q-ml-md">
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
        <div class="row justify-center items-center">
          <q-btn
            outline
            no-caps
            :color="upgradeButtonColor"
            :label="upgradeButtonLabel"
            @click.prevent="upgradeUser"
          />
        </div>
      </div>
    </q-card>
  </div>
</template>
<script>
import { mapGetters } from 'vuex';

export default {
  name: 'UsageSummary',

  computed: {
    ...mapGetters({
      products: 'settings/getProducts',
    }),
    user() {
      return this.$auth.user(false, true, 'timestamps');
    },
    allowedTimestamps() {
      return this.products[this.user.tier].noOfStamps;
    },
    timestampsUsed() {
      return this.user.monthlyAllowanceUsage;
    },
    totalUsage() {
      return this.user.totalTimestamps;
    },
    usedPercentage() {
      return (this.user.monthlyAllowanceUsage / this.allowedTimestamps) * 100;
    },
    usedClass() {
      return this.usedPercentage >= 100 ? 'text-red' : 'text-green';
    },
    upgradeButtonColor() {
      return this.usedPercentage >= 100 ? 'red' : 'secondary';
    },
    upgradeButtonLabel() {
      return this.usedPercentage >= 100 ? this.$t('upgradeToCreateMoreProofs') : this.$t('needAnUpgrade');
    },
  },

  methods: {
    upgradeUser() {
      this.$router.push('/upgrade');
    },

  },
};
</script>
<style lang="scss" scoped>
.usage-summary {
    border: 2px solid rgba(0, 0, 0, 0.12);
    max-width: 400px;
}
</style>
