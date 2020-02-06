const { auth, jwtDecode } = require('../utils');

module.exports = async function upgradeUser(context, req) {
    context.log('Node.js HTTP trigger function processed a request. RequestUri=%s', req.originalUrl);
    context.log(req.body.data.object.display_items);
    const accessToken = req.body.data.object.client_reference_id;
    const user = (jwtDecode(accessToken)).sub;
    const plan = req.body.data.object.display_items[0].plan.nickname;
    await auth.updateMembership(user, plan);
   
    context.done();
};