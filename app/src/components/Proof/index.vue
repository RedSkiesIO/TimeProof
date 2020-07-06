<template>
  <div
    :class="scope.dialog ? 'proof-dialog' : 'proof'"
  >
    <div
      v-if="ready"
      id="proof"
      class="q-px-lg flex flex-center column text-center q-pt-lg"
    >
      <q-icon
        :name="icon.name"
        :class="icon.class"
        style="font-size: 100px"
        :style="scope.dialog ?
          'background:radial-gradient(circle at center, white 50%, #4cbbc2 60%)' : ''"
      />

      <span
        class="text-h6 q-my-sm"
        data-test-key="stampProofTitle"
      >{{ title }}</span>
      <a
        v-if="proof.status !== 0"
        class="q-mt-sm"
        :class="scope.dialog ? 'text-dark-blue q-mb-lg' : 'text-blue'"
        :href="etherscanTx"
        target="_blank"
      >
        View Transaction
      </a>
    </div>

    <div
      v-if="scope.dialog"
      class="column q-px-md"
    >
      <div
        class="row justify-between"
        :class="scope.dialog ? 'proof-dialog-item' : 'proof-item'"
      >
        <template v-if="scope.dialog">
          <div class="col">
            <q-input
              v-model="proof.name"
              filled
              :bg-color="bgColor"
              readonly
              flat
              stack-label
              :label="$t('file')"
              autogrow
            />
          </div>
        </template>
        <template v-else>
          <div class="col-auto">
            {{ $t('file') }}:
          </div>
          <div class="col-auto wrapword">
            {{ proof.name }}
          </div>
        </template>
      </div>

      <div>
        <div
          class="row justify-between"
          :class="scope.dialog ? 'proof-dialog-item' : 'proof-item'"
        >
          <template v-if="scope.dialog">
            <div class="col">
              <q-input
                :value="proof.timestampDate+' UTC'"
                filled
                :bg-color="bgColor"
                readonly
                stack-label
                :label="$t('date')"
                autogrow
              />
            </div>
          </template>
          <template v-else>
            <div class="col-auto">
              {{ $t('date') }}:
            </div>
            <div
              v-if="proof.status === 0"
              class="col-auto"
            >
              <q-chip
                square
                color="orange"
                text-color="white"
                data-test-key="stampPending"
              >
                pending
              </q-chip>
            </div>
            <div
              v-if="ready && proof.status !== 0"
              class="col-auto"
            >
              {{ proof.timestampDate }}
            </div>
          </template>
        </div>

        <div
          class="row justify-between"
          :class="scope.dialog ? 'proof-dialog-item' : 'proof-item'"
        >
          <template v-if="scope.dialog">
            <div class="col">
              <q-input
                v-model="signedBy"
                filled
                :bg-color="bgColor"
                readonly
                stack-label
                :label="$t('signedBy')"
                autogrow
              />
            </div>
          </template>
          <template v-else>
            <div class="col-auto">
              {{ $t('signedBy') }}:
            </div>
            <div
              v-if="!proof.verify"
              class="col-auto q-pl-sm"
            >
              {{ user.name }} ({{ user.email }})
            </div>
            <div
              v-else
              class="col-auto q-pl-sm"
            >
              {{ proof.pubKey.toLowerCase() }}
            </div>
          </template>
        </div>
        <div
          class="row justify-between"
          :class="heightClass"
        >
          <div class="col">
            <q-input
              v-model="proof.txId"
              filled
              :bg-color="bgColor"
              readonly
              stack-label
              :label="$t('id')"
              autogrow
            >
              <q-btn
                flat
                rounded
                size="sm"
                icon="filter_none"
                class="copy-button absolute-bottom-right shade-color"
                @click="copy(proof.txId)"
              >
                <q-tooltip anchor="top middle">
                  {{ copyLabel }}
                </q-tooltip>
              </q-btn>
            </q-input>
          </div>
        </div>
        <q-badge
          v-if="scope.dialog"
          class="q-ml-md"
          style="fontSize: 1rem"
        >
          * you need to enter this in the verify tab
        </q-badge>

        <div
          class="row justify-between"
          :class="heightClass"
        >
          <div class="col">
            <q-input
              v-model="proof.hash"
              filled
              :bg-color="bgColor"
              readonly
              stack-label
              :label="$t('hash')"
              autogrow
            >
              <q-btn
                flat
                rounded
                size="sm"
                icon="filter_none"
                class="copy-button absolute-bottom-right shade-color"
                @click="copy(proof.hash)"
              >
                <q-tooltip anchor="top middle">
                  {{ copyLabel }}
                </q-tooltip>
              </q-btn>
            </q-input>
          </div>
        </div>

        <div
          class="row justify-between"
          :class="heightClass"
        >
          <div class="col">
            <q-input
              v-model="proof.signature"
              filled
              :bg-color="bgColor"
              readonly
              stack-label
              :label="$t('signature')"
              autogrow
            >
              <q-btn
                flat
                rounded
                size="sm"
                icon="filter_none"
                class="copy-button absolute-bottom-right shade-color"
                @click="copy(proof.signature)"
              >
                <q-tooltip anchor="top middle">
                  {{ copyLabel }}
                </q-tooltip>
              </q-btn>
            </q-input>
          </div>
        </div>
      </div>
    </div>
    <div
      v-else
      class="text-italic flex flex-center column text-center q-pt-md q-pb-md"
    >
      “Please ensure to store a copy of the file you just stamped.<br>
      If you edit the file, you will have to create a new stamp.”
    </div>

    <div v-if="scope.dialog">
      <div class="row q-pb-md justify-center q-pt-md">
        <q-btn
          v-if="proof.status !== 0 && ready"
          no-caps
          class="shade-color"
          label="Download Certificate"
          @click="getCertificate"
        />
      </div>
    </div>
    <div v-else>
      <div class="row q-pb-md justify-center q-pt-md">
        <template>
          <q-btn
            no-caps
            text-color="white"
            :class="proof.status !== 0 && ready ? 'shade-color q-mr-md': 'shade-color'"
            :label="$t('stampAnotherFile')"
            @click="selectAnotherFile(scope)"
          />
        </template>
        <template>
          <q-btn
            v-if="proof.status !== 0 && ready"
            flat
            no-caps
            class="shade-color q-ml-md"
            label="Download Certificate"
            @click="getCertificate"
          />
        </template>
      </div>
    </div>
  </div>
