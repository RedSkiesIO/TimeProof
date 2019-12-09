<template>
  <q-page class="flex justify-center">
    <div
      class="q-mt-lg"
    >
      <div class="column q-mb-md justify-center text-center text-primary">
        <div class="col text-h4  text-weight-bold justify-center">
          {{ $t('verifyTimestamp') }}
        </div>
        <div class="col text-h6">
          Check if a timstamp exists on the blockchain
        </div>
      </div>

      <div
        class="row q-gutter-x-lg"
      >
        <div>
          <q-card
            flat
          >
            <Timestamps />
          </q-card>
        </div>
      </div>
    </div>
  </q-page>
</template>

<script>
import Timestamps from '../components/Timestamps';

export default {
  name: 'Dashboard',
  components: {
    Timestamps,
  },
};
</script>
<style lang="scss">
.verify .q-card {
  padding: 0;
}

.sign .q-uploader__list {
    border-radius: 15px;
    width: 80vw;
    border: 2px dashed lightgrey;
}

</style>
