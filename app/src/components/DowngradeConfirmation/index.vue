<template>
  <q-card
    flat
    class="q-pa-sm"
  >
    <q-card-section class="row items-center">
      <div>
        <div
          class="row justify-center text-weight-bold text-h6 q-mb-xs"
        >
          <div>
            {{ $t('downgradePlanTitle') }}
          </div>
        </div>
        <hr>
        <div
          class="row text-center justify-center"
        >
          {{ $t('downgradePlanContent') }}
          <br>
          <br>
        </div>
      </div>
    </q-card-section>

    <q-card-actions align="center">
      <div class="row justify-center q-mb-sm">
        <q-btn
          id="cancelDowngradeBtn"
          v-close-popup
          flat
          data-test-key="cancelDowngrade"
          :label="$t('cancel')"
          class="shade-color q-mr-md"
          @click="buttonClose"
        />
        <q-btn
          id="confirmDowngradeBtn"
          flat
          data-test-key="confirmDowngrade"
          :label="$t('confirm')"
          class="shade-color q-ml-md"
          @click="buttonAction"
        />
      </div>
    </q-card-actions>
  </q-card>
</template>
<script>

export default {
  name: 'DowngradeConfirmation',

  methods: {
    async buttonAction() {
      this.$emit('confirmDialog');
    },
    async buttonClose() {
      this.$emit('closeDialog');
    },

  },
};
</script>
