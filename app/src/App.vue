<template>
  <div
    id="q-app"
  >
    <router-view :display="ready" />
  </div>
</template>

<script>
import { mapActions } from 'vuex';
import moment from 'moment';
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
    this.setProducts(await this.$paymentServer.listAllPriceplans());
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

        try {
          this.$axios.defaults.headers.common.Authorization = `Bearer ${token.idToken.rawIdToken}`;
          const verifyResult = await this.$userServer.verifyUserDetails();
          console.log('WOWOWOWOOWOWOWw');
          console.log(verifyResult);
          if (verifyResult) {
            User.update({
              data: {
                accountIdentifier: this.account.accountIdentifier,
                givenName: this.account.idToken.given_name,
                familyName: this.account.idToken.family_name,
                email: this.account.idToken.emails[0],
                tokenExpires: token.idToken.expiration,
                userId: verifyResult.id,
                tier: verifyResult.pricePlanId,
                clientSecret: verifyResult.clientSecret,
                customerId: verifyResult.customerId,
                paymentIntentId: verifyResult.paymentIntentId,
                subscriptionStart: verifyResult.membershipStartDate,
              },
            });
          }

          if (!this.user.secretKey) {
            this.$router.push('/new-key');
          }

          await this.$userServer.fetchTimestamps();
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
