/* eslint-disable prefer-destructuring */
/* eslint-disable no-undef */
import puppeteer from 'puppeteer';
import {
  click,
  changeText,
  selectOption,
  getFrame,
  changeTextInFrame,
  Type,
} from '../util';

require('events').EventEmitter.defaultMaxListeners = 15;

jest.setTimeout(1000000);

const userList = [
  {
    cardNumber: '4242424242424242', // Visa
    email: 'veyseltosun.vt+1@gmail.com',
    password: 'BiCo2288!',
  },
  {
    cardNumber: '4000056655665556', // Visa (debit)
    email: 'veyseltosun.vt+2@gmail.com',
    password: 'BiCo2288!',
  },
  {
    cardNumber: '5555555555554444', // Mastercard
    email: 'veyseltosun.vt+3@gmail.com',
    password: 'BiCo2288!',
  },
  {
    cardNumber: '2223003122003222', // Mastercard (2-series)
    email: 'veyseltosun.vt+4@gmail.com',
    password: 'BiCo2288!',
  },
  {
    cardNumber: '5200828282828210', // Mastercard (debit)
    email: 'veyseltosun.vt+5@gmail.com',
    password: 'BiCo2288!',
  },
  {
    cardNumber: '5105105105105100', // Mastercard (prepaid)
    email: 'veyseltosun.vt+6@gmail.com',
    password: 'BiCo2288!',
  },
  {
    cardNumber: '6011111111111117', // Discover
    email: 'veyseltosun.vt+7@gmail.com',
    password: 'BiCo2288!',
  },
  {
    cardNumber: '6011000990139424', // Discover
    email: 'veyseltosun.vt+8@gmail.com',
    password: 'BiCo2288!',
  },
  {
    cardNumber: '3056930009020004', // Diners Club
    email: 'veyseltosun.vt+9@gmail.com',
    password: 'BiCo2288!',
  },
  {
    cardNumber: '3566002020360505', // JCB
    email: 'veyseltosun.vt+10@gmail.com',
    password: 'BiCo2288!',
  },
  {
    cardNumber: '6200000000000005', // UnionPay
    email: 'veyseltosun.vt+11@gmail.com',
    password: 'BiCo2288!',
  },
];

describe.skip('Payment e2e Test', () => {
  let browser;
  beforeAll(async () => {
    browser = await puppeteer.launch({
      headless: false,
      slowMo: 5,
      args: [
        '--disable-web-security',
        '--window-size=1280,800',
        '--disable-features=IsolateOrigins,site-per-process',
      ],
    });
  });

  userList.forEach((user, index) => {
    if (index <= 4 && index >= 0) {
      test(`Payment e2e test email:${user.email}, carNumber:${user.cardNumber}`, async () => {
        const page = await browser.newPage();
        const app = 'http://localhost:6420';
        await page.goto(app, { waitUntil: 'load' });

        await changeText(page, 'input#logonIdentifier', user.email, false, 10000, 30);
        await changeText(page, 'input#password', user.password, false, 10000, 30);
        await page.waitFor(1000);
        await page.click('button#next');

        // TO DO when user hasn't a key make this control
        await page.waitForSelector('button[data-test-key="createYourSigningKey"]',
          { visible: true, timeout: 10000 });

        const createYourSigningKey = await page.$('button[data-test-key="createYourSigningKey"]');

        if (createYourSigningKey) {
          expect(page.url()).toEqual(expect.stringContaining('new-key'));
          // await createYourSigningKey.click();
          await page.evaluate(el => el.click(), createYourSigningKey);

          await changeText(page, 'encryptKeyPassword', 'BiCo2288!', true, 10000, 50);// enter new  secret key
          await click(page, 'newKeyContinue', 10000, false, Type.button);// Continue
          await click(page, 'saveKeyDownloadKeystore', 10000, false, Type.button); // download the keystore
          await click(page, 'newKeyContinue', 10000, false, Type.button); // Continue
          await click(page, 'newKeyContinue', 10000, false, Type.button); // Go To DashBoard
          await click(page, 'cancelCreateFirstTimeStamp', 10000, false, Type.button); // Cancel Create Timestamp Popup -- go dashboard
        }


        await click(page, 'upgradeButton', 10000, false, Type.button); // Upgrade the plan
        await click(page, 'choosePlanButton', 10000, true, Type.button); // Choose a plan

        // Billing details
        await selectOption(page, 'paymentBillingCountry', 'GB', true, 10000);// payment feilds: country
        await changeText(page, 'paymentBillingName', 'Veysel TOSUN', true, 10000, 30);// payment feilds: name
        await changeText(page, 'paymentBillingAddress', '75 Linden Avenue Wembley', true, 10000, 30);// payment feilds: address
        await changeText(page, 'paymentBillingCity', 'London', true, 10000, 30);// payment feilds: city
        await changeText(page, 'paymentBillingState', 'London', true, 10000, 30);// payment feilds: state
        await changeText(page, 'paymentBillingPostCode', 'HA9 8BB', true, 10000, 30);// payment feilds: post code

        // card details
        const frame = await getFrame(page, '__privateStripeFrame5', 10000);

        await changeTextInFrame(frame, 'input[name="cardnumber"]', user.cardNumber, 10000, 50);// payment feilds: cardnumber
        await changeTextInFrame(frame, 'input[name="exp-date"]', '1223', 10000, 50);// payment feilds: exp-date
        await changeTextInFrame(frame, 'input[name="cvc"]', '233', 10000, 50);// payment feilds: cvc
        await changeTextInFrame(frame, 'input[name="postal"]', '56789', 10000, 50);// payment feilds: postal

        await click(page, 'paymentButton', 10000, false, Type.button);// make payment

        const successPayment = await page.waitForSelector('.payment-page #confirmation .status.success',
          { visible: true, timeout: 10000 });

        await page.waitFor(500);

        expect(successPayment).not.toBeNull(); // payment is finished

        await page.waitFor(2000);

        await click(page, 'logout', 10000, false, Type.div); // logout
      });
    }
  });

  afterAll(() => {
    browser.close();
  });
});
