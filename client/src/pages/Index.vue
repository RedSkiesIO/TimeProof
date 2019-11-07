<template>
  <q-page class="flex flex-center">
    <q-card
      v-if="isLoggedIn"
      flat
    >
      <q-tabs
        v-model="tab"
        dense
        class="bg-white text-primary"
      >
        <q-tab
          name="sign"
          :label="$t('sign')"
        />
        <q-tab
          name="verify"
          :label="$t('verify')"
        />
      </q-tabs>
      <q-tab-panels
        v-model="tab"
        animated
      >
        <q-tab-panel name="sign">
          <AddFile :mode="'sign'" />
        </q-tab-panel>

        <q-tab-panel name="verify">
          <AddFile :mode="'verify'" />
        </q-tab-panel>
      </q-tab-panels>
    </q-card>
    <q-card
      v-else
      flat
    >
      {{ $t('notSignedIn') }}
    </q-card>
  </q-page>
</template>

<script>
import AddFile from '../components/AddFile';

export default {
  name: 'PageIndex',
  components: {
    AddFile,
  },

  data() {
    return {
      tab: 'sign',
    };
  },

  computed: {
    isLoggedIn() {
      const account = this.$auth.account();
      if (!account) {
        return false;
      }
      return true;
    },
  },
};
</script>
<style lang="scss">
.q-tab-panel {
  padding: 0;
  height: inherit;
}

.q-panel {
  height: inherit;
}

.q-card > div:first-child {
    border-top: 2px solid rgba(0, 0, 0, 0.12);
    border-left: 2px solid rgba(0, 0, 0, 0.12);
    border-right: 2px solid rgba(0, 0, 0, 0.12);

}
</style>
