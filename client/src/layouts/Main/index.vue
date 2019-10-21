<template>
  <q-layout view="lHh Lpr lFf">
    <q-header flat unelevated>
      <q-toolbar>
        <q-toolbar-title>
          Document Signer
        </q-toolbar-title>
      </q-toolbar>
    </q-header>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script>
export default {
  name: 'MyLayout',

  data() {
    return {
      leftDrawerOpen: false,
    };
  },
};
</script>
