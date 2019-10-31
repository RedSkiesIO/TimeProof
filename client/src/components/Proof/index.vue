<template>
  <div>
    <div class="q-pa-sm flex flex-center column text-center">
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
      <span
        v-else
        class="text-h6 q-my-sm"
      >{{ $t('proofConfirmed') }}</span>
    </div>

    <div class="column">
      <div class="row proof-item justify-between">
        <div class="col-auto">
          {{ $t('file') }}:
        </div>
        <div class="col-auto">
          {{ proof.name }}
        </div>
      </div>

      <div
        v-if="proof.type"
        class="row proof-item justify-between"
      >
        <div class="col-auto">
          {{ $t('type') }}:
        </div>
        <div class="col-auto">
          {{ proof.type }}
        </div>
      </div>

      <div class="row proof-item justify-between">
        <div class="col-auto">
          {{ $t('size') }}:
        </div>
        <div class="col-auto">
          {{ proof.size }}
        </div>
      </div>
      <div v-if="!proof.verify || (proof.verify && proof.verified)">
        <div class="row proof-item justify-between">
          <div class="col-auto">
            {{ $t('signed') }}:
          </div>
          <div class="col-auto">
            {{ getDate }}
          </div>
        </div>

        <div class="row proof-item justify-between">
          <div class="col-auto">
            {{ $t('signedBy') }}:
          </div>
          <div
            v-if="!proof.verify"
            class="col-auto"
          >
            {{ user.name }} ({{ user.email }})
          </div>
          <div
            v-else
            class="col-auto"
          >
            {{ proof.pubKey }}
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
      class="q-mt-sm text-blue text-center"
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
    };
  },

  computed: {
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
