# Changelog

All notable changes to the Livepasses C# SDK will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.3.0] - 2026-09-28

### Added
- `WebhookEventType.PassInstalled` and `WebhookEventType.PassRemoved` webhook events: the holder saved a pass to a wallet (first device), or removed its last copy. Holder-initiated removals only.
- Pass operations the API shipped since June: `Passes.RedeemGiftCardAsync`, `Passes.MembershipCheckInAsync`, `Passes.StampAsync`, `Passes.UnstampAsync`, `Passes.RedeemByScanAsync`.
  `stamp` and `unstamp` send an empty JSON body rather than none: both endpoints bind a request
  DTO, and a bodyless POST carries no `Content-Type`, which the API answers with `415`.
- `ValidationException.Fields` — a field name → validation message list, populated only when the
  API's `error.code` is `VALIDATION_ERROR`.

### Changed
- **BREAKING:** `UpdatePassParams` now matches the API: `UpdatedFields` (the field changes, keyed
  by field name such as `validUntil`, `memberTier` or `points`), `Reason`, `MessageHeader`,
  `MessageBody` and `Notify`. Its old `BusinessData` and `BusinessContext` were never read by the
  API, so `Passes.UpdateAsync` changed nothing while answering success; the API now refuses them
  with a `400`. Send a non-empty `UpdatedFields`, a non-empty `MessageBody`, or both.
- **BREAKING:** The API now answers every refusal with a real HTTP status
  (`400`/`403`/`404`/`409`/`422`/`429`/`500`/`502`/`503`), always with the
  `{success:false,data:null,error:{...}}` envelope, instead of `200` with `success:false` for
  most refusals. The SDK now raises a typed exception from **any** non-2xx status or a parsed
  envelope with `success:false` — on every call path, including `GetPagedAsync`/paged list
  calls — not only when the body said `success:false`. An empty body (a challenge `401`) or a
  non-JSON body (a proxy error page) still raises a typed exception, built from the HTTP status
  alone; classification checks a `401`/`403` status first, then `error.code`, then the remaining
  statuses. If your
  code depended on a `200` response for a refusal, or on catching only exceptions built from a
  JSON `error` payload, it will now see a typed exception it didn't before.
- Automatic 5xx retries now apply **only to idempotent HTTP methods** (`GET`, `HEAD`, `PUT`,
  `DELETE`). A `POST` that hits a `5xx` is no longer retried — no SDK request carries an
  `Idempotency-Key`, so retrying a `POST` risked double-executing it. `429` retries are
  unaffected.
- Every typed exception's `Status` is now the response's real HTTP status instead of a fixed number per class — `QuotaExceededException.Status` is `422`, not `403`. A `401` is always an `AuthenticationException` and a `403` always a `ForbiddenException`, whatever the code: a `403` carrying `UNAUTHORIZED` is a permission refusal, not a bad API key. After that the error code decides, then the status (`400`/`404`/`422`/`429`); a `409` with no mapped code is a plain `LivepassesException`. Each exception gained a constructor overload that takes the status; the existing constructors are unchanged and delegate with the old fixed status, so code that constructs these exceptions still compiles.
- **Upgrade recommended.** Older SDK versions retry a failed request on any `5xx`, including a `POST`. The API now answers server-side failures with a real `500`, `502` or `503` where it used to answer `200`, so an older SDK can send the same `POST` twice — for example, generate the same passes twice. This version retries a `5xx` only for `GET`, `HEAD`, `PUT` and `DELETE`.
- `RateLimitException.RetryAfter` is now actually populated from the `Retry-After` header; it was
  previously always `null` because the header wasn't threaded through error construction.

### Removed
- **BREAKING:** `Notes` from `RedeemPassParams`, `CheckInParams` and `RedeemCouponParams`. The API
  never read it, and now refuses it with a `400`. Record free-form data with the new `Metadata`
  (a string → string dictionary) instead.
- **BREAKING:** `PassExpired`, `PassCheckedIn`, `BatchCompleted` and `BatchFailed` from `WebhookEventType`. The API rejects all four with a `400`, so no
  subscription using them could ever have worked.

### Fixed
- `RedeemAsync` documented itself as generic redemption. It is single-use only: multi-use
  passes are refused with a `422`. The XML doc now says so and names `StampAsync`,
  `MembershipCheckInAsync`, `RedeemCouponAsync` and `RedeemGiftCardAsync` as the operations
  to use instead.
- Webhook event catalogue now mirrors the server allow-list, adding `loyalty.transacted`,
  `coupon.applied`, the five `transfer.*` events and the `*` wildcard. The runnable webhook
  example no longer subscribes to events the API rejects.
- The template example and README put invented flat keys (`passType`, `hasSeating`,
  `hasGateInfo`, `supportedPlatforms`) in `BusinessFeatures`, which the API now refuses with a
  `400`. They now send a nested `event` block (and `branding`).

## [0.2.0] - 2026-05-23

### Changed
- **BREAKING:** `Passes.BulkUpdateAsync(BulkUpdatePassesParams)` replaced by `Passes.PushTemplateAsync(string templateId, PushTemplatePassesParams)`, targeting `POST /api/passes/template/{templateId}/push` with `{ updatedFields, reason }`. `BulkUpdatePassesParams` renamed to `PushTemplatePassesParams`.

## [0.1.0] - 2026-02-27

### Added

- Initial release of the Livepasses C# SDK
- `LivepassesClient` with configurable base URL, timeout, and retry settings
- **Passes resource**: `GenerateAsync`, `GenerateAndWaitAsync`, `ListAsync`, `ListAutoPaginateAsync`, `LookupAsync`, `ValidateAsync`, `UpdateAsync`, `BulkUpdateAsync`, `RedeemAsync`, `CheckInAsync`, `RedeemCouponAsync`, `LoyaltyTransactAsync`, `GetBatchStatusAsync`
- **Templates resource**: `ListAsync`, `GetAsync`, `CreateAsync`, `UpdateAsync`, `ActivateAsync`, `DeactivateAsync`
- **Webhooks resource**: `CreateAsync`, `ListAsync`, `DeleteAsync`
- Exception hierarchy: `AuthenticationException`, `ValidationException`, `ForbiddenException`, `NotFoundException`, `RateLimitException`, `QuotaExceededException`, `BusinessRuleException`
- `ApiErrorCodes` constants with 27+ error code values
- Automatic retry with exponential backoff for 429 and 5xx responses
- Auto-pagination via `IAsyncEnumerable<T>` (`ListAutoPaginateAsync`)
- `CancellationToken` support on async polling and pagination methods
- Full nullable reference type annotations
- Zero runtime dependencies, targets .NET 8.0

[0.1.0]: https://github.com/livepasses/livepasses-csharp/releases/tag/csharp-v0.1.0
