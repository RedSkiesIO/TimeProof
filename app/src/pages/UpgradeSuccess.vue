<template>
  <div />
</template>
<script>
// import User from '../store/User';

export default {
//   computed: {
//     membership() {
//       const account = this.$auth.account();
//       return account.idToken.extension_membershipTier || 'free';
//     },

  //     user() {
  //       const account = this.$auth.account();
  //       if (account) {
  //         const user = User.query().whereId(account.accountIdentifier).with('timestamps').get();
  //         if (user) {
  //           return user[0];
  //         }
  //       }
  //       return null;
  //     },
  //   },

  mounted() {
    setTimeout(() => {
      this.$auth.logout();
    }, 3000);
  },
};
</script>
