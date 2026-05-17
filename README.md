# ATM Cash Dispenser API

[🇯🇵 日本語版のREADMEはこちら](./README.ja.md)

## 🎯 What This Project Solves

This project simulates a real-world ATM cash dispensing system under realistic business and technical constraints:

- **Limited cash inventory** management
- **Concurrent withdrawal requests** (multi-threaded environment)
- **Atomic transaction requirements** (All-or-Nothing guarantees)

It focuses on solving **consistency and correctness under concurrent state mutations**, a common challenge in financial, payment, and highly transactional systems.

---

## 👤 Project Intent & My Role

This project was fully designed and implemented from scratch to demonstrate a deep understanding of:

- **Strong domain modeling** with strict invariant enforcement at the type level.
- **Safe handling of shared mutable state** in a high-concurrency web environment.
- **Robust error handling via the Result Pattern**, eliminating the performance overhead of domain exceptions.
- **Clear separation of concerns** utilizing a three-tier layered architecture (DDD-inspired).

All design decisions (e.g., `lock`, Singleton lifecycle, Value Objects) were chosen intentionally to balance architectural tradeoffs.

---

## 🏗 Architecture

The project strictly adheres to a clean three-layer structure, ensuring that the core business logic remains completely isolated from infrastructure frameworks and external input models.

```text
src/
├── AtmCashDispenser.Domain/       # Domain Layer (Pure business logic)
│   ├── Dispensing/                # CashInventory state, Calculator algorithm
│   └── Shared/                    # Money, Denomination (Value Objects)
├── AtmCashDispenser.Application/  # Application Layer (Use case orchestration)
│   └── DispenseCash/              # DispenseCashUseCase, DTOs
└── AtmCashDispenser.Api/          # API Layer (HTTP Minimal API endpoint)

```

* **Domain**: Pure business rules. Zero dependencies on external libraries or web frameworks.
* **Application**: Orchestrates use case execution by mapping input data, invoking the domain, and returning DTOs.
* **API**: Handles HTTP routing, request binding, and JSON serialization.

---

## 🚀 Key Design Decisions & Tradeoffs

### 1. Atomic Transactions via `lock` (Thread-Safety)

In an API context where multiple withdrawal requests execute concurrently, preventing race conditions on cash inventory is critical.

* **The Choice**: We implemented a synchronized `lock` block to create a critical section during the inventory check and deduction phase.
* **The Justification**: While `ConcurrentDictionary` ensures thread-safety for single-key operations, an ATM dispensing operation requires **multi-key atomicity (All-or-Nothing)**. We must verify that *all* required denominations are sufficient before deducting *any* of them. A `lock` statement guarantees that the inventory state remains strictly consistent without mid-operation corruption.

### 2. Singleton Lifecycle for Hardware Vault Simulation

* **The Choice**: The `CashInventory` instance is registered as a `Singleton` in the dependency injection (DI) container.
* **The Justification**: This simulates a physical ATM hardware vault shared across all incoming HTTP requests. While a production enterprise system would delegate transaction isolation and persistence to a Relational Database (RDB) using Scoped repositories with row-locks, this in-memory architecture leverages a Singleton to model hardware state constraints cleanly.

### 3. Invariant Protection via Value Objects

* **Money**: Enforces that an amount cannot be negative at the moment of initialization.
* **Denomination**: Utilizes a strict mapping to ensure only valid Japanese currency denominations (e.g., 10000, 5000, 1000) can ever exist within the domain layer.
* **The Goal**: Enforces business rules at the type level, making invalid domain states **unrepresentable by design**.

### 4. High-Performance Error Handling with the `Result<T>` Pattern

Rather than throwing expensive domain exceptions for expected business failures (such as "insufficient inventory" or "unsupported amount combination"), the system utilizes an explicit, type-safe `Result` structure to return success or failure data.

* **The Choice**: Business validation failures are encapsulated and propagated back to the application layer via a strongly-typed Result wrapper.
* **The Justification**: Native exception throwing in C# incurs a significant performance cost due to stack trace generation. By treating business failures as predictable control flows rather than exceptional system errors, the API achieves maximum throughput, safety, and clear code expressiveness.

---

## 🧪 Testing Strategy

This repository demonstrates complete test-driven confidence by maintaining three distinct test layers using **xUnit**:

* **✔ Domain Unit Tests (100% Core Coverage)**: Validates calculation correctness, mathematical edge cases, and verifies that the `Result` container correctly captures success/failure boundaries under extreme scenarios.
* **✔ Application Integration Tests**: Verifies the orchestration flow of the `DispenseCashUseCase`, ensuring input mapping is correct and that the inventory state mutates properly.
* **✔ API End-to-End Tests**: Uses `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`) to spin up an in-memory test server. This validates endpoint routing, appropriate HTTP status codes (200 OK vs 400 BadRequest), and strict JSON serialization contracts.

---

## 💪 Core Guarantees of the System

1. **Zero Race Conditions** under heavily concurrent HTTP requests.
2. **No Partial State Corruption** if a dispensing calculation fails mid-way.
3. **Zero Invalid Data** (negative values or unsupported bills) entering the domain.
4. **Zero Exception Overhead** for expected business logic failures.

---

## 🛠 Tech Stack

* **Framework**: .NET 9.0 (Minimal API)
* **Testing**: xUnit
* **Integration Utility**: Microsoft.AspNetCore.Mvc.Testing
* **Documentation**: Swagger / OpenAPI

---

## 📡 API Usage

### `POST /dispense`

Calculates and executes the optimal cash dispensing combination.

**Request Body**

```json
{
  "amount": 16000
}

```

**Response (200 OK)**

```json
{
  "items": [
    {
      "denomination": 10000,
      "count": 1
    },
    {
      "denomination": 5000,
      "count": 1
    },
    {
      "denomination": 1000,
      "count": 1
    }
  ]
}

```

**Error Responses (400 Bad Request)**
Returns a clean bad request context when:

* The requested amount is negative or invalid.
* The amount cannot be broken down using valid available denominations.
* The internal ATM vault has insufficient inventory to fulfill the request.

---

## 🔮 Future Improvements & Roadmap

This project is intentionally designed as an extensible MVP. The next production-ready milestones are:

1. **Persistence Layer**: Introduce Entity Framework Core with SQLite/PostgreSQL and implement the Repository Pattern to replace the in-memory Singleton vault with database-level row locks.
2. **Global Exception Handling Middleware**: Build a custom middleware to catch unexpected system crashes and format them into standard **RFC 7807 (Problem Details)** JSON compliance.
3. **Structured Logging**: Integrate Serilog to establish observability and audit logs for dispensing transactions.