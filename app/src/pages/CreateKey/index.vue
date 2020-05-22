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
import moment from 'moment';
import SigningKeyIntro from '../../components/SigningKeyIntro';
import NewKeyStepper from '../../components/NewKeyStepper';

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
    const keyMoment = moment(user.keyEmailDate, 'YYYY-MM-DD');
    if ((user && user.secretKey) || keyMoment.year() !== 1) {
      this.$router.push('/dashboard');
    }
  },
};
</script>
<style lang="scss">

</style>
