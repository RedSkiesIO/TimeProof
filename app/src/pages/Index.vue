<template>
  <q-page class="flex justify-center">
    <div
      v-if="isLoggedIn"
      class="q-mt-lg"
    >
      <div
        v-if="!showTimestamps"
        class="row q-gutter-x-lg"
      >
        <div class=" q-gutter-y-md column">
          <Account />
          <Usage />
          <Key />
        </div>

        <div class="sign-verify">
          <q-card
            flat
            class="sign-verify"
          >
            <q-tabs
              v-model="tab"
              dense
              class="bg-white text-secondary"
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
                    color="secondary"
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
        <div>
          <RecentTimestamps
            v-if="user.timestamps.length > 0"
            @open="showTimestamps=true"
          />
        </div>
      </div>
      <div v-else>
        <Timestamps @close="showTimestamps=false" />
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
import { mapGetters } from 'vuex';
import AddFile from '../components/AddFile';
import Usage from '../components/Usage';
import Account from '../components/Account';
import Key from '../components/Key';
import Timestamps from '../components/Timestamps';
import RecentTimestamps from '../components/RecentTimestamps';

export default {
  name: 'PageIndex',
  components: {
    AddFile,
    Usage,
    Timestamps,
    RecentTimestamps,
    Account,
    Key,
  },

  data() {
    return {
      showTimestamps: false,
      tab: 'sign',
    };
  },

  computed: {
    ...mapGetters({
      products: 'settings/getProducts',
    }),
    isLoggedIn() {
      return !!this.account;
    },
    account() {
      return this.$auth.account();
    },
    user() {
      return this.$auth.user(false, true, 'timestamps');
    },
    allowed() {
      return this.user.remainingTimeStamps > 0;
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
.sign-verify .q-card {
  padding: 0;
  border: 0;
}
.sign-verify .q-card > div:first-child {
    border-top: 1px solid rgba(0, 0, 0, 0.12);
    border-left: 1px solid rgba(0, 0, 0, 0.12);
    border-right: 1px solid rgba(0, 0, 0, 0.12);

}

.upgrade {
  width: inherit;
  max-height:inherit;
  min-width: 25rem;
  min-height: 25rem;
  border: 2px solid rgba(0, 0, 0, 0.12);
}
</style>
