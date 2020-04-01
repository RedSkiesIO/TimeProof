<template>
  <div>
    <q-card
      flat
      class="left-box text-weight-bold q-px-md"
    >
      <div
        class="column justify-center q-gutter-y-md q-px-md"
        style="height:100%"
      >
        <div class="row q-gutter-x-sm q-mb-xs q-mt-lg">
          <div><q-icon name="fas fa-user" /></div>
          <div>{{ user.name }}</div>
        </div>
        <div class="row q-gutter-x-sm q-mb-xs">
          <div><q-icon name="fas fa-envelope" /></div>
          <div>{{ user.email }}</div>
        </div>
        <div
          class="row justify-center"
          style="margin-top: 9px;"
        >
          <q-item to="/account">
            <q-item-section class="text-blue text-uppercase">
              View Account
            </q-item-section>
          </q-item>
        </div>
      </div>
    </q-card>
  </div>
</template>
<script>

export default {
  name: 'Account',

  data() {
    return {
      copyLabel: this.$t('copyPubKey'),
      isPwd: true,
    };
  },

  computed: {
    user() {
      return this.$auth.user();
    },
    key() {
      return this.user.secretKey;
    },
  },

  methods: {
    copy(text) {
      navigator.clipboard.writeText(text.toLowerCase()).then(() => {
        this.copyLabel = this.$t('copied');
        setTimeout(() => {
          this.copyLabel = this.$t('copyPubKey');
        }, 1500);
      }, (err) => {
        console.error('Async: Could not copy text: ', err);
      });
    },
  },
};
</script>
<style lang="scss">

.signing-key {
  width: 100%;
}
</style>
