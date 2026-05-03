const target =
  process.env['services__ballastlane-taskmanager-api__https__0'] ||
  process.env['services__ballastlane-taskmanager-api__http__0'] ||
  'https://localhost:5001';

console.log(`[proxy] API target: ${target}`);

module.exports = {
  '/auth': { target, secure: false, changeOrigin: true },
  '/tasks': { target, secure: false, changeOrigin: true }
};
