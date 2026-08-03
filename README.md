# TaskManager
Technical assessment: A full-stack task management system using Angular, ASP.NET Core, and EF Core In-Memory.

## Published Application URLs (Render)

- **Frontend (Angular):** [https://taskmanager-frontend-uani.onrender.com](https://taskmanager-frontend-uani.onrender.com)
- **Backend & Swagger (ASP.NET Core):** [https://taskmanager-backend-9x0l.onrender.com](https://taskmanager-backend-9x0l.onrender.com)

**NOTE: The frontend and backend are deployed on separate Render instances, it takes around 1 minute for the instances to be ready. Wait for the backend to be ready before accessing the frontend.**

---

## Instructions for Local Execution

### 1. Backend (ASP.NET Core)
1. Ensure you have [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed.
2. Navigate to the API project folder:
   ```bash
    cd backend/TaskManager.Api
   ```

3. Restore dependencies and run the application:
    ```bash
        dotnet restore
        dotnet run

    ```


4. The API will run and the Swagger interface will open directly at the root (`/`).

### 2. Frontend (Angular)

1. Ensure you have [Node.js](https://nodejs.org/) and Angular CLI installed.
2. Navigate to the frontend folder:
```bash
cd frontend/task-manager-ui

```


3. Install dependencies:
```bash
npm install

```


4. Start the development server:
```bash
ng serve

```


5. Open your browser at: `http://localhost:4200`

---

## Instructions for Running Tests

To run the automated backend test suite:

```bash
dotnet test

```

---

## Relevant Technical Decisions

* **Layered Architecture:** Clear separation of concerns (Controllers, Services, and Data Context) to improve maintainability and readability.
* **Database Flexibility:** Configured EF Core to support PostgreSQL in production and EF Core In-Memory as a fallback.
* **Configured CORS:** Structured policies for secure communication between local and production domains (Render).
* **XML Documentation in Swagger:** Inclusion of documentation comments safely integrated into the build pipeline.

---

## Assumptions Made

* The system assumes direct task management focusing on agility, responsiveness, and usability.
* In cloud environments, sensitive data and connection strings are injected via environment variables.

---

## Improvements to Implement with More Time

* **Filters & Pagination:** Enhancement of task listings with server-side pagination and advanced filters.

---

## AI Tools Used

* **Windsurf IDE** for the assisted development workflow (auto-completion and debugging).
* Creation and initial structuring of components and boilerplates.
* Diagnostics, debugging, and fixing compilation errors and Linux environment constraints (`inotify` / status 134) on Render.
* Implementation and refinement of automated tests.
* CSS styling adjustments and UI refinements.
* Cloud deployment configuration (Render) and Swagger documentation integration.
