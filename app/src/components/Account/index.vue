<template>
  <q-card
    flat
    class="row bg-grey-2"
  >
    <div class="column justify-center q-mx-sm">
      <q-btn
        flat
        color="blue"
        label="Edit Profile"
        @click="editProfile"
      />
      <q-btn
        flat
        color="blue"
        label="change password"
        @click="forgotPassword"
      />
    </div>
    <div
      class="column justify-center q-gutter-y-md q-mr-sm"
      style="height:100%; margin-top: -9px;"
    >
      <div class="row q-gutter-x-sm q-mb-xs">
        <div><q-icon name="fas fa-user" /></div>
        <div>{{ user.name }}</div>
      </div>
      <div class="row q-gutter-x-sm q-mb-xs">
        <div><q-icon name="fas fa-envelope" /></div>
        <div>{{ user.email }}</div>
      </div>
      <div
        v-if="user.pubKey"
        class="row q-gutter-x-sm q-mb-xs"
      >
        <div><q-icon name="fas fa-key" /></div>
        <div
          class="row overflow"
          @click="copy(user.pubKey)"
        >
          {{ user.pubKey.toLowerCase() }}
          <q-tooltip>
            {{ copyLabel }}
          </q-tooltip>
        </div>
      </div>
    </div>
  </q-card>
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

    editProfile(e) {
      e.preventDefault();
      this.$auth.editProfile();
    },

    forgotPassword(e) {
      e.preventDefault();
      this.$auth.forgotPassword();
    },
  },
};
</script>
<style lang="scss">

.signing-key {
  width: 100%;
}
</style>
