<template>
  <q-card
    flat
    class="q-pa-sm"
  >
    <div v-if="mode==='new' || mode==='import'">
      <div
        class="row justify-center text-weight-bold text-h6 q-mb-xs"
      >
        <div v-if="mode==='new'">
          {{ $t('createKeyLabel') }}
        </div>
        <div v-else>
          {{ $t('encryptKey') }}
        </div>
      </div>
      <div class="row justify-center q-mb-sm text-weight-bold">
        {{ $t('newKeyDesc') }}
      </div>
      <div
        v-if="user.pubKey"
        class="row text-center justify-center"
      >
        {{ $t('overwriteKey') }} <br>
        {{ $t('overwriteKeyDesc') }}
      </div>
    </div>

    <div v-if="mode==='unlock'">
      <div
        class="row justify-center text-weight-bold text-h6 q-mb-xs"
      >
        <div>{{ $t('unlockKey') }}</div>
      </div>
      <div class="row justify-center">
        {{ $t('unlockKeyDesc') }}
      </div>
    </div>

    <div class="row">
      <q-input
        ref="passwordInput"
        v-model="password"
        :label="$t('enterPassword')"
        :type="isPwd ? 'password' : 'text'"
        :error="!isValid"
        :rules="mode !=='unlock' ?
          [ val => val && val.length >= 9 || $t('invalidPasswordLength')] : []"
        class="q-ma-sm signing-key"
        @keyup.enter="buttonAction"
      >
        <template v-slot:append>
          <q-icon
            :name="isPwd ? 'visibility_off' : 'visibility'"
            class="cursor-pointer"
            @click="isPwd = !isPwd"
          />
        </template>
        <template v-slot:error>
          {{ $t('wrongPassword') }}
        </template>
      </q-input>
    </div>
    <div class="row justify-center q-mb-sm">
      <q-btn
        outline
        :label="buttonLabel"
        color="primary"
        @click="buttonAction"
      />
    </div>
    <div
      v-if="mode==='new' || mode==='import'"
      class="q-pa-md justify-center text-center text-weight-bold"
    >
      <span class="text-red">{{ $t('doNotForget') }}</span> {{ $t('savePassword') }}
      <br><span class="text-red">{{ $t('passwordKeystore') }}</span>
      {{ $t('unlockSigningKey') }}
    </div>
  </q-card>
</template>
<script>
import User from '../../../store/User';

export default {
  name: 'NewKey',

  props: {
    mode: {
      type: String,
      required: true,
    },
    keypair: {
      default: null,
      type: Object,
      required: false,
    },
  },

  data() {
    return {
      password: null,
      isPwd: true,
      isValid: true,
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
    buttonLabel() {
      if (this.mode === 'new') {
        return this.$t('createKeyLabel');
      }
      if (this.mode === 'import') {
        return this.$t('importKeyLabel');
      }
      return this.$t('unlockKeyLabel');
    },
  },

  methods: {
    async buttonAction() {
      try {
        if (this.mode === 'new') {
          await this.addKey(this.password);
        }
        if (this.mode === 'import') {
          await this.addKey(this.password);
        }
        await this.unlockKey(this.password);
      } catch (e) {
        console.log('ERROR: ', e);
      }
    },


    async addKey(password) {
      this.$refs.passwordInput.validate();
      if (!this.$refs.passwordInput.hasError()) {
        let { keypair } = this;

        if (this.mode === 'new') {
          keypair = this.$keypair.new();
        }
        const encrypted = await this.$crypto.encrypt(keypair.secretKey, password);

        await User.update({
          data: {
            accountIdentifier: this.account.accountIdentifier,
            pubKey: keypair.publicKey,
            secretKey: encrypted,
          },
        });

        await this.unlockKey(password);
      }
    },

    async unlockKey(password) {
      const decrypted = await this.$crypto.decrypt(this.user.secretKey, password);
      if (decrypted) {
        await this.$store.dispatch('settings/setAuthenticatedAccount', decrypted);
        this.$emit('sign');
        this.$emit('closeUnlock');
        this.$emit('close');
      } else {
        this.isValid = false;
        setTimeout(() => {
          this.isValid = true;
        }, 2000);
      }
    },
  },
};
</script>
