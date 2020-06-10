/* eslint-disable func-names */
console.log('TEST');
console.log(process.env.test);
console.log('PROD');
console.log(process.env.prod);

module.exports = function (isDev, isTest, isProd) {
  console.log('app.config');
  console.log(isDev, isTest, isProd);
  let envVar = {};

  if (isDev) {
    envVar = {
      API: JSON.stringify('http://localhost:5000/api'),
      ETHERSCAN: JSON.stringify('https://kovan.etherscan.io/tx'),
      INFURA: JSON.stringify('https://kovan.infura.io/v3/679bbc6759454bf58a924bfaf55576b9'),
      DEV: JSON.stringify(true),
      B2C_SCOPES: JSON.stringify('https://timeproof.onmicrosoft.com/api/read'),
      CLIENT_ID: JSON.stringify('caead9d0-3263-42b9-b25e-2ca36d0ff535'),
      AUTHORITY_SIGNUP_SIGNIN: JSON.stringify('https://timeproof.b2clogin.com/timeproof.onmicrosoft.com/B2C_1_SignUpSignIn'),
      AUTHORITY_EDIT_PROFILE: JSON.stringify('https://timeproof.b2clogin.com/timeproof.onmicrosoft.com/B2C_1_EditProfile'),
      AUTHORITY_FORGOT_PASSWORD: JSON.stringify('https://timeproof.b2clogin.com/timeproof.onmicrosoft.com/B2C_1_ForgotPassword'),
      REDIRECT_URI: JSON.stringify('http://localhost:6420/'),
      STRIPE_PUBLISH_KEY: JSON.stringify('pk_test_YD41bvTMwCibc2T9yW2UYtPS00Jzsaku4h'),
    };
  } else if (isTest) {
    envVar = {
      API: JSON.stringify('https://timescribeapitest.azurewebsites.net/api'),
      ETHERSCAN: JSON.stringify('https://kovan.etherscan.io/tx'),
      INFURA: JSON.stringify('https://kovan.infura.io/v3/679bbc6759454bf58a924bfaf55576b9'),
      TEST: JSON.stringify(true),
      B2C_SCOPES: JSON.stringify('https://timeproof.onmicrosoft.com/api/read'),
      CLIENT_ID: JSON.stringify('caead9d0-3263-42b9-b25e-2ca36d0ff535'),
      AUTHORITY_SIGNUP_SIGNIN: JSON.stringify('https://timeproof.b2clogin.com/timeproof.onmicrosoft.com/B2C_1_SignUpSignIn'),
      AUTHORITY_EDIT_PROFILE: JSON.stringify('https://timeproof.b2clogin.com/timeproof.onmicrosoft.com/B2C_1_EditProfile'),
      AUTHORITY_FORGOT_PASSWORD: JSON.stringify('https://timeproof.b2clogin.com/timeproof.onmicrosoft.com/B2C_1_ForgotPassword'),
      REDIRECT_URI: JSON.stringify('https://testapp.timescribe.io/'),
      STRIPE_PUBLISH_KEY: JSON.stringify('pk_test_YD41bvTMwCibc2T9yW2UYtPS00Jzsaku4h'),
    };
  } else if (isProd) {
    envVar = {
      API: JSON.stringify('https://timescribeapiprod.azurewebsites.net/api'),
      ETHERSCAN: JSON.stringify('https://etherscan.io/tx'),
      INFURA: JSON.stringify('https://mainnet.infura.io/v3/679bbc6759454bf58a924bfaf55576b9'),
      PROD: JSON.stringify(true),
      B2C_SCOPES: JSON.stringify('https://timescribe.onmicrosoft.com/api/read'),
      CLIENT_ID: JSON.stringify('9361fd25-6b7f-438d-8e16-7a24a43cd992'),
      AUTHORITY_SIGNUP_SIGNIN: JSON.stringify('https://timescribe.b2clogin.com/timescribe.onmicrosoft.com/B2C_1_SignUpSignIn'),
      AUTHORITY_EDIT_PROFILE: JSON.stringify('https://timescribe.b2clogin.com/timescribe.onmicrosoft.com/B2C_1_EditProfile'),
      AUTHORITY_FORGOT_PASSWORD: JSON.stringify('https://timescribe.b2clogin.com/timescribe.onmicrosoft.com/B2C_1_ForgotPassword'),
      REDIRECT_URI: JSON.stringify('https://app.timescribe.io/'),
      STRIPE_PUBLISH_KEY: JSON.stringify('pk_live_UOWWkQwRgPJRQGCXfxiAiYf700KdAGDGBn'),
    };
  }

  envVar.B2C_1_SIGNUP_SIGNIN = JSON.stringify('B2C_1_SignUpSignIn');
  envVar.FORGOT_PASSWORD_ERROR_CODE = JSON.stringify('AADB2C90118');
  envVar.CANCEL_BUTTON_ERROR_CODE = JSON.stringify('AADB2C90091');
  envVar.STRIPE_ACCOUNT_COUNTRY = JSON.stringify('GB');

  return envVar;
};
