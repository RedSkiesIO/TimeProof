<template>
  <q-page class="flex justify-center text-primary">
    <div
      v-if="isLoggedIn"
      class="q-my-lg"
      style="width:71.5vw"
    >
      <div
        class="row q-gutter-x-lg q-mb-md"
      >
        <div class="col">
          <div class="row text-h6">
            <q-icon
              class="icon-spacing q-mr-sm text-weight-bold"
              name="account_circle"
              size="sm"
            />
            Your Account
          </div>
          <Account class="dash-top-box" />
        </div>
        <div class="col">
          <div class="row text-h6 text-weight-bold">
            <q-icon
              class="icon-spacing q-mr-sm"
              name="fas fa-tachometer-alt"
              size="sm"
            />
            Usage Summary
          </div>
          <Usage />
        </div>
      </div>
      <div
        v-if="user.timestamps.length > 0"
        class="row q-mt-lg q-gutter-x-lg"
      >
        <div style="width:100%">
          <Timestamps />
        </div>
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
  </q-page>
</template>

<script>
import Timestamps from '../components/Timestamps';
import Account from '../components/AccountBox';
import Usage from '../components/Usage';
import User from '../store/User';


export default {
  name: 'Dashboard',
  components: {
    Timestamps,
    Account,
    Usage,
  },

  computed: {
    isLoggedIn() {
      const account = this.$auth.account();
      if (!account || account.idToken.tfp !== 'B2C_1_TimestampSignUpSignIn') {
        return false;
      }
      return true;
    },
    account() {
      const account = this.$auth.account();
      if (!account || account.idToken.tfp !== 'B2C_1_TimestampSignUpSignIn') {
        return null;
      }
      return account;
    },
    user() {
      if (this.account) {
        const user = User.query().whereId(this.account.accountIdentifier).with('timestamps').get();
        if (user) {
          return user[0];
        }
      }
      return null;
    },
  },
};
</script>
<style lang="scss">
.verify .q-card {
  padding: 0;
}

.icon-spacing {
  margin-top: 4px;
}

.dash-top-box {
  height: 220px;
}
</style>
