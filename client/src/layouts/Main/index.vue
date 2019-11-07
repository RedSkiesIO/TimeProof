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
          <q-tab
            size="sm"
            name="account"
            icon="account_circle"
          />
        </q-tabs>
      </q-toolbar>
    </q-header>

    <q-page-container>
      <router-view />
    </q-page-container>
    <div class="user-dialog">
      <q-dialog
        v-model="dialog"
        seamless
        :position="position"
        content-class="user-dialog"
      >
        <q-card style="width: 350px">
          <q-card-section class="column items-center no-wrap">
            <div>
              <div class=" text-center text-weight-bold">
                {{ user.name }}
              </div>
              <q-space />
</div>
          </q-card-section>
        </q-card>
      </q-dialog>
    </div>
  </q-layout>
</template>

<script>
import User from '../../store/User';

export default {
  name: 'MainLayout',

  data() {
    return {
      tab: '',
      dialog: true,
      position: 'top',
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
    account() {
      const account = this.$auth.account();
      if (!account) {
        return null;
      }
      return account;
    },
    user() {
      if (this.account) {
        const user = User.find(this.account.accountIdentifier);
        if (user) {
          return user;
        }
      }
      return null;
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
<style lang="scss">
.user-dialog .fixed-top {
  top: 50px;
  right: 0;
  left: auto;
  padding: 0;
}
</style>
