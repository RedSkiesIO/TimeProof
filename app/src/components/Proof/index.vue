<template>
  <div
    class="proof"
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

    <div class="column q-px-md">
      <div class="row proof-item justify-between">
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
        <div class="row proof-item justify-between">
          <template v-if="scope.dialog">
            <div class="col">
              <q-input
                v-model="getDate"
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
              {{ getDate }}
            </div>
          </template>
        </div>

        <div class="row proof-item justify-between">
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
          class="row proof-item justify-between text-height"
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
                color="grey"
                icon="filter_none"
                class="copy-button absolute-bottom-right"
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
          class="row proof-item justify-between"
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
                color="grey"
                icon="filter_none"
                class="copy-button absolute-bottom-right"
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
          class="row proof-item justify-between"
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
                color="grey"
                icon="filter_none"
                class="copy-button absolute-bottom-right"
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
      v-if="!scope.dialog"
      class="q-mt-sm text-blue text-center q-pb-md cursor-pointer"
      @click="selectAnotherFile(scope)"
    >
      {{ $t('anotherFile') }}
    </div>
    <div class="q-px-lg flex flex-center column text-center q-pt-lg">
      <template v-if="scope.dialog">
        <q-btn
          v-if="proof.status !== 0 && ready"
          text-color="secondary"
          color="white"
          label="Download Certificate"
          @click="getCertificate"
        />
      </template>
      <template v-else>
        <q-btn
          v-if="proof.status !== 0 && ready"
          outline
          color="secondary"
          label="Download Certificate"
          @click="getCertificate"
        />
      </template>
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
      return this.proof.name && this.proof.name.length < 51 ? 'text-height-short' : 'text-height-long';
    },

    signedBy() {
      return !this.proof.verify ? `${this.user.name}(${this.user.email})` : this.proof.pubKey.toLowerCase();
    },

    getDate() {
      console.log(this.proof.date);
      const date = new Date(this.proof.date);

      return `${date.toLocaleTimeString()} ${date.toLocaleDateString()}`;
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

.copy-button {
  right: -10px;
}

@media screen and (max-width: 1500px) {
  .text-height-long textarea{
    height: 2rem !important;
  }
  .text-height-short textarea{
    height: 4rem !important;
  }
}

@media screen and (max-width: 1000px) {
  .text-height-long textarea{
    height: 3rem !important;
  }
  .text-height-short textarea{
    height: 4rem !important;
  }
}

@media screen and (max-width: 750px) {
  .text-height-long textarea{
    height: 4rem !important;
  }
  .text-height-short textarea{
    height: 4rem !important;
  }
}

@media screen and (max-width: 300px) {
  .text-height-long textarea{
    height: 5rem !important;
  }
  .text-height-short textarea{
    height: 4rem !important;
  }
}

</style>
