### 🧪 Unit Tests - Setup and Execution

This project is set up to:

- Run unit tests
- Generate **HTML test** and **coverage reports**
- Open both reports automatically.

##### 🔧 Requirements
- Clone this repository and run the application in the folder radAichallenge [using the original repo instructions](https://github.com/radaisystems/food-trucks-challenge)
- Install [.NET SDK 8.x](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (If not installed)
- install ReportGenerator (install once via terminal):

```bash
dotnet tool install --global dotnet-reportgenerator-globaltool
```

##### 🚀 How to Run
In the cloned repository folder:

```bash
cd radAichallenge
dotnet restore
dotnet test
```

##### 📂 Report Output

- HTML test report: `Reports/TestResults/TestResults.html`
- Coverage dashboard: `Reports/Coverage/Html/index.html`

### 🧪 Playwright E2E Tests -  Setup & Execution

> Minimal guide to install dependencies and run existing Playwright UI tests with tags, headed mode, browser selection, and HTML reports.

---

##### ✅ 1. Install Prerequisites

Install Node.js and npm (https://nodejs.org)
Make sure the application is running on http://localhost:5000/foodTruckSearch

---

##### 📦 2. Install Dependencies

Run this in RadAiTests\PlaywrightTests:

```bash
npm install --save-dev @playwright/test
npx playwright install
```

---

##### 🏃 3. Run All E2E Tests

```bash
npm run test:e2e
```

---

##### 🏷️ 4. Run Tests by Tags (No HTML report)

```bash
npm run test:e2e:tag --grep "@ui"
With tags (AND)
npm run test:e2e:tag --grep "@ui" --grep "@search"
With tags (OR)
npm run test:e2e:tag --grep "@ui|@search"
```

---

##### 🧭 5. Run Tests in Headed Mode (Show browser UI)

```bash
npx playwright test tests/e2e --headed

you can combine with tags adding --grep "@tag" (should be in the end)
and select the browser adding --project=chromium
```

---

##### 🌐 6. Run Tests in a Specific Browser

Available browsers: `chromium`, `firefox`, `webkit`

```bash
npx playwright test tests/e2e --project=chromium
npx playwright test tests/e2e --project=firefox
npx playwright test tests/e2e --project=webkit
```

---

##### 📊 7. Generate and Open HTML Report

Run all tests and open the HTML report:

```bash
npm run test:e2e
```
<p> You can automatically generate and open the report for the other 
commands by adding the following command at the end:

```bash
&& npx playwright show-report
```
---

##### 🧪 8. Run with Playwright Test Runner UI

```bash
npx playwright test tests/e2e --ui
```

##### ▶️ 9. Example with tags, browser, UI and autoreport

```bash
npx playwright test tests/e2e --headed --project=chromium --grep "@ui" && npx playwright show-report
```
