# Backend Build Checklist

Legend: `[x]` = done, `[ ]` = pending / partial.

---

## 1. API Error Response — RFC 7807 / RFC 9457 (Problem Details)

- [x] Return `application/problem+json` with `type`, `title`, `status`, `detail`, `instance`
- [x] Central exception handling middleware (`GlobalExceptionMiddleware`)
- [x] Map common exceptions to HTTP status codes (400 / 403 / 404 / 500)
- [x] Add `traceId` extension member to every problem response
- [ ] Return `ValidationProblemDetails` with per-field `errors` for model/command validation
- [ ] Consistent handling for 401 (unauthenticated) and 429 (rate limited)
- [ ] Document error schemas in OpenAPI/Swagger

## 2. Distributed Tracing — W3C Trace Context & OpenTelemetry

- [ ] Enable `System.Diagnostics.Activity` / ASP.NET Core tracing
- [ ] Propagate `traceparent` and `tracestate` headers between services
- [ ] Add OpenTelemetry SDK + exporter (OTLP / Jaeger / Tempo)
- [ ] Include `TraceId` and `SpanId` in every structured log event
- [ ] Instrument EF Core, Npgsql, HttpClient, and cache calls
- [ ] Configure sampling strategy

## 3. Idempotency — IETF `Idempotency-Key`

- [ ] Accept `Idempotency-Key` header on mutating endpoints
- [ ] Store request fingerprint + response in Redis/distributed cache
- [ ] Replay stored response for duplicate keys
- [ ] Handle in-progress duplicates (`409 Conflict` / retry hint)
- [ ] Apply to payments and order creation first
- [ ] Define TTL and key scoping (user + endpoint)
- [ ] Add Redis/cache abstraction in Infrastructure

## 4. Pagination Standard

### Offset-based

- [ ] Request model: `pageNumber`, `pageSize`
- [ ] Response schema: `items`, `pageNumber`, `pageSize`, `totalCount`, `totalPages`
- [ ] Implement in `BaseRepository.GetListAsync`
- [ ] Clamp `pageSize` and validate `pageNumber`

### Cursor-based / Keyset

- [x] `PagedRequest` / `PagedResult` cursor fields (`LastCursorTime`, `LastCursorId`, `HasNextPage`)
- [ ] Implement keyset query in repository
- [ ] Stable ordering + opaque cursor encoding
- [ ] Expose for large lists / infinite scroll

## 5. HTTP Caching — RFC 7234 / RFC 9111

- [ ] `Cache-Control` policies (public/private, max-age, no-store)
- [ ] Generate and validate `ETag`
- [ ] Handle `If-None-Match` and return `304 Not Modified`
- [ ] Cache invalidation on writes
- [ ] Output caching / response caching middleware
- [ ] Distributed cache (Redis) for shared cache

## 6. Health Checks

- [ ] `/healthz/live` (liveness)
- [ ] `/healthz/ready` (readiness)
- [ ] Database check (PostgreSQL)
- [ ] Cache check (Redis)
- [ ] External dependency checks
- [ ] Wire into Docker/Kubernetes probes
- [ ] Restrict or secure health endpoints

## 7. Rate Limiting — IETF RateLimit Headers

- [ ] Use ASP.NET Core rate limiting middleware
- [ ] Return `429 Too Many Requests`
- [ ] Return `RateLimit-Limit`, `RateLimit-Remaining`, `RateLimit-Reset`
- [ ] Per-user / per-IP / per-endpoint policies
- [ ] Distributed counters (Redis) for multi-instance deployments
- [ ] Return rate-limit errors as Problem Details

## 8. Resiliency & Fault Handling — Polly

- [ ] Configure `HttpClientFactory`
- [ ] Retry with exponential backoff + jitter
- [ ] Circuit breaker
- [ ] Timeout policy
- [ ] Fallback / degraded response
- [ ] Apply to external service clients
- [ ] Add telemetry for retries and circuit state

## 9. Structured Logging — Message Templates

- [x] Serilog installed and wired (`Serilog.AspNetCore`, `Serilog.Sinks.File`)
- [x] File sink output template with source location and exception
- [x] `LogWithLocation` extension with caller file/member/line
- [ ] Use named message-template properties everywhere (`logger.LogInformation("User {UserId} ...", id)`)
- [ ] Remove string interpolation/concatenation in log calls
- [ ] JSON/compact sink for log aggregation
- [ ] Enrichers: `TraceId`, `SpanId`, `Environment`, `MachineName`
- [ ] Log level configuration per namespace

## 10. Security Headers — OWASP

- [ ] `Content-Security-Policy`
- [ ] `X-Content-Type-Options: nosniff`
- [ ] `X-Frame-Options: DENY`
- [ ] `Strict-Transport-Security` (HSTS)
- [ ] `Referrer-Policy`
- [ ] `Permissions-Policy`
- [ ] Remove server/framework disclosure headers

---

## Architecture & Foundation (current state)

### Done

- [x] Upgrade all projects to .NET 10
- [x] Clean Architecture layers: Domain / Application / Infrastructure / Presentation
- [x] EF Core 10 + Npgsql installed in Infrastructure
- [x] `ApplicationDbContext` with `ApplyConfigurationsFromAssembly`
- [x] One `IEntityTypeConfiguration` per table in `Infrastructure/Persistence/Configurations`
- [x] Base configs: `AuditableEntityConfiguration<T>` and `SoftDeleteEntityConfiguration<T>`
- [x] Soft-delete query filter, audit columns, max lengths, indexes, FKs
- [x] EF Core migrations + `DatabaseInitializer` runs them at startup
- [x] Dapper and FluentMigrator removed
- [x] Domain abstractions: `BaseEntity`, `GuidEntity`, soft delete, `AggregateRoot`, domain events
- [x] Repository abstractions (`IBaseRepository<T,TKey>`, `IUnitOfWork`, `ISpecification<T>`)
- [x] Generic `BaseRepository` basic CRUD + `UnitOfWork` transaction handling
- [x] CQRS abstractions (`ICommand`, `ICommand<T>`, `IQuery<T>`, handlers)
- [x] MediatR registered in Application
- [x] Serilog + global exception middleware
- [x] Dockerfile present
- [x] Paging result/request models (cursor fields)

### Pending

- [ ] Wire Application DI into Presentation (`AddDependencyInjection` is internal and never called)
- [ ] Register repositories in DI (`BaseRepository` is not registered)
- [ ] Complete `BaseRepository`: paged queries, bulk insert/update/delete, streaming
- [ ] Pipeline behaviors (transaction, logging, validation, performance)
- [ ] FluentValidation + validators
- [ ] Input validation at API boundaries
- [ ] API versioning
- [ ] Swagger/OpenAPI enrichment (auth, problem details, examples)
- [ ] Authentication & authorization (JWT, policies, roles)
- [ ] Mapping strategy (manual or Mapster/AutoMapper)
- [ ] Caching abstraction
- [ ] Background jobs / outbox processor for `outbox_messages`
- [ ] Inbox deduplication for `inbox_messages`
- [ ] Unit tests, integration tests, architecture tests
- [ ] CI/CD pipeline
- [ ] `Directory.Build.props` with shared analyzer/settings
