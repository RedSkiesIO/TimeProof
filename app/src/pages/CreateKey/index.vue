<template>
  <div>
    <SigningKeyIntro
      v-if="!start"
      :button-action="() => start=true"
    />
    <NewKeyStepper v-else />
  </div>
</template>
<script>
import SigningKeyIntro from '../../components/SigningKeyIntro';
import NewKeyStepper from '../../components/NewKeyStepper';
import User from '../../store/User';

export default {
  name: 'CreateKey',
  components: {
    SigningKeyIntro,
    NewKeyStepper,
  },
  data() {
    return {
      start: false,
    };
  },
  created() {
    const user = this.$auth.user();
    if (user && user.secretKey) {
      this.$router.push('/dashboard');
    } else {
      User.update({
        data: {
          accountIdentifier: this.$auth.account().accountIdentifier,
          firstTimeDialog: true,
        },
      });
    }
  },
};
</script>
<style lang="scss">

</style>
