import {
  SET_AUTHENTICATED_ACCOUNT,
  SET_SELLING_PRODUCT,
  SET_PRODUCTS,
} from './mutationTypes';

export default {
  [SET_AUTHENTICATED_ACCOUNT]: (state, account) => {
    state.authenticatedAccount = account;
  },
  [SET_SELLING_PRODUCT]: (state, product) => {
    state.sellingProduct = product;
  },
  [SET_PRODUCTS]: (state, products) => {
    state.products = products;
  },
};
