<template>
  <q-page
    class="text-secondary"
  >
    <div class="q-pa-md">
      <div class="row justify-center text-h6 text-weight-bold">
        Account details
      </div>
      <div class="row q-mt-sm q-gutter-x-md">
        <AccountDetails />
        <AccountKey />
      </div>
      <div class="row q-mt-sm q-gutter-x-md">
        <AccountSubscription />
      </div>
    </div>
  </q-page>
</template>
<script>
import AccountDetails from '../components/Account';
import AccountKey from '../components/Key';
import AccountSubscription from '../components/Subscription';

export default {
  name: 'Account',
  components: {
    AccountDetails,
    AccountKey,
    AccountSubscription,
  },
};
</script>
