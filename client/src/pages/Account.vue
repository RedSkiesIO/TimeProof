<template>
  <div>
    <AccountDetails />
    <AccountKey />
  </div>
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
