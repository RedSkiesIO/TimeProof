import config from './config';

export default (() => {
  if (window && window.Stripe) {
    return window.Stripe(config.stripe.publishableKey, {
      apiVersion: config.apiVersion,
    });
  }
  return null;
})();
