<template>
  <q-page class="flex justify-center">
    <div
      class="q-mt-lg justify-center"
    >
      <!-- <div class="column justify-start  text-left text-secondary">
       <div class="col text-h4  text-weight-bold justify-center">
          {{ $t('createTimestamp') }}
        </div>
        <div class="col text-h6 q-mb-sm text-weight-bold">
          <q-icon
            class="q-mr-sm"
            name="fas fa-stamp"
            size="22px"
          />
          Stamp File
        </div>
      </div> -->
      <div class="column q-mb-md justify-center text-center text-secondary">
        <div class="col text-h4 text-weight-bold justify-center">
          Create a timestamp
        </div>
        <!-- <div class="col text-h6">
          Check if a timstamp exists on the blockchain
        </div> -->
      </div>
      <div
        v-if="!key"
        class="row q-mb-lg justify-center"
      >
        <Key />
      </div>

      <div
        class="row sign"
        style="width:100%"
      >
        <AddFile
          :mode="'sign'"
        />
      </div>
    </div>
  </q-page>
</template>

<script>
import AddFile from '../components/AddFile';
import Key from '../components/KeyTopBar';

export default {
  name: 'Stamp',
  components: {
    AddFile,
    Key,
  },

  computed: {
    key() {
      return this.$store.state.settings.authenticatedAccount;
    },
  },
};
</script>
<style lang="scss">
.sign .q-card {
  padding: 0;
}

.sign .q-uploader__list {
    border: 0px dashed lightgrey;
    background-color: white;
    padding: 0;
    margin-top: 0;
}

</style>
