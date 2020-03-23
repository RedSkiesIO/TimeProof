import { SET_AUTHENTICATED_ACCOUNT, SET_SELLING_PRODUCT } from './mutationTypes';
// import Router from '../../router';

// const router = Router();

export default {
  setAuthenticatedAccount: (context, account) => {
    context.commit(SET_AUTHENTICATED_ACCOUNT, account);
  },
  setSellingProduct: ({ commit }, product) => {
    commit(SET_SELLING_PRODUCT, product);
    // this.$router.push({ name: 'payment', params: { price: 25.6 } });
    // this.$router.push({ path: `/payment/${price}` });
    // router.push('/payment');
  },
};
