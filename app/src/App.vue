<template>
  <div
    id="q-app"
  >
    <router-view :display="ready" />
  </div>
</template>

<script>
import { mapActions } from 'vuex';
import User from './store/User';
import { disableLog } from './util/log';

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
  watch: {
    isAppIdle(val) {
      if (val) {
        this.$auth.logout();
      }
    },
  },

  async beforeCreate() {
    disableLog();
  },

  async created() {
    if (this.account) {
      localStorage.setItem('loggedOut', '');
    } else {
      localStorage.setItem('loggedOut', true);
    }
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
          const verifyResult = await
          this.$userServer.verifyUserDetails();

          console.log('START APP CHECK USER');
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
                paymentIntentId: verifyResult.paymentIntentId,
                membershipRenewDate: verifyResult.membershipRenewDate,
                remainingTimeStamps: verifyResult.remainingTimeStamps,
                pendingPricePlanId: verifyResult.pendingPricePlanId,
                pubKey: verifyResult.keyValue
                  ? JSON.parse(verifyResult.keyValue).publicKey : null,
                secretKey: verifyResult.keyValue
                  ? JSON.parse(verifyResult.keyValue).cipher : null,
              },
            });

            console.log('USERERRR');
            console.log(this.user);

            if (!this.user.secretKey) {
              this.$router.push('/new-key');
            }

            await this.$userServer.fetchTimestamps(this.account.accountIdentifier);

            setInterval(async () => {
              const pending = this.user.pendingTimestamps;
              if (pending && pending.length > 0) {
                // await this.$web3.updateTimestamps(this.user, pending);
                this.$timestampServer.updateTimestamps(pending);
              }
            }, 5000);
          }
        } catch (e) {
          console.log(e);
        }
      } else {
        this.$auth.signIn();
      }

      this.ready = true;
    },
  },

};
</script>
