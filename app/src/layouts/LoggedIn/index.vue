<template>
  <q-layout view="hHh Lpr lff">
    <q-header
      flat
      unelevated
      class="bg-primary text-white"
    >
      <q-toolbar>
        <q-btn
          flat
          round
          dense
          icon="menu"
          @click="drawer = !drawer"
        />
        <div
          class="logo text-center text-weight-bold q-pt-sm"
          @click="$router.push('/')"
        >
          <img
            src="~assets/logo.png"
            style="width: 175px"
          >
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
              data-test-key="logout"
              :label="$t('logout')"
              @click="logOut"
            />
          </div>
        </q-tabs>
      </q-toolbar>
    </q-header>

    <q-drawer
      v-model="drawer"
      side="left"
      show-if-above
      :mini="miniState"
      mini-to-overlay
      :width="200"
      :breakpoint="500"
      :height="900"
      content-class="bg-primary text-white"
      @mouseover="miniState = false"
      @mouseout="miniState = true"
    >
      <q-scroll-area class="fit">
        <div class="q-mt-lg columm justify-between text-left">
          <q-list>
            <q-item
              v-for="item in menuList"
              :key="item.label"
              v-ripple
              clickable
              :data-test-key="item.label"
              :class="item.redirect ? 'fixed-bottom' : ''"
              :to="item.route"
              :disable="currentPath === '/new-key' && !item.redirect"
              :active-class="!item.redirect ? 'my-menu-link' : ''"
              @click="redirectToExternalUrl(item.redirect)"
            >
              <q-item-section avatar>
                <q-icon
                  :size="item.size"
                  :name="item.icon"
                />
              </q-item-section>
              <q-item-section>
                {{ item.label }}
              </q-item-section>
            </q-item>
          </q-list>
        </div>
      </q-scroll-area>
    </q-drawer>

    <q-page-container>
      <router-view v-if="display" />
      <q-inner-loading :showing="!display">
        <q-spinner-grid
          size="70px"
          color="primary"
        />
        <div
          class="text-body1 q-mt-lg"
        >
          {{ $t('loadingDashboard') }}
        </div>
      </q-inner-loading>
    </q-page-container>

    <div class="user-dialog">
      <q-dialog
        v-if="account && user"
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

const menuList = [
  {
    icon: 'fas fa-home',
    label: 'Dashboard',
    route: '/dashboard',
    size: '1rem',
  },
  {
    icon: 'fas fa-stamp',
    label: 'Stamp',
    route: '/stamp',
    size: '1rem',
  },
  {
    icon: 'img:statics/icons/fingerprint.png',
    label: 'Verify',
    route: '/verify',
    size: '1.1rem',
  },
  {
    icon: 'fas fa-user',
    label: 'Account',
    route: '/account',
    size: '1rem',
  },
  {
    icon: 'fas fa-info-circle',
    label: 'Need Help ?',
    route: '/',
    redirect: 'https://www.timeproof.it/faq',
    size: '1rem',
  },
];

export default {
  name: 'LoggedInLayout',

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
      drawer: false,
      miniState: true,
    };
  },

  computed: {
    isLoggedIn() {
      return !!this.account;
    },
    account() {
      return this.$auth.account();
    },
    user() {
      return this.$auth.user();
    },
    currentPath() {
      return this.$route.path;
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
    redirectToExternalUrl(url) {
      if (url) {
        window.open(url);
      }
    },
  },
};
</script>
<style lang="scss" scoped>

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

.q-drawer--left {
  position: fixed;
}
</style>
<style lang="scss" scoped>
.logo {
  font-size: 1.35rem;
  width: 176px;
  letter-spacing: 2px;
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
.my-menu-link{
  color: white;
  font-weight: bold;
  background: transparentize($color: white, $amount: 0.6);
}
</style>
