<template>
  <div
    class="flex column justify-center text-center q-pa-md q-gutter-sm"
    style="margin-top: 73px;"
  >
    <img
      src="~assets/signing-key.svg"
      style="height: 15vw"
    >
    <div class="text-h4 text-primary">
      Welcome to Trustamp {{ user.givenName }}!
    </div>
    <div class="row justify-center">
      <div
        class="text-body1"
        style="width: 50vw;"
      >
        To get you started on Trustamp we need to create your signing key. <br>
        Your signing key is used to prove your ownership of a timestamp.
        It's stored locally on your device so that only you have access to it.
      </div>
    </div>

    <div class="row justify-center q-mt-md">
      <q-btn
        outline
        color="primary"
        label="Create your signing key"
        style="width: 250px"
        @click="buttonAction"
      />
    </div>
  </div>
</template>
<script>
import User from '../../store/User';

export default {
  name: 'SigningKeyIntro',

  props: {
    buttonAction: {
      required: true,
      type: Function,
    },
  },

  computed: {
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
  },

};
</script>
