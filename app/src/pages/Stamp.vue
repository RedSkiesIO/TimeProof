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
      <div v-if="!userHasReachedToLimit">
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
      <div v-else>
        <q-banner class="bg-grey-3 fixed-center absolute-center">
          <template v-slot:avatar>
            <q-icon
              name="signal_wifi_off"
              color="primary"
            />
          </template>
          You have exhausted your monthly allowance.
          Your allowance resets on {{ subscriptionEnd }}.
          Please upgrade from your current plan to create more timestamps.
          <template v-slot:action>
            <q-btn
              flat
              color="primary"
              label="Upgrade"
              @click="$router.push('/upgrade')"
            />
          </template>
        </q-banner>
      </div>
    </div>
  </q-page>
</template>

<script>
import moment from 'moment';
import AddFile from '../components/AddFile';
import Key from '../components/KeyTopBar';

export default {
  name: 'Stamp',
  components: {
    AddFile,
    Key,
  },
  data() {
    return {
      userHasReachedToLimit: false,
    };
  },
  computed: {
    key() {
      return this.$store.state.settings.authenticatedAccount;
    },
    user() {
      return this.$auth.user(false, true, 'timestamps');
    },
    subscriptionEnd() {
      return moment(this.user.membershipRenewDate).format('DD/MM/YYYY');
    },
  },
  mounted() {
    console.log('STAMP MOUNTED');
    this.userHasReachedToLimit = this.user.remainingTimeStamps <= 0;
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
