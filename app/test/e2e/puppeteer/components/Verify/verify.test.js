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

describe('Verify e2e Test', () => {
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
    const context = browser.defaultBrowserContext();
    context.clearPermissionOverrides();
    context.overridePermissions('http://localhost:6420', ['clipboard-read', 'clipboard-write']);
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
      let createYourSigningKey;
      try {
        await page.waitForSelector('button[data-test-key="createYourSigningKey"]',
          { visible: true, timeout: 10000 });
        createYourSigningKey = await page.$('button[data-test-key="createYourSigningKey"]');
      } catch (err) {
        console.log('createYourSigningKey is not loaded: ', err);
      }

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

      await click(page, 'timeStampsTxId', 10000, true, Type.button); //
      await page.waitFor(2000);
      const txId = await page.evaluate(() => navigator.clipboard.readText());
      await click(page, 'Verify', 10000, false, Type.a); // Open Stamp page

      const [fileChooser] = await Promise.all([
        page.waitForFileChooser(),
        click(page, 'stampBrowseFile', 10000, false, Type.span), // Browse file
      ]);
      await fileChooser.accept(['test/e2e/puppeteer/components/tmp/imagetest.png']);

      changeText(page, 'verifyProofId', txId, true, 10000, 30);
      click(page, 'verify', 10000, false, Type.button);

      await page.waitFor(500);

      const stampVerifyPage = await page.waitForSelector("[data-test-key='stampVerify']",
        { visible: true, timeout: 30000 });

      const stampVerifyTitle = await page.waitForSelector("[data-test-key='stampVerifyTitle']",
        { visible: true, timeout: 10000 });

      expect(stampVerifyPage).not.toBeNull(); // verify is finished

      expect(await stampVerifyTitle.evaluate(node => node.innerText)).toBe("Your timestamp is on it's way");

      await page.waitFor(2000);

      // await clickButton(page, 'logout', 10000, false); // logout
    });
  });

  afterAll(() => {
    // browser.close();
  });
});
