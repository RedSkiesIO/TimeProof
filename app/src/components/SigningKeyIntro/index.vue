<template>
  <div
    class="flex column justify-center text-center q-pa-md q-gutter-sm"
    style="margin-top: 73px;"
  >
    <img
      src="~assets/signing-key.svg"
      style="height: 15vw"
    >
    <div class="text-h4 text-secondary">
      Welcome to Timescribe {{ user.givenName }}!
    </div>
    <div class="row justify-center">
      <div
        class="text-body1"
        style="width: 50vw;"
      >
        To get you started on Timescribe we need to create your signing key. <br>
        Your signing key is used to prove your ownership of a timestamp.
        It's stored locally on your device so that only you have access to it.
      </div>
    </div>

    <div class="row justify-center q-mt-md">
      <q-btn
        outline
        class="shade-color"
        label="Create your signing key"
        style="width: 250px"
        data-test-key="createYourSigningKey"
        @click="buttonAction"
      />
    </div>
  </div>
</template>
<script>

export default {
  name: 'SigningKeyIntro',

  props: {
    buttonAction: {
      required: true,
      type: Function,
    },
  },

  computed: {
    account() {
      return this.$auth.account();
    },
    user() {
      return this.$auth.user();
    },
  },

};
</script>
