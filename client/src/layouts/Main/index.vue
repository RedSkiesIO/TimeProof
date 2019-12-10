<template>
  <q-layout view="hHh lpR fFf">
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
          <q-tab
            v-if="!isLoggedIn"
            name="signin"
            :label="$t('signUpSignIn')"
            @click.prevent="logIn"
          />
          <div
            v-else
            class="flex"
          >
            <q-tab
              name="logout"
              :label="$t('logout')"
              @click="logOut"
            />
          </div>
        </q-tabs>
      </q-toolbar>
    </q-header>

    <q-drawer
      v-if="isLoggedIn"
      v-model="drawer"
      show-if-above
      :width="250"
      :breakpoint="500"
      bordered
    >
      <div class="q-mt-lg columm text-weight-bold text-h6 justify-end text-left">
        <div class="col q-px-lg">
          <q-btn
            flat
            color="primary"
            label="Dashboard"
            size="lg"
            to="/dashboard"
          />
        </div>
        <div class="col q-px-lg">
          <q-btn
            flat
            color="primary"
            label="Stamp"
            size="lg"
            to="/stamp"
          />
        </div>
        <div class="col q-px-lg">
          <q-btn
            flat
            color="primary"
            label="Verify"
            size="lg"
            to="/verify"
          />
        </div>
      </div>
    </q-drawer>

    <q-page-container>
      <router-view v-if="display" />
      <q-inner-loading :showing="!display">
        <q-spinner-grid
          size="70px"
          color="primary"
        />
        <div class="text-body1 q-mt-lg">
          {{ $t('loadingDashboard') }}
        </div>
      </q-inner-loading>
    </q-page-container>
    <div class="user-dialog">
      <q-dialog
        v-if="account"
        v-model="dialog"
        :position="position"
        transition-show="fade"
        transition-hide="fade"
        content-class="user-dialog"
      >
        <q-card
          flat
          bordered
          style="width: 350px"
        >
          <q-card-section class="column items-center no-wrap">
            <div class="text-center text-weight-bold">
              <q-icon
                class="text-grey-6"
                name="account_circle"
                size="xl"
              />
              <div class=" q-mt-sm text-center text-weight-bold">
                {{ user.name }}
              </div>
              <div class="text-center text-weight-bold">
                {{ user.email }}
              </div>
              <div
                class="text-body2 text-blue"
                @click="$auth.editProfile()"
              >
                {{ $t('editProfile') }}
              </div>
              <q-btn
                flat
                :label="$t('logout')"
                @click.prevent="logOut"
              />
            </div>
          </q-card-section>
        </q-card>
      </q-dialog>
    </div>
  </q-layout>
</template>

<script>
import User from '../../store/User';

const menuList = [
  {
    icon: 'inbox',
    label: 'DASHBOARD',
    separator: true,
  },
  {
    icon: 'send',
    label: 'Stamp',
    separator: false,
  },
  {
    icon: 'delete',
    label: 'Verify',
    separator: false,
  },
];

export default {
  name: 'MainLayout',

  props: {
    display: {
      type: Boolean,
      required: true,
    },
  },

  data() {
    return {
      tab: '',
      dialog: false,
      position: 'top',
      left: true,
      menuList,
      drawer: true,
    };
  },

  computed: {
    isLoggedIn() {
      const account = this.$auth.account();
      if (!account || account.idToken.tfp !== 'B2C_1_TimestampSignUpSignIn') {
        return false;
      }
      return true;
    },
    account() {
      const account = this.$auth.account();
      if (!account || account.idToken.tfp !== 'B2C_1_TimestampSignUpSignIn') {
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

.user-dialog .q-dialog__backdrop {
    background: none;
}

.left-nav .q-item-section {
  text-transform: 'uppercase';
  font-weight: 'bold';
  font-size: large;
}
</style>
