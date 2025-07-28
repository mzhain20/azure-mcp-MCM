// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace AzureMcp.CostManagement.Models;

public record BudgetSummary(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("eTag")] string? ETag,
    [property: JsonPropertyName("category")] string Category,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("timeGrain")] string TimeGrain,
    [property: JsonPropertyName("startDate")] DateTime StartDate,
    [property: JsonPropertyName("endDate")] DateTime EndDate,
    [property: JsonPropertyName("currentSpendAmount")] decimal? CurrentSpendAmount,
    [property: JsonPropertyName("currentSpendUnit")] string? CurrentSpendUnit,
    [property: JsonPropertyName("forecastSpendAmount")] decimal? ForecastSpendAmount,
    [property: JsonPropertyName("forecastSpendUnit")] string? ForecastSpendUnit
);
