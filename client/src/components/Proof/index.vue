<template>
  <div>
    <div class="q-pa-sm flex flex-center column text-center">
      <q-icon
        name="fas fa-check-circle"
        class="text-green"
        style="font-size: 100px"
      />
      <span class="text-h6 q-mb-sm">Your proof has been confirmed!</span>
    </div>

    <div class="column">
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

      <div class="row proof-item justify-between">
        <div class="col-2">
          id:
        </div>
        <div class="col-auto">
          {{ user.pubKey }}
        </div>
      </div>

      <div class="row proof-item justify-between">
        <div class="col">
          <q-input
            v-model="proof.base32Hash"
            filled
            readonly
            stack-label
            label="hash"
            autogrow
          />
        </div>
      </div>

      <div class="row proof-item justify-between">
        <div class="col">
          <q-input
            v-model="proof.signature"
            filled
            readonly
            stack-label
            label="signature"
            autogrow
          />
        </div>
      </div>
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
  },

  data() {
    return {
      fileHash: this.proof.base32Hash,
    };
  },

  computed: {
    user() {
      if (User.all().length > 0) {
        return User.query().first();
      }
      return null;
    },
  },
};
</script>
<style lang="scss">

.proof-item{
    padding: 16px;
}

.q-textarea.q-field--labeled .q-field__native {
    min-height: 0;
}
</style>
