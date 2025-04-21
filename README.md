### Test Documentation and Bug Reporting

The `Documents` folder contains comprehensive documentation for all testing layers in the project:

- **Unit Tests Documentation.xlsx**  
  Contains detailed descriptions of all unit test cases.

- **API Tests Documentation.docx**  
  Includes full documentation of all Playwright API test cases, along with identified bugs and improvement suggestions for the API.

- **E2E Test Cases – Food Truck Search UI.docx**  
  Describes the Playwright end-to-end test scenarios for the frontend, including functionality coverage and usage context.


### ⚠️ Important
- Always use powershell as admin
- If at any point of the process you get an error message containing
```
running scripts is disabled on this system
```
You need to run this command in you powershell to enable scripts running:
```
Set-ExecutionPolicy -Scope CurrentUser RemoteSigned
```
select Y or A when prompted.
### 🧪 Unit Tests - Setup and Execution

This project is set up to:

- Run unit tests
- Generate **HTML test** and **coverage reports**
- Open both reports automatically.

##### 🔧 Requirements
- Clone this repository and run the application in the folder `radAichallenge` [using the original repo instructions](https://github.com/radaisystems/food-trucks-challenge)
- Install [.NET SDK 8.x](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (If not installed)
- Install ReportGenerator (install once via terminal):

```
dotnet tool install --global dotnet-reportgenerator-globaltool
```

##### 🚀 How to Run
In the cloned repository folder:

```
cd radAichallenge
dotnet restore
dotnet build
dotnet test
```

##### 📂 Report Output

- HTML test report: `Reports/TestResults/TestResults.html`
- Coverage dashboard: `Reports/Coverage/Html/index.html`

---

### 🎭 Playwright Tests - Common Setup

> This section applies to **both E2E and API** Playwright tests.

##### ✅ 1. Install Prerequisites

- Install [Node.js and npm](https://nodejs.org)
- Make sure the application is running on `http://localhost:5000`

##### 📦 2. Install Dependencies

From the root of the `PlaywrightTests` folder:

```
cd PlaywrightTests
npm install
npx playwright install
```

---

### 🧪 Playwright E2E Tests -  Setup & Execution

> Minimal guide to install dependencies and run existing Playwright UI tests with tags, headed mode, browser selection, and HTML reports.

##### 🏃 Run All E2E Tests

```
npm run test:e2e
```

##### 🏷️ Run Tests by Tags (No HTML report)

```
npm run test:e2e:tag --grep "@ui"
```

**With tags (AND):**
```
npm run test:e2e:tag --grep "@ui" --grep "@search"
```

**With tags (OR):**
```
npm run test:e2e:tag --grep "@ui|@search"
```

##### 🧭 Run Tests in Headed Mode (Show browser UI)

```
npx playwright test tests/e2e --headed
```

**Combine with tag and browser:**
```
npx playwright test tests/e2e --headed --project=chromium --grep "@tag"
```

##### 🌐 Run Tests in a Specific Browser

Available browsers: `chromium`, `firefox`, `webkit`

```
npx playwright test tests/e2e --project=chromium
npx playwright test tests/e2e --project=firefox
npx playwright test tests/e2e --project=webkit
```

##### 📊 Generate and Open HTML Report

Run all tests and open the HTML report:

```
npm run test:e2e
```

To auto-open the report for any run (add at the end of npx playwright test commands):
```
&& npx playwright show-report
```

##### 🧪 Run with Playwright Test Runner UI

```
npx playwright test tests/e2e --ui
```

##### ▶️ Example with tags, browser, UI and autoreport

```
npx playwright test tests/e2e --headed --project=chromium --grep "@ui" && npx playwright show-report
```

---

### 🔌 Playwright API Tests - Setup & Execution

> Minimal guide to run API tests using Playwright (non-browser).

##### 🚀 Run All API Tests

```
npm run test:api
```

This will run all `.spec.ts` files under `tests/api`, generate an HTML report, and open it automatically.

##### 🧪 Run a Specific Test File

```
npx playwright test tests/api/myTest.spec.ts
```

##### 🧪 Run with UI

```
npx playwright test tests/api --ui
```

##### 📊 API Test Report Location

After running:

- Report opens automatically
- Located at: `playwright-report/index.html`

##### 🧠 Notes

- API tests are tagged by project name `"API"` in the config.
- No browser context is loaded unless manually specified.
