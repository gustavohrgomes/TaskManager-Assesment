const { env } = require('process');

const target =
  env['services__ballastlane-taskmanager-api__https__0'] ||
  env['services__ballastlane-taskmanager-api__http__0'] ||
  'https://localhost:5001';

console.log(`[proxy] API target: ${target}`);

const PROXY_CONFIG = [
  {
    context: ['/auth', '/tasks', '/scalar'],
    target,
    secure: env['NODE_ENV'] !== 'development',
    changeOrigin: true,
  },
];

module.exports = PROXY_CONFIG;
