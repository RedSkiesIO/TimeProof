<template>
  <div class="flex column justify-center text-center q-gutter-sm">
    <img
      src="~assets/keystore.svg"
      style="height: 15vw"
    >
    <div class="text-h6 text-secondary">
      Please download the keystore file
    </div>
    <div class="row justify-center text-center">
      <div class="desc">
        The keystore file is your signing key.
        You will require it if you change your device or browser.
        <br>
        This keystore file cannot be regenerated,
        so make sure to store it somewhere safe and take backups
      </div>
    </div>
    <div class="row justify-center">
      <q-btn
        outline
        label="download keystore file"
        style="width: 250px"
        data-test-key="saveKeyDownloadKeystore"
        @click="buttonAction"
      />
    </div>
  </div>
</template>
<script>

export default {
  name: 'SaveKeystore',

  props: {
    buttonAction: {
      required: true,
      type: Function,
    },
  },
};
</script>
<style lang="scss">
.desc {
  width: 50vw;
}
</style>
