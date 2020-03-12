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
};
</script>
<style lang="scss">

</style>
