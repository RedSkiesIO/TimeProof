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
import Timestamp from './store/Timestamp';

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
    async insertTimestamp(file) {
      Timestamp.insert({
        data: {
          txId: file.txId,
          hash: file.base32Hash,
          signature: file.signature,
          accountIdentifier: this.user.accountIdentifier,
          name: file.name,
          date: file.timestamp,
          type: file.type,
          size: file.size,
        },
      });
    },
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

        const getTotal = await this.$axios.get(`${process.env.API}GetTotalStamps${process.env.TOTAL_STAMP_KEY}`);
        const totalTs = getTotal.data.value;
        if (totalTs > 0) {
          const timestamps = await this.$axios.get(`${process.env.API}GetStamps/1/${totalTs}${process.env.GET_STAMP_KEY}`);
          console.log(timestamps);
          const files = timestamps.data.value.map(file => ({
            txId: file.stampDocumentProof.transactionId,
            hash: file.stampDocumentProof.userProof.hash,
            signature: file.stampDocumentProof.userProof.signature,
            accountIdentifier: this.user.accountIdentifier,
            name: file.fileName,
            date: file.stampDocumentProof.timeStamp,
          }));
          await Timestamp.create({
            data: files,
          });
        }
        User.update({
          data: {
            accountIdentifier: this.account.accountIdentifier,
            totalTimestamps: totalTs,
          },
        });
      }

      this.ready = true;
    },
  },
};
</script>
