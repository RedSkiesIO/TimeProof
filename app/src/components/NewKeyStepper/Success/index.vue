<template>
  <q-card
    flat
    class="q-pa-sm"
  >
    <div class="row justify-end">
      <q-icon
        v-close-popup
        size="md"
        name="close"
        class="cursor-pointer"
        @click="$emit('close')"
      />
    </div>
    <q-card-section class="row items-center">
      <div class="flex justify-center text-center">
        <img
          src="~assets/tick-animate.gif"
          style="height: 8vw; width: 8vw"
        >
        <!-- <img
          src="~assets/success.svg"
          style="height: 10vw"
        > -->
        <div class="text-box">
          <div class="text-h5 text-weight-bold text-secondary q-mt-md">
            Your signing key has been created
          </div>
          <div class="encrypt-desc text-body2 q-mt-md">
            You can now head to the dashboard and start making timestamps
          </div>
        </div>
      </div>
    </q-card-section>
  </q-card>
</template>
<script>

export default {
  name: 'Success',

  data() {
    return {
      password: '',
    };
  },
};
</script>
<style lang="scss">

</style>
