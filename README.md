# 🌍 TripTale — Web Platform for Travel Planning & Storytelling

[![.NET Core](https://img.shields.io/badge/.NET%20Core-6.0%2B-512BD4?style=for-the-badge&logo=.net&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Razor%20Pages-512BD4?style=for-the-badge&logo=.net)](https://learn.microsoft.com/en-us/aspnet/core/)
[![MS SQL Server](https://img.shields.io/badge/SQL%20Server-2019%2B-CC292B?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/sql-server)
[![License](https://img.shields.io/badge/License-Academic%20/%20Open-blue?style=for-the-badge)](#-авторски-права-и-лиценз)

> **TripTale** е уеб-базирана платформа, обединяваща динамичното планиране на маршрути, визуалното съхранение на желани дестинации (Bucket Lists) и публикуването на авторски туристически истории с ролева модерация. Платформата е проектирана с многослойна структура на C# / .NET.

---

## 📋 Съдържание

- [🌟 Основна концепция и цел](#-основна-концепция-и-цел)
- [✨ Ключови функционалности](#-ключови-функционалности)
- [👥 Ролева архитектура (RBAC)](#-ролева-архитектура-rbac)
- [🏗️ Архитектура на системата](#-архитектура-на-системата)
- [🗄️ База данни и Модели (ER Schema)](#️-база-данни-и-модели-er-schema)
- [🔌 Външни интеграции & APIs](#-външни-интеграции--apis)
- [💻 Технологичен стек](#-технологичен-стек)
- [⚙️ Инсталация и локално стартиране](#️-инсталация-и-локално-стартиране)

---

## 🌟 Основна концепция и цел

Съвременният пътешественик често използва отделни платформи за търсене на вдъхновение, организиране на логистиката и споделяне на спомени. **TripTale** отстранява това фрагментиране, като предоставя единна дигитална среда.

Основната цел на проекта е създаването на интуитивно и сигурно уеб приложение, което:
1. **Упростява планирането:** Автоматизира изграждането на маршрути и избора на начални/крайни локации.
2. **Осигурява качествено съдържание:** Чрез въвеждане на роля „Историк“ и администраторска модерация се гарантира висока стойност на публикуваните статии.
3. **Визуализира географски данни:** Вгражда интерактивни карти за максимално потребителско улеснение.

---

## ✨ Ключови функционалности

### ✈️ Планиране на пътувания (Trips Management)
- Създаване на маршрути с избор на начална страна/град през интерактивна карта.
- Задаване на начални и крайни дати, заглавие и подробно описание.
- Автоматично генериране на маршрути въз основа на географски координати.
- Редактиране и премахване на лични пътувания.

### 📌 Списък с желания (Bucket List)
- Персонално изграждане на списъци с дестинации за бъдещи пътешествия.
- Интеграция с предварително сийдната база от градове и забележителности (`Places`).
- Бързо добавяне и преглед на желанията през графичен интерфейс.

### 📖 Авторски истории (Storytelling)
- Споделяне на изживявания и съвети, свързани с конкретни географски обекти.
- Поддръжка на статуси на статиите: `Pending` (в очакване), `Approved` (публикувана) и `Rejected` (отхвърлена).
- Възможност за преглед само на одобрени истории от стандартните потребители.

### 🛡️ Административен контрол
- Модерация в реално време на изпратените истории от потребители с роля *Historian*.
- Управление на потребителските профили и динамично променяне на техните роли.
- Глобален достъп за редактиране/премахване на неподходящо съдържание.

---

## 👥 Ролева архитектура (RBAC)

Приложението използва **ASP.NET Core Identity** за стриктен ролеви контрол на достъпа (Role-Based Access Control):

| Роля | Права и достъп в системата |
| :--- | :--- |
| 👤 **User** *(Потребител)* | Създава лични пътувания, управлява личен Bucket List, разглежда обществено достъпни истории и интерактивни карти. |
| 📚 **Historian** *(Историк)* | Всички права на `User` + право да пише и изпраща дълги туристически/исторически разкази за модерация. |
| 👑 **Administrator** | Всички права на `Historian` + пълен достъп до Admin Panel, одобряване/отхвърляне на истории, управление на потребителски роли и глобална поддръжка. |

---

## 🏗️ Архитектура на системата

Платформата е изградена по многослоен модел (**Layered Architecture**), осигуряващ висока модулност, слаба свързаност (Loose Coupling) и лесна поддръжка:

```
                  ┌─────────────────────────────────────────┐
                  │            Presentation Layer           │
                  │   (Razor Pages, Controllers, ViewModels)│
                  └────────────────────┬────────────────────┘
                                       │
                                       ▼
                  ┌─────────────────────────────────────────┐
                  │              Service Layer              │
                  │   (Managers, External API Orchestration)│
                  └────────────────────┬────────────────────┘
                                       │
                                       ▼
                  ┌─────────────────────────────────────────┐
                  │              Business Layer             │
                  │     (Domain Models, Business Rules)     │
                  └────────────────────┬────────────────────┘
                                       │
                                       ▼
                  ┌─────────────────────────────────────────┐
                  │               Data Layer                │
                  │   (EF Core DbContext, Repositories)     │
                  └────────────────────┬────────────────────┘
                                       │
                ┌──────────────────────┴──────────────────────┐
                ▼                                             ▼
     ┌─────────────────────┐                       ┌─────────────────────┐
     │   MS SQL Server     │                       │  Seeding Data Layer │
     │    (Database)       │                       │  (GeoNames Import)  │
     └─────────────────────┘                       └─────────────────────┘
```

---

## 🗄️ База данни и Модели (ER Schema)

Базата данни `TravelApp` е релационна и нормализирана. Управлява се чрез **Entity Framework Core (Code-First approach)**.

### Основни ентитети и връзки:
- **AspNetUsers:** Управление на акаунти (наследява `IdentityUser`).
- **Trips:** Връзка $1 \rightarrow N$ с `AspNetUsers` и $M \rightarrow N$ с `Places`.
- **Stories:** Връзка $1 \rightarrow N$ с `AspNetUsers` и съдържа поле `Status` (`Pending`, `Approved`).
- **BucketLists:** Връзка $1 \rightarrow N$ с `AspNetUsers` и `Places`.
- **Places & Countries:** Географска йерархия със съхранени Latitude/Longitude координати и Alpha-3 кодове.

---

## 🔌 Външни интеграции & APIs

1. **Google Maps JavaScript & Directions API:**
   - Използва се за поставяне на динамични маркери, геокодиране и визуализация на маршрутите между начална и крайна точка.
2. **Leaflet.js:**
   - Олекотена JavaScript библиотека за интерактивно избиране на географски региони и полигони при стартиране на ново пътуване (`CreateTripStart`).
3. **GeoNames Web Services:**
   - Автоматично извличане на актуални данни за държави, валути, езици и столици по време на сийдването на базата данни.

---

## 💻 Технологичен стек

- **Backend Framework:** ASP.NET Core 6.0+ / C#
- **ORM & Data Access:** Entity Framework Core (EF Core)
- **Security & Auth:** ASP.NET Core Identity & BCrypt Hashing
- **Frontend Technologies:** Razor Pages, HTML5, CSS3, JavaScript (ES6+)
- **Map Rendering:** Google Maps API, Leaflet.js
- **Database Server:** Microsoft SQL Server 2019 / SSMS
- **Unit Testing Framework:** xUnit & EF Core InMemory Database
- **Version Control:** Git / GitHub

---

## ⚙️ Инсталация и локално стартиране

### Предварителни изисквания
- [.NET 6.0 SDK](https://dotnet.microsoft.com/download) или по-нов
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (с инсталиран *ASP.NET and web development* уеб поток)
- [MS SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads) или пълна версия + SSMS

### Инструкции за настройка

1. **Клонирайте репозиторито:**
   ```bash
   git clone https://github.com/MartinPetkov27/TravelApp.git
   cd TravelApp
   ```

2. **Настройте Connection String:**
   Отворете `appsettings.json` във файла на уеб проекта и актуализирайте връзката към Вашия локален SQL Server:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=TravelAppDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

3. **Приложете миграциите и базирайте базата данни:**
   Отворете **Package Manager Console** във Visual Studio (`Tools` > `NuGet Package Manager` > `Package Manager Console`) и изпълнете:
   ```powershell
   Update-Database
   ```
   *Забележка: При първоначално стартиране модулът `SeedingLayer` автоматично ще зареди държавите от GeoNames API и ще създаде тестови потребители.*

4. **Стартирайте проекта:**
   Натиснете `F5` или изпълнете през CLI:
   ```bash
   dotnet run --project PresentationLayer
   ```
   Приложението ще бъде достъпно на `https://localhost:7001` (или съответния конфигуриран порт).
