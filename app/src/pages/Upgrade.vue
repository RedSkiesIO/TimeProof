<template>
  <q-page
    class="q-pt-md price-plans"
  >
    <div
      class="row"
    >
      <div
        v-for="item in products"
        :key="item.id"
        class="col-xs-12 col-md-4 q-mb-md"
      >
        <q-card
          class="q-mt-md"
          bordered
        >
          <q-card-section
            vertical
            class="price-card-section"
          >
            <q-card-section class="text-center">
              <div class="plan-title text-white text-weight-bold">
                {{ item.title }}
              </div>
            </q-card-section>

            <q-card-section class="text-center">
              <div class="price text-white text-weight-bold">
                {{ `£${(item.price/100).toFixed(2)}` }}
              </div>
              <div class="price-subtitle text-white">
                {{ item.freqDesc }}
              </div>
            </q-card-section>

            <div class="pricing-bg-circle-right" />
            <div class="pricing-bg-circle-left" />
          </q-card-section>
          <q-card-section>
            <div class="text-h3 q-mt-sm q-mb-md text-center">
              <strong class="content-title">{{ item.noOfStamps }}</strong>
              <span class="text-h6 text-grey q-ml-sm">timestamps</span>
            </div>
            <div class="text-h8 text-grey q-mb-md text-center">
              {{ item.description }}
            </div>
          </q-card-section>
          <q-separator />

          <q-card-actions class="row justify-between">
            <template v-if="user.pendingPricePlanId === item.id">
              <q-btn
                flat
                data-test-key="cancelFuturePlan"
                color="secondary text-weight-bold"
                @click="cancelPlan"
              >
                Cancel
              </q-btn>
              <span class="info text-italic text-grey">
                (This plan will be activated on
                {{ moment(user.membershipRenewDate).format('DD-MM-YYYY') }})
              </span>
              <q-badge
                outline
                class="q-ml-sm"
                color="blue"
                label="Future"
              />
            </template>
            <template v-else>
              <q-btn
                :disable="currentMemberShip === item.id"
                flat
                data-test-key="choosePlanButton"
                color="primary text-weight-bold"
                @click="choosePlan(item)"
              >
                Choose Plan
              </q-btn>
              <q-badge
                v-if="currentMemberShip === item.id"
                outline
                class="q-ml-md"
                color="orange"
                label="Active"
              />
            </template>
          </q-card-actions>
        </q-card>
      </div>
    </div>
    <q-dialog
      v-model="downgradeConfirmationDialog"
    >
      <DowngradeConfirmation
        @closeDialog="downgradeConfirmationDialog = false"
        @confirmDialog="confirmDowngrade"
      />
    </q-dialog>
  </q-page>
</template>
<script>
import { mapGetters, mapActions } from 'vuex';
import moment from 'moment';
import DowngradeConfirmation from '../components/DowngradeConfirmation';

export default {
  components: {
    DowngradeConfirmation,
  },

  data() {
    return {
      token: null,
      loading: false,
      downgradeConfirmationDialog: false,
      moment,
    };
  },
  computed: {
    ...mapGetters({
      products: 'settings/getProducts',
    }),
    currentMemberShip() {
      return this.$auth.account().idToken.extension_membershipTier || this.$auth.user().tier;
    },
    user() {
      return this.$auth.user();
    },
  },
  mounted() {
    this.getToken();
  },
  methods: {
    ...mapActions('settings', [
      'setSellingProduct',
    ]),
    async getToken() {
      const accessToken = await this.$auth.getToken();
      this.token = accessToken.idToken.rawIdToken;
    },
    choosePlan(item) {
      this.setSellingProduct(item);
      if (item.price === 0) {
        this.downgradeConfirmationDialog = true;
      } else {
        this.$router.push('/payment');
      }
    },
    confirmDowngrade() {
      this.$router.push('/payment');
    },
    cancelPlan() {
      this.$userServer.cancelFuturePlan();
    },
  },
};
</script>
<style lang="scss" scoped>
.price-plans {
  .q-card{
    padding: 0;
    margin: 0 16px;
    border-radius: 1.8rem;
  }
  .q-card:hover{
    background: lightcyan;
    box-shadow: 10px 10px 8px 8px gray;
  }
  .plan-title{
    font-size: 1.5rem;
  }
  .content-title {
    font-size: 2rem;
  }
  .price-subtitle{
    font-size: 0.8rem;
  }
  .price {
    font-size: 2.4rem;
  }
  .pricing-bg-circle-right{
    position: absolute;
    top: -50%;
    right: -10%;
    width: 50%;
    padding-bottom: 50%;
    border-radius: 100%;
    background-color: hsla(0, 0%, 100%, 0.1);
  }
  .pricing-bg-circle-left {
    position: absolute;
    left: -46%;
    top: 40%;
    width: 100%;
    padding-bottom: 100%;
    border-radius: 100%;
    background-color: hsla(0, 0%, 100%, 0.1);
  }
  .price-card-section{
    background-color: #4cbbc2;
    overflow: hidden;
  }
  .info{
    font-size: 0.8rem;
  }
}
</style>
