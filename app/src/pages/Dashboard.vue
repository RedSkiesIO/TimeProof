<template>
  <q-page class="flex flex-center text-secondary">
    <div
      v-if="isLoggedIn"
      class="q-my-lg"
      style="width:82vw;"
    >
      <div
        class="q-gutter-x-lg q-mb-md"
      >
        <div class="col-auto">
          <div class="row text-h6 text-weight-bold">
            <q-icon
              class="icon-spacing q-mr-sm"
              name="fas fa-tachometer-alt"
              size="1.25rem"
            />
            Usage Summary
          </div>
          <Usage />
        </div>
      </div>
      <div>
        <Timestamps />
      </div>
    </div>
    <div
      v-else
      flat
      class="q-pa-xl flex flex-center column text-center"
    >
      <div class="text-h6 text-weight-bold text-grey-6">
        {{ $t('notSignedIn') }}
      </div>
      <q-btn
        unelevated
        flat
        color="blue"
        :label="$t('signUpSignIn')"
        class="q-mt-md"
        @click="$auth.signIn()"
      />
    </div>
    <q-dialog
      v-if="user"
      v-model="firstTimeDialog"
    >
      <CreateFirstTimestampPopup
        @closeDialog="closeTimestampDialog"
      />
    </q-dialog>
  </q-page>
</template>

<script>
import moment from 'moment';
import Timestamps from '../components/Timestamps';
import Usage from '../components/Usage';
import User from '../store/User';
import CreateFirstTimestampPopup from '../components/CreateFirstTimestampPopup';

export default {
  name: 'Dashboard',
  components: {
    Timestamps,
    Usage,
    CreateFirstTimestampPopup,
  },

  data() {
    return {
      firstTimeDialog: false,
    };
  },

  computed: {
    isLoggedIn() {
      return !!this.account;
    },
    account() {
      return this.$auth.account();
    },
    user() {
      return this.$auth.user();
    },
  },

  created() {
    this.checkFirstTimeDialog();
  },

  methods: {
    async closeTimestampDialog() {
      const verifyResult = await
      this.$userServer.verifyUserDetails();
      if (verifyResult) {
        await User.update({
          data: {
            accountIdentifier: this.account.accountIdentifier,
            keyEmailDate: verifyResult.keyEmailDate,
          },
        });
        this.checkFirstTimeDialog();
      }
    },

    checkFirstTimeDialog() {
      const keyMoment = moment(this.user.keyEmailDate, 'YYYY-MM-DD');
      this.firstTimeDialog = keyMoment.year() === 1;
    },
  },
};
</script>
<style lang="scss" scoped>
.verify .q-card {
  padding: 0;
}
.icon-spacing {
  margin-top: 4px;
}
</style>
