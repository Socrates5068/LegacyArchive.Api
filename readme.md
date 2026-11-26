# Legacy Archive API

A REST API to digitize, preserve and showcase the life's work of an artist: more than 30 years of comics, paintings and books, most of which still exist only on paper.

The project has two goals:

1. **Archive**: store high-resolution scans of every piece, with its metadata, safely and in order.
2. **Showcase**: power a public gallery (React, in progress) where new work can also be offered online.

It is also a learning project, built step by step to practice a production-style .NET stack.

> **Status: work in progress.** The API is functional for local use. Authentication is not implemented yet, so **do not expose it to the internet** (see [Known limitations](#known-limitations)).

---

## Tech stack

| Layer | Technology |
|---|---|
| Language / runtime | C# 13, .NET 9 |
| Web API | ASP.NET Core Web API (controllers) |
| Data access | Entity Framework Core 9 + Npgsql |
| Database | PostgreSQL 17 (Docker) |
| Validation | FluentValidation |
| Image processing | SixLabors.ImageSharp |
| API testing | Postman (collection versioned in this repo) |
| Frontend | React *(planned)* |
| Deployment | Docker on a self-hosted server *(planned)* |

---

## Architecture

The solution follows **Clean Architecture**. Dependencies point inwards: the domain knows nothing about the database, the web or the file system.

```
LegacyArchive.Api              → Controllers, middleware, composition root (Program.cs)
        │
        ▼
LegacyArchive.Infrastructure   → EF Core, PostgreSQL, repositories, local file storage, ImageSharp
        │
        ▼
LegacyArchive.Application      → Services, DTOs, validators, mappings, interfaces (IFileStorage, IImageProcessor, ...)
        │
        ▼
LegacyArchive.Domain           → Entities: Artwork, Category, MediaFile
```

- **Application** defines *what* it needs through interfaces (`IArtworkRepository`, `IFileStorage`, `IImageProcessor`); **Infrastructure** decides *how* (Dependency Inversion).
- Each layer registers its own services through an extension method (`AddApplication()`, `AddInfrastructure()`), so `Program.cs` does not know about `DbContext`, repositories or PostgreSQL.
- Application does not depend on ASP.NET types: the controller converts `IFormFile` into an `UploadMediaRequest` with a plain `Stream`.

### Domain model

```
Category 1 ──── * Artwork 1 ──── * MediaFile
         (Restrict)        (Cascade)
```

- Deleting a **category** that still has artworks is rejected (`409 Conflict`) both in the service and by the database (`ON DELETE RESTRICT`), so no artwork can be lost by accident.
- Deleting an **artwork** deletes its media files (`ON DELETE CASCADE`) and then removes its folder from disk.

---

## Features

- CRUD for **categories** and **artworks**.
- **Offset pagination** for artworks, with a deterministic order (`CreatedAt DESC, Id`).
- **Validation** with FluentValidation and shared rules; errors returned as `ValidationProblemDetails`.
- Centralized **error handling** middleware returning `ProblemDetails`; unexpected errors are logged and never leak internal details.
- **Media upload** (scans) with streaming to disk, extension whitelist and size limit (100 MB).
- **Real content verification**: files are decoded with ImageSharp, so a renamed PDF or executable is rejected.
- Protection against **path traversal** and **decompression bombs** (images over 150 megapixels are rejected before decoding).
- Automatic generation of a **thumbnail** (400 px) and a **preview** (1600 px) in WebP, with EXIF auto-orientation.
- Media served with **immutable HTTP caching** and **range requests** (resumable downloads of originals).
- **Reordering** of pages (e.g. the pages of a comic) with a single idempotent request.
- **Cover thumbnail** for each artwork, loaded with a filtered include (no N+1 queries).

---

## Getting started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- EF Core CLI tools:

  ```powershell
  dotnet tool install --global dotnet-ef --version 9.0.*
  ```

### 1. Start PostgreSQL

```powershell
cd LegacyArchive.Api
copy .env.example .env      # then edit .env and set a real password
docker compose up -d
```

### 2. Configure the connection string (User Secrets)

The connection string is never committed. Store it with User Secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=legacyarchive;Username=postgres;Password=<your-password>" --project LegacyArchive.Api
```

### 3. Configure the media storage folder

In `LegacyArchive.Api/appsettings.Development.json`, choose a folder **outside the repository**:

```json
"FileStorage": {
  "RootPath": "D:\\LegacyArchiveStorage"
}
```

### 4. Apply the migrations

From the solution folder:

```powershell
dotnet ef database update --project Infrastructure --startup-project LegacyArchive.Api
```

### 5. Run the API

```powershell
dotnet run --project LegacyArchive.Api --launch-profile https
```

The API listens on `https://localhost:7029` and `http://localhost:5065`. In Development, the OpenAPI document is available at `/openapi/v1.json`.

---

## Database migrations

All commands are run from the solution folder.

```powershell
# Create a migration (always read the generated Up() and Down() before applying it)
dotnet ef migrations add <Name> --project Infrastructure --startup-project LegacyArchive.Api

# Apply pending migrations
dotnet ef database update --project Infrastructure --startup-project LegacyArchive.Api

# Remove the last migration (only if it has NOT been applied yet)
dotnet ef migrations remove --project Infrastructure --startup-project LegacyArchive.Api

# List migrations and see which ones are pending
dotnet ef migrations list --project Infrastructure --startup-project LegacyArchive.Api

# Roll the database back to a previous migration
dotnet ef database update <PreviousMigrationName> --project Infrastructure --startup-project LegacyArchive.Api
```

---

## API endpoints

### Categories

| Method | Route | Description | Success |
|---|---|---|---|
| `GET` | `/api/categories` | List all categories | `200` |
| `GET` | `/api/categories/{id}` | Get a category | `200` |
| `POST` | `/api/categories` | Create a category | `201` |
| `PUT` | `/api/categories/{id}` | Rename a category | `200` |
| `DELETE` | `/api/categories/{id}` | Delete a category (only if it has no artworks) | `204` |

### Artworks

| Method | Route | Description | Success |
|---|---|---|---|
| `GET` | `/api/artworks?page=1&pageSize=20` | Paged list (max `pageSize`: 100), with cover thumbnail | `200` |
| `GET` | `/api/artworks/{id}` | Get an artwork | `200` |
| `POST` | `/api/artworks` | Create an artwork | `201` |
| `PUT` | `/api/artworks/{id}` | Update an artwork | `200` |
| `DELETE` | `/api/artworks/{id}` | Delete an artwork, its media and its files | `204` |

### Media (scans of an artwork)

| Method | Route | Description | Success |
|---|---|---|---|
| `GET` | `/api/artworks/{artworkId}/media` | List the media of an artwork, ordered by page | `200` |
| `POST` | `/api/artworks/{artworkId}/media` | Upload a scan (`multipart/form-data`, field `file`) | `201` |
| `GET` | `/api/artworks/{artworkId}/media/{mediaId}/{variant}` | Get the image: `thumbnail`, `preview` or `original` | `200` / `206` |
| `PUT` | `/api/artworks/{artworkId}/media/order` | Reorder pages: `{ "mediaIds": [...] }` with **all** media ids | `200` |
| `DELETE` | `/api/artworks/{artworkId}/media/{mediaId}` | Delete a scan and its files | `204` |

Accepted formats: `.jpg`, `.jpeg`, `.png`, `.tif`, `.tiff`, `.webp` (max. 100 MB).

### Error responses

| Status | When |
|---|---|
| `400 Bad Request` | Validation errors, invalid image, invalid page order |
| `404 Not Found` | Category, artwork or media not found |
| `409 Conflict` | Duplicate category name, or category still in use |
| `500 Internal Server Error` | Unexpected error (details are logged, never returned) |

---

## Media storage

Files live on disk; the database only stores metadata and relative paths.

```
{RootPath}/
└── artworks/
    └── {artworkId}/
        └── {mediaId}/
            ├── original.tif      ← untouched archival file
            ├── preview.webp      ← 1600 px, full-screen viewing
            └── thumbnail.webp    ← 400 px, gallery grids
```

- One folder per media file and one per artwork: deleting a scan or an artwork is a single folder deletion, and the folder path can always be derived from the ids.
- Stored file names are generated by the server; the user's file name is kept only as metadata.
- Each `mediaId` is unique and its files never change, so they are served with `Cache-Control: public, max-age=31536000, immutable`.

---

## Testing with Postman

The Postman collection is versioned in [`postman/`](postman/) and synced by a GitHub Action.

Media tests use files in `.postman/test-files/`, which are **not committed** (some are larger than 100 MB). Generate them with PowerShell inside that folder:

```powershell
# Copy any JPG into this folder as scan.jpg, then:
Copy-Item scan.jpg SCAN_UPPER.JPG              # upper-case extension
New-Item empty.jpg -ItemType File              # 0 bytes
"hello" | Out-File notes.pdf                   # extension not allowed
Copy-Item notes.pdf fake.jpg                   # not a real image
fsutil file createnew big.jpg 105000000        # ~100.1 MB: rejected by the validator
fsutil file createnew huge.jpg 125000000       # ~120 MB: rejected by the request size limit
```

---

## Design decisions

| Decision | Why |
|---|---|
| Files on disk, metadata in PostgreSQL | Keeps the database and its backups small and fast. |
| Delete from the database first, then from disk | If disk deletion fails, the result is orphaned files (recoverable, logged) instead of database rows pointing to missing scans (data loss). |
| Compensating action on failed uploads | Disk and database cannot share a transaction; if the insert fails, the media folder is removed. |
| Three image variants | Browsers cannot display TIFF, and serving 50 MB originals in a gallery is not viable. |
| `Restrict` for Category → Artwork, `Cascade` for Artwork → MediaFile | An artwork has value on its own; a scan does not exist without its artwork. |
| Offset pagination | Allows jumping to any page; enough for thousands of artworks. |
| Non-unique `(ArtworkId, SortOrder)` index | Speeds up ordering and `MAX(SortOrder)`; a unique index would block swapping pages. |
| Validation in two layers | FluentValidation checks the shape of a request; services check consistency with the data (e.g. existing ids). |

---

## Known limitations

These are known and accepted for the current stage of the project:

1. **No authentication or authorization yet.** Every endpoint is public, including delete operations and downloads of original scans. The API must not be exposed to the internet until this is implemented.
2. **Concurrent uploads to the same artwork can produce duplicate page numbers.** `SortOrder` is calculated as `MAX + 1` without a lock (a race condition). The planned frontend will upload files sequentially; duplicates can be fixed with the reorder endpoint.
3. **Orphaned files are not cleaned automatically.** If a folder cannot be deleted from disk, a warning is logged, but there is no background job yet that removes folders without a matching database row.
4. **Category name uniqueness is checked only in the service.** Two simultaneous requests could create the same name; a unique index in the database would close this gap.
5. **Image processing runs inside the upload request.** Large TIFF scans take several seconds and a significant amount of memory (about 4 bytes per pixel while decoding). A background queue would make uploads faster and limit memory usage.
6. **Single local storage, no automated backups yet.** The storage is behind `IFileStorage`, so it can be moved to MinIO or S3 later, but backups of the database and the scans are still manual.
7. **`CancellationToken` is only propagated in the media endpoints.** Category and artwork endpoints do not stop their database work when the client disconnects.
8. **The request size limit error is generated by ASP.NET Core** and reports the limit in bytes. Friendly size validation will be done client-side in the frontend.

---

## Roadmap

- [x] CRUD for categories and artworks
- [x] Validation, error handling and pagination
- [x] Media upload, image variants, serving, reordering and deletion
- [ ] Authentication and authorization (ASP.NET Core Identity + JWT); private originals
- [ ] React frontend: public gallery and admin panel
- [ ] Automated tests (unit and integration with Testcontainers)
- [ ] Dockerfile for the API and full `docker compose` setup
- [ ] Deployment on the self-hosted server (reverse proxy, HTTPS, automated backups)
- [ ] CI with GitHub Actions (build and tests)
