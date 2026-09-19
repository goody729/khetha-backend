# TertiaryInstitutions API

An ASP.NET Core Web API for exploring South African public universities, their courses, NSC (matric) high school subjects, careers, and checking whether a learner's subject results meet a course's admission requirements. Also supports learner accounts (with saved location), an APS calculator, a career directory with match-scoring, distance/prospectus-aware institution browsing with bulk APS matching, a Holland Code (RIASEC) job-fit quiz, and a 5-step journey tracker.

## Features

- Browse public South African universities, optionally filtered by province and distance from a location
- Browse official NSC high school subjects, optionally filtered by category, designated-subject status, or elective status
- Compare a learner's NSC subject achievement levels (1-7) against a course's admission requirements, in bulk across one or all institutions
- Learner accounts: register/login (JWT), profile (grade, name, language, track, saved location)
- Calculate a generic APS (Admission Point Score) from a subject/level list
- Career directory (title, demo OFO code, responsibilities, required subjects, pathways, RIASEC tags) with match-scoring against a learner's subjects and/or job-fit quiz result
- Find courses (grouped by faculty) a subject combination could unlock
- Job-fit quiz: serve Holland Code (RIASEC) questions, submit answers, get a personality-code result (saved to the learner's profile when signed in)
- Journey tracker: save careers to a shortlist, track progress across 5 fixed roadmap steps (Explore → Assess → Shortlist → Apply → Enroll)
- User guide: a YouTube video playlist exposed via `/api/guide` and linked from Swagger
- Term marks tracking: learners still in school enter their report-card marks each term, see whether they're on track for their career goals, and get AI coaching
- School calendar & reminders: the four-term SA school year as JSON and `.ics`, plus push-notification report-card reminders when schools reopen and monthly after

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
| GET | `/api/careers` | Get the career directory, optionally filtered by `riasecType` and/or `keyword` |
| GET | `/api/careers/{id}` | Get a single career by id |
| POST | `/api/careers/unlocked` | Find courses (grouped by faculty) a subject combination could unlock |
| POST | `/api/careers/match` | Rank all careers against supplied subjects and/or (if signed in) the learner's latest job-fit quiz result |

`OfoCode` values in the career directory are **fabricated placeholders for structural/demo purposes only** — not verified official South African OFO codes.

### Auth

| Method | Route | Description |
| --- | --- | --- |
| POST | `/api/auth/register` | Register a new learner account, returns a JWT |
| POST | `/api/auth/login` | Log in, returns a JWT |

### Learners (requires `Authorization: Bearer <token>`)

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/learners/me` | Get the signed-in learner's profile |
| PUT | `/api/learners/me` | Update the signed-in learner's profile (including saved location) |
| GET | `/api/learners/me/assessments` | Get the signed-in learner's past job-fit quiz results |

### Institutions

A richer view over the same catalog as `/api/universities`. TVET colleges are not yet included.

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/institutions` | Get all institutions, optionally filtered by `province`, sorted by distance when `lat`/`lng` or a saved profile location is available |
| GET | `/api/institutions/{id}` | Get a single institution, with distance and prospectus-link fallback |
| POST | `/api/institutions/{id}/matching-courses` | Find which of one institution's courses a learner's subjects/levels qualify for |
| POST | `/api/institutions/matching-courses` | Same, across every institution |

`lat`/`lng` query params take precedence over a signed-in learner's saved profile location. Distance is straight-line (Haversine), approximated from city-centroid coordinates — not routing distance.

### Assessment

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/assessment/questions` | Get the job-fit quiz question bank |
| POST | `/api/assessment/submit` | Submit answers and get a Holland Code result (saved automatically if signed in) |

### Journey (requires `Authorization: Bearer <token>`)

| Method | Route | Description |
| --- | --- | --- |
| POST | `/api/journey/careers/{careerId}` | Save a career to the learner's shortlist (idempotent) |
| DELETE | `/api/journey/careers/{careerId}` | Remove a career from the shortlist (idempotent) |
| GET | `/api/journey/careers` | Get the learner's saved careers, with full career detail |
| GET | `/api/journey/progress` | Get roadmap progress and the computed current step |
| POST | `/api/journey/progress/{step}/complete` | Mark a step complete (idempotent; `step` is one of `Explore`, `Assess`, `Shortlist`, `Apply`, `Enroll`) |

Completing a later step does not auto-complete earlier ones — each step is tracked independently.

### Term results (requires `Authorization: Bearer <token>`)

For learners still in school. Marks are report-card percentages per subject per term; the API converts them to NSC levels (7 = 80-100%, 6 = 70-79, 5 = 60-69, 4 = 50-59, 3 = 40-49, 2 = 30-39, 1 = 0-29).

| Method | Route | Description |
| --- | --- | --- |
| POST | `/api/termresults` | Enter/correct marks for one term (`year`, `term` 1-4, `subjects[]`); re-submitting a subject overwrites it |
| GET | `/api/termresults` | Get entered marks grouped by year/term, optionally filtered by `year` |
| GET | `/api/termresults/progress` | Are the latest marks on track for the learner's career goals? Uses saved careers, or one career via `careerId`. Returns per-subject trends and per-career status (`OnTrack`, `Close`, `NeedsAttention`, `NoMarks`, `NoRequirements`) |
| POST | `/api/termresults/coach` | Same progress plus AI coaching (summary, focus subjects, encouragement). Falls back to progress-only if the AI is unavailable |
| GET | `/api/termresults/status` | Which terms' marks are due but missing, and the next reminder date (for an in-app prompt) |

The on-track check is deterministic (it reuses the career/subject requirement matching); the AI only explains it and never sees the learner's name or email. Term marks are an early indicator, not final NSC results. Coaching needs `Anthropic:ApiKey` (same setting as `/api/ask`).

### Calendar

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/calendar/{year}` | School year as JSON: term open/close dates, holidays, and report-card reminder dates |
| GET | `/api/calendar/{year}/ics` | Same as an iCalendar (`.ics`) file for Google/Apple/Outlook calendars, with an 08:00 alarm on each reminder |

Term dates live in `Data/SchoolCalendar.cs` (currently 2026 only) and **must be verified against the official DBE calendar**; add each new year there.

### Notifications (requires `Authorization: Bearer <token>`)

| Method | Route | Description |
| --- | --- | --- |
| POST | `/api/notifications/devices` | Register the device's push token (`token`, `platform`: `android`/`ios`/`web`) |
| DELETE | `/api/notifications/devices?token=` | Unregister a device token (e.g. on sign-out) |

A background worker sends "Update your report card" on the day schools reopen after each holiday, then every month through the term, to learners with a registered device who haven't yet entered the marks that are due (Term 1 asks for the previous year's Term 4). It sends at most once per learner per day. Delivery goes through `IPushSender`; the default `LoggingPushSender` only logs, so **register a real provider (e.g. Firebase Cloud Messaging) in `Program.cs` before notifications reach phones**.

