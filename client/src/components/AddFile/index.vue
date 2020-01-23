<template>
  <q-uploader
    flat
    bordered
    :label="$t('selectFile')"
    auto-upload
    hide-upload-btn
    :factory="hashFile"
  >
    <template v-slot:list="scope">
      <div v-if="scope.files < 1">
        <div
          class="bg-white dash-border"
        >
          <div

            class="q-my-xl q-pa-xl flex flex-center column text-center"
          >
            <img
              src="~assets/add-file.svg"
              style="height: 12vw"
            >

            <span
              v-if="mode==='sign'"
              class="q-pt-sm text-h6 text-weight-bold text-grey-6"
            >{{ $t('dragDrop') }} {{ $t('sign') }}</span>
            <span
              v-else
              class="q-pt-sm text-h6 text-weight-bold text-grey-6"
            >{{ $t('dragDrop') }} {{ $t('verify') }}</span>

            <span class="text-body1 text-grey-7">
              {{ $t('or') }} <span
                class="text-blue"
                @click="scope.pickFiles()"
              >{{ $t('browse') }}</span> {{ $t('chooseFile') }}</span>
          </div>
        </div>
        <div
          class="row text-secondary text-h6 justify-center q-mt-sm"
        >
          The file never leaves your system
        </div>
      </div>
      <div
        v-else-if="error"
        class="bg-white"
      >
        <div

          class="q-mt-xl q-pa-xl flex flex-center column text-center"
        >
          <q-icon
            name="error"
            class="text-grey-4"
            style="font-size: 100px"
          />

          <span
            class="text-h6 text-weight-bold text-grey-6"
          >{{ $t('somethingWrong') }}</span>
          <span
            class="text-blue"
            @click="reset(scope)"
          >{{ $t('tryAgain') }}</span>
        </div>
      </div>
      <div
        v-else
        class="bg-white"
      >
        <div
          v-if="!confirmed"
          class="q-pa-xl flex flex-center column text-center"
        >
          <q-icon
            :name="fileIcon"
            class="text-grey-4"
            style="font-size: 100px"
          />
          <span class="q-mt-md text-h6 text-secondary">
            {{ file.name }}</span>
          <span
            v-if="file.type"
            class="text-body1 text-grey-7"
          >
            {{ $t('type') }}: {{ file.type }}</span>
          <span class="q-mb-lg text-body1 text-grey-7">
            {{ $t('size') }}: {{ file.size }}</span>
          <q-btn
            v-if="mode==='sign'"
            unelevated
            size="lg"
            color="secondary"
            :label="$t('sign')"
            @click="signHash"
          />
          <div v-else>
            <q-input
              ref="proofId"
              v-model="proofId"
              outlined
              rounded
              bottom-slots
              :label="$t('proofId')"
              lazy-rules
              :rules="[ val => val && val.length > 102
                && val.length < 105 || $t('invalidProofId')]"
            >
              <template v-slot:append>
                <q-btn
                  unelevated
                  rounded
                  color="secondary"
                  :label="$t('verify')"
                  @click="verifyProof"
                />
              </template>
            </q-input>
          </div>

          <span
            class="q-mt-sm text-blue q-pb-md"
            @click="scope.reset()"
          >{{ $t('differentFile') }}</span>
        </div>
        <Proof
          v-if="confirmed && !file.verify"
          :proof-id="txId"
          :scope="scope"
          class="add-border"
        />
        <div class="row justify-center q-pa-md">
          <q-btn
            v-if="confirmed && !file.verify"
            flat
            label="Go back to the dashboard"
            color="blue"
            class="row justify-center q-pa-md text-blue"
            @click="backToDashboard"
          />
        </div>

        <VerifyResult
          v-if="confirmed && file.verify"
          :proof="file"
          :scope="scope"
          class="add-border"
        />
      </div>
      <q-dialog v-model="dialog">
        <UnlockKey
          v-if="unlockKey"
          mode="unlock"
          @closeUnlock="dialog=false"
          @sign="signHash"
        />
        <NewKey
          v-if="newKey"
          @close="dialog=false"
          @sign="signHash"
        />
      </q-dialog>
      <q-inner-loading :showing="visible">
        <q-spinner-grid
          size="50px"
          color="secondary"
        />
      </q-inner-loading>
    </template>
  </q-uploader>
</template>
<script>
import User from '../../store/User';
import Timestamp from '../../store/Timestamp';
import Proof from '../Proof';
import VerifyResult from '../VerifyResult';
import UnlockKey from '../Key/NewKey';
import NewKey from '../Key';

