// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace AzureMcp.CostManagement.Options.Budgets;

public class BudgetsListOptions : BaseCostManagementOptions
{
    [JsonPropertyName(CostManagementOptionDefinitions.BudgetName)]
    public string? BudgetName { get; set; }

    [JsonPropertyName(CostManagementOptionDefinitions.TopCountName)]
    public int? Top { get; set; }

    [JsonPropertyName(CostManagementOptionDefinitions.FilterName)]
    public string? Filter { get; set; }
}
