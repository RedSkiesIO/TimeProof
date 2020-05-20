/* eslint-disable prefer-destructuring */
/* eslint-disable no-undef */
import puppeteer from 'puppeteer';
import { changeText, click, Type } from '../util';

require('events').EventEmitter.defaultMaxListeners = 15;

jest.setTimeout(1000000);

const userList = [
  {
    email: 'veyseltosun.vt+2@gmail.com',
    password: 'BiCo2288!',
  },
];

describe('Stamp e2e Test', () => {
  let browser;
  beforeAll(async () => {
    browser = await puppeteer.launch({
      headless: false,
      slowMo: 5,
      args: [
        '--disable-web-security',
        '--window-size=1600,800',
        '--disable-features=IsolateOrigins,site-per-process',
      ],
    });
  });

  userList.forEach((user) => {
    test(`Stamp e2e test email:${user.email}`, async () => {
      const page = await browser.newPage();
      const app = 'http://localhost:6420';
      await page.goto(app, { waitUntil: 'load' });

      await changeText(page, 'input#logonIdentifier', user.email, false, 20000, 30);
      await changeText(page, 'input#password', user.password, false, 20000, 30);
      await page.waitFor(1000);
      await page.click('button#next');

      // TO DO when user hasn't a key make this control
      try {
        await page.waitForSelector('button[data-test-key="createYourSigningKey"]',
          { visible: true, timeout: 10000 });
      } catch (err) {
        console.log('createYourSigningKey is not loaded: ', err);
      }

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

      await click(page, 'Stamp', 10000, false, Type.a); // Open Stamp page

      const [fileChooser] = await Promise.all([
        page.waitForFileChooser(),
        click(page, 'stampBrowseFile', 10000, false, Type.span), // Browse file
      ]);
      await fileChooser.accept(['/tmp/loadin.js']);

      // const successPayment =
      // await page.waitForSelector('.payment-page #confirmation .status.success',
      //   { visible: true, timeout: 10000 });

      await page.waitFor(500);

      // expect(successPayment).not.toBeNull(); // payment is finished

      await page.waitFor(2000);

      // await clickButton(page, 'logout', 10000, false); // logout
    });
  });

  afterAll(() => {
    // browser.close();
  });
});
