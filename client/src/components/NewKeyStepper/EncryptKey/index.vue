<template>
  <div class="flex column justify-center text-center q-gutter-sm">
    <img
      src="~assets/encrypt-key.svg"
      style="height: 15vw"
    >
    <div class="text-box">
      <div class="text-h6 text-primary">
        Choose a password to encrypt your key
      </div>
      <div class="encrypt-desc">
        In order to keep your signing key safe
        while it's stored in your browser storage we encrypt it with a password.
      </div>
    </div>
    <div class="row justify-center text-center q-mt-md">
      <q-input
        ref="newEncryptPassword"
        v-model="password"
        filled
        autofocus
        :label="$t('enterPassword')"
        style="width: 25vw"
      />
    </div>
  </div>
</template>
<script>

export default {
  name: 'EncryptKey',

  data() {
    return {
      password: '',
    };
  },
};
</script>
<style lang="scss">

</style>
