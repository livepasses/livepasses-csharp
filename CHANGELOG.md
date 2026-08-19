# Changelog

All notable changes to the Livepasses C# SDK will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Pass operations the API shipped since June: `Passes.RedeemGiftCardAsync`, `Passes.MembershipCheckInAsync`, `Passes.StampAsync`, `Passes.UnstampAsync`, `Passes.RedeemByScanAsync`.
  `stamp` and `unstamp` send an empty JSON body rather than none: both endpoints bind a request
  DTO, and a bodyless POST carries no `Content-Type`, which the API answers with `415`.

### Removed
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
