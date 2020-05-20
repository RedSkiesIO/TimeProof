<template>
  <keep-alive>
    <component
      :is="selectedComponent"
      v-bind="currentProperties"
    />
  </keep-alive>
</template>

<script>

import LoggedIn from '../LoggedIn';
import NotLoggedIn from '../NotLoggedIn';

export default {
  name: 'MainLayout',

  components: {
    loggedIn: LoggedIn,
    notLoggedIn: NotLoggedIn,
  },

  props: {
    display: {
      type: Boolean,
      required: true,
    },
  },

  computed: {
    selectedComponent() {
      return this.$auth.account() ? 'loggedIn' : 'notLoggedIn';
      // lazy
      // const componentName = this.account ? 'LoggedIn' : 'NotLoggedIn';
      // return () => import(`../${componentName}`)
    },

    currentProperties() {
      if (this.selectedComponent === 'loggedIn') {
        return { display: this.display };
      }

      return null;
    },
  },

};
</script>
