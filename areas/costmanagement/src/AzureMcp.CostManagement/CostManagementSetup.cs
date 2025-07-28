// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AzureMcp.Core.Extensions;
using AzureMcp.CostManagement.Commands.Budgets;
using AzureMcp.CostManagement.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AzureMcp.CostManagement;

public static class CostManagementSetup
{
    public static IServiceCollection AddCostManagement(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<ICostManagementService, CostManagementService>();

        // Register commands
        services.AddMcpCommand<BudgetsListCommand>("costmanagement", "budgets");

        return services;
    }
}
