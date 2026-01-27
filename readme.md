# TeachHub API

[English](readme.md) | [Русский](readme.RU.md)

RESTful API for managing courses, lessons, and user interactions on a learning platform. Built with C# using ASP.NET 9 following Clean Architecture principles.

## Project Overview

TeachHub API provides a complete solution for creating, managing, and distributing educational content. The system supports a hierarchical course structure (Courses → Chapters → Lessons), reviews & ratings, subscription system, and favorites. All entity IDs are protected using HashID encryption to hide the database structure.

## Technology Stack

**Backend:**
- **ASP.NET 9** — API framework
- **Clean Architecture** — layered responsibility separation
- **Entity Framework Core** — ORM (DB-first approach)
- **Ardalis Specifications** — typed query pattern + base repository

**Database:**
- **PostgreSQL** — relational database
- **Full-text search** — TSVECTOR used for course search
- **Triggers** — automatic course rating calculation, reaction counters update

**Security & Encryption:**
- **JWT (JSON Web Tokens)** — authentication & authorization
- **Argon2** — password hashing with strong cryptographic protection
- **HashID** — ID obfuscation preventing sequence guessing and DB structure leaks

**Cloud Storage:**
- **Backblaze B2** — storage for course icons and lesson files
- **AWS SDK** — compatibility layer for object storage operations

**Performance:**
- **In-Memory Caching** — ASP.NET built-in cache for frequently accessed data

## Data Structure

The system is built around the following core entities:

### Main Tables

| Table              | Purpose                                                                 |
|--------------------|-------------------------------------------------------------------------|
| `users`            | User data (Email, Password, Name)                                       |
| `profiles`         | User profiles (Bio, Avatar, Social links)                               |
| `roles`            | User roles (User, Admin)                                                |
| `usersroles`       | Many-to-many relation between users and roles                           |
| `courses`          | Core course information (Title, Description, Rating)                    |
| `categories`       | Course categories with hierarchy support                                |
| `course_categories`| Many-to-many relation between courses and categories                    |
| `chapter`          | Course chapters                                                         |
| `lesson`           | Lessons inside chapters                                                 |
| `lessonfiles`      | Files attached to lessons                                               |
| `reviews`          | Course reviews and ratings                                              |
| `reviewreaction`   | Like/Dislike reactions on reviews                                       |
| `subscription`     | User-to-user following/subscriptions                                    |
| `favorit`          | User's favorite courses                                                 |

### Important Features

- **Full-text search**: `searchvector` column (TSVECTOR) in `courses` table for fast search across titles, descriptions, and categories
- **Cascading deletes**: Deleting a course removes chapters, lessons, files, and reviews automatically
- **Database triggers**: Auto-recalculate course rating on review change
- **Reaction counters**: Tracks Like/Dislike count per review

## API Endpoints

### Authentication

- `POST /api/auth/login`    — Login + receive JWT token
- `POST /api/auth/refresh`  — Refresh access token

### Courses

- `GET    /api/courses`            — List all courses (sorting + pagination)
- `POST   /api/courses`            — Create course
- `PATCH  /api/courses`            — Update course
- `GET    /api/courses/{courseid}` — Get single course
- `GET    /api/courses/my`         — Get current user's courses
- `DELETE /api/courses/{id}`       — Delete course
- `PUT    /api/courses/icon`       — Upload/update course icon

### Categories

- `GET /api/categories` — List categories (search + pagination supported)

### Chapters

- `GET    /api/chapters/{courseId}`     — Get all chapters of a course
- `POST   /api/chapters`                — Create chapter
- `PATCH  /api/chapters`                — Update chapter
- `DELETE /api/chapters/{chapterid}`    — Delete chapter
- `DELETE /api/chapters/admin/{chapterid}/{userid}` — Admin delete chapter

### Lessons

- `GET    /api/lessons/{chapterid}`       — Get lessons in chapter
- `GET    /api/lessons/my/{chapterid}`    — Get lessons (author edit mode)
- `POST   /api/lessons`                   — Create lesson
- `PATCH  /api/lessons`                   — Update lesson
- `DELETE /api/lessons/{lessonid}`        — Delete lesson
- `PATCH  /api/lessons/{lessonid}/visibility` — Change lesson visibility
- `DELETE /api/lessons/admin/{lessonid}`  — Admin delete lesson

