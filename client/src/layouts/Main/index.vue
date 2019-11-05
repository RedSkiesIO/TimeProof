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
          <span
            v-if="!$auth.account()"
            class="flex"
          >
            <!-- <q-route-tab
              name="login"
              :label="$t('login')"
              to="/login"
            />
            <q-route-tab
              name="register"
              :label="$t('register')"
              to="/register"
            /> -->
            <q-tab
              name="signin"
              :label="$t('signin / register')"
              @click="$auth.signIn()"
            />
          </span>

          <!-- <a
            href="https://easyauthtest3.azurewebsites.net/.auth/login/microsoftaccount?post_login_redirect_url=http%3A%2F%2Flocalhost%3A6420?"
          >
            <q-tab
              name="loginSignin"
              :label="$t('log in / register')"
            />
          </a> -->

          <q-tab
            v-else
            name="logout"
            :label="$t('logout')"
            @click="$auth.logout"
          />

          <q-tab
            name="token"
            :label="$t('token')"
            @click="$auth.getSessionToken()"
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

  methods: {
    async getToken() {
      const me = await this.$axios.get('https://easyauthtest3.azurewebsites.net/.auth/me');
      console.log(me);
    },
  },
};
</script>
