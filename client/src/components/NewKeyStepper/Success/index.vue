<template>
  <div class="flex column justify-center text-center">
    <img
      src="~assets/success.svg"
      style="height: 15vw"
    >
    <div class="text-box">
      <div class="text-h4 text-weight-bold text-primary">
        You're all set!
      </div>
      <div class="encrypt-desc text-body2">
        You can now head to the dashboard and start making timestamps
      </div>
    </div>
    <div class="row justify-center text-center" />
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
