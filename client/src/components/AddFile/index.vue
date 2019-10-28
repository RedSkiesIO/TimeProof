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
      <div
        v-if="scope.files < 1"
        class="q-pa-xl flex flex-center column text-center"
        style="height: -webkit-fill-available;"
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
              v-model="proofId"
              :label="$t('proofId')"
              stack-label
              dense
            />
            <q-btn
              class="q-mt-md"
              unelevated
              rounded
              size="lg"
              color="primary"
              :label="$t('verify')"
              @click="verifyProof"
            />
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
    </template>
  </q-uploader>
</template>
<script>
import User from '../../store/User';
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
    };
  },

  computed: {
    user() {
      if (User.all().length > 0) {
        return User.query().first();
      }
      return null;
    },

    fileIcon() {
      const { type } = this.file;
      if (type === 'application/pdf') {
        return 'fas fa-file-pdf';
      }

      if (type === 'application/zip') {
        return 'fas fa-file-archive';
      }

      if (type === 'image/png' || type === 'image/gif' || type === 'image/jpeg') {
        return 'fas fa-file-image';
      }

      return 'fas fa-file';
    },
  },

  methods: {
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
      console.log(this.$store.state.settings.authenticatedAccount);

      this.confirmed = false;
      this.file = {
        name: files[0].name,
        type: files[0].type,
        size: this.getSize(files[0].size),
      };
      const reader = await new FileReader();
      reader.onload = (evt) => {
        const input = Buffer.from(evt.target.result);
        const output = new Uint8Array(64);
        this.file.hash = this.$blake2b(output.length).update(input).digest();
      };
      reader.readAsArrayBuffer(files[0]);
    },

    signHash() {
      const sig = this.$keypair.signMessage(this.file.hash, this.user.secretKey);
      this.file.signature = this.$base32(sig);
      this.file.base32Hash = this.$base32(this.file.hash);
      this.sendProof();
    },

    async sendProof() {
      const tx = await this.$axios.post('http://localhost:7071/api/StampDocumentFunction', {
        hash: this.file.base32Hash,
        publicKey: this.user.pubKey,
        signature: this.file.signature,
      });

      if (tx.data.success) {
        this.file.txId = tx.data.value.transactionId;
        this.file.timestamp = tx.data.value.timeStamp;
        this.confirmed = true;
      }
    },

    async verifyProof() {
      this.file.txId = this.proofId;
      this.file.timestamp = Date.now();
      this.file.verify = true;
      this.file.verified = false;
      this.confirmed = true;
    },
  },
};
</script>
<style lang="scss">
.q-uploader {
  width: inherit;
  max-height: inherit;
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
</style>
