import config from './config';

export default window.Stripe(config.stripe.publishableKey, {
  apiVersion: config.apiVersion,
});
