<template>
  <div>
    <q-card
      v-if="key && user.secretKey"
      flat
      class="signing-box bg-grey-2"
    >
      <div class="column bg-red-1 q-pa-sm">
        <div class="row text-center text-weight-bold text-red justify-center">
          {{ $t('backupKey') }}
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
            <q-btn
              flat
              round
              class="shade-color"
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
          color="secondary"
          icon="lock"
          @click="lockKey"
        />
      </div> -->
      <!-- <div class="row justify-center q-gutter-x-xs">
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
      </div> -->
      <div class="row justify-end" />
    </q-card>

    <q-card
      v-if="!key && user.secretKey"
      flat
      class="signing-box bg-grey-2"
    >
      <div class="q-mt-xs q-px-sm">
        <div class="row justify-center text-weight-bold text-h6 q-mb-xs">
          <div>{{ $t('signingKeyLocked') }}</div>
        </div>
        <div class="row">
          <q-input
            v-model="password"
            :label="$t('enterPassword')"
            mask="######"
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
              <!--<q-btn
                flat
                round
                icon="lock_open"
                class="shade-color"
                @click="unlockKey(password)"
              />-->
            </template>
            <template v-slot:error>
              {{ $t('wrongPassword') }}
            </template>
          </q-input>
        </div>
        <div class="row justify-center q-mb-sm">
          <q-btn
            flat
            :label="$t('unlock')"
            class="shade-color"
            @click="unlockKey(password)"
          />
        </div>
        <!-- <div class="row justify-center q-gutter-x-xs">
          <q-btn
            flat
            color="blue"
            size="md"
            label="import key"
            @click="openImportDialog"
          /> -->
        <!-- <q-btn
            flat
            color="blue"
            size="md"
            label="new key"
            @click="openNewKeyDialog"
          /> -->
        <!-- </div> -->
        <!--<div class="row justify-end" /> -->
      </div>
    </q-card>

    <q-card
      v-if="!user.secretKey"
      flat
      class="signing-box bg-grey-2"
    >
      <div
        class="column flex-center"
        style="height: 100%;"
      >
        <template v-if="!userHasSavedKeyBefore">
          <div class="row justify-center text-weight-bold text-h6 q-mb-xs">
            <div>{{ $t('createKey') }}</div>
          </div>
          <!-- <div class="row justify-center text-center">
            {{ $t('createKeyDesc') }}
          </div> -->
          <div
            class="row justify-center q-my-sm"
          >
            <q-btn
              outline
              :label="$t('createKeyLabel')"
              class="shade-color"
              @click="newKey=true"
            />
          </div>
        </template>
        <template v-else>
          <div class="row justify-center text-weight-bold text-h6 q-mb-xs">
            <div>{{ $t('importKey') }}</div>
          </div>
          <!-- <div class="row justify-center text-center">
            {{ $t('importKeyContent') }}
          </div> -->
          <div
            class="row justify-center text-blue q-mb-sm q-pt-md cursor-pointer"
            @click="openImportDialog"
          >
            {{ $t('importKey') }}
          </div>
        </template>
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
import moment from 'moment';
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

    userHasSavedKeyBefore() {
      const keyMoment = moment(this.user.keyEmailDate, 'YYYY-MM-DD');
      return keyMoment.year() !== 1;
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
