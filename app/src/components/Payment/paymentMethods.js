// List of relevant countries for the payment methods supported in this demo.
// Read the Stripe guide: https://stripe.com/payments/payment-methods-guide
export const paymentMethods = {
  ach_credit_transfer: {
    name: 'Bank Transfer',
    flow: 'receiver',
    countries: ['US'],
    currencies: ['usd'],
  },
  alipay: {
    name: 'Alipay',
    flow: 'redirect',
    countries: ['CN', 'HK', 'SG', 'JP'],
    currencies: [
      'aud',
      'cad',
      'eur',
      'gbp',
      'hkd',
      'jpy',
      'nzd',
      'sgd',
      'usd',
    ],
  },
  bancontact: {
    name: 'Bancontact',
    flow: 'redirect',
    countries: ['BE'],
    currencies: ['eur'],
  },
  card: {
    name: 'Card',
    flow: 'none',
  },
  eps: {
    name: 'EPS',
    flow: 'redirect',
    countries: ['AT'],
    currencies: ['eur'],
  },
  ideal: {
    name: 'iDEAL',
    flow: 'redirect',
    countries: ['NL'],
    currencies: ['eur'],
  },
  giropay: {
    name: 'Giropay',
    flow: 'redirect',
    countries: ['DE'],
    currencies: ['eur'],
  },
  multibanco: {
    name: 'Multibanco',
    flow: 'receiver',
    countries: ['PT'],
    currencies: ['eur'],
  },
  sepa_debit: {
    name: 'SEPA Direct Debit',
    flow: 'none',
    countries: [
      'FR',
      'DE',
      'ES',
      'BE',
      'NL',
      'LU',
      'IT',
      'PT',
      'AT',
      'IE',
      'FI',
    ],
    currencies: ['eur'],
  },
  sofort: {
    name: 'SOFORT',
    flow: 'redirect',
    countries: ['DE', 'AT'],
    currencies: ['eur'],
  },
  wechat: {
    name: 'WeChat',
    flow: 'none',
    countries: ['CN', 'HK', 'SG', 'JP'],
    currencies: [
      'aud',
      'cad',
      'eur',
      'gbp',
      'hkd',
      'jpy',
      'nzd',
      'sgd',
      'usd',
    ],
  },
};

export const uiPaymentTypeList = [
  {
    id: 'payment-card',
    value: 'card',
    label: 'Card',
    visible: true,
  },
  {
    id: 'payment-ach_credit_transfer',
    value: 'ach_credit_transfer',
    label: 'Bank Transfer',
    visible: false,
  },
  {
    id: 'payment-alipay',
    value: 'alipay',
    label: 'Alipay',
    visible: false,
  },
  {
    id: 'payment-bancontact',
    value: 'bancontact',
    label: 'Bancontact',
    visible: false,
  },
  {
    id: 'payment-eps',
    value: 'eps',
    label: 'EPS',
    visible: false,
  },
  {
    id: 'payment-ideal',
    value: 'ideal',
    label: 'iDEAL',
    visible: false,
  },
  {
    id: 'payment-giropay',
    value: 'giropay',
    label: 'Giropay',
    visible: false,
  },
  {
    id: 'payment-multibanco',
    value: 'multibanco',
    label: 'Multibanco',
    visible: false,
  },
  {
    id: 'payment-sepa_debit',
    value: 'sepa_debit',
    label: 'SEPA Direct Debit',
    visible: false,
  },
  {
    id: 'payment-sofort',
    value: 'sofort',
    label: 'SOFORT',
    visible: false,
  },
  {
    id: 'payment-wechat',
    value: 'wechat',
    label: 'WeChat Pay',
    visible: false,
  },
];
