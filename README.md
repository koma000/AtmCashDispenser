
# ATM Cash Dispenser API

## 🎯 What This Project Solves

This project simulates a real-world ATM cash dispensing system under realistic constraints:

- Limited cash inventory
- Concurrent withdrawal requests
- Atomic transaction requirements (all-or-nothing)

It focuses on solving **consistency and correctness under concurrent state mutations**, a common challenge in financial and transactional systems.

---

## 👤 My Role & Intent

This project was fully designed and implemented to demonstrate:

- Strong domain modeling with enforced invariants
- Safe handling of shared mutable state in a web environment
- Clear separation of concerns using layered architecture (DDD-inspired)

All design decisions (e.g., `lock`, Singleton lifecycle, Value Objects) were made intentionally and are explained below.

---

## 🏗 Architecture

The project follows a clean three-layer structure:

```text
src/
├── AtmCashDispenser.Domain/       # Core business logic (Pure C#)
│   ├── Dispensing/                # CashInventory, Calculator, DispensePlan
│   └── Shared/                    # Money, Denomination (Value Objects)
├── AtmCashDispenser.Application/  # Use case orchestration
│   └── DispenseCash/              # UseCase, DTOs
└── AtmCashDispenser.Api/          # Minimal API (HTTP layer)
````

* **Domain**: Pure business logic, no framework dependency
* **Application**: Coordinates use cases
* **API**: Handles HTTP and serialization

---

## 🚀 Key Design Decisions

### 1. Atomic Transactions with `lock`

ATM dispensing requires **multi-denomination consistency**.

* All required bills must be available before dispensing
* Partial deduction must never occur

**Why not ConcurrentDictionary?**

* It guarantees thread-safety per key
* But **cannot ensure atomic operations across multiple keys**

👉 A `lock` ensures **all-or-nothing consistency** across the entire operation.

---

### 2. Singleton Cash Inventory

```csharp
services.AddSingleton<CashInventory>();
```

**Why Singleton?**

* Simulates a physical ATM vault shared across all requests
* Ensures consistent in-memory state

In real systems:

* This would be replaced by a database with transaction isolation

---

### 3. Value Objects for Invariants

#### Money

* Prevents negative values
* Ensures domain correctness at creation time

#### Denomination

* Only valid currency values allowed (e.g., 10000, 5000, 1000)
* Invalid states are impossible by design

👉 Domain rules are enforced **at the type level**

---

### 4. Exception-Based Domain Validation

Failures such as:

* Insufficient inventory
* Impossible denomination breakdown

are expressed via domain exceptions.

**Why exceptions?**

* Clear and idiomatic for invariant violations
* Keeps domain logic expressive

**Future improvement:**

* Replace with `Result<T>` for predictable control flow and performance

---

## 🧪 Testing Strategy

This project emphasizes **high confidence through testing**:

### ✔ Domain Tests

* 100% coverage of core logic
* Edge cases and failure scenarios fully validated

### ✔ Application Tests

* Verifies use case orchestration
* Ensures correct state transitions

### ✔ API Tests

* End-to-end testing using `WebApplicationFactory`
* Validates:

  * Routing
  * Status codes
  * JSON contracts

---

## 💪 What This System Guarantees

* No race conditions under concurrent requests
* No partial state corruption
* Strict domain invariant enforcement
* Deterministic and testable behavior

---

## 🛠 Tech Stack

* .NET 9.0 (Minimal API)
* xUnit
* Microsoft.AspNetCore.Mvc.Testing
* Swagger / OpenAPI

---

## 📡 API Usage

### POST `/dispense`

**Request**

```json
{
  "amount": 16000
}
```

**Response**

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

**Error Cases (400 Bad Request)**

* Invalid amount
* Unsupported denomination breakdown
* Insufficient inventory

---

## 🔮 Future Improvements

This project is intentionally designed as an MVP.

Next steps toward production readiness:

* Introduce persistence layer (EF Core + RDB)
* Implement transaction boundaries with DB-level locking
* Replace exceptions with `Result<T>` pattern
* Add global exception handling (Problem Details / RFC 7807)
* Add structured logging and observability

---

## 📌 Summary

This project demonstrates:

* Practical application of DDD principles
* Safe concurrency handling in web applications
* Thoughtful design trade-offs with clear justification
* Strong automated testing practices

It is designed not just to "work", but to be **correct, safe, and explainable**.