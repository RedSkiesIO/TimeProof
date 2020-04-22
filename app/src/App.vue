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
          const verifyResult = await
          this.$userServer.verifyUserDetails();

          console.log('WOWOWOWOOWOWOWw');
          console.log(verifyResult);
          if (verifyResult) {
            User.insertOrUpdate({
              data: {
                accountIdentifier: this.account.accountIdentifier,
                givenName: verifyResult.firstName,
                familyName: verifyResult.lastName,
                email: verifyResult.email,
                tokenExpires: token.idToken.expiration,
                userId: verifyResult.id,
                tier: verifyResult.pricePlanId,
                clientSecret: verifyResult.clientSecret,
                customerId: verifyResult.paymentCustomerId,
                paymentIntentId: verifyResult.paymentIntentId,
                subscriptionStart: verifyResult.membershipStartDate,
              },
            });

            if (!this.user.secretKey) {
              this.$router.push('/new-key');
            }

            await this.$userServer.fetchTimestamps(this.account.accountIdentifier,
              this.user.userId);

            let timer = 0;

            setInterval(async () => {
              const pending = this.user.pendingTimestamps;
              if (pending && pending.length > 0) {
                // await this.$web3.updateTimestamps(this.user, pending);
                this.$timestampServer.updateTimestamps(pending);
              }

              if (process.env.DEV) {
                timer += 5;
                if (timer === 2000) {
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
          }
        } catch (e) {
          console.log(e);
        }
      }

      this.ready = true;
    },
  },
};
</script>
