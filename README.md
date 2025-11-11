# Batch Integration for XM Cloud using Azure Function

## Overview
While migrating a project from **Sitecore XP** to **Sitecore XM Cloud**, we faced a requirement to migrate a scheduler written in C#.  
The scheduler was responsible for inserting new items and updating existing ones in Sitecore, based on data fetched from a **third-party API**.

To replicate this functionality in **XM Cloud**, we decided to use an **Azure Function** — a lightweight, serverless backend service that integrates seamlessly with XM Cloud.

---

## Why Azure Functions with Sitecore XM Cloud?

**Azure Functions** are serverless, meaning:
- No server management is required.
- Code runs only when triggered.
- Automatically scales and remains cost-effective.

By connecting **Azure Functions** with **XM Cloud**, you can:

✅ Handle API calls or execute custom logic outside of Sitecore  
✅ Process or store data for custom fields or components  
✅ Integrate securely with external systems  
✅ Keep your Sitecore environment clean and modular  

This setup is ideal for extending XM Cloud’s capabilities without deploying heavy backend code.

---

## Working with Sitecore Authoring & Management API

One challenge was integrating the **Sitecore XM Cloud Authoring API** with our C# code.

The **Sitecore Authoring and Management API** provides a **GraphQL** endpoint and schema that allows you to manage Sitecore content programmatically.  
To perform **insert** and **update** operations, we converted GraphQL queries into **C#-compatible requests** and executed them within the Azure Function.

---

## Azure Function Setup in Visual Studio

1. Open **Visual Studio** and create a **new Azure Function project**.
2. Add a new function class (e.g., `RunCron.cs`).
3. Use a **TimerTrigger** to define the function’s schedule.
4. Configure the timer schedule in **local.settings.json**.

### Example Code Snippet

```csharp
[Function("RunCron")]
public async Task RunCronAsync([TimerTrigger("%TimerSchedule%")] TimerInfo timer)
{
    // Your code to fetch data from third-party API
    // Transform data as needed

    // Call Sitecore XM Cloud Authoring API
    // Insert or update items using GraphQL requests

    _logger.LogInformation("Batch Integration completed successfully at: {time}", DateTime.Now);
}
```

In the `local.settings.json`, define the timer schedule as follows:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "TimerSchedule": "0 0 */2 * * *" // Runs every 2 hours
  }
}
```

---

## Benefits of This Approach

- **Scalable:** Azure Function scales automatically based on execution needs.  
- **Modular:** Keeps XM Cloud focused only on content management.  
- **Cost-efficient:** Pay only for execution time.  
- **Extensible:** Easily integrate multiple APIs and automation tasks.

---

## Conclusion

Using **Azure Functions** for **batch integration** in **Sitecore XM Cloud** enables developers to build efficient, modular, and scalable integrations.  
This approach simplifies handling background jobs, external data syncs, and custom business logic without overloading your Sitecore instance.

---

### 🧩 Technologies Used
- **Sitecore XM Cloud**
- **Azure Functions (C#)**
- **GraphQL**
- **Visual Studio**
- **Third-party APIs**
