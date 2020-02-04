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
      <div
        v-if="!proof.verified"
        class="text-body2"
      >
        {{ proof.error }}
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

      <div v-if="proof.verified">
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

export default {
  name: 'VerifyResult',

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

    getDate() {
      const date = new Date(this.proof.date);

      return `${date.toLocaleTimeString()} ${date.toLocaleDateString()}`;
    },

    icon() {
      if (!this.proof.verified) {
        return {
          name: 'error',
          class: 'text-red',
        };
      }

      return {
        name: 'fas fa-check-circle',
        class: 'text-green',
      };
    },

    title() {
      if (this.proof.verify && !this.proof.verified) {
        return this.$t('proofNotVerified');
      }

      return this.$t('proofVerified');
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