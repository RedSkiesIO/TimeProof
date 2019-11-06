<template>
  <q-page class="flex flex-center">
    <q-card
      flat
    >
      <q-tabs
        v-model="panel"
        dense
        class="bg-white text-primary"
      >
        <q-tab
          name="login"
          :label="$t('login')"
        />
        <!-- <q-tab
          name="register"
          :label="$t('signUp')"
        /> -->
      </q-tabs>
      <q-tab-panels
        v-model="panel"
        animated
      >
        <q-tab-panel
          name="login"
          class="q-gutter-y-md flex flex-center column text-center"
        >
          <!-- <q-input
            v-model="email"
            outlined
            :label="$t('email')"
          />
          <q-input
            v-model="password"
            outlined
            :label="$t('password')"
          /> -->
          <a href="/.auth/login/aad">
            <q-btn
              unelevated
              color="primary"
              :label="$t('log in with azure ad')"
              @click="login"
            />
          </a>
          <a href="/.auth/login/microsoftaccount">
            <q-btn
              unelevated
              color="primary"
              :label="$t('log in with microsoft')"
              @click="login"
            />
          </a>
          <a href="/.auth/login/google">
            <q-btn
              unelevated
              color="primary"
              :label="$t('log in with google')"
              @click="login"
            />
          </a>
        </q-tab-panel>

        <!-- <q-tab-panel
          name="register"
          class="q-gutter-y-md flex flex-center column text-center"
        >
          <q-input
            ref="name"
            v-model="name"
            outlined
            :label="$t('fullName')"
            lazy-rules
            :rules="[ val => val && val.length > 0 || $t('emptyName')]"
          />
          <q-input
            ref="email"
            v-model="email"
            outlined
            :label="$t('email')"
            lazy-rules
            :rules="[ val => val && emailRegex.test(val.toLowerCase()) ||
              $t('invalidEmail')]"
          />
          <q-input
            ref="password"
            v-model="password"
            outlined
            :label="$t('password')"
            type="password"
            lazy-rules
            :rules="[ val => val && val.length > 8 || $t('minCharactersErr'),
                      val => /\d/.test(val) || $t('noNumbersErr'),
                      val => /[A-Z]/.test(val) || $t('noUppercaseErr')]"
          />
          <q-btn
            unelevated
            color="primary"
            :label="$t('register')"
            @click="signUp"
          />
        </q-tab-panel> -->
      </q-tab-panels>
    </q-card>
  </q-page>
</template>

<script>
import User from '../../store/User';

export default {
  name: 'Login',
  props: {
    tab: {
      type: String,
      required: true,
    },
  },

  data() {
    return {
      name: null,
      email: null,
      password: null,
      emailRegex: /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,
    };
  },

  computed: {
    panel: {
      get() {
        return this.tab;
      },
      set(val) {
        this.$router.push({ path: `/${val}` });
      },
    },
  },


  methods: {
    login() {
      const account = User.find(this.email);
      if (account) {
        this.$store.dispatch('settings/setAuthenticatedAccount', account);
        this.$router.push({ path: '/' });
      }
      console.log('not found');
    },
    async signUp() {
      this.$refs.name.validate();
      this.$refs.email.validate();
      this.$refs.password.validate();

      if (!this.$refs.name.hasError
        || !this.$refs.email.hasError
        || !this.$refs.password.hasError) {
        console.log('success');
        // const keypair = this.$keypair.new();

        // User.insert({
        //   data: {
        //     pubKey: keypair.publicKey,
        //     secretKey: keypair.secretKey,
        //     name: this.name,
        //     email: this.email,
        //   },
        // });
        // this.$store.dispatch('settings/setAuthenticatedAccount', User.find(this.email));
        // this.$router.push({ path: '/' });
      }
    },
  },
};
</script>
<style lang="scss" scoped>
.q-tab-panel {
  padding: 0;
}

.q-card > div:first-child {
    border-top: 2px solid rgba(0, 0, 0, 0.12);
    border-left: 2px solid rgba(0, 0, 0, 0.12);
    border-right: 2px solid rgba(0, 0, 0, 0.12);

}

.q-tab-panel {
  width: inherit;
  max-height: inherit;
  min-width: 25rem;
  min-height: 25rem;
  border: 2px solid rgba(0, 0, 0, 0.12);
}

.q-tab-panel label {
    width: 75%;
}

.q-gutter-y-md, .q-gutter-md {
    margin-top: 0;
}
</style>
