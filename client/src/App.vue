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

    timestampsUsed() {
      const d = new Date();
      const thisMonth = `${d.getMonth()}${d.getFullYear()}`;
      const timestamps = this.user.timestamps.filter((stamp) => {
        const date = new Date(stamp.date);
        const month = `${date.getMonth()}${date.getFullYear()}`;
        return month === thisMonth;
      });
      return timestamps.length;
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
          blockNumber: file.blockNumber,
        },
      });
    },

    async fetchTimestamps() {
      const { timestamps } = (await this.$axios.get(`https://document-timestamp.azurewebsites.net/api/getTimestamps/${this.user.accountIdentifier}`)).data;
      return timestamps.map(file => ({
        txId: file.id,
        hash: file.fileHash,
        signature: file.signature,
        pubKey: file.publicKey.toLowerCase(),
        accountIdentifier: this.user.accountIdentifier,
        name: file.fileName,
        date: file.timestamp,
        type: this.re.exec(file.fileName)[1],
        blockNumber: file.blockNumber,
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
              name: `${this.account.idToken.given_name} ${this.account.idToken.family_name}`,
              email: this.account.idToken.emails[0],
              tokenExpires: token.idToken.expiration,
            },
          });
        } else if (this.user) {
          User.update({
            data: {
              accountIdentifier: this.account.accountIdentifier,
              name: `${this.account.idToken.given_name} ${this.account.idToken.family_name}`,
              email: this.account.idToken.emails[0],
              tokenExpires: token.idToken.expiration,
            },
          });
        }
        try {
          const timestamps = await this.fetchTimestamps();

          const pendingStamps = timestamps.filter(({ blockNumber }) => blockNumber === -1);

          if (pendingStamps) await this.$web3.updateTimestamps(pendingStamps);

          await Timestamp.create({
            data: timestamps,
          });

          User.update({
            data: {
              accountIdentifier: this.account.accountIdentifier,
              timestampsUsed: this.timestampsUsed,
              totalTimestamps: timestamps.length,
            },
          });
        } catch (e) {
          console.log(e);
        }
      }

      this.ready = true;
    },
  },
};
</script>
