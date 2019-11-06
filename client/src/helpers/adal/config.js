export const getAdalConfig = () => ({
  instance: 'https://login.microsoftonline.com/',
  tenant: 'e76987d2-a8b1-490c-89bb-08e77b63d40b',
  clientId: '413664b7-41ea-4096-ae23-3abcb7f8c359',
  postLogoutRedirectUri: 'http://localhost:6420',
  apiId: 'a66530d7-0a2c-4c81-af11-d7721befc5e8',
});

export const getApiConfig = () => ({
  url: 'http://easyauthtest3.azurewebsites.net',
  version: '/v1',
});
