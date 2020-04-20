const { jwtDecode } = require('../utils');

module.exports = async function timestamp(context, req, timestamps) {
  context.log('req: ', req);
  if(!req.headers.authorization) {
    return {
      status: 500,
      body: 'You are not authorized to access this resource',
    }
  }
  const user = jwtDecode(req.headers.authorization).sub;
  if (!req.params.id || user !== req.params.id) {
    return {
      status: 401,
      body: 'You are not authorized to access this resource',
    };
  }

  const updates = req.body;

  if(updates && updates.length > 0 ) {
    updates.forEach((update) => {
      context.log('called');
      const timestamp = context.bindings.timestamps.find(stamp => {
        return stamp.id === update.txId;
      });
      context.log(timestamp);
      if(timestamp) {
        context.bindings.timestampDatabase = timestamp;
        context.log(context.bindings.timestampDatabase);
        context.bindings.timestampDatabase.blockNumber = Number(update.blockNumber);
        context.bindings.timestampDatabase.timestamp = Number(update.date);
      }
    });
  }
 
  return {
    status: 200,
    body:{
      timestamps,
    }
  };
};
