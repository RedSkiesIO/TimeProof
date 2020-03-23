import { SET_AUTHENTICATED_ACCOUNT, SET_SELLING_PRODUCT } from './mutationTypes';

export default {
  [SET_AUTHENTICATED_ACCOUNT]: (state, account) => {
    state.authenticatedAccount = account;
  },
  [SET_SELLING_PRODUCT]: (state, product) => {
    state.sellingProduct = product;
  },
};
