<template>
  <div
    id="q-app"
  >
    <router-view :display="ready" />
  </div>
</template>

<script>
import User from './store/User';
//  import Timestamp from './store/Timestamp';

export default {
  name: 'App',

  data() {
    return {
      ready: false,
    };
  },

  computed: {
    account() {
      return this.$auth.account();
    },

    user() {
      const account = this.$auth.account();
      if (account) {
        const user = User.query().whereId(account.accountIdentifier).with('timestamps').get();
        if (user) {
          return user[0];
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
        const membership = this.account.idToken.extension_membershipTier || 'free';
        const token = await this.$auth.getToken();
        this.$axios.defaults.headers.common.Authorization = `Bearer ${token.idToken.rawIdToken}`;
        if (!this.user) {
          User.insert({
            data: {
              accountIdentifier: this.account.accountIdentifier,
              givenName: this.account.idToken.given_name,
              familyName: this.account.idToken.family_name,
              email: this.account.idToken.emails[0],
              tokenExpires: token.idToken.expiration,
              tier: membership,
            },
          });
        } else if (this.user) {
          console.log(this.account);
          User.update({
            data: {
              accountIdentifier: this.account.accountIdentifier,
              givenName: this.account.idToken.given_name,
              familyName: this.account.idToken.family_name,
              email: this.account.idToken.emails[0],
              tokenExpires: token.idToken.expiration,
              tier: membership,
            },
          });
        }
        try {
          if (!this.user.secretKey) {
            this.$router.push('/new-key');
          }

          await this.user.fetchTimestamps();

          setInterval(async () => {
            const pending = this.user.pendingTimestamps;
            if (pending && pending.length > 0) {
              await this.$web3.updateTimestamps(this.user, pending);
            }
          }, 5000);
        } catch (e) {
          console.log(e);
        }
      }

      this.ready = true;
    },
  },
};
</script>
