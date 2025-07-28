// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.ResourceManager.Consumption;
using AzureMcp.Core.Options;
using AzureMcp.Core.Services.Azure;
using AzureMcp.Core.Services.Azure.Subscription;
using AzureMcp.Core.Services.Azure.Tenant;
using AzureMcp.CostManagement.Models;

namespace AzureMcp.CostManagement.Services;

public class CostManagementService(ISubscriptionService subscriptionService, ITenantService tenantService)
    : BaseAzureService(tenantService), ICostManagementService
{
    private readonly ISubscriptionService _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));

    public async Task<List<BudgetSummary>> ListBudgets(
        string subscription,
        string? budgetName = null,
        int? top = null,
        string? filter = null,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null)
    {
        ValidateRequiredParameters(subscription);

        var subscriptionResource = await _subscriptionService.GetSubscription(subscription, tenant, retryPolicy);
        var budgets = new List<BudgetSummary>();

        try
        {
            // Get consumption budgets collection
            var budgetCollection = subscriptionResource.GetConsumptionBudgets();
            
            // Create filter parameter if provided
            string? actualFilter = null;
            if (!string.IsNullOrEmpty(filter))
            {
                actualFilter = filter;
            }
            else if (!string.IsNullOrEmpty(budgetName))
            {
                actualFilter = $"properties/category eq 'Cost'";
            }

            // Apply the filter if provided
            var budgetPages = actualFilter != null 
                ? budgetCollection.GetAllAsync(filter: actualFilter) 
                : budgetCollection.GetAllAsync();

            await foreach (var budget in budgetPages)
            {
                // If specific budget name is requested, filter by name
                if (!string.IsNullOrEmpty(budgetName) && !budget.Data.Name.Equals(budgetName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var budgetSummary = new BudgetSummary(
                    Id: budget.Data.Id?.ToString() ?? string.Empty,
                    Name: budget.Data.Name ?? string.Empty,
                    Type: budget.Data.ResourceType?.ToString() ?? string.Empty,
                    ETag: budget.Data.ETag?.ToString(),
                    Category: budget.Data.Category?.ToString() ?? string.Empty,
                    Amount: budget.Data.Amount ?? 0,
                    TimeGrain: budget.Data.TimeGrain?.ToString() ?? string.Empty,
                    StartDate: budget.Data.TimePeriod?.StartOn?.DateTime ?? DateTime.MinValue,
                    EndDate: budget.Data.TimePeriod?.EndOn?.DateTime ?? DateTime.MinValue,
                    CurrentSpendAmount: budget.Data.CurrentSpend?.Amount,
                    CurrentSpendUnit: budget.Data.CurrentSpend?.Unit,
                    ForecastSpendAmount: budget.Data.ForecastSpend?.Amount,
                    ForecastSpendUnit: budget.Data.ForecastSpend?.Unit
                );

                budgets.Add(budgetSummary);

                // Apply top limit if specified
                if (top.HasValue && budgets.Count >= top.Value)
                {
                    break;
                }
            }
        }
        catch (Exception ex) when (ex.Message.Contains("NotFound"))
        {
            // Return empty list if no budgets found
            return budgets;
        }

        return budgets;
    }
}
