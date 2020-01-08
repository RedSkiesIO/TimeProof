<template>
  <q-page class="flex justify-center text-primary">
<div class="q-pa-md">
      <div class="row justify-center text-h6 text-weight-bold">
        Account details
      </div>
      <div class="row q-mt-sm q-gutter-x-md">
        <AccountDetails />
        <AccountKey />
      </div>
    </div>
  </q-page>
</template>
<script>
import AccountDetails from '../components/Account';
import AccountKey from '../components/Key';

export default {
  name: 'Account',
  components: {
    AccountDetails,
    AccountKey,
  },
};
</script>
