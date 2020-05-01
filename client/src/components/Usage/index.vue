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
            <div class="column q-gutter-y-sm">
              <div>{{ $t('moreTimestamps') }}</div>
              <q-btn
                outline
                color="secondary"
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
import { mapGetters } from 'vuex';
import User from '../../store/User';

export default {
  name: 'UsageSummary',

  computed: {
    ...mapGetters({
      products: 'settings/getProducts',
    }),
    account() {
      return this.$auth.account();
    },
    user() {
      return this.$auth.user(false, true, 'timestamps');
    },
    allowedTimestamps(){
      return this.products[this.user.tier].noOfStamps;
    },
    timestampsUsed() {
      const used = this.allowedTimestamps - this.user.remainingTimeStamps;

      return `${used}/${this.allowedTimestamps}`;
    },
    usedPercentage() {
      return ((this.allowedTimestamps - this.user.remainingTimeStamps) / this.allowedTimestamps) * 100;
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
