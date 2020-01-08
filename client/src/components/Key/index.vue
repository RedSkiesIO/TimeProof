<template>
  <div>
    <q-card
      v-if="key && user.secretKey"
      flat
      class="signing-box"
    >
      <div class="column bg-red-1 q-pa-sm">
        <div class="row text-center text-weight-bold text-red justify-center">
          {{ $t('backupKey') }}
        </div>
        <div class="row justify-center q-mt-sm">
          <q-btn
            outline
            color="black"
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
            <q-btn
              outline
              round
              color="primary"
              icon="lock"
              @click="lockKey"
            />
          </template>
        </q-input>
      </div>
      <!-- <div class="row justify-center q-mb-sm">
        <q-btn
          outline
          round
          color="primary"
          icon="lock"
          @click="lockKey"
        />
      </div> -->
      <div class="row justify-center q-gutter-x-xs">
        <q-btn
          flat
          color="blue"
          size="md"
          label="import key"
          @click="openImportDialog"
        />
        <q-btn
          flat
          color="blue"
          size="md"
          label="new key"
          @click="openNewKeyDialog"
        />
      </div>
      <div class="row justify-end" />
    </q-card>

    <q-card
      v-if="!key && user.secretKey"
      flat
      class="signing-box"
    >
      <div class="q-mt-xs q-px-sm">
        <div class="row justify-center text-weight-bold text-h6 q-mb-xs">
          <div>{{ $t('signingKeyLocked') }}</div>
        </div>
        <div class="row justify-center">
          {{ $t('signingKeyLockedDesc') }}
        </div>
        <div class="row">
          <q-input
            v-model="password"
            :label="$t('enterPassword')"
            :type="isPwd ? 'password' : 'text'"
            :error="!isValid"
            class="q-my-sm signing-key"
            @keyup.enter="unlockKey(password)"
          >
            <template v-slot:append>
              <q-icon
                :name="isPwd ? 'visibility_off' : 'visibility'"
                class="cursor-pointer"
                @click="isPwd = !isPwd"
              />
              <q-btn
                outline
                round
                icon="lock_open"
                color="primary"
                @click="unlockKey(password)"
              />
            </template>
            <template v-slot:error>
              {{ $t('wrongPassword') }}
            </template>
          </q-input>
        </div>
        <!-- <div class="row justify-center q-mb-sm">
          <q-btn
            outline
            :label="$t('unlock')"
            color="primary"
            @click="unlockKey(password)"
          />
        </div> -->
        <div class="row justify-center q-gutter-x-xs">
          <q-btn
            flat
            color="blue"
            size="md"
            label="import key"
            @click="openImportDialog"
          />
          <q-btn
            flat
            color="blue"
            size="md"
            label="new key"
            @click="openNewKeyDialog"
          />
        </div>
        <div class="row justify-end" />
      </div>
    </q-card>

    <q-card
      v-if="!user.secretKey"
      flat
      class="signing-box"
    >
      <div
        class="column flex-center"
        style="height: 100%;"
      >
        <div class="row justify-center text-weight-bold text-h6 q-mb-xs">
          <div>{{ $t('createKey') }}</div>
        </div>
        <div class="row justify-center text-center">
          {{ $t('createKeyDesc') }}
        </div>
        <div class="row justify-center q-my-sm">
          <q-btn
            outline
            :label="$t('createKeyLabel')"
            color="primary"
            @click="newKey=true"
          />
        </div>
        <div
          class="row justify-center text-blue q-mb-sm"
          @click="openImportDialog"
        >
          {{ $t('importKey') }}
        </div>
      </div>
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
      tiers: {
        free: 50,
        basic: 1000,
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
.signing-box {
  height: 220px;
}

.q-card {
  border: 1px solid lightgrey;
}
</style>
