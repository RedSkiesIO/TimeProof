<template>
  <q-page class="flex justify-center">
    <div
      class="q-mt-lg"
    >
      <div class="column q-mb-md justify-center text-center text-secondary">
        <div class="col text-h4  text-weight-bold justify-center">
          {{ $t('verifyTimestamp') }}
        </div>
        <!-- <div class="col text-h6">
          Check if a timstamp exists on the blockchain
        </div> -->
      </div>

      <div
        class="row q-gutter-x-lg"
      >
        <div class="verify">
          <AddFile :mode="'verify'" />
        </div>
      </div>
    </div>
  </q-page>
</template>

<script>
import AddFile from '../components/AddFile';

export default {
  name: 'Verify',
  components: {
    AddFile,
  },
};
</script>
<style lang="scss">
.verify .q-card {
  padding: 0;
}

.verify .q-uploader__list {
    width: 50vw;
    background-color: white;
    padding: 0;
}

</style>
