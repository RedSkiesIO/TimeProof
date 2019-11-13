<template>
  <div
    v-if="ready"
    id="q-app"
  >
    <router-view />
  </div>
</template>

<script>
import User from './store/User';

export default {
  name: 'App',

  data() {
    return {
      ready: false,
    };
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

  mounted() {
    this.start();
  },

  methods: {
    async start() {
      if (this.account) {
        const token = await this.$auth.getToken();
        this.$axios.defaults.headers.common.Authorization = `Bearer ${token.accessToken}`;
        if (!this.user) {
          const keypair = this.$keypair.new();
          User.insert({
            data: {
              accountIdentifier: this.account.accountIdentifier,
              pubKey: keypair.publicKey,
              secretKey: keypair.secretKey,
              name: `${this.account.idToken.given_name} ${this.account.idToken.family_name}`,
              email: this.account.idToken.emails[0],
            },
          });
        } else if (this.user) {
          User.update({
            data: {
              accountIdentifier: this.account.accountIdentifier,
              name: `${this.account.idToken.given_name} ${this.account.idToken.family_name}`,
              email: this.account.idToken.emails[0],
            },
          });
        }
      }

      this.ready = true;
    },
  },
};
</script>