### Lesson Files

- `GET    /api/lessonsfiles/{lessonid}`  — Get lesson files
- `POST   /api/lessonsfiles/files`       — Upload file to lesson
- `DELETE /api/lessonsfiles/{fileid}`    — Delete file

### Reviews & Ratings

- `GET    /api/reviews`                — All reviews
- `GET    /api/reviews/{courseid}`     — Reviews for specific course
- `POST   /api/reviews`                — Create review
- `PATCH  /api/reviews/{reviewid}`     — Update review text/rating
- `DELETE /api/reviews/{reviewid}`     — Delete review

### Review Reactions

- `PUT /api/reviews/reactions` — Add/change Like/Dislike reaction

### Subscriptions / Following

- `GET    /api/user/follows`                    — Who current user follows
- `GET    /api/user/follows/followers`          — Current user's followers
- `POST   /api/user/follows/{userid}`           — Follow user
- `DELETE /api/user/follows/{following}`        — Unfollow user
- `GET    /api/user/follows/{userid}`           — Who specific user follows
- `GET    /api/user/follows/{userid}/followers` — Specific user's followers

### Favorites

- `GET    /api/favorites`         — Current user's favorite courses
- `POST   /api/favorites/{courseid}`   — Add course to favorites
- `DELETE /api/favorites/{courseid}`   — Remove from favorites

### Users

- `POST   /api/users`                    — Register new user
- `GET    /api/users/exists`             — Check if email exists
- `DELETE /api/users/remove`             — Soft-delete own account
- `DELETE /api/users/admin/{id}/soft`    — Admin soft-delete user
- `DELETE /api/users/admin/{id}/hard`    — Admin hard-delete user

## Key Features

### ID Protection

All IDs in requests/responses are HashID-encoded. Real database IDs remain hidden.

Example:
GET /api/courses/jR8vWd   // jR8vWd = encrypted ID (e.g. 1, 2, 3…)


### Full-text Search

Fast PostgreSQL TSVECTOR-based search across course titles, descriptions, and categories.

### Caching

ASP.NET In-Memory Cache used for:
- Category list
- Popular courses
- User profile data

### Cloud Storage

Course icons and lesson materials stored in Backblaze B2 using AWS S3-compatible SDK. Supports local/cloud switching via `cloudstore` flag.

### Hierarchical Structure
```
Course
├── Chapter 1
│   ├── Lesson 1
│   │   └── LessonFiles
│   └── Lesson 2
└── Chapter 2
└── Lesson 3
```

Categories also support hierarchy via `parentid`.

### Rating System

Course rating (0–10) auto-recalculated by DB trigger on every review change.

## Security

- **Authentication**: Signed JWT tokens
- **Authorization**: Role-based (User / Admin)
- **Password hashing**: Argon2 + salt
- **ID obfuscation**: HashID
- **Soft deletes**: Users and content marked deleted instead of hard removal

## Architecture

Clean Architecture with clear layer separation:

- **Domain** → Entities & business rules
- **Application** → Use cases, DTOs
- **Infrastructure** → Data access, external services (Backblaze, SMTP…)
- **Presentation** → API Controllers

Uses **Ardalis Specification Pattern** for reusable, typed queries.

## Development Tools

- **Visual Studio 2022** or **VS Code**
- **Entity Framework Core** migrations
- **Swagger / OpenAPI** documentation
- **PostgreSQL** admin tools

## Project Setup (Docker)

Docker Compose is used for quick deployment of the local development environment. This will automatically spin up the PostgreSQL database and apply the initial data schema from `init.sql`.

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for Windows/Mac) or Docker Engine with the Compose plugin (for Linux).

### Quick Start

1. Clone the repository.
2. Open a terminal in the project root folder and run the command depending on your OS:

**Linux / macOS (Bash):**
bash docker-compose up -d

**Windows (Command Prompt / PowerShell):**
docker compose up -d


## Notes

- DB-first approach: database created first, then EF models generated
- Pagination parameters are optional (sensible defaults used)
- Example IDs (jR8vWd) are HashID values
- Protected endpoints require `Authorization: Bearer {token}` header


