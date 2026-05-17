
# ATM Cash Dispenser API

## 📌 Overview
This project is an API for an ATM cash dispensing system. Given a withdrawal amount, the system calculates the optimal combination of denominations to dispense while maintaining the consistency of the current cash inventory.

This project was built as a portfolio piece to demonstrate practical architectural design, emphasizing **Domain-Driven Design (DDD)** principles, robust state management, and high-quality testing strategies.

## 🚀 Technical Highlights & Design Decisions
* **Strict Domain Layering (DDD)**: Core business rules (e.g., `Money`, `Denomination`, `CashInventory`) are completely isolated from infrastructure and external dependencies.
* **Invariant Protection**: Entity and Value Object constructors strictly prevent invalid states (e.g., negative amounts, non-existent denominations).
* **Atomic Transactions (In-Memory)**: The `CashInventory` uses thread-safe mechanisms (`lock`) to ensure that inventory deductions are atomic. If dispensing fails midway, no inventory is corrupted.
* **Control Flow without Exceptions**: Implemented the Result pattern to handle domain failures (like insufficient funds) as values, avoiding the performance overhead of throwing exceptions for expected business rules.
* **API DTO Mapping**: Separated Domain models from API contracts using dedicated Response DTOs to provide a clean, predictable JSON structure for frontend clients.

## 🧪 Testing Strategy
The project places a strong emphasis on reliability, utilizing **xUnit** and **FluentAssertions**:
* **Domain Layer (100% Coverage)**: Comprehensive unit tests covering all business rules, boundary values, and algorithm edge cases.
* **Application Layer**: In-memory integration tests to verify use case orchestration without mocking the core domain.
* **API Layer**: End-to-end HTTP integration testing using `WebApplicationFactory` to validate routing, JSON serialization, and status codes.

## 🛠 Tech Stack
* **.NET 9.0** (Minimal API)
* **xUnit / FluentAssertions** (Testing)
* **Microsoft.AspNetCore.Mvc.Testing** (Integration Testing)
* **Swagger / OpenAPI** (Documentation)

## 📡 API Usage

### `POST /dispense`
Calculates and executes cash dispensing.

**Request**
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

**Error Handling**

* `400 Bad Request`: When the amount is invalid (e.g., `<= 0`), cannot be dispensed with available denominations, or insufficient inventory.

## 🔮 Future Improvements

While the core domain is robust, this MVP focuses on in-memory operations. Future enhancements would include:

* Persist inventory with a database (Entity Framework Core)
* Introduce the Repository pattern for data access
* Implement Global Exception Handling Middleware for unhandled system errors