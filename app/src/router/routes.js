const routes = [
  {
    path: '/',
    component: () => import('layouts/Main'),
    children: [
      { path: '', component: () => import('pages/Dashboard.vue') },
      { path: 'verify', component: () => import('pages/Verify.vue') },
      { path: 'stamp', component: () => import('pages/Stamp.vue') },
      { path: 'dashboard', component: () => import('pages/Dashboard.vue') },
      { path: 'account', component: () => import('pages/Account.vue') },
      { path: 'new-key', component: () => import('pages/CreateKey') },
      { path: 'upgrade', component: () => import('pages/Upgrade') },
      { path: 'upgrade-success', component: () => import('pages/UpgradeSuccess') },
      { path: 'payment', component: () => import('components/Payment') },
    ],
  },
];

// Always leave this as last one
if (process.env.MODE !== 'ssr') {
  routes.push({
    path: '*',
    component: () => import('pages/Error404.vue'),
  });
}

export default routes;
