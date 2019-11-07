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
            :label="$t('about')"
            to="/about"
          />
          <q-tab
            v-if="!isLoggedIn"
            name="signin"
            :label="$t('signUpSignIn')"
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

export default {
  name: 'MainLayout',

  data() {
    return {
      tab: '',
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

  methods: {
    logIn(e) {
      e.preventDefault();
      this.$auth.signIn();
    },

    logOut(e) {
      e.preventDefault();
      this.$auth.logout();
    },
  },
};
</script>
