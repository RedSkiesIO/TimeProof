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
        <q-space />
        <q-tabs
          v-model="tab"
          indicator-color="primary"
          shrink
        >
          <q-route-tab
            name="about"
            label="About"
            to="/about"
          />
          <q-tab
            v-if="!isLoggedIn"
            name="signin"
            :label="$t('signin / register')"
            @click.prevent="logIn"
          />
          <q-tab
            v-else
            name="logout"
            :label="$t('logout')"
            @click.prevent="logOut"
          />
        </q-tabs>
      </q-toolbar>
    </q-header>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script>
import { auth } from '../../helpers/adal';

export default {
  name: 'MainLayout',

  data() {
    return {
      tab: '',
    };
  },

  computed: {
    isLoggedIn() {
      return auth.isLoggedIn();
    },
  },

  methods: {
    logIn(e) {
      e.preventDefault();
      auth.login();
    },

    logOut(e) {
      e.preventDefault();
      auth.logout();
    },
  },
};
</script>
