<template>
  <div class="proof">
    <div
      v-if="ready"
      id="proof"
      class="q-px-lg flex flex-center column text-center q-pt-lg"
    >
      <q-icon
        v-if="proof.verify && !proof.verified"
        name="error"
        class="text-red"
        style="font-size: 100px"
      />
      <q-icon
        v-else
        name="fas fa-check-circle"
        class="text-green"
        style="font-size: 100px"
      />
      <span
        v-if="proof.verify && proof.verified"
        class="text-h6 q-my-sm"
      >{{ $t('proofVerified') }}</span>
      <div
        v-else-if="proof.verify && !proof.verified"
        class="text-h6 q-my-sm"
      >
        <div>{{ $t('proofNotVerified') }}</div>
        <div class="text-body2">
          {{ proof.error }}
        </div>
      </div>
      <div
        v-else
        class="column"
      >
        <span
          class="text-h6 q-my-sm"
        >{{ $t('proofConfirmed') }}</span>
        <q-btn
          outline
          color="primary"
          label="Download Certificate"
          @click="getCertificate"
        />
        <a
          class="text-blue q-mt-sm"
          :href="etherscanTx"
          target="_blank"
        >
          View Transaction
        </a>
      </div>
    </div>

    <div class="column q-px-md">
      <div class="row proof-item justify-between">
        <div class="col-auto">
          {{ $t('file') }}:
        </div>
        <div class="col-auto">
          {{ proof.name }}
        </div>
      </div>

      <div v-if="!proof.verify || (proof.verify && proof.verified)">
        <div class="row proof-item justify-between">
          <div class="col-auto">
            {{ $t('date') }}:
          </div>
          <div
            v-if="ready"
            class="col-auto"
          >
            {{ getDate }}
          </div>
        </div>

        <div class="row proof-item justify-between">
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
        </div>
        <div class="row proof-item justify-between">
          <div class="col">
            <q-input
              v-model="proof.txId"
              filled
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

        <div class="row proof-item justify-between">
          <div class="col">
            <q-input
              v-model="proof.base32Hash"
              filled
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
                @click="copy(proof.base32Hash)"
              >
                <q-tooltip anchor="top middle">
                  {{ copyLabel }}
                </q-tooltip>
              </q-btn>
            </q-input>
          </div>
        </div>

        <div class="row proof-item justify-between">
          <div class="col">
            <q-input
              v-model="proof.signature"
              filled
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
      class="q-mt-sm text-blue text-center q-pb-md"
      @click="scope.reset()"
    >
      {{ $t('anotherFile') }}
    </div>
  </div>
</template>
<script>
import User from '../../store/User';

export default {
  name: 'Proof',

  props: {
    proof: {
      type: Object,
      required: true,
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
    };
  },

  computed: {
    etherscanTx() {
      return `https://kovan.etherscan.io/tx/${this.proof.txId}`;
    },

    user() {
      if (User.all().length > 0) {
        return User.query().first();
      }
      return null;
    },

    getDate() {
      const date = new Date(this.proof.timestamp);
      return `${date.toLocaleTimeString()} ${date.toLocaleDateString()}`;
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
    viewTx() {

    },

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

    reset() {
      this.scope.reset();
    },

    getCertificate() {
      const name = `${this.proof.timestamp}.pdf`;
      const splitString = (string, index) => ({
        one: string.substr(0, index),
        two: string.substr(index),
      });

      const hash = splitString(this.proof.base32Hash.toLowerCase(), 65);
      const proofId = splitString(this.proof.txId.toLowerCase(), 65);
      const signature = splitString(this.proof.signature.toLowerCase(), 65);
      const file = {
        file: this.proof.name,
        hash,
        proofId,
        signature,
        user: this.user.name,
        timestamp: this.getDate,
      };
      this.$pdf(name, file);
    },
  },
};
</script>
<style lang="scss">

.proof-item{
    padding: 16px;
}

.break {
  word-break: break-all;
}
.copy-button {
  right: -10px;
}

</style>
