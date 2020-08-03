<template>
  <div class="flex column justify-center text-center q-gutter-sm">
    <img
      src="~assets/encrypt-key.svg"
      style="height: 15vw"
    >
    <div class="text-box">
      <div class="text-h6 text-secondary">
        6-digit PIN
      </div>
      <div class="encrypt-desc">
        Please set a PIN to protect your signing key.
        <br>
        <strong>Make sure to note down your PIN as it
          cannot be reset or recreated if you lose it.
        </strong>
        You will require your PIN for creating asset evidences.
      </div>
    </div>
    <div class="row justify-center text-center q-mt-md">
      <q-input
        ref="input"
        v-model="password"
        mask="######"
        :type="isPwd ? 'password' : 'text'"
        filled
        autofocus
        data-test-key="encryptKeyPassword"
        :label="$t('choosePin')"
        style="width: 25vw"
        :rules="[val => val && val.length === 6 || $t('invalidPinLength')]"
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
  </div>
</template>
<script>

export default {
  name: 'EncryptKey',

  data() {
    return {
      password: '',
      isPwd: true,
    };
  },
};
</script>
<style lang="scss">

</style>
