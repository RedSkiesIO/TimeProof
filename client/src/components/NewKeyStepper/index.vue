<template>
  <div class="q-pa-md">
    <q-stepper
      ref="stepper"
      v-model="step"
      color="secondary"
      animated
    >
      <q-step
        :name="1"
        title="Encrypt your key"
        icon="fas fa-key"
        :done="step > 1"
      >
        <EncryptKey ref="form" />
      </q-step>

      <q-step
        :name="2"
        title="Backup your key file"
        icon="fas fa-file-download"
        :done="step > 2"
      >
        <SaveKeystore :button-action="downloadKeystore" />
      </q-step>

      <q-step
        :name="3"
        title="Signing key created"
        icon="fas fa-thumbs-up"
      >
        <Success />
      </q-step>

      <template v-slot:navigation>
        <q-stepper-navigation>
          <div class="text-center">
            <q-btn
              v-if="step > 1"
              flat
              color="secondary"
              label="Back"
              class="q-ml-sm"
              @click="$refs.stepper.previous()"
            />
            <q-btn
              color="secondary"
              :label="step === 3 ? 'Go to Dashboard' : 'Continue'"
              @click="clickAction"
            />
          </div>
        </q-stepper-navigation>
      </template>
    </q-stepper>
  </div>
</template>
<script>
import EncryptKey from './EncryptKey';
import SaveKeystore from './SaveKeystore';
import Success from './Success';
import User from '../../store/User';


export default {
  name: 'NewKeyStepper',
  components: {
    EncryptKey,
    SaveKeystore,
    Success,
  },

  data() {
    return {
      step: 1,
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
  },

  methods: {

    clickAction() {
      switch (this.step) {
        case 1:
          this.addKey();
          break;
        case 3:
          this.$router.push('/dashboard');
          break;
        default:
          this.$refs.stepper.next();
      }
    },

    async addKey() {
      const { input } = this.$refs.form.$refs;
      input.validate();

      if (!input.hasError) {
        let { keypair } = this;

        keypair = this.$keypair.new();

        const encrypted = await this.$crypto.encrypt(keypair.secretKey, input.value);
        await User.update({
          data: {
            accountIdentifier: this.account.accountIdentifier,
            pubKey: keypair.publicKey,
            secretKey: encrypted,
          },
        });

        await this.unlockKey(input.value);
      }
    },

    async unlockKey() {
      const decrypted = await this.$crypto.decrypt(
        this.user.secretKey, this.$refs.form.$refs.input.value,
      );
      await this.$store.dispatch('settings/setAuthenticatedAccount', decrypted);
      this.$refs.stepper.next();
    },

    downloadKeystore() {
      this.$crypto.createKeystore(this.user);
    },
  },
};
</script>
<style lang="scss">

</style>
<style lang="scss" scoped>
.q-stepper {
    box-shadow: none;
}
</style>
