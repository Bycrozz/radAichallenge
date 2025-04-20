import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './tests',
  use: {
    baseURL: 'http://localhost:5000/',
  },
  projects: [
    {
      name: 'API',
      testMatch: '**/api/**/*.spec.ts', // 🔥 More precise glob match
      use: {
        baseURL: 'http://localhost:5000/api',
      },
    },
    {
      name: 'chromium',
      testMatch: '**/e2e/**/*.spec.ts', // 🔥 More precise glob match
      use: { ...devices['Desktop Chrome'] },
    },
    {
      name: 'firefox',
      testMatch: '**/e2e/**/*.spec.ts',
      use: { ...devices['Desktop Firefox'] },
    },
    {
      name: 'webkit',
      testMatch: '**/e2e/**/*.spec.ts',
      use: { ...devices['Desktop Safari'] },
    },
  ],
});