export default {
  name: 'AddFile',
  components: {
    Proof,
    VerifyResult,
    UnlockKey,
    NewKey,
  },

  props: {
    mode: {
      type: String,
      required: true,
    },
  },

  data() {
    return {
      dialog: false,
      newKey: false,
      unlockKey: false,
      file: null,
      confirmed: false,
      tab: 'sign',
      proofId: '',
      scope: null,
      visible: false,
      error: false,
      re: /(?:\.([^.]+))?$/,
      txId: null,
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
        const user = User.find(this.account.accountIdentifier);
        if (user) {
          return user;
        }
      }
      return null;
    },

    fileIcon() {
      const { type } = this.file;
      if (type === 'pdf') {
        return 'fas fa-file-pdf';
      }

      if (type === 'zip') {
        return 'fas fa-file-archive';
      }

      if (type === 'png' || type === 'gif' || type === 'jpeg') {
        return 'fas fa-file-image';
      }

      return 'fas fa-file';
    },

    key() {
      return this.$store.state.settings.authenticatedAccount;
    },
  },

  methods: {
    reset(scope) {
      scope.reset();
      this.error = false;
    },

    backToDashboard() {
      this.$router.push('/dashboard');
    },

    async insertTimestamp(file) {
      Timestamp.insert({
        data: {
          txId: file.txId,
          hash: file.hash,
          signature: file.signature,
          accountIdentifier: this.user.accountIdentifier,
          name: file.name,
          date: file.timestamp,
          type: file.type,
          size: file.size,
          blockNumber: file.blockNumber,
        },
      });
    },

    getSize(bytes) {
      const decimals = 2;
      if (bytes === 0) return '0 Bytes';
      const k = 1024;
      const dm = decimals < 0 ? 0 : decimals;
      const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];

      const i = Math.floor(Math.log(bytes) / Math.log(k));

      return `${parseFloat((bytes / (k ** i)).toFixed(dm))} ${sizes[i]}`;
    },

    async hashFile(files) {
      this.confirmed = false;
      this.file = {
        name: files[0].name,
        type: this.re.exec(files[0].name)[1],
        size: this.getSize(files[0].size),
      };
      const reader = await new FileReader();
      reader.onload = (evt) => {
        const input = Buffer.from(evt.target.result);
        const output = new Uint8Array(64);
        this.file.hashBuffer = this.$blake2b(output.length).update(input).digest();
        this.file.hash = this.$base32(this.file.hashBuffer).toLowerCase();
      };
      reader.readAsArrayBuffer(files[0]);
    },

    signHash() {
      if (this.key) {
        const sig = this.$keypair.signMessage(this.file.hashBuffer, this.key);
        this.file.signature = this.$base32(sig).toLowerCase();
        this.sendProof();
      } else if (!this.user.secretKey) {
        this.newKey = true;
        this.dialog = true;
      } else {
        this.unlockKey = true;
        this.dialog = true;
      }
    },

    async sendProof() {
      this.visible = true;
      try {
        if (Date.now() > this.user.tokenExpires) {
          const token = await this.$auth.getToken();
          this.$axios.defaults.headers.common.Authorization = `Bearer ${token.idToken.rawIdToken}`;
          User.update({
            data: {
              accountIdentifier: this.account.accountIdentifier,
              tokenExpires: token.idToken.expiration,
            },
          });
        }
        const tx = await this.$axios.post('https://document-timestamp.azurewebsites.net/api/timestamp', {
          fileName: this.file.name,
          hash: this.file.hash,
          publicKey: this.user.pubKey,
          signature: this.file.signature,
        });

        if (tx.data.success) {
          this.file.txId = tx.data.value.id;
          this.file.timestamp = tx.data.value.timestamp;
          this.file.blockNumber = tx.data.value.blockNumber;
          const timestamps = this.user.timestampsUsed + 1;
          User.update({
            data: {
              accountIdentifier: this.account.accountIdentifier,
              timestampsUsed: timestamps,
            },
          });
          await this.insertTimestamp(this.file);
          this.txId = tx.data.value.id;
          this.visible = false;
          this.confirmed = true;
        }
      } catch (e) {
        console.log(e);
        this.error = true;
        this.visible = false;
      }
    },

    async verifyProof() {
      this.file.verify = true;
      const txId = this.proofId.replace(/\s+/g, '');
      try {
        const tx = await this.$web3.verifyTimestamp(txId, this.file.hash.toLowerCase());
        if (tx && tx.verified) {
          this.file.txId = txId;
          this.file.date = tx.timestamp;
          this.file.signature = tx.signature;
          this.file.pubKey = tx.publicKey;
          this.file.verified = true;
        } else if (tx && !tx.verified) {
          this.file.error = this.$t('filesDoNotMatch');
          this.file.verified = false;
        } else {
          this.file.error = this.$t('noProofFound');
          this.file.verified = false;
        }
        this.confirmed = true;
      } catch (e) {
        this.file.error = this.$t('noProofFound');
        this.file.verified = false;
        this.confirmed = true;
      }
    },
  },
};
</script>
<style lang="scss">
.q-uploader {
  width: inherit;
  max-height:inherit;
  min-width: 25rem;
  min-height: 25rem;
}
.q-uploader__subtitle {
    display: none !important;
}
.q-uploader__file-status {
    display: none;
}
.q-uploader__header {
    display: none;
}

.q-uploader--bordered {
    border: 0px solid rgba(0, 0, 0, 0.12);
}

.q-field__append .q-icon {
  display: none;
}

.dash-border {
  border: 2px dashed rgba(0, 0, 0, 0.12);
}

.add-border {
border: 1px solid lightgray;
}
</style>
