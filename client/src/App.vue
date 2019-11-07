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
// import { auth } from './helpers/adal';

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
    // auth.handleLoginCallback();
    this.start();
  },

  methods: {
    async start() {
      // if (auth.isLoggedIn) {
      //   auth.acquireTokenForAPI((error, token) => {
      //     if (!error) {
      //       console.log(token);
      //       this.$axios
      //         .get('https://easyauthtest4.azurewebsites.net/api/HttpTrigger1',
      //           {
      //             headers: {
      //               Authorization: `Bearer ${token}`,
      //               Accept: 'application/json',
      //             },
      //             params: {
      //               name: 'stephen',
      //             },
      //           })
      //         .then((data) => {
      //           console.log(data);
      //         });
      //     }
      //   });
      // }

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