### Guide

The "After School" YouTube playlist, a short-video series on planning your study and career path, serves as a user guide. The same link is shown in the Swagger UI description.

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/guide` | Get the user-guide YouTube playlist (watch URL, playlist URL, embeddable player URL, and episode list with `videoId` and `thumbnailUrl` per episode) |

Example request body for `POST /api/compare/{courseId}` and `POST /api/aps/calculate`:

```json
[
  { "subject": "Mathematics", "level": 6 },
  { "subject": "English Home Language", "level": 5 }
]
```

Example request body for `POST /api/institutions/{id}/matching-courses`, `POST /api/institutions/matching-courses`, and `POST /api/careers/match`:

```json
{
  "subjects": [
    { "subject": "Mathematics", "level": 6 },
    { "subject": "English Home Language", "level": 5 }
  ]
}
```

## Project structure

```
Controllers/   API controllers (Universities, Subjects, Compare, Aps, Careers, Auth, Learners, Institutions, Assessment, Journey, Guide, TermResults, Calendar, Notifications)
Models/        Domain models and DTOs (University, Course, Subject, Comparison, Learner, Aps, Careers, Institutions, Assessment, Journey)
Data/          Static datasets (universities, courses per institution, NSC subjects, city coordinates, careers, quiz questions) + EF Core DbContext/migrations
Services/      Business logic (course comparison, subject matching, APS calculation, career matching/scoring, geo-distance, auth, JWT, quiz scoring)
```

## License

No license specified.
