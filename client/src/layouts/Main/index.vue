<template>
  <q-layout view="hHh lpR fFf">
    <q-header
      flat
      unelevated
      class="bg-primary text-white"
    >
      <q-toolbar>
        <div
          class="logo text-center text-weight-bold"
          style="width: 176px; letter-spacing: 1px;"
        >
          Trustamp
        </div>
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
      :width="200"
      :breakpoint="500"
      content-class="bg-primary text-white"
    >
      <div class="q-mt-lg columm justify-end text-left">
        <q-list>
          <q-item
            v-for="item in menuList"
            :key="item.label"
            v-ripple
            clickable
            :to="item.route"
          >
            <q-item-section avatar>
              <q-icon :name="item.icon" />
            </q-item-section>
            <q-item-section>
              {{ item.label }}
            </q-item-section>
          </q-item>
        </q-list>
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
    icon: 'fas fa-home',
    label: 'Dashboard',
    route: '/dashboard',
  },
  {
    icon: 'fas fa-stamp',
    label: 'Stamp',
    route: '/stamp',
  },
  {
    icon: 'fas fa-fingerprint',
    label: 'Verify',
    route: '/verify',
  },
  {
    icon: 'settings',
    label: 'Settings',
    route: '/account',
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
    drawerClick(e) {
      // if in "mini" state and user
      // click on drawer, we switch it to "normal" mode
      if (this.miniState) {
        this.miniState = false;

        // notice we have registered an event with capture flag;
        // we need to stop further propagation as this click is
        // intended for switching drawer to "normal" mode only
        e.stopPropagation();
      }
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

.q-item.q-router-link--active {
  color: #ffffff;
  font-weight: bold;
}
</style>
<style lang="scss" scoped>
.logo {
  font-size: 1.35rem;

}
.q-item__section--side > .q-icon {
    font-size: 15px;
}

.q-item__section--avatar {
  min-width: 0;
  padding-bottom: 1px;
}

.q-item__section {
  font-size: 15px;
}
</style>
