<template>
  <div class="flex column justify-center text-center q-gutter-sm">
    <img
      src="~assets/keystore.svg"
      style="height: 15vw"
    >
    <div class="text-h6 text-secondary">
      Please download your signing key file
    </div>
    <div class="row justify-center text-center">
      <div class="desc">
        You will require this file when you sign in on a new device or browser.
        <br>
        <strong>The signing key file cannot be regenerated.</strong>
        So make sure to store it somewhere safe and take backups.
      </div>
    </div>
    <div class="row justify-center">
      <q-btn
        outline
        label="download keystore file"
        style="width: 250px"
        class="shade-color"
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
