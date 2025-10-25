# 🧠 DynamicHead.Blazor
### Dynamic, component-driven `<head>` management for Blazor

[![Build](https://github.com/SimonLiebers-Dev/DynamicHead.Blazor/actions/workflows/build-and-release.yml/badge.svg)](https://github.com/simonliebers/DynamicHead.Blazor/actions)
[![NuGet](https://img.shields.io/nuget/v/DynamicHead.Blazor.svg?style=flat-round&logo=nuget)](https://www.nuget.org/packages/DynamicHead.Blazor/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

---

## 🚀 Overview

**DynamicHead.Blazor** brings *true dynamic head management* to Blazor.  
Unlike Blazor’s built-in `<HeadOutlet>` and `<HeadContent>`, this library allows **any component** to declaratively add, update, or remove `<head>` content — such as `<title>`, `<meta>`, or `<link>` tags — without overwriting others.

---

## 💡 Why DynamicHead.Blazor?

Blazor’s built-in head management works only at the **page level**, and each `<HeadContent>` instance overwrites the previous one.

| Limitation (Blazor’s built-in) | Solved by DynamicHead.Blazor |
|--------------------------------|-------------------------------|
| Only page-level head content | ✅ Any component can register head content |
| Each `<HeadContent>` overwrites previous | ✅ All content merges dynamically |
| No automatic cleanup | ✅ Automatically unregisters on dispose |
| No runtime updates | ✅ Dynamically re-renders live |
| No parallel or nested support | ✅ Works in any nested component |

---

## ✨ Features

- ⚙️ Works with **Blazor WebAssembly** and **Blazor Server**
- 🔄 Automatically adds and removes head elements
- 🧠 Updates dynamically as component content changes
- 🧱 Lightweight, no external dependencies
- 🧪 Fully unit-tested (NUnit + bUnit + Moq)

---

## 📦 Installation

Install from NuGet:

```bash
dotnet add package DynamicHead.Blazor
```

---

## ⚙️ Setup (WebAssembly Example)

**Program.cs**

```csharp
using DynamicHead.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Register the DynamicHead service
builder.Services.AddDynamicHead();

// Mount your app and the DynamicHeadOutlet
builder.RootComponents.Add<App>("#app");

// 🧠 Add DynamicHeadOutlet to the document head
builder.RootComponents.Add<DynamicHeadOutlet>("head::after");

await builder.Build().RunAsync();
```

💡 `head::after` ensures the outlet is rendered directly inside the `<head>` element,  
just like Blazor’s built-in `<HeadOutlet>` — but this version dynamically merges updates.

---

## 🧠 Usage

Use `<DynamicHeadContent>` anywhere in your app — even inside nested components:

```razor
@page "/about"

<DynamicHeadContent>
    <title>About – MyApp</title>
    <meta name="description" content="Learn more about MyApp." />
    <link rel="stylesheet" href="about.css" />
</DynamicHeadContent>

<h1>About</h1>
```

✅ When this component renders:
- Its content is **registered** in the shared head service
- `<DynamicHeadOutlet>` re-renders the document `<head>`
- When it’s disposed (navigated away), the content is **automatically removed**

## 🧠 Comparison

| Feature | Blazor `<HeadContent>` | DynamicHead.Blazor |
|----------|------------------------|--------------------|
| Nested components | ❌ | ✅ |
| Multiple fragments | ❌ Overwrites | ✅ Merges |
| Auto cleanup | ❌ | ✅ |
| Runtime updates | ⚠️ Limited | ✅ Full |
| Works with `head::after` | ✅ | ✅ |

---

## 🧪 Testing

DynamicHead.Blazor is fully covered by automated tests using:
- [bUnit](https://bunit.dev)
- [Moq](https://github.com/moq)
- [NUnit](https://nunit.org)

---

## 📄 License

MIT © 2025 Simon Liebers