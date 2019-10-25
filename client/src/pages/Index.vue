<template>
  <q-page class="flex flex-center">
    <q-card
      flat
      bordered
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
};
</script>
<style lang="scss" scoped>
.q-tab-panel {
  padding: 0;
}
</style>
