<template>
  <div>
    <q-card
      flat
      class="account q-pa-sm"
    >
      <div class="row text-weight-bold text-h6 q-mb-xs">
        {{ $t('signingKey') }}
      </div>
      <div class="column bg-red-1 q-pa-sm">
        <div class="row text-center text-weight-bold text-red justify-center">
          {{ $t('backupKey') }}
        </div>
        <div class="row text-center justify-center">
          {{ $t('backupKeyDesc') }}
        </div>
        <div class="row justify-center q-mt-sm">
          <q-btn
            outline
            color="black"
            :label="$t('backup')"
          />
        </div>
      </div>
      <div class="row">
        <q-input
          v-model="key"
          :label="$t('signing')"
          readonly
          :type="isPwd ? 'password' : 'text'"
          class="q-my-sm signing-key"
        >
          <template v-slot:append>
            <q-icon
              :name="isPwd ? 'visibility_off' : 'visibility'"
              class="cursor-pointer"
              @click="isPwd = !isPwd"
            />
          </template>
        </q-input>
      </div>
      <div class="row justify-center text-blue q-mb-sm">
        {{ $t('importKey') }}
      </div>
      <div class="row justify-center text-blue">
        {{ $t('newKey') }}
      </div>
      <div class="row justify-end" />
    </q-card>
  </div>
</template>
<script>
import User from '../../store/User';

export default {
  name: 'Key',

  data() {
    return {
      isPwd: true,
      tiers: {
        free: 50,
        basic: 1000,
        standard: 10000,
        premium: 100000,
      },
    };
  },

  computed: {
    account() {
      const account = this.$auth.account();
      if (!account || account.idToken.tfp !== 'B2C_1_TimestampSignUpSignIn') {
        return null;
      }
      return account;
    },

    user() {
      if (this.account) {
        const user = User.query().whereId(this.account.accountIdentifier).with('timestamps').get();
        if (user) {
          return user[0];
        }
      }
      return null;
    },

    key() {
      return this.user.secretKey;
    },


  },
};
</script>
<style lang="scss">
.account {
    border: 2px solid rgba(0, 0, 0, 0.12);
    max-width: 350px;
}

.signing-key {
  width: 100%;
}

.signing-key .q-field__append .q-icon {
  display: flex;
}
</style>
