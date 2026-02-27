using System.Text.Json;

namespace Livepasses.Sdk.Tests;

internal static class MockResponses
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public static string ApiResponse<T>(T data) => JsonSerializer.Serialize(new
    {
        success = true,
        data,
        meta = new { traceId = "test-trace-id", timestamp = "2026-02-25T00:00:00Z" }
    }, JsonOptions);

    public static string ApiError(string code, string message, int status = 400) => JsonSerializer.Serialize(new
    {
        success = false,
        error = new { code, message }
    }, JsonOptions);

    public static string PagedResponse<T>(List<T> items, int totalItems, int page = 1, int pageSize = 10) =>
        JsonSerializer.Serialize(new
        {
            success = true,
            items,
            pagination = new
            {
                currentPage = page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                totalItems
            }
        }, JsonOptions);

    public static string PassGenerationSync() => ApiResponse(new
    {
        batchId = "batch-001",
        templateId = "template-001",
        generatedAt = "2026-02-25T10:00:00Z",
        totalPasses = 1,
        isAsyncProcessing = false,
        passes = new[]
        {
            new
            {
                id = "pass-001",
                customerEmail = "john@example.com",
                confirmationCode = "CONF-001",
                platforms = new
                {
                    apple = new { available = true, addToWalletUrl = "https://wallet.apple.com/pass/001", features = new[] { "barcode" } },
                    google = new { available = true, addToWalletUrl = "https://pay.google.com/pass/001", features = new[] { "barcode" } }
                },
                businessData = new { section = "A", row = "12", seat = "5", ticketType = "VIP" },
                status = "Active"
            }
        }
    });

    public static string PassGenerationAsync() => ApiResponse(new
    {
        batchId = "batch-002",
        templateId = "template-001",
        generatedAt = "2026-02-25T10:00:00Z",
        totalPasses = 5,
        isAsyncProcessing = true,
        batchOperation = new
        {
            id = "batch-002",
            status = "Processing",
            totalRecipients = 5,
            passesGenerated = 0,
            generationFailures = 0,
            progressPercentage = 0.0,
            statusPollUrl = "/api/passes/batch/batch-002/status"
        },
        passes = Array.Empty<object>()
    });

    public static string BatchStatusProcessing() => ApiResponse(new
    {
        id = "batch-002",
        status = "Processing",
        totalRecipients = 5,
        passesGenerated = 2,
        passesDelivered = 0,
        generationFailures = 0,
        deliveryFailures = 0,
        progressPercentage = 40.0,
        isCompleted = false,
        isActive = true
    });

    public static string BatchStatusCompleted() => ApiResponse(new
    {
        id = "batch-002",
        status = "Completed",
        totalRecipients = 5,
        passesGenerated = 5,
        passesDelivered = 5,
        generationFailures = 0,
        deliveryFailures = 0,
        progressPercentage = 100.0,
        isCompleted = true,
        isActive = false,
        generatedPasses = new[]
        {
            new { id = "pass-001", passNumber = "PN-001", holderEmail = "john@example.com", holderName = "John Doe", status = "Active", hasApplePass = true, hasGooglePass = true, generatedAt = "2026-02-25T10:00:00Z" },
            new { id = "pass-002", passNumber = "PN-002", holderEmail = "jane@example.com", holderName = "Jane Doe", status = "Active", hasApplePass = true, hasGooglePass = true, generatedAt = "2026-02-25T10:00:01Z" }
        }
    });

    public static string PassLookup() => ApiResponse(new
    {
        passId = "pass-001",
        passNumber = "PN-001",
        templateId = "template-001",
        templateName = "VIP Event",
        templateType = "Event",
        holderName = "John Doe",
        holderEmail = "john@example.com",
        status = "Active",
        isValid = true,
        canBeRedeemed = true,
        isExpired = false,
        generatedAt = "2026-02-25T10:00:00Z"
    });

    public static string PassValidation() => ApiResponse(new
    {
        passId = "pass-001",
        passNumber = "PN-001",
        status = "Active",
        canBeRedeemed = true,
        isExpired = false,
        validationMessage = "Pass is valid and can be redeemed",
        templateType = "Event",
        holderName = "John Doe",
        verificationMethods = new[] { "qr", "nfc" }
    });

    public static string RedemptionResult() => ApiResponse(new
    {
        passId = "pass-001",
        passNumber = "PN-001",
        redeemedAt = "2026-02-25T12:00:00Z",
        redemptionMethod = "manual",
        previousStatus = "Active",
        newStatus = "Redeemed",
        alreadyRedeemed = false
    });

    public static string GlobalPassList() => PagedResponse(
        new List<object>
        {
            new
            {
                id = "pass-001",
                serialNumber = "SN-001",
                platform = "Both",
                status = "Active",
                generatedAt = "2026-02-25T10:00:00Z",
                holderEmail = "john@example.com",
                templateId = "template-001",
                templateName = "VIP Event",
                templateType = "Event"
            }
        },
        totalItems: 1
    );

    public static string BatchStatus() => ApiResponse(new
    {
        id = "batch-001",
        status = "Completed",
        totalRecipients = 5,
        passesGenerated = 5,
        passesDelivered = 5,
        generationFailures = 0,
        deliveryFailures = 0,
        progressPercentage = 100.0,
        isCompleted = true,
        isActive = false
    });
}
