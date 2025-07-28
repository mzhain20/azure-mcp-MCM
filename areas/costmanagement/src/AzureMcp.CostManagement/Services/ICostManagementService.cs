// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AzureMcp.Core.Options;
using AzureMcp.CostManagement.Models;

namespace AzureMcp.CostManagement.Services;

public interface ICostManagementService
{
    Task<List<BudgetSummary>> ListBudgets(
        string subscription,
        string? budgetName = null,
        int? top = null,
        string? filter = null,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null);
}
