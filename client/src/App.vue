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
    async start() {
      console.log(this.$auth.account());
      if (!this.$auth.account()) {
        console.log('signin ', this.$auth.signIn());
      } else {
        const token = await this.$auth.getSessionToken();
        console.log(token);
      }

      if (!this.user) {
        const keypair = this.$keypair.new();
        User.insert({
          data: {
            pubKey: keypair.publicKey,
            secretKey: keypair.secretKey,
          },
        });
      }
      this.ready = true;
    },
  },
};
</script>
