/* eslint-disable prefer-destructuring */
/* eslint-disable no-undef */
const Type = {
  div: 'div',
  button: 'button',
  a: 'a',
  span: 'span',
};

const click = async (page, name, timeout, multiple, type) => {
  const buttonSelector = `${type}[data-test-key=${name}]`;

  await page.waitForSelector(buttonSelector, { visible: true, timeout });
  let button;
  if (!multiple) {
    button = await page.$(buttonSelector);
  } else {
    const buttons = await page.$$(buttonSelector);
    if (Array.isArray(buttons) && buttons.length > 0) {
      button = buttons[1];
    }
  }

  if (name === 'paymentButton') {
    expect(button).not.toBeNull();
    expect(await button.evaluate(node => node.innerText)).toBe('Pay £4.99');
  }
  await page.evaluate(el => el.click(), button);
};

const changeText = async (page, name, content, internal, timeout, delay) => {
  const inputSelector = internal ? `input[data-test-key=${name}]` : name;

  const input = await page.waitForSelector(inputSelector, { visible: true, timeout });
  await input.focus();
  await input.type(content, { delay });
};

const selectOption = async (page, name, content, internal, timeout) => {
  const selectSelector = internal ? `select[data-test-key=${name}]` : name;

  const dropdown = await page.waitForSelector(selectSelector, { visible: true, timeout });
  await dropdown.focus();
  await dropdown.select(content);
};

const getFrame = async (page, frameName, timeout) => {
  const frameSelector = `iframe[name=${frameName}]`;

  const elementHandle = await page.waitForSelector(frameSelector, { visible: true, timeout });
  const frame = await elementHandle.contentFrame();

  return frame;
};

const changeTextInFrame = async (frame, contentSelector, content, timeout, delay) => {
  const input = await frame.waitForSelector(contentSelector, { visible: true, timeout });
  await input.type(content, { delay });
};


export {
  click,
  changeText,
  selectOption,
  getFrame,
  changeTextInFrame,
  Type,
};
