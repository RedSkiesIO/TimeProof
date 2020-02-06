const { auth } = require('../utils');

module.exports = async function upgradeUser(context, req) {
    context.log('Node.js HTTP trigger function processed a request. RequestUri=%s', req.originalUrl);
    context.log(req.body);
    context.log(req.body.data.object.display_items);
    const user = req.body.data.object.client_reference_id;
    const plan = req.body.data.object.display_items[0].plan.nickname;
    await auth.updateMembership(user, plan);
    // if (req.query.name || (req.body && req.body.name)) {
    //     context.log = {
    //         // status defaults to 200 */
    //         body: "Hello " + (req.query.name || req.body.name)
    //     };
    // }
    // else {
    //     context.log = {
    //         status: 400,
    //         body: "Please pass a name on the query string or in the request body"
    //     };
    // }
    context.done();
};