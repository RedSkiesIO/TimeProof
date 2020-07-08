<template>
  <div>
    <q-card
      v-if="key && user.secretKey"
      flat
      content-class="q-pa-xl top-box"
    >
      <div class="row justify-between text-weight-bold text-h6 q-mb-xs">
        <div>{{ $t('signingKey') }}</div>
      </div>
      <div class="column bg-red-1 q-pa-sm">
        <div class="row text-center text-weight-bold text-red justify-center">
          {{ $t('backupKey') }}
        </div>
        <div class="row text-center justify-center">
          {{ $t('backupKeyDesc') }}
        </div>
        <div class="row justify-center q-mt-sm">
          <q-btn
            outline
            class="shade-color"
            :label="$t('backup')"
            @click="openBackupdialog"
          />
        </div>
      </div>
      <div class="row">
        <q-input
          v-model="key"
          :label="$t('signing')"
          readonly
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
      <div class="row justify-end" />
    </q-card>

    <q-card
      v-if="!key && user.secretKey"
      flat
      class="top-box"
    >
      <div class="row justify-center text-weight-bold text-h6 q-mb-xs">
        <div>{{ $t('signingKeyLocked') }}</div>
      </div>
      <div class="row justify-center">
        <div class="col-5">
          <q-input
            v-model="password"
            dense
            mask="######"
            :label="$t('enterPin')"
            :type="isPwd ? 'password' : 'text'"
            :error="!isValid"
            class="q-my-sm signing-key"
            :rules="[val => val && val.length === 6 || $t('invalidPinLength')]"
            @keyup.enter="unlockKey(password)"
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
        <div class="col-2 q-ml-lg unlock-button">
          <q-btn
            flat
            :label="$t('unlock')"
            class="shade-color"
            @click="unlockKey(password)"
          />
        </div>
      </div>
      <div class="row justify-end" />
    </q-card>

    <q-dialog
      v-model="newKey"
    >
      <Import
        v-if="dialogMode==='import'"
        @close="newKey=false"
      />
      <Backup
        v-if="dialogMode=='backup'"
        @backup="backup"
      />
      <NewKey
        v-if="dialogMode=='new'"
        :mode="dialogMode"
        @close="closeDialog"
      />
    </q-dialog>
  </div>
</template>
<script>
import User from '../../store/User';
import NewKey from './NewKey';
import Backup from './DownloadKey';
import Import from './ImportKey';

export default {
  name: 'Key',
  components: {
    NewKey,
    Backup,
    Import,
  },

  data() {
    return {
      isValid: true,
      dialogMode: 'new',
      newKey: false,
      signKey: false,
      isPwd: true,
      password: '',
    };
  },

  computed: {
    account() {
      return this.$auth.account();
    },

    user() {
      return this.$auth.user();
    },

    key() {
      return this.$store.state.settings.authenticatedAccount;
    },

  },

  methods: {
    closeDialog() {
      this.newKey = false;
      this.$emit('close');
    },
    openNewKeyDialog() {
      this.newKey = true;
      this.dialogMode = 'new';
    },
    openBackupdialog() {
      this.newKey = true;
      this.dialogMode = 'backup';
    },
    openImportDialog() {
      this.newKey = true;
      this.dialogMode = 'import';
    },

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
    },
    async lockKey() {
      await this.$store.dispatch('settings/setAuthenticatedAccount', null);
    },

    async unlockKey(password) {
      const decrypted = await this.$crypto.decrypt(this.user.secretKey, password);
      if (decrypted) {
        await this.$store.dispatch('settings/setAuthenticatedAccount', decrypted);
        this.$emit('sign');
        this.$emit('closeUnlock');
        this.password = null;
      } else {
        this.isValid = false;
        setTimeout(() => {
          this.isValid = true;
        }, 2000);
      }
    },

    async backup() {
      await this.$crypto.createKeystore(this.user);
      this.newKey = false;
    },
  },
};
</script>
<style lang="scss">
.account {
    border: 2px solid rgba(0, 0, 0, 0.12);
    max-width: 350px;
}

.signing-key {
  width: 100%;
}

.signing-key .q-field__append .q-icon {
  display: flex;
}

.unlock-button {
  margin-top: 12px;
}
</style>
