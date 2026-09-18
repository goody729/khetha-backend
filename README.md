# TertiaryInstitutions API

An ASP.NET Core Web API for exploring South African public universities, their courses, NSC (matric) high school subjects, and checking whether a learner's subject results meet a course's admission requirements.

## Features

- Browse public South African universities, optionally filtered by province
- Browse official NSC high school subjects, optionally filtered by category or designated-subject status
- Compare a learner's NSC subject achievement levels (1-7) against a course's admission requirements

## Tech stack

- .NET 8 / ASP.NET Core Web API
- Swashbuckle (Swagger / OpenAPI)

## Getting started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Run

```bash
dotnet restore
dotnet run
```

Once running, Swagger UI is available at the app's root (or `/swagger`) for browsing and testing the endpoints interactively.

## API endpoints

### Universities

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/universities` | Get all universities, optionally filtered by `province` |
| GET | `/api/universities/{id}` | Get a single university by id |

### Subjects

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/subjects` | Get all NSC subjects, optionally filtered by `category` and/or `designated` |
| GET | `/api/subjects/{id}` | Get a single subject by id |

### Compare

| Method | Route | Description |
| --- | --- | --- |
| POST | `/api/compare/{courseId}` | Compare a learner's subjects/levels against a course's requirements |

Example request body for `POST /api/compare/{courseId}`:

```json
[
  { "subject": "Mathematics", "level": 6 },
  { "subject": "English Home Language", "level": 5 }
]
```

## Project structure

```
Controllers/   API controllers (Universities, Subjects, Compare)
Models/        Domain models (University, Course, Subject, Comparison)
Data/          Static datasets (universities, courses per institution, NSC subjects)
Services/      Business logic (course comparison)
```

## License

No license specified.
