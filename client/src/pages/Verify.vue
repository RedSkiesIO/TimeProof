<template>
  <q-page class="flex justify-center">
    <div
      v-if="isLoggedIn"
      class="q-mt-lg"
    >
      <div class="row q-mb-md justify-center text-h5 text-weight-bold">
        {{ $t('verifyTimestamp') }}
      </div>
      <div
        class="row q-gutter-x-lg"
      >
        <div class="verify">
          <q-card
            flat
          >
            <AddFile :mode="'verify'" />
          </q-card>
        </div>
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

export default {
  name: 'Verify',
  components: {
    AddFile,
  },

  data() {
    return {
      tab: 'verify',
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
};
</script>
<style lang="scss">
.verify .q-card {
  padding: 0;
}

.verify .q-uploader__list {
    border-radius: 15px;
}

</style>
