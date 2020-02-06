const { jwtDecode } = require('../utils');

module.exports = async function timestamp(context, req, timestamps) {
  context.log(req);
  context.log(timestamps);
  if(!req.headers.authorization) {
    return {
      status: 500,
      body: 'You are not authorized to access this resource',
    }
  }
  const user = (jwtDecode(req.headers.authorization)).sub;
  if (!req.params.id || user !== req.params.id) {
    return {
      status: 401,
      body: 'You are not authorized to access this resource',
    };
  }
  context.log(req);

  if (timestamps) {
    const amount = timestamps.length;

    return {
      status: 200,
      body: {
        noOfTimestamps: amount,
        timestamps,
      },
    };
  }
  return {
    status: 200,
    body: {
      noOfTimestamps: amount,
      timestamps,
    },
  };
};
