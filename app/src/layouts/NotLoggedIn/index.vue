<template>
  <div class="q-pa-md q-gutter-md relative-position">
    <div
      class="row justify-center q-pt-md text-weight-bold"
    >
      <img
        class="logo-white-bg"
        src="~assets/logo-white-bg.png"
      >
    </div>
  </div>
</template>

<script>
import { QSpinnerGrid } from 'quasar';

export default {
  name: 'NotLoggedInLayout',

  created() {
    /* This is for Codepen (using UMD) to work */
    const spinner = typeof QSpinnerGrid !== 'undefined'
      ? QSpinnerGrid // Non-UMD, imported above
        : Quasar.components.QSpinnerGrid // eslint-disable-line
      /* End of Codepen workaround */

    this.$q.loading.show({
      spinner,
      spinnerColor: 'primary',
      spinnerSize: 70,
      backgroundColor: 'transparent',
      message: this.$t('loadingLoginSignupPage'),
      messageColor: 'black',
      delay: 0,
    });

    // hiding in 3s
    this.timer = setTimeout(() => {
      this.$q.loading.hide();
      clearTimeout(this.timer);
    }, 100000);
  },


  beforeDestroy() {
    if (this.timer) {
      clearTimeout(this.timer);
      this.$q.loading.hide();
    }
  },
};
</script>

<style lang="scss" scoped>
.logo-white-bg{
  font-size: 3rem;
  letter-spacing: 2px;
}
</style>
