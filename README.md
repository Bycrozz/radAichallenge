#### 🧪 Running Unit Tests

This project is set up to:

- Run unit tests
- Generate **HTML test** and **coverage reports**
- Open both reports automatically

##### 🔧 Requirements

- Install [.NET SDK 8.x](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- ReportGenerator (install once via terminal):

```bash
dotnet tool install --global dotnet-reportgenerator-globaltool
```

##### 🚀 How to Run

```bash
git clone https://github.com/lucaspin92/radAichallenge.git
cd radAichallenge
dotnet restore
dotnet test
```

##### 📂 Output

- HTML test report: `Reports/TestResults/TestResults.html`
- Coverage dashboard: `Reports/Coverage/Html/index.html`
