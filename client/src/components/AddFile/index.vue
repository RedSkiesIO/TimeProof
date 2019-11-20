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

          class="q-mt-xl q-pa-xl flex flex-center column text-center"
        >
          <q-icon
            name="backup"
            class="text-grey-4"
            style="font-size: 100px"
          />

          <span
            v-if="mode==='sign'"
            class="text-h6 text-weight-bold text-grey-6"
          >{{ $t('dragDrop') }} {{ $t('sign') }}</span>
          <span
            v-else
            class="text-h6 text-weight-bold text-grey-6"
          >{{ $t('dragDrop') }} {{ $t('verify') }}</span>

          <span class="text-body1 text-grey-7">
            {{ $t('or') }} <span
              class="text-blue"
              @click="scope.pickFiles()"
            >{{ $t('browse') }}</span> {{ $t('chooseFile') }}</span>
        </div>
      </div>
      <div v-else-if="error">
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
          <span class="q-mt-md text-h6 text-primary">
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
            color="primary"
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
                  color="primary"
                  :label="$t('verify')"
                  @click="verifyProof"
                />
              </template>
            </q-input>
          </div>

          <span
            class="q-mt-sm text-blue"
            @click="scope.reset()"
          >{{ $t('differentFile') }}</span>
        </div>

        <Proof
          v-if="confirmed"
          :proof="file"
          :scope="scope"
        />
      </div>
      <q-inner-loading :showing="visible">
        <q-spinner-grid
          size="50px"
          color="primary"
        />
      </q-inner-loading>
    </template>
  </q-uploader>
</template>
<script>
import User from '../../store/User';
import Timestamp from '../../store/Timestamp';
import Proof from '../Proof';

export default {
  name: 'AddFile',
  components: {
    Proof,
  },

  props: {
    mode: {
      type: String,
      required: true,
    },
  },

  data() {
    return {
      file: null,
      confirmed: false,
      tab: 'sign',
      proofId: '',
      scope: null,
      visible: false,
      error: false,
      re: /(?:\.([^.]+))?$/,
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
    async insertTimestamp(file) {
      Timestamp.insert({
        data: {
          txId: file.txId,
          hash: file.base32Hash,
          signature: file.signature,
          accountIdentifier: this.user.accountIdentifier,
          name: file.name,
          date: file.timestamp,
          type: file.type,
          size: file.size,
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
        this.file.hash = this.$blake2b(output.length).update(input).digest();
        this.file.base32Hash = this.$base32(this.file.hash).toLowerCase();
      };
      reader.readAsArrayBuffer(files[0]);
    },

    signHash() {
      if (this.key) {
        const sig = this.$keypair.signMessage(this.file.hash, this.key);
        this.file.signature = this.$base32(sig).toLowerCase();
        this.sendProof();
      }
    },

    async sendProof() {
      this.visible = true;
      try {
        const tx = await this.$axios.post(`${process.env.API}StampDocument${process.env.STAMP_KEY}`, {
          fileName: this.file.name,
          hash: this.file.base32Hash,
          publicKey: this.user.pubKey,
          signature: this.file.signature,
        });

        if (tx.data.success) {
          this.file.txId = tx.data.value.stampDocumentProof.transactionId;
          this.file.timestamp = tx.data.value.stampDocumentProof.timeStamp;
          const timestamps = this.user.timestampsUsed + 1;
          User.update({
            data: {
              accountIdentifier: this.account.accountIdentifier,
              timestampsUsed: timestamps,
            },
          });
          await this.insertTimestamp(this.file);

          this.visible = false;
          this.confirmed = true;
        }
      } catch (e) {
        this.error = true;
        this.visible = false;
      }
    },

    async verifyProof() {
      this.$refs.proofId.validate();
      if (!this.$refs.proofId.hasError) {
        this.file.verify = true;
        const txId = this.proofId.replace(/\s+/g, '');
        try {
          const tx = await this.$axios.get(`${process.env.API}VerifyStampDocument/${txId.toUpperCase()}${process.env.VERIFY_KEY}`);

          if (tx.data.success) {
            const fileHash = tx.data.value.stampDocumentProof.userProof.hash;
            if (fileHash === this.file.base32Hash.toLowerCase()) {
              this.file.txId = tx.data.value.stampDocumentProof.transactionId;
              this.file.timestamp = tx.data.value.stampDocumentProof.timeStamp;
              this.file.signature = tx.data.value.stampDocumentProof.userProof.signature;
              this.file.pubKey = tx.data.value.stampDocumentProof.userProof.publicKey;
              this.file.verified = true;
            } else {
              this.file.error = this.$t('filesDoNotMatch');
              this.file.verified = false;
            }
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
    border: 2px solid rgba(0, 0, 0, 0.12);
}
 .q-field__append .q-icon {
   display: none;
 }

</style>
