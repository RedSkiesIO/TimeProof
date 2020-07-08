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
        title="Signing key created"
        icon="fas fa-thumbs-up"
      >
        <q-dialog
          v-model="successMode"
          persistent
          transition-show="flip-down"
          transition-hide="flip-up"
        >
          <Success @close="successClose" />
        </q-dialog>
      </q-step>

      <template
        v-if="step === 1"
        v-slot:navigation
      >
        <q-stepper-navigation>
          <div class="text-center">
            <q-btn
              class="shade-color"
              data-test-key="newKeyContinue"
              :disable="!isFilledInput && step === 1"
              label="Continue"
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
// import SaveKeystore from './SaveKeystore';
import Success from './Success';
import User from '../../store/User';


export default {
  name: 'NewKeyStepper',
  components: {
    EncryptKey,
    // SaveKeystore,
    Success,
  },

  data() {
    return {
      step: 1,
      // disableButton: true,
      successMode: false,
      mounted: false,
    };
  },

  computed: {
    account() {
      return this.$auth.account();
    },
    user() {
      return this.$auth.user();
    },
    isFilledInput() {
      if (this.mounted) {
        const { input } = this.$refs.form.$refs;
        input.validate();

        if (!input.hasError) {
          return true;
        }
      }

      return false;
    },
  },

  mounted() {
    this.mounted = true;
  },

  methods: {

    clickAction() {
      switch (this.step) {
        case 1:
          this.addKey();
          this.successMode = true;
          break;
        // case 2:
        //   this.$refs.stepper.next();
        //   this.successMode = true;
        //   break;
        default:
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

        this.$crypto.createKeystore(this.user);
      }
    },

    async unlockKey() {
      const decrypted = await this.$crypto.decrypt(
        this.user.secretKey, this.$refs.form.$refs.input.value,
      );
      await this.$store.dispatch('settings/setAuthenticatedAccount', decrypted);
      this.$refs.stepper.next();
    },

    // downloadKeystore() {
    //   this.disableButton = false;
    //   this.$crypto.createKeystore(this.user);
    // },

    successClose() {
      this.successMode = false;
      this.$router.push('/dashboard');
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
