const B2C_SCOPES = process.env.DEV
  ? 'https://timeproof.onmicrosoft.com/api/read'
  : 'https://timescribe.onmicrosoft.com/api/read';

// Client id for active directory
const CLIENT_ID = process.env.DEV
  ? 'caead9d0-3263-42b9-b25e-2ca36d0ff535'
  : '9361fd25-6b7f-438d-8e16-7a24a43cd992';

// This is your tenant info
const AUTHORITY_SIGNUP_SIGNIN = process.env.DEV
  ? 'https://timeproof.b2clogin.com/timeproof.onmicrosoft.com/B2C_1_SignUpSignIn'
  : 'https://timescribe.b2clogin.com/timescribe.onmicrosoft.com/B2C_1_SignUpSignIn';

const AUTHORITY_EDIT_PROFILE = process.env.DEV
  ? 'https://timeproof.b2clogin.com/timeproof.onmicrosoft.com/B2C_1_EditProfile'
  : 'https://timescribe.b2clogin.com/timescribe.onmicrosoft.com/B2C_1_EditProfile';

const AUTHORITY_FORGOT_PASSWORD = process.env.DEV
  ? 'https://timeproof.b2clogin.com/timeproof.onmicrosoft.com/B2C_1_ForgotPassword'
  : 'https://timescribe.b2clogin.com/timescribe.onmicrosoft.com/B2C_1_ForgotPassword';

const REDIRECT_URI = process.env.DEV
  ? 'http://localhost:6420/'
  : 'https://timeproof.netlify.app/';

const B2C_1_SIGNUP_SIGNIN = 'B2C_1_SignUpSignIn';
const FORGOT_PASSWORD_ERROR_CODE = 'AADB2C90118'; // forgot password clicked
const CANCEL_BUTTON_ERROR_CODE = 'AADB2C90091'; // Cancel buttton clicked

export default {
  B2C_SCOPES,
  CLIENT_ID,
  AUTHORITY_SIGNUP_SIGNIN,
  AUTHORITY_EDIT_PROFILE,
  AUTHORITY_FORGOT_PASSWORD,
  REDIRECT_URI,
  B2C_1_SIGNUP_SIGNIN,
  FORGOT_PASSWORD_ERROR_CODE,
  CANCEL_BUTTON_ERROR_CODE,
};
