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
          @click="$emit('backup')"
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
    };
  },

  methods: {
    async importFromKey() {
      this.keypair = this.$keypair.keypairFromSecretKey(this.secretKey);
      this.openEncrypt = true;
    },
  },
};
</script>
