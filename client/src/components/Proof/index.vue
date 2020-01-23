<template>
  <div class="proof">
    <div
      v-if="ready"
      id="proof"
      class="q-px-lg flex flex-center column text-center q-pt-lg"
    >
      <q-icon
        :name="icon.name"
        :class="icon.class"
        style="font-size: 100px"
      />

      <span
        class="text-h6 q-my-sm"
      >{{ title }}</span>
      <q-btn
        v-if="proof.blockNumber !== -1"
        outline
        color="secondary"
        label="Download Certificate"
        @click="getCertificate"
      />
      <a
        v-if="proof.blockNumber !== -1"
        class="text-blue q-mt-sm"
        :href="etherscanTx"
        target="_blank"
      >
        View Transaction
      </a>
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

      <div>
        <div class="row proof-item justify-between">
          <div class="col-auto">
            {{ $t('date') }}:
          </div>
          <div
            v-if="proof.blockNumber === -1"
            class="col-auto"
          >
            <q-chip
              square
              color="orange"
              text-color="white"
            >
              pending
            </q-chip>
          </div>
          <div
            v-if="ready && proof.blockNumber !== -1"
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
              v-model="proof.hash"
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
                @click="copy(proof.hash)"
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
    };
  },

  computed: {
    etherscanTx() {
      return `https://kovan.etherscan.io/tx/${this.proofId}`;
    },

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

    proof() {
      console.log(this.proofId);
      return this.proofId ? Timestamp.find(this.proofId) : null;
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

      if (this.proof.blockNumber === -1) {
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
      if (this.proof.blockNumber === -1) {
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

    reset() {
      this.scope.reset();
    },

    getCertificate() {
      const name = `${this.proof.date}.pdf`;
      this.$pdf(name, this.proof.certificate);
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
