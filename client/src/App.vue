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
      return this.$store.state.settings.authenticatedAccount;
    },

    user() {
      if (User.all().length > 0) {
        return User.query().first();
      }
      return null;
    },
  },

  mounted() {
    this.start();
  },

  methods: {
    start() {
      this.$auth.signIn();
      // if (!this.account) {
      if (!this.user) {
        const keypair = this.$keypair.new();
        User.insert({
          data: {
            pubKey: keypair.publicKey,
            secretKey: keypair.secretKey,
          },
        });
        // this.$router.push({ path: '/register' });
      }
      // this.$router.push({ path: '/login' });
      // }
      this.ready = true;
    },
  },
};
</script>
