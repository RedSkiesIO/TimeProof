<template>
  <div class="flex column justify-center text-center q-pa-md q-gutter-sm">
    <img
      src="~assets/signing-key.svg"
      style="height: 15vw"
    >
    <div class="text-box">
      <div class="text-h5 text-primary">
        Welcome {{ user.givenName }}!
      </div>
      <div class="text-h6">
        Before we can get you started on Trustamp we need to create your signing key
      </div>
      <div class="encrypt-desc">
        In order to keep your signing key safe
        while it's stored in your browser storage we encrypt it with a password.
      </div>
    </div>
  </div>
</template>
<script>
import User from '../../store/User';

export default {
  name: 'SigningKeyIntro',

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
