<template>
  <q-card
    flat
    class="row"
  >
    <div class="column justify-center q-mx-sm">
      <q-btn
        id="accountEditProfileBtn"
        flat
        class="shade-color"
        label="Edit Profile"
        @click="editProfile"
      />
      <q-btn
        id="accountForgotPasswordBtn"
        flat
        class="shade-color"
        label="change password"
        @click="forgotPassword"
      />
      <q-btn
        id="accountUpgradePlanBtn"
        flat
        class="shade-color"
        label="upgrade plan"
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
      <div class="row q-gutter-x-md">
        <div class="">
          <div>membership:</div>
          <div class="text-secondary">
            {{ user.tier }} tier
          </div>
        </div>
        <div class="">
          <div>your allowance:</div>
          <div class="text-secondary">
            5 timestamps per month
          </div>
        </div>
      </div>
    </div>
  </q-card>
</template>
<script>
import User from '../../store/User';

export default {
  name: 'Account',

  data() {
    return {
      copyLabel: this.$t('copyPubKey'),
      isPwd: true,
    };
  },

  computed: {
    account() {
      return this.$auth.account();
    },

    user() {
      //return this.$auth.user(false, true, 'timestamps');
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
