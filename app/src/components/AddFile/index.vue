<template>
  <q-uploader
    flat
    bordered
    :label="$t('selectFile')"
    auto-upload
    hide-upload-btn
    :factory="hashFile"
    :filter="checkFileNameLength"
    @rejected="onRejected"
  >
    <template v-slot:list="scope">
      <div v-if="scope.files < 1">
        <div
          class="bg-white dash-border"
        >
          <div

            class="q-my-xl q-pa-xl flex flex-center column text-center"
          >
            <template v-if="mode === 'sign'">
              <img
                src="~assets/stamp-image.svg"
                style="height: 12vw"
              >

              <span
                class="q-pt-md text-h6 text-weight-bold text-grey-6"
              >
                {{ $t('dragDrop') }} {{ $t('sign') }}
              </span>
            </template>

            <template v-else>
              <img
                src="~assets/verify-image.svg"
                style="height: 12vw"
              >
              <span
                class="q-pt-md text-h6 text-weight-bold text-grey-6"
              >
                {{ $t('dragDrop') }} {{ $t('verify') }}
              </span>
            </template>

            <span class="text-body1 text-grey-7">
              {{ $t('or') }} <span
                class="text-blue cursor-pointer"
                data-test-key="stampBrowseFile"
                @click="scope.pickFiles()"
              >{{ $t('browse') }}</span> {{ $t('chooseFile') }}</span>
          </div>
        </div>
        <div
          class="row text-secondary text-h6 justify-center q-mt-sm"
        >
          The file never leaves your device
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
            :name="fileIcon(file.type)"
            class="text-grey-4"
            style="font-size: 100px"
          />
          <div>
            <q-knob
              v-if="fileIsLoading"
              :value="uploadPercent"
              readonly
              show-value
              instant-feedback
              font-size="15px"
              size="70px"
              :thickness="0.1"
              color="cyan-4"
              track-color="grey-3"
              class="q-ma-md"
            >
              {{ uploadPercent }}%
            </q-knob>
          </div>
          <span class="q-mt-md text-h6 text-secondary wrapword">
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
            :disabled="fileIsLoading"
            unelevated
            size="lg"
            color="secondary"
            :data-test-key="$t('sign')"
            :label="$t('sign')"
            @click="signHash"
          />
          <div v-else>
            <q-input
              ref="proofId"
              v-model="proofId"
              outlined
              bottom-slots
              data-test-key="verifyProofId"
              :placeholder="$t('proofId')"
              :rules="[
                val => !!val || '* Required',
                val => val.length === 66 || $t('invalidProofId'),
              ]"
              lazy-rules
            />
            <div class="row justify-center q-pa-md">
              <q-btn
                unelevated
                :disable="!proofId"
                color="secondary"
                :data-test-key="$t('verify')"
                :label="$t('verify')"
                @click="verifyProof"
              />
            </div>
          </div>

          <span
            class="q-mt-sm text-blue q-pb-md cursor-pointer"
            @click="scope.reset()"
          >{{ $t('differentFile') }}</span>
        </div>
        <Proof
          v-if="confirmed && !file.verify"
          :proof-id="txId"
          :scope="scope"
          class="add-border"
          data-test-key="stampProof"
          @userHasReachedToLimit="$emit('userHasReachedToLimit')"
        />

        <VerifyResult
          v-if="confirmed && file.verify"
          :proof="file"
          :scope="scope"
          data-test-key="stampVerify"
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
import { fileIcon } from '../../util';

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
      fileIcon,
      previous: [],
      lastOffset: 0,
      chunkSize: 1024 * 1024,
      blake2b: null,
      fileIsLoading: false,
      counter: 0,
      increment: 0,
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
    uploadPercent() {
      if (this.file && this.file.byteSize > 0) {
        return parseInt((this.counter / this.file.byteSize) * 100, 10);
      }
      return 0;
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
          id: file.id,
          txId: file.txId,
          hash: file.hash,
          status: file.status,
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
      this.counter = 0;
      this.fileIsLoading = true;
      let offset = 0;
      this.lastOffset = 0;
      const size = this.chunkSize;
      let partial;
      let index = 0;


      try {
        this.blake2b = this.$blake2b((new Uint8Array(64)).length);
        const currentFile = files[0];
        this.confirmed = false;
        this.file = {
          name: currentFile.name,
          type: this.re.exec(currentFile.name)[1],
          size: this.getSize(currentFile.size),
          byteSize: currentFile.size,
        };

        if (currentFile.size !== 0) {
          this.$q.loadingBar.start();
          while (offset < currentFile.size) {
            partial = currentFile.slice(offset, offset + size);
            const reader = new FileReader();
            reader.size = size;
            reader.offset = offset;
            reader.index = index;
            // eslint-disable-next-line no-loop-func
            reader.onload = (evt) => {
              this.callbackRead(reader, currentFile, evt,
                this.callbackProgress, this.callbackFinal);
            };
            reader.readAsArrayBuffer(partial);
            offset += this.chunkSize;
            index += 1;
          }
        }
      } catch (ex) {
        this.$q.loadingBar.stop();
        console.log('File upload error: ', ex);
      }
    },

    // memory reordering
    callbackRead(reader, currentFile, evt, callbackProgress, callbackFinal) {
      if (this.lastOffset !== reader.offset) {
        this.previous.push({ offset: reader.offset, size: reader.size, result: reader.result });
        return;
      }

      const parseResult = (offset, size, result) => {
        this.lastOffset = offset + size;
        if (this.lastOffset < currentFile.size) {
          callbackProgress(result);
        } else {
          callbackFinal(result);
          this.lastOffset = 0;
        }
      };

      parseResult(reader.offset, reader.size, reader.result);

      while (this.previous && this.previous.length > 0) {
        const previousInitSize = this.previous.length;
        this.previous = this.previous.filter((item) => {
          if (item.offset === this.lastOffset) {
            parseResult(item.offset, item.size, item.result);
            return false;
          }
          return true;
        });

        if (this.previous.length === previousInitSize) {
          break;
        }
      }
    },

    callbackProgress(data) {
      this.blake2b.update(Buffer.from(data));
      this.counter += data.byteLength;
      setTimeout(() => {
        this.$q.loadingBar.increment(parseFloat((this.counter / this.file.byteSize).toFixed(5)));
      }, 100);
    },

    callbackFinal(data) {
      this.file.hashBuffer = this.blake2b.update(Buffer.from(data)).digest();
      this.file.hash = this.$base32(this.file.hashBuffer).toLowerCase();
      this.counter += data.byteLength;
      this.$q.loadingBar.increment(parseFloat((this.counter / this.file.byteSize).toFixed(5)));
      this.$q.loadingBar.stop();
      this.fileIsLoading = false;
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
        const tx = await this.$timestampServer.createTimestamps(this.file, this.user.pubKey);
        console.log('NEW TIMESTAMP');
        console.log(tx);
        if (tx.status === 409) { // Not sufficient stamps left for the user
          const verifyResult = await
          this.$userServer.verifyUserDetails();

          if (verifyResult) {
            User.insertOrUpdate({
              data: {
                accountIdentifier: this.account.accountIdentifier,
                remainingTimeStamps: verifyResult.remainingTimeStamps,
              },
            });
          }
        } else if (tx.data || tx.status === 201) {
          this.file.id = tx.data.id;
          this.file.txId = tx.data.transactionId;
          this.file.timestamp = tx.data.timestamp;
          this.file.blockNumber = tx.data.blockNumber;
          this.file.status = tx.data.status;
          User.update({
            data: {
              accountIdentifier: this.account.accountIdentifier,
              remainingTimeStamps: this.user.remainingTimeStamps - 1,
            },
          });
          await this.insertTimestamp(this.file);
          this.txId = tx.data.transactionId;
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
      } catch (e) {
        this.file.error = this.$t('noProofFound');
        this.file.verified = false;
      }
      this.confirmed = true;
      this.proofId = '';
    },
    checkFileNameLength(files) {
      return files.filter(file => file && file.name && file.name.length <= 200);
    },

    onRejected(rejectedEntries) { // rejectedEntries
      // Notify plugin needs to be installed
      // https://quasar.dev/quasar-plugins/notify#Installation
      if (rejectedEntries && Array.isArray(rejectedEntries) && rejectedEntries.length > 0) {
        /* if (rejectedEntries[0].failedPropValidation === 'max-file-size') {
          this.$q.notify({
            type: 'warning-notify',
            // type: 'negative',
            // ${rejectedEntries.length}
            message: 'The file size must not exceed 50 MB.',
          });
        } else { // failedPropValidation === 'filter'
          this.$q.notify({
            type: 'warning-notify',
            message: 'The file name must not exceed 200 characters.',
          });
        } */

        if (rejectedEntries[0].failedPropValidation === 'filter') {
          this.$q.notify({
            type: 'warning-notify',
            message: 'The file name must not exceed 200 characters.',
          });
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

.centered {
  position: fixed; /* or absolute */
  top: 28%;
  left: 48.5%;
}
</style>
