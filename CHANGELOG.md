# Changelog

All notable changes to the Livepasses C# SDK will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
