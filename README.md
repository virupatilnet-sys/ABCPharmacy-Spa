# ABC Pharmacy SPA

A runnable ASP.NET Core 10 single-page application for managing pharmacy stock and medicine sales. It uses no database: `Data/pharmacy-data.json` is the server-side data store.

## Run it

Prerequisite: [.NET SDK 10](https://dotnet.microsoft.com/download).

```powershell
cd src/PharmacySpa.Api
dotnet run
```

Open the HTTPS URL printed in the terminal (typically `https://localhost:7xxx`). The SPA is served by the API, so there is no separate Node or frontend build step.

To build and run tests from the repository root:

```powershell
dotnet build PharmacySpa.sln
dotnet test PharmacySpa.sln
```

## Features

- Grid listing full name, expiry date, quantity, price, and brand (notes remain in the add/edit form only).
- Red row for a medicine expiring in under 30 days; yellow row for stock below 10. Expiry warning takes precedence when both apply.
- Debounced, case-insensitive name search.
- Add, edit, and delete medicines with server-side validation.
- Record sales from the grid. A sale and stock decrement happen together under a process-wide file lock, and insufficient stock is refused.
- Recent-sale ledger, including the price captured at the time of sale.
- OpenAPI 3.0 description at `/openapi/v1.json` while running. CORS is enabled for a separately hosted SPA during development.

## API

| Method | Endpoint | Purpose |
| --- | --- | --- |
| GET | `/api/medicines?search=para` | List/search medicines |
| GET | `/api/medicines/{id}` | Fetch a medicine |
| POST | `/api/medicines` | Create a medicine |
| PUT | `/api/medicines/{id}` | Update a medicine |
| DELETE | `/api/medicines/{id}` | Delete a medicine |
| GET | `/api/sales` | List sales, newest first |
| POST | `/api/sales` | Record sale and reduce stock |

Create/update body:

```json
{
  "fullName": "Ibuprofen 200mg",
  "notes": "Take with food.",
  "expiryDate": "2027-08-18",
  "quantity": 50,
  "price": 5.25,
  "brand": "Example Brand"
}
```

Sale body: `{ "medicineId": "GUID", "quantity": 2 }`.

## Storage and safety

The repository serializes reads/writes through `SemaphoreSlim`, writes JSON to a sibling temporary file, and atomically replaces the data file only after serialization completes. That ensures simultaneous sale requests in this web process cannot oversell inventory or leave a partial JSON file. For multi-process hosting, replace the local lock with a distributed/file-system lock or move persistence to a database.

Seed data lives in `src/PharmacySpa.Api/Data/pharmacy-data.json`; it is copied into the build output. During local development, changing the data file resets the sample inventory after the next build.
