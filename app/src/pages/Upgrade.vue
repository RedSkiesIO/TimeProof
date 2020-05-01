<template>
  <q-page
    class="q-mt-md justify-start price-plans"
  >
    <div
      class="row"
      style="width: 100%; height: '300px'"
    >
      <div
        v-for="item in products"
        :key="item.id"
        class="col-xs-12 col-md-4 price-plan-div q-mb-md"
      >
        <q-card
          class="q-mt-md"
          bordered
        >
          <q-card-section horizontal>
            <div class="row col-12 justify-around">
              <q-card-section class="col-6">
                <div
                  class="text-overline text-center"
                  :style="{backgroundColor: item.color}"
                >
                  <div class="text-weight-bold text-white plan-title">
                    {{ item.title }}
                  </div>
                </div>
              </q-card-section>

              <q-card-section class="col-6 text-right">
                <div class="price text-weight-bold">
                  {{ `£${(item.price/100).toFixed(2)}` }}
                </div>
                <div class="price-subtitle">
                  Per Month
                </div>
              </q-card-section>
            </div>
          </q-card-section>
          <q-card-section>
            <div class="text-h5 q-mt-sm q-mb-md">
              Product Detail
            </div>
            <div class="text-h8 text-grey q-mb-md">
              {{ item.description }}
            </div>
            <div class="text-caption text-grey q-mb-md text-italic">
              {{ item.confirmationDescription }}
            </div>
            <div>
              <strong class="plan-title">{{ item.noOfStamps }}</strong>
              <span class="text-caption text-grey q-ml-sm">timestamps rights</span>
            </div>
          </q-card-section>
          <q-separator />

          <q-card-actions class="row justify-between">
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
          </q-card-actions>
        </q-card>
      </div>
    </div>
  </q-page>
</template>
<script>
import { mapGetters, mapActions } from 'vuex';

export default {

  data() {
    return {
      token: null,
      loading: false,
    };
  },
  computed: {
    ...mapGetters({
      products: 'settings/getProducts',
    }),
    currentMemberShip() {
      return this.$auth.account().idToken.extension_membershipTier || this.$auth.user().tier;
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
      this.$router.push('/payment');
    },
  },
};
</script>
<style lang="scss" scoped>
.price-plans {
  .q-card{
    padding: 0;
    margin: 0 16px;
    border-radius: 20px;
  }
  .q-card:hover{
    background: lightcyan;
    box-shadow: 10px 10px 8px 8px gray;
  }
  .plan-title{
    font-size: 20px;
  }
  .price-title {
    font-size: 18px;
  }
  .price {
    font-size: 24px;
  }
  .price-timestamps {
    padding: 16px 20px;
    margin: 0 25px;
    border-bottom: 1px solid lightgrey;
    color: $secondary;
    font-weight: bold;
  }
  .price-button-container {
    padding-top: 32px;
    padding-bottom: 24px;
  }
  .top-section {
    padding: 40px 20px;
  }

}
</style>
