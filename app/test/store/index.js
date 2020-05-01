import Vue from 'vue';
import Vuex from 'vuex';
import {
  SET_SELLING_PRODUCT,
} from '../../src/store/settings/mutationTypes';

const mutations = {
  [SET_SELLING_PRODUCT]: (state, product) => {
    state.sellingProduct = product;
  },
};

const actions = {
  [SET_SELLING_PRODUCT]: ({ commit }) => commit(SET_SELLING_PRODUCT, {
    id: Math.random().toString(15),
    title: 'PREMIUM',
    price: 5,
  }),
};

const getters = {
  getSellingProduct: state => state.sellingProduct,
};

const state = {
  sellingProduct: {
    id: Math.random().toString(15),
    title: 'Basic',
    price: 1,
  },
};

Vue.use(Vuex);

const store = new Vuex.Store({
  modules: {
    settings: {
      namespaced: true,
      mutations,
      actions,
      getters,
      state,
    },
  },
});


export default store;
