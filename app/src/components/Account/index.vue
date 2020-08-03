<template>
  <q-card
    flat
    class="row bg-grey-2 flex flex-center"
  >
    <div class="col-xs-12 col-sm-12 col-md-4 col-lg-4">
      <div class="row">
        <q-btn
          id="editProfileBtn"
          flat
          class="shade-color q-mb-md"
          label="Edit Profile"
          @click="editProfile"
        />
      </div>
      <div class="row">
        <q-btn
          id="changePasswordBtn"
          flat
          class="shade-color"
          label="change password"
          @click="forgotPassword"
        />
      </div>
    </div>

    <div
      class="col-xs-12 col-sm-12 col-md-8 col-lg-8 q-gutter-y-md q-mt-xs"
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
          class="row wrapword"
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
<style lang="scss" scoped>
.signing-key {
  width: 100%;
}
</style>
