<template>
  <q-page class="flex justify-center">
    <div
      class="q-pa-md"
    >
      <a
        class="go-dashboard q-pb-sm cursor-pointer"
        @click="backToDashboard"
      >
        <span style="color:#336699">&nbsp;Go back to the dashboard</span>
      </a>
    </div>

    <div
      class="q-mt-lg justify-center"
    >
      <div v-if="!needUpgrade">
        <div class="column q-mb-md justify-center text-center text-secondary">
          <div class="col text-h5 text-weight-bold justify-center">
            Create a timestamp
          </div>
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
            @userHasReachedToLimit="needUpgrade = true"
          />
        </div>
      </div>
      <div v-else>
        <q-banner class="bg-grey-3 fixed-center absolute-center">
          <template v-slot:avatar>
            <q-icon
              name="warning"
              color="primary"
            />
          </template>
          You have exhausted your monthly allowance.
          Your allowance resets on {{ subscriptionEnd }}.
          Please upgrade from your current plan to create more timestamps.
          <template v-slot:action>
            <q-btn
              id="pagesStampUpgradeBtn"
              flat
              class="shade-color"
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
      needUpgrade: false,
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
    this.needUpgrade = this.user.remainingTimeStamps <= 0;
  },

  methods: {
    backToDashboard() {
      this.$router.push('/dashboard');
    },
  },

};
</script>
<style lang="scss">
.sign .q-card {
  padding: 0;
}

.go-dashboard:before{
  content: url('../statics/icons/left-arrow.svg');
}

.go-dashboard {
  display: flex;
  position: fixed;
  top: 12%;
  left: 15%;
  right: 0;
  color: #4cbbc2;
}
.sign .q-uploader__list {
  border: 0px dashed lightgrey;
  background-color: white;
  margin-top: 0;
}


</style>
