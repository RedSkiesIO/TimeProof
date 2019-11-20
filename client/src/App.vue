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
          const encryptedKey = await this.$crypto.encrypt(this.user.secretKey, 'password');
          console.log(encryptedKey);
          console.log(this.$base32(encryptedKey.cipherText));
          const decryptedKey = await this.$crypto.decrypt(encryptedKey, 'password');
          console.log(decryptedKey);
          User.update({
            data: {
              accountIdentifier: this.account.accountIdentifier,
              name: `${this.account.idToken.given_name} ${this.account.idToken.family_name}`,
              email: this.account.idToken.emails[0],
            },
          });
        }
        try {
          const getTotal = await this.$axios.get(`${process.env.API}GetTotalStamps${process.env.TOTAL_STAMP_KEY}`);
          const totalTs = getTotal.data.value;
          if (totalTs > 0) {
            const timestamps = await this.$axios.get(`${process.env.API}GetStamps/1/${totalTs}${process.env.GET_STAMP_KEY}`);
            const files = timestamps.data.value.map(file => ({
              txId: file.stampDocumentProof.transactionId,
              hash: file.stampDocumentProof.userProof.hash,
              signature: file.stampDocumentProof.userProof.signature,
              accountIdentifier: this.user.accountIdentifier,
              name: file.fileName,
              date: file.stampDocumentProof.timeStamp,
              type: this.re.exec(file.fileName)[1],
            }));
            await Timestamp.create({
              data: files,
            });
          }

          User.update({
            data: {
              accountIdentifier: this.account.accountIdentifier,
              timestampsUsed: this.timestampsUsed,
              totalTimestamps: totalTs,
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
