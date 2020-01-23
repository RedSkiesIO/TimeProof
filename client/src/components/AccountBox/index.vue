<template>
  <div>
    <q-card
      flat
      class="left-box text-weight-bold q-px-md"
    >
      <div
        class="column justify-center q-gutter-y-md q-px-md"
        style="height:100%"
      >
        <div class="row q-gutter-x-sm q-mb-xs q-mt-lg">
          <div><q-icon name="fas fa-user" /></div>
          <div>{{ user.name }}</div>
        </div>
        <div class="row q-gutter-x-sm q-mb-xs">
          <div><q-icon name="fas fa-envelope" /></div>
          <div>{{ user.email }}</div>
        </div>
        <div
          class="row justify-center"
          style="margin-top: 9px;"
        >
          <q-item to="/account">
            <q-item-section class="text-blue text-uppercase">
              View Account
            </q-item-section>
          </q-item>
        </div>
      </div>
    </q-card>
  </div>
</template>
<script>
import User from '../../store/User';

export default {
  name: 'Account',

  data() {
    return {
      copyLabel: this.$t('copyPubKey'),
      isPwd: true,
      tiers: {
        free: 50,
        basic: 30,
        standard: 10000,
        premium: 100000,
      },
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

    key() {
      return this.user.secretKey;
    },
  },

  methods: {
    copy(text) {
      navigator.clipboard.writeText(text.toLowerCase()).then(() => {
        this.copyLabel = this.$t('copied');
        setTimeout(() => {
          this.copyLabel = this.$t('copyPubKey');
        }, 1500);
      }, (err) => {
        console.error('Async: Could not copy text: ', err);
      });
    },
  },
};
</script>
<style lang="scss">

.signing-key {
  width: 100%;
}
</style>
