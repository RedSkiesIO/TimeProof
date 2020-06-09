try {
  ((original) => {
    console.enableLogging = () => {
      console.log = original;
    };
    console.disableLogging = () => {
      console.log = () => {};
    };
  })(console.log);
} catch (err) {
  console.log('log initialize error:', err);
}

const disableLog = () => {
  if (process.env.PROD) {
    console.disableLogging();
  }
};

const enableLog = () => {
  if (!process.env.PROD) {
    console.enableLogging();
  }
};

export { disableLog, enableLog };
