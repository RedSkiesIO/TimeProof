<template>
  <div class="q-pa-md">
    <q-stepper
      ref="stepper"
      v-model="step"
      color="primary"
      animated
    >
      <q-step
        :name="1"
        title="Encrypt your key"
        icon="settings"
        :done="step > 1"
      >
        <EncryptKey />
      </q-step>

      <q-step
        :name="2"
        title="Backup your key file"
        icon="create_new_folder"
        :done="step > 2"
      >
        <SaveKeystore />
      </q-step>

      <q-step
        :name="3"
        title="Success"
        icon="assignment"
      >
        <Success />
      </q-step>

      <template v-slot:navigation>
        <q-stepper-navigation>
          <div class="text-center">
            <q-btn
              v-if="step > 1"
              flat
              color="primary"
              label="Back"
              class="q-ml-sm"
              @click="$refs.stepper.previous()"
            />
            <q-btn
              color="primary"
              :label="step === 3 ? 'Go to Dashboard' : 'Continue'"
              @click="$refs.stepper.next()"
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
};
</script>
<style lang="scss">

</style>
<style lang="scss" scoped>
.q-stepper {
    box-shadow: none;
}
</style>
