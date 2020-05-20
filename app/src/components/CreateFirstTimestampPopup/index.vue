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
            {{ $t('createFirstTimeStamp') }}
          </div>
        </div>
        <hr>
        <div
          class="row text-center justify-center"
        >
          {{ $t('createFirstTimeStampContent') }}
          <br>
          <br>
        </div>
      </div>
    </q-card-section>

    <q-card-actions align="center">
      <div class="row justify-center q-mb-sm">
        <q-btn
          v-close-popup
          flat
          data-test-key="cancelCreateFirstTimeStamp"
          :label="$t('later')"
          color="blue"
          @click="buttonClose"
        />
        <q-btn
          flat
          data-test-key="continueCreateFirstTimeStamp"
          :label="$t('sign')"
          color="blue"
          @click="buttonAction"
        />
      </div>
    </q-card-actions>
  </q-card>
</template>
<script>

export default {
  name: 'CreateFirstTimestampPopup',

  methods: {
    async buttonAction() {
      this.$emit('closeDialog');
      this.$router.push('/stamp');
    },
    async buttonClose() {
      this.$emit('closeDialog');
    },

  },
};
</script>
