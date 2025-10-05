# auth-solution

Skeleton repo cho dự án Authentication (Next.js frontend + ASP.NET backend).

## Cấu trúc

=============================
Backend (AuthBackend)
=============================

Setup & Run:
- Open terminal, go to backend folder:
  cd src/backend/AuthBackend

- Restore packages:
  dotnet restore

- Run backend:
  dotnet run

  Backend runs by default at https://localhost:5038

=============================
Frontend (frontend)
=============================

Setup & Run:
- Open terminal, go to frontend folder:
  cd src/frontend

- Install dependencies:
  npm install

- Run frontend:
  npm run dev

  Open browser at http://localhost:3000

=============================
Unit Tests (AuthBackend.Tests)
=============================

Run Tests:
- Open terminal, go to test project:
  cd src/tests/AuthBackend.Tests

- Run tests:
  dotnet test

Notes:
- Basic tests for backend AuthController
- Uses xUnit framework
