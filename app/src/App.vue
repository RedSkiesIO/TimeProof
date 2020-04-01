<template>
  <div
    id="q-app"
  >
    <router-view :display="ready" />
  </div>
</template>

<script>
import moment from 'moment';
import { mapActions } from 'vuex';
import User from './store/User';

export default {
  name: 'App',

  data() {
    return {
      ready: false,
    };
  },

  computed: {
    account() {
      return this.$auth.account();
    },

    user() {
      return this.$auth.user(false, true, 'timestamps');
    },
  },
  async created() {
    this.setProducts({
      Starter: {
        plan: 'plan_1',
        quantity: 1,
        tier: 'Starter',
        price: 0,
        freq: 'Per Month',
        timestamps: 10,
        color: 'green',
      },
      Basic: {
        plan: 'plan_2',
        quantity: 1,
        tier: 'Basic',
        price: 30.56,
        freq: 'Per Month',
        timestamps: 30,
        color: 'blue',
      },
      Premium: {
        plan: 'plan_3',
        quantity: 1,
        tier: 'Premium',
        price: 97.45,
        freq: 'Per Year',
        timestamps: 115,
        color: 'orange',
      },
      Gold: {
        plan: 'plan_4',
        quantity: 1,
        tier: 'Gold',
        price: 129.87,
        freq: 'Per Year',
        timestamps: 225,
        color: 'purple',
      },
    });
  },
  mounted() {
    this.start();
  },

  methods: {
    ...mapActions('settings', [
      'setProducts',
    ]),
    async start() {
      if (this.account) {
        const token = await this.$auth.getToken();
        this.$axios.defaults.headers.common.Authorization = `Bearer ${token.idToken.rawIdToken}`;
        if (!this.user) {
          const membership = this.account.idToken.extension_membershipTier || 'Starter';
          User.insert({
            data: {
              accountIdentifier: this.account.accountIdentifier,
              givenName: this.account.idToken.given_name,
              familyName: this.account.idToken.family_name,
              email: this.account.idToken.emails[0],
              tokenExpires: token.idToken.expiration,
              tier: membership,
            },
          });
        } else if (this.user) {
          const membership = this.account.idToken.extension_membershipTier || this.user.tier;
          User.update({
            data: {
              accountIdentifier: this.account.accountIdentifier,
              givenName: this.account.idToken.given_name,
              familyName: this.account.idToken.family_name,
              email: this.account.idToken.emails[0],
              tokenExpires: token.idToken.expiration,
              tier: membership,
            },
          });
        }
        try {
          if (!this.user.secretKey) {
            this.$router.push('/new-key');
          }

          await this.user.fetchTimestamps();
          let timer = 0;

          setInterval(async () => {
            const pending = this.user.pendingTimestamps;
            if (pending && pending.length > 0) {
              await this.$web3.updateTimestamps(this.user, pending);
            }

            if (process.env.DEV) {
              timer += 5;
              if (timer === 600) {
                User.update({
                  data: {
                    accountIdentifier: this.user.accountIdentifier,
                    subscriptionStart: moment().toISOString(),
                    subscriptionEnd: moment().add(1, 'months').toISOString(),
                  },
                });
                timer = 0;
              }
            } else if (moment().isAfter(moment(this.user.subscriptionEnd))) {
              User.update({
                data: {
                  accountIdentifier: this.user.accountIdentifier,
                  subscriptionStart: moment().toISOString(),
                  subscriptionEnd: moment().add(1, 'months').toISOString(),
                },
              });
            }
          }, 5000);
        } catch (e) {
          console.log(e);
        }
      }

      this.ready = true;
    },
  },
};
</script>
