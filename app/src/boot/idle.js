import IdleVue from 'idle-vue';

export default ({ Vue, store }) => {
  Vue.use(IdleVue, {
    store, idleTime: 20 * 60000,
  });
};
