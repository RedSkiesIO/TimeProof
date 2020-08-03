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
            <div
              class="row text-h3 text-weight-bold q-mb-md usage-color"
            >
              {{ timestampsUsed }}
            </div>
            <div class="row text-h7 text-weight-bold">
              Stamps Used
            </div>
            <div class="row text-h12">
              This Month
            </div>
          </div>
          <q-separator
            vertical
            inset
          />

          <div class="col-md-3 col-sm-2 col-xs-2 column flex-center q-mr-md q-ml-md">
            <div
              class="row text-h3 text-weight-bold q-mb-md usage-color"
            >
              {{ allowedTimestamps }}
            </div>
            <div class="row text-h7 text-weight-bold">
              Stamps Allowed
            </div>
            <div class="row text-h12">
              This Month
            </div>
          </div>
          <q-separator
            vertical
            inset
          />
          <div class="col-md-3 col-sm-2 col-xs-2 column flex-center q-mr-md q-ml-md">
            <div class="row text-h3 text-weight-bold q-mb-md usage-color">
              {{ totalUsage }}
            </div>
            <div class="row text-h7 text-weight-bold">
              Stamps Used
            </div>
            <div class="row text-h12">
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
            v-if="!upgradeIsNeeded"
            id="usageUpgradeBtn"
            flat
            no-caps
            class="shade-color"
            data-test-key="upgradeButton"
            :label="upgradeButtonLabel"
            @click.prevent="upgradeUser"
          />
          <q-btn
            v-else
            id="usageUpgradeBtn"
            outline
            no-caps
            color="red"
            data-test-key="upgradeButton"
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
      return this.allowedTimestamps - this.user.remainingTimeStamps;
    },
    totalUsage() {
      return this.user.totalTimestamps;
    },
    upgradeIsNeeded() {
      return this.user.remainingTimeStamps <= 0;
    },
    upgradeButtonLabel() {
      return this.user.remainingTimeStamps <= 0 ? this.$t('upgradeToCreateMoreProofs') : this.$t('needAnUpgrade');
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
.usage-color{
  color:#4cbbc2;
}
</style>
