# TertiaryInstitutions API

An ASP.NET Core Web API for exploring South African public universities, their courses, NSC (matric) high school subjects, and checking whether a learner's subject results meet a course's admission requirements. Also supports learner accounts, an APS calculator, a "careers unlocked by subject combo" lookup, and a Holland Code (RIASEC) job-fit quiz.

## Features

- Browse public South African universities, optionally filtered by province
- Browse official NSC high school subjects, optionally filtered by category, designated-subject status, or elective status
- Compare a learner's NSC subject achievement levels (1-7) against a course's admission requirements
- Learner accounts: register/login (JWT), profile (grade, name, language, track)
- Calculate a generic APS (Admission Point Score) from a subject/level list
- Find courses (grouped by faculty) a subject combination could unlock
- Job-fit quiz: serve Holland Code (RIASEC) questions, submit answers, get a personality-code result (saved to the learner's profile when signed in)

## Tech stack

- .NET 8 / ASP.NET Core Web API
- PostgreSQL via EF Core (Npgsql) — learner accounts and assessment results only; university/course/subject data stays static/in-memory
- JWT bearer authentication
- Swashbuckle (Swagger / OpenAPI)

## Getting started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A local PostgreSQL instance
- `dotnet-ef` CLI tool (`dotnet tool install --global dotnet-ef`)

### Database setup

1. Configure the connection string and JWT signing key with `dotnet user-secrets` (never commit real values into `appsettings.json`):
   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5433;Database=khetha;Username=postgres;Password=<your-password>;Ssl Mode=Disable"
   dotnet user-secrets set "Jwt:Key" "<a random 32+ byte string>"
   ```
2. Apply migrations (this also creates the `khetha` database if it doesn't exist yet):
   ```bash
   dotnet ef database update
   ```

Outside local development, set `ConnectionStrings__DefaultConnection` and `Jwt__Key` as environment variables instead.

### Run

```bash
dotnet restore
dotnet run
```

Once running, Swagger UI is available at the app's root (or `/swagger`) for browsing and testing the endpoints interactively — use the "Authorize" button to attach a JWT bearer token to protected requests.

## API endpoints

### Universities

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/universities` | Get all universities, optionally filtered by `province` |
| GET | `/api/universities/{id}` | Get a single university by id |

### Subjects

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/subjects` | Get all NSC subjects, optionally filtered by `category`, `designated`, and/or `elective` |
| GET | `/api/subjects/{id}` | Get a single subject by id |

### Compare

| Method | Route | Description |
| --- | --- | --- |
| POST | `/api/compare/{courseId}` | Compare a learner's subjects/levels against a course's requirements |

### APS

| Method | Route | Description |
| --- | --- | --- |
| POST | `/api/aps/calculate` | Calculate a generic APS score (best 6 subjects, excluding Life Orientation) |

### Careers

| Method | Route | Description |
| --- | --- | --- |
| POST | `/api/careers/unlocked` | Find courses (grouped by faculty) a subject combination could unlock |

### Auth

| Method | Route | Description |
| --- | --- | --- |
| POST | `/api/auth/register` | Register a new learner account, returns a JWT |
| POST | `/api/auth/login` | Log in, returns a JWT |

### Learners (requires `Authorization: Bearer <token>`)

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/learners/me` | Get the signed-in learner's profile |
| PUT | `/api/learners/me` | Update the signed-in learner's profile |
| GET | `/api/learners/me/assessments` | Get the signed-in learner's past job-fit quiz results |

### Assessment

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/assessment/questions` | Get the job-fit quiz question bank |
| POST | `/api/assessment/submit` | Submit answers and get a Holland Code result (saved automatically if signed in) |

Example request body for `POST /api/compare/{courseId}` and `POST /api/aps/calculate`:

```json
[
  { "subject": "Mathematics", "level": 6 },
  { "subject": "English Home Language", "level": 5 }
]
```

## Project structure

```
Controllers/   API controllers (Universities, Subjects, Compare, Aps, Careers, Auth, Learners, Assessment)
Models/        Domain models and DTOs (University, Course, Subject, Comparison, Learner, Aps, Careers, Assessment)
Data/          Static datasets (universities, courses per institution, NSC subjects, quiz questions) + EF Core DbContext/migrations
Services/      Business logic (course comparison, subject matching, APS calculation, career matching, auth, JWT, quiz scoring)
```

## License

No license specified.
