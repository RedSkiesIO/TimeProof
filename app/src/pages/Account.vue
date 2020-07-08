<template>
  <q-page
    class="text-secondary"
  >
    <div class="q-pa-sm q-mr-md">
      <div class="row justify-center text-h6 text-weight-bold">
        Account details
      </div>
      <div class="row justify-center q-mt-sm q-gutter-x-md">
        <AccountDetails
          class="col-xs-12 q-mt-sm col-md-7 col-lg-7"
        />
      </div>
      <div class="row justify-center q-mt-sm q-gutter-x-md">
        <AccountSubscription
          class="col-xs-12 col-md-7 col-lg-7"
        />
      </div>
    </div>
  </q-page>
</template>
<script>
import AccountDetails from '../components/Account';
// import AccountKey from '../components/Key';
import AccountSubscription from '../components/Subscription';

export default {
  name: 'Account',
  components: {
    AccountDetails,
    // AccountKey,
    AccountSubscription,
  },
};
</script>
