
const routes = [
  {
    path: '/',
    component: () => import('layouts/Main'),
    children: [
      { path: '', component: () => import('pages/Dashboard.vue') },
    ],
  },
  {
    path: '/verify',
    component: () => import('layouts/Main'),
    children: [
      { path: '', component: () => import('pages/Verify.vue') },
    ],
  },
  {
    path: '/stamp',
    component: () => import('layouts/Main'),
    children: [
      { path: '', component: () => import('pages/Stamp.vue') },
    ],
  },
  {
    path: '/dashboard',
    component: () => import('layouts/Main'),
    children: [
      { path: '', component: () => import('pages/Dashboard.vue') },
    ],
  },
  {
    path: '/account',
    component: () => import('layouts/Main'),
    children: [
      { path: '', component: () => import('pages/Account.vue') },
    ],
  },
  {
    path: '/new-key',
    component: () => import('layouts/Main'),
    children: [
      { path: '', component: () => import('pages/CreateKey') },
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
