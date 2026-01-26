# 🛡️ ApiWatchdog

> **A plug-and-play, real-time monitoring dashboard for ASP.NET Core Web APIs.** > *Track requests, errors, and latency with a beautiful Dark Mode UI.*

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Platform](https://img.shields.io/badge/platform-ASP.NET%20Core%208.0-purple)
![Status](https://img.shields.io/badge/status-Stable-green)

---

## ✨ Features

* **🚀 Real-time Monitoring:** Watch requests stream in live using **SignalR**.
* **📊 Beautiful Dashboard:** A modern, dark-themed UI with Glassmorphism design, interactive charts, and health indicators.
* **📉 Deep Insights:** Track response times (latency), status codes, HTTP methods, and throughput (RPM).
* **🐞 Error Tracking:** Capture full stack traces and exception messages for failed requests (500 errors).
* **💾 Persistent Storage:** Automatically logs data to SQL Server (using EF Core).
* **🔌 Zero Configuration:** Just install and run! Database tables are created automatically (Auto-Migration).
* **⚡ Lightweight:** Uses an embedded UI (Single DLL) and buffered logging to minimize performance impact.

---

## 📦 Installation

You can install the package via NuGet Package Manager or CLI:

```bash
dotnet add package ApiWatchdog


🚀 Quick Start
1. Register Services
In your Program.cs, add the service and provide your SQL Server connection string:

C#
var builder = WebApplication.CreateBuilder(args);

// Add ApiWatchdog Services
builder.Services.AddApiWatchdog(builder.Configuration.GetConnectionString("DefaultConnection"));

var app = builder.Build();
2. Enable Middleware
Add the middleware before MapControllers to ensure all requests are captured:

C#
app.UseHttpsRedirection();

// Enable ApiWatchdog (Ensure this is before MapControllers)
app.UseApiWatchdog();

app.UseAuthorization();
app.MapControllers();

app.Run();
🖥️ Accessing the Dashboard
Once your API is running, navigate to:

https://localhost:YOUR_PORT/api-watchdog/dashboard
⚙️ Configuration
The library automatically handles the database creation. Ensure your appsettings.json has a valid connection string:

JSON
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=ApiLogsDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
Note: The library will create a table named ApiLogs automatically on the first run.

🛠️ Technology Stack
Backend: ASP.NET Core 8.0, Entity Framework Core.

Real-time: SignalR.

Frontend: Vue.js 3, Chart.js, Tailwind CSS.

Architecture: Embedded Resource (Single DLL Distribution).

🤝 Contributing
Contributions are welcome! Please feel free to submit a Pull Request.

Fork the project.

Create your feature branch (git checkout -b feature/AmazingFeature).

Commit your changes (git commit -m 'Add some AmazingFeature').

Push to the branch (git push origin feature/AmazingFeature).

Open a Pull Request.

📝 License
Distributed under the MIT License. See LICENSE for more information.

Made with ❤️ by [Mustafa Aqeel]
