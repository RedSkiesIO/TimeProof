<template>
  <div class="flex column justify-center text-center q-gutter-sm">
    <img
      src="~assets/encrypt-key.svg"
      style="height: 15vw"
    >
    <div class="text-box">
      <div class="text-h6 text-secondary">
        Choose a password to encrypt your signing key
      </div>
      <div class="encrypt-desc">
        To maintain security,
        we strongly recommend that you choose a password different from your account password.
        <br>
        <strong>Please record this password</strong>
        as it cannot be reset/retrieved once it has been setup.
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
        :label="$t('enterPassword')"
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
