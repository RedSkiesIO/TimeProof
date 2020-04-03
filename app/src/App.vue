<template>
  <div
    id="q-app"
  >
    <router-view :display="ready" />
  </div>
</template>

<script>
import axios from 'axios';
import moment from 'moment';
import { mapActions } from 'vuex';
import User from './store/User';
import Tier from './util/tier';

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
    const { data, status } = await axios.get(`${process.env.API}/priceplans`);
    const productsData = {};
    const colorList = ['orange', 'green', 'blue'];
    if (status === 200 && data) {
      data.forEach((plan, index) => {
        productsData[plan.title] = plan;
        productsData[plan.title].color = colorList[index];
      });
      this.setProducts(productsData);
    }
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
          const membership = this.account.idToken.extension_membershipTier || Tier.Free;
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
          const verifyResult = await this.user.verifyUserDetails();
          console.log('WOWOWOWOOWOWOWw');
          console.log(verifyResult);
          if (verifyResult && verifyResult.data) {
            User.update({
              data: {
                accountIdentifier: this.account.accountIdentifier,
                userId: verifyResult.data.userId,
                clientSecret: verifyResult.data.clientSecret,
                customerId: verifyResult.data.customerId,
              },
            });
          }

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
