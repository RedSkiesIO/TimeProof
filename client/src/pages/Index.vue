<template>
  <q-page class="flex flex-center">
    <div
      v-if="isLoggedIn"
      class="row q-gutter-x-lg"
    >
      <Usage />
      <div class="sign-verify">
        <q-card
          flat
          class="sign-verify"
        >
          <q-tabs
            v-model="tab"
            dense
            class="bg-white text-primary"
          >
            <q-tab
              name="sign"
              :label="$t('sign')"
            />
            <q-tab
              name="verify"
              :label="$t('verify')"
            />
          </q-tabs>
          <q-tab-panels
            v-model="tab"
            animated
          >
            <q-tab-panel name="sign">
              <AddFile
                v-if="allowed"
                :mode="'sign'"
              />
              <div
                v-else
                class="column q-gutter-y-md q-pa-md upgrade flex flex-center text-center"
              >
                <div class="text-h6 text-weight-bold text-grey-8">
                  {{ $t('noAllowance') }} <br> {{ $t('timestampAllowance') }}
                </div>
                <div class="text-body1 text-weight-bold">
                  {{ $t('pleaseUpgrade') }}
                </div>
                <q-btn
                  outline
                  color="primary"
                  :label="$t('upgrade')"
                />
              </div>
            </q-tab-panel>

            <q-tab-panel name="verify">
              <AddFile :mode="'verify'" />
            </q-tab-panel>
          </q-tab-panels>
        </q-card>
      </div>
    </div>

    <q-card
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
    </q-card>
  </q-page>
</template>

<script>
import AddFile from '../components/AddFile';
import Usage from '../components/Usage';
import User from '../store/User';

export default {
  name: 'PageIndex',
  components: {
    AddFile,
    Usage,
  },

  data() {
    return {
      tab: 'sign',
      tiers: {
        free: 50,
        basic: 1000,
        standard: 10000,
        premium: 100000,
      },
    };
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
        const user = User.find(this.account.accountIdentifier);
        if (user) {
          return user;
        }
      }
      return null;
    },
    allowed() {
      if (this.user.timestampsUsed < this.tiers[this.user.tier]) {
        return true;
      }
      return false;
    },
  },
};
</script>
<style lang="scss">
.q-tab-panel {
  padding: 0;
  height: inherit;
}

.q-panel {
  height: inherit;
}

.sign-verify .q-card > div:first-child {
    border-top: 2px solid rgba(0, 0, 0, 0.12);
    border-left: 2px solid rgba(0, 0, 0, 0.12);
    border-right: 2px solid rgba(0, 0, 0, 0.12);

}

.upgrade {
  width: inherit;
  max-height:inherit;
  min-width: 25rem;
  min-height: 25rem;
  border: 2px solid rgba(0, 0, 0, 0.12);
}
</style>
