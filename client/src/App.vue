<template>
  <div
    id="q-app"
  >
    <router-view :display="ready" />
  </div>
</template>

<script>
import User from './store/User';
import Timestamp from './store/Timestamp';

export default {
  name: 'App',

  data() {
    return {
      ready: false,
      re: /(?:\.([^.]+))?$/,
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
        const user = User.query().whereId(this.account.accountIdentifier).with('timestamps').get();
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
    async fetchTimestamps() {
      const { timestamps } = (await this.$axios.get(`https://document-timestamp.azurewebsites.net/api/getTimestamps/${this.user.accountIdentifier}`)).data;
      return timestamps.map(file => ({
        txId: file.id,
        hash: file.fileHash,
        signature: file.signature,
        pubKey: file.publicKey.toLowerCase(),
        accountIdentifier: this.user.accountIdentifier,
        name: file.fileName,
        date: Number(file.timestamp),
        type: this.re.exec(file.fileName)[1],
        blockNumber: Number(file.blockNumber),
      }));
    },


    async start() {
      if (this.account) {
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
            },
          });
        }
        try {
          if (!this.user.secretKey) {
            this.$router.push('/new-key');
          }
          const timestamps = await this.fetchTimestamps();

          await Timestamp.create({
            data: timestamps,
          });

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
