<template>
  <q-page class="flex flex-center">
    <div
      v-if="isLoggedIn"
      class="sign-verify"
    >
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
            <AddFile :mode="'sign'" />
          </q-tab-panel>

          <q-tab-panel name="verify">
            <AddFile :mode="'verify'" />
          </q-tab-panel>
        </q-tab-panels>
      </q-card>
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

export default {
  name: 'PageIndex',
  components: {
    AddFile,
  },

  data() {
    return {
      tab: 'sign',
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
  },

  // mounted() {
  //   this.$pdf();
  // },
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
</style>
