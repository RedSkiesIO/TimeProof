import {
  SET_AUTHENTICATED_ACCOUNT,
  SET_SELLING_PRODUCT,
  SET_PRODUCTS,
} from './mutationTypes';
// import Router from '../../router';

// const router = Router();

export default {
  setAuthenticatedAccount: (context, account) => {
    context.commit(SET_AUTHENTICATED_ACCOUNT, account);
  },
  setSellingProduct: ({ commit }, product) => {
    commit(SET_SELLING_PRODUCT, product);
  },
  setProducts: ({ commit }, products) => {
    commit(SET_PRODUCTS, products);
  },
};
