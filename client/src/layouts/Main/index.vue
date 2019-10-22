<template>
  <q-layout view="lHh Lpr lFf">
    <q-header
      flat
      unelevated
    >
      <q-toolbar>
        <q-toolbar-title>
          {{ $t('documentSigner') }}
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
  name: 'MainLayout',
};
</script>
