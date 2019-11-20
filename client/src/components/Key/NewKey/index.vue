<template>
  <q-card
    flat
    class="account q-pa-sm"
  >
    <div class="row justify-center text-weight-bold text-h6 q-mb-xs">
      <div>{{ $t('createKeyLabel') }}</div>
    </div>
    <div class="row justify-center">
      {{ $t('newKeyDesc') }}
    </div>
    <div class="row">
      <q-input
        v-model="password"
        :label="$t('enterPassword')"
        :type="isPwd ? 'password' : 'text'"
        class="q-my-sm signing-key"
      >
        <template v-slot:append>
          <q-icon
            :name="isPwd ? 'visibility_off' : 'visibility'"
            class="cursor-pointer"
            @click="isPwd = !isPwd"
          />
        </template>
      </q-input>
    </div>
    <div class="row justify-center q-mb-sm">
      <q-btn
        outline
        :label="$t('createKeyLabel')"
        color="primary"
        @click="addKey(password)"
      />
    </div>
  </q-card>
</template>
<script>
import User from '../../../store/User';

export default {
  name: 'NewKey',
  data() {
    return {
      password: null,
      isPwd: true,
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

  methods: {
    async addKey(password) {
      const keypair = this.$keypair.new();
      const encrypted = await this.$crypto.encrypt(keypair.secretKey, password);

      await User.update({
        data: {
          accountIdentifier: this.account.accountIdentifier,
          pubKey: keypair.publicKey,
          secretKey: encrypted,
        },
      });

      await this.unlockKey(password);
    },

    async unlockKey(password) {
      const decrypted = await this.$crypto.decrypt(this.user.secretKey, password);
      this.$store.dispatch('settings/setAuthenticatedAccount', decrypted);
    },
  },
};
</script>