</template>
<script>
import Timestamp from '../../store/Timestamp';

export default {
  name: 'Proof',

  props: {
    proofId: {
      type: String,
      default: null,
      required: false,
    },
    scope: {
      type: Object,
      required: true,
    },
  },

  data() {
    return {
      copyLabel: this.$t('copy'),
      ready: false,
      bgColor: this.scope.dialog ? 'white' : '',
    };
  },

  computed: {
    etherscanTx() {
      return `${process.env.ETHERSCAN}/${this.proofId}`;
    },

    account() {
      return this.$auth.account();
    },

    user() {
      return this.$auth.user();
    },

    proof() {
      return this.proofId ? Timestamp.query().where('txId', this.proofId).first() : null;
    },

    heightClass() {
      if (this.scope.dialog) {
        return 'proof-dialog-item ';
      }
      return this.proof.name && this.proof.name.length < 51
        ? 'proof-item text-height-short' : 'proof-item text-height-long';
    },

    signedBy() {
      return !this.proof.verify ? `${this.user.name}(${this.user.email})` : this.proof.pubKey.toLowerCase();
    },

    icon() {
      if (this.proof.verify && !this.proof.verified) {
        return {
          name: 'error',
          class: 'text-red',
        };
      }

      if (this.proof.status === 0) { // Pending
        return {
          name: 'fas fa-clock',
          class: 'text-grey',
        };
      }

      return {
        name: 'fas fa-check-circle',
        class: 'text-green',
      };
    },

    title() {
      if (this.proof.status === 0) {
        return 'Your timestamp is on it\'s way';
      }

      return this.$t('proofConfirmed');
    },

  },

  mounted() {
    if (this.scope.dialog) {
      this.proof.timestamp = this.proof.date;
      this.proof.base32Hash = this.proof.hash;
    }
    this.ready = true;
  },

  methods: {
    copy(text) {
      navigator.clipboard.writeText(text).then(() => {
        this.copyLabel = this.$t('copied');
        setTimeout(() => {
          this.copyLabel = this.$t('copy');
        }, 1500);
      }, (err) => {
        console.error('Async: Could not copy text: ', err);
      });
    },

    selectAnotherFile(scope) {
      if (this.user.remainingTimeStamps <= 0) {
        this.$emit('userHasReachedToLimit');
      }
      scope.reset();
    },

    getCertificate() {
      const name = `Timescribe Certificate ${this.proof.date}.pdf`;
      this.$pdf.create(name, this.proof.certificate);
    },
  },
};
</script>
<style lang="css">

.proof-item{
  padding: 16px;
}

.proof-dialog-item{
  padding: 16px;
}

.copy-button {
  right: -10px;
}

.proof-dialog{
  width: 100%;
}

@media screen and (max-width: 1500px) {
  .proof{
    width: 35rem;
  }
  .text-height-long textarea{
    height: 3rem !important;
  }
  .text-height-short textarea{
    height: 3rem !important;
  }
  .proof-dialog-item textarea{
    height: 3rem !important;
  }
}

@media screen and (max-width: 1000px) {
  .proof{
    width: 26rem;
  }
  .text-height-long textarea{
    height: 3.5rem !important;
  }
  .text-height-short textarea{
    height: 3.5rem !important;
  }
  .proof-dialog-item textarea{
    height: 4rem !important;
  }
}

@media screen and (max-width: 850px) {
  .proof{
    width: 28rem;
  }
  .text-height-long textarea{
    height: 4rem !important;
  }
  .text-height-short textarea{
    height: 4rem !important;
  }
  .proof-dialog-item textarea{
    height: 5rem !important;
  }
}

@media screen and (max-width: 575px) {
  .proof-dialog-item{
    width: 20rem !important;
  }
}

@media screen and (max-width: 540px) {
  .proof{
    width: 100%;
  }
  .text-height-long textarea{
    height: 5rem !important;
  }
  .text-height-short textarea{
    height: 5rem !important;
  }
  .proof-dialog-item{
    width: 15rem !important;
  }
  .proof-dialog-item textarea{
    height: 7rem !important;
  }
}

@media screen and (max-width: 250px) {
  .proof{
    width: 100%;
  }
  .text-height-long textarea{
    height: 6rem !important;
  }
  .text-height-short textarea{
    height: 6rem !important;
  }
  .proof-dialog-item{
    width: 10rem !important;
  }
  .proof-dialog-item textarea{
    height: 8rem !important;
  }
}

</style>
