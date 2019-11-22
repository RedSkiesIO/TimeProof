<template>
  <div>
    <q-card
      v-if="!importKey && !importKeystore"
      class="q-pa-lg q-gutter-y-sm justify-center"
    >
      <div class="row text-h6 text-weight-boldtext-center justify-center">
        {{ $t('importKey') }}
      </div>
      <div class="row  q-mb-sm text-center justify-center">
        {{ $t('importKeyDesc') }}
      </div>
      <div class="column q-gutter-y-md justify-center">
        <q-btn
          outline
          color="primary"
          label="Keystore file"
          @click="importKeystore=true"
        />
        <q-btn
          outline
          color="primary"
          label="Private Key"
          @click="importKey=true"
        />
      </div>
    </q-card>
    <q-card
      v-if="importKey && !keypair"
      class="q-pa-lg q-gutter-y-sm justify-center"
    >
      <div class="row">
        {{ $t('enterSigningKey') }}
      </div>
      <div class="row">
        <q-input
          v-model="secretKey"
          :label="$t('signingKey')"
          :type="isPwd ? 'password' : 'text'"
          :error="!isValid"
          class="q-my-sm signing-key"
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
      <div class="row justify-center">
        <q-btn
          outline
          :label="$t('addKey')"
          color="primary"
          @click="importFromKey()"
        />
      </div>
    </q-card>
    <q-card
      v-if="importKeystore && !keypair"
      class="q-pa-lg q-gutter-y-sm justify-center"
    >
      <div class="row">
        {{ $t('importKeystoreFile') }}
      </div>
      <div class="row">
        <q-input
          filled
          type="file"
          hint="Native file"
          @input="val => { file = val[0] }"
        />
      </div>
      <div class="row">
        <q-btn
          outline
          :label="$t('addKey')"
          color="primary"
          @click="importFromKeystore()"
        />
      </div>
    </q-card>
    <Encrypt
      v-if="openEncrypt"
      :keypair="keypair"
      mode="import"
      @closeUnlock="$emit('close')"
    />
  </div>
</template>
<script>
import Encrypt from '../NewKey';
import User from '../../../store/User';


export default {
  components: {
    Encrypt,
  },

  data() {
    return {
      importKey: false,
      importKeystore: false,
      secretKey: null,
      isValid: true,
      isPwd: true,
      keypair: null,
      openEncrypt: false,
      file: null,
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
  },

  methods: {
    async importFromKey() {
      this.keypair = this.$keypair.keypairFromSecretKey(this.secretKey);
      this.openEncrypt = true;
    },

    async importFromKeystore() {
      console.log(this.file);
      const reader = await new FileReader();
      reader.onload = (evt) => {
        try {
          const json = JSON.parse(evt.target.result);
          if (json.cipher) {
            console.log('called');
            this.$store.dispatch('settings/setAuthenticatedAccount', null);
            User.update({
              data: {
                accountIdentifier: this.account.accountIdentifier,
                pubKey: json.pubKey,
                secretKey: json.cipher,
              },
            });
          } else {
            console.log('invalid keystore file');
          }
          console.log(json);
        } catch (e) {
          console.log(e);
        }
      };
      reader.readAsText(this.file);
    },
  },
};
</script>
