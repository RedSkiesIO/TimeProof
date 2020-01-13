<template>
  <div class="flex column justify-center text-center q-gutter-sm">
    <img
      src="~assets/keystore.svg"
      style="height: 15vw"
    >
    <div class="text-h6 text-primary">
      Save your keystore file
    </div>
    <div class="row justify-center text-center">
      <div class="desc">
        If you change your device or switch browser you'll need to import your keystore file
        to restore your signing key so make sure you store it somewhere safe and make backups
      </div>
    </div>
    <div class="row justify-center">
      <q-btn
        outline
        label="download keystore file"
        style="width: 250px"
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
