<template>
  <div>
    <q-card
      flat
      class=" usage-summary q-pa-sm"
    >
      <div class="row text-weight-bold text-h6">
        {{ $t('usageSummary') }}
      </div>
      <div class="row q-gutter-x-md">
        <div class="column flex-center">
          <q-knob
            v-model="value"
            readonly
            size="70px"
            :thickness="0.22"
            color="blue"
            track-color="blue-3"
            class="text-blue q-ma-md"
          />
          <div>
            {{ $t('timestampsUsed') }}: 22/50
          </div>
        </div>
        <div class="column q-gutter-y-md">
          <div class="column">
            <div>{{ $t('subscription') }}:</div>
            <div class="text-green">
              {{ $t('freeTier') }}
            </div>
          </div>
          <div class="column q-gutter-y-sm">
            <div>{{ $t('moreTimestamps') }}</div>
            <q-btn :label="$t('upgrade')" />
          </div>
        </div>
      </div>
    </q-card>
  </div>
</template>
<script>
export default {
  name: 'UsageSummary',
  data() {
    return {
      value: 22,
    };
  },
};
</script>
<style lang="scss">
.usage-summary {
    border: 2px solid rgba(0, 0, 0, 0.12);
}
</style>
