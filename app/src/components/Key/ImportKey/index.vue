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
          id="importKeyKeystoreFileBtn"
          outline
          class="shade-color"
          :label="$t('keystoreFile')"
          @click="importKeystore=true"
        />
        <q-btn
          id="importKeyPrivateKeyBtn"
          outline
          class="shade-color"
          :label="$t('privateKey')"
          @click="importKey=true"
        />
      </div>
    </q-card>
    <q-card
      v-if="importKey && !keypair"
      class="q-pa-lg q-gutter-y-sm justify-center"
    >
      <div class="row text-h6 text-weight-bold justify-center">
        {{ $t('enterSigningKey') }}
      </div>
      <div class="row justify-center">
        <q-input
          v-model="secretKey"
          :label="$t('signingKey')"
          :type="isPwd ? 'password' : 'text'"
          :error="!isValid"
          class="q-my-sm justify-center signing-key"
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
          id="importKeyAddKeyBtn"
          flat
          :label="$t('addKey')"
          class="shade-color"
          @click="importFromKey()"
        />
      </div>
    </q-card>
    <q-card
      v-if="importKeystore && !keypair"
      class="q-pa-lg q-gutter-y-sm justify-center"
    >
      <div class="row text-center justify-center text-h6 text-weight-bold">
        {{ $t('importKeystoreFile') }}
      </div>
      <div
        v-if="user.pubKey"
        class="row text-center justify-center"
      >
        {{ $t('overwriteKey') }} <br>
        {{ $t('overwriteKeyDesc') }}
      </div>
      <div class="row justify-center">
        <q-input
          type="file"
          :error="!isValid"
          @change="validFile"
        >
          <template v-slot:error>
            {{ $t('invalidKeystore') }}
          </template>
        </q-input>
      </div>
      <div class="row justify-center">
        <q-btn
          id="importKeyStoreAddKeyBtn"
          flat
          :label="$t('addKey')"
          class="shade-color"
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
      importKeystore: true,
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
      return this.$auth.account();
    },
    user() {
      return this.$auth.user();
    },
  },

  methods: {
    validFile(e) {
      // eslint-disable-next-line prefer-destructuring
      try {
        // eslint-disable-next-line prefer-destructuring
        this.file = e.target.files[0];
        if (this.file.type !== 'text/plain') {
          this.isValid = false;
        } else {
          this.isValid = true;
        }
      } catch (ex) {
        console.log(ex);
      }
    },
    async importFromKey() {
      this.keypair = this.$keypair.keypairFromSecretKey(this.secretKey);
      this.openEncrypt = true;
    },

    async importFromKeystore() {
      const reader = await new FileReader();
      reader.onload = async (evt) => {
        try {
          const json = JSON.parse(evt.target.result);
          if (json.cipher) {
            this.$store.dispatch('settings/setAuthenticatedAccount', null);
            await User.update({
              data: {
                accountIdentifier: this.account.accountIdentifier,
                pubKey: json.publicKey,
                secretKey: json.cipher,
              },
            });
            this.$emit('close');
          } else {
            this.isValid = false;
          }
        } catch (e) {
          this.isValid = false;
        }
      };
      reader.readAsText(this.file);
    },
  },
};
</script>
