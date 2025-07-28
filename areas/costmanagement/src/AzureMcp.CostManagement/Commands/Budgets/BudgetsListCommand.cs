// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AzureMcp.Core.Commands;
using AzureMcp.Core.Services.Telemetry;
using AzureMcp.CostManagement.Commands;
using AzureMcp.CostManagement.Models;
using AzureMcp.CostManagement.Options;
using AzureMcp.CostManagement.Options.Budgets;
using AzureMcp.CostManagement.Services;
using Microsoft.Extensions.Logging;

namespace AzureMcp.CostManagement.Commands.Budgets;

public sealed class BudgetsListCommand(ILogger<BudgetsListCommand> logger) 
    : BaseCostManagementCommand<BudgetsListOptions>
{
    private const string CommandTitle = "List Cost Management Budgets";
    private readonly ILogger<BudgetsListCommand> _logger = logger;

    private readonly Option<string> _budgetNameOption = CostManagementOptionDefinitions.BudgetNameOption;
    private readonly Option<int> _topOption = CostManagementOptionDefinitions.TopCount;
    private readonly Option<string> _filterOption = CostManagementOptionDefinitions.Filter;

    public override string Name => "list";

    public override string Description =>
        """
        List all Cost Management budgets in a subscription. This command retrieves budgets configured
        for cost tracking and alerts within the specified subscription. You can optionally filter by
        budget name, apply OData filters, or limit the number of results returned.
        
        Returns detailed budget information including:
        - Budget name, category, and amount
        - Time grain (Monthly, Quarterly, etc.)
        - Start and end dates of the budget period  
        - Current spend amount and currency
        - Forecast spend (if available)
        
        Required options:
        - subscription: Azure subscription ID or name
        
        Optional options:
        - budget-name: Filter to a specific budget by name
        - top: Maximum number of budgets to return (default: 100)
        - filter: OData filter expression (e.g., "properties/category eq 'Cost'")
        """;

    public override string Title => CommandTitle;

    public override ToolMetadata Metadata => new() { Destructive = false, ReadOnly = true };

    protected override void RegisterOptions(Command command)
    {
        base.RegisterOptions(command);
        command.AddOption(_budgetNameOption);
        command.AddOption(_topOption);
        command.AddOption(_filterOption);
    }

    protected override BudgetsListOptions BindOptions(ParseResult parseResult)
    {
        var options = base.BindOptions(parseResult);
        options.BudgetName = parseResult.GetValueForOption(_budgetNameOption);
        options.Top = parseResult.GetValueForOption(_topOption);
        options.Filter = parseResult.GetValueForOption(_filterOption);
        return options;
    }

    public override async Task<CommandResponse> ExecuteAsync(CommandContext context, ParseResult parseResult)
    {
        var options = BindOptions(parseResult);

        try
        {
            // Required validation step
            if (!Validate(parseResult.CommandResult, context.Response).IsValid)
            {
                return context.Response;
            }

            context.Activity?.WithSubscriptionTag(options);

            // Get the cost management service from DI
            var service = context.GetService<ICostManagementService>();

            // Call the service method
            var budgets = await service.ListBudgets(
                options.Subscription!,
                options.BudgetName,
                options.Top,
                options.Filter,
                options.Tenant,
                options.RetryPolicy);

            // Set results if any were returned
            context.Response.Results = budgets?.Count > 0 ?
                ResponseResult.Create(
                    new BudgetsListCommandResult(budgets),
                    CostManagementJsonContext.Default.BudgetsListCommandResult) :
                null;
        }
        catch (Exception ex)
        {
            // Log error with all relevant context
            _logger.LogError(ex,
                "Error listing budgets. Subscription: {Subscription}, BudgetName: {BudgetName}, Top: {Top}, Filter: {Filter}, Options: {@Options}",
                options.Subscription, options.BudgetName, options.Top, options.Filter, options);
            HandleException(context, ex);
        }

        return context.Response;
    }

    // Implementation-specific error handling
    protected override string GetErrorMessage(Exception ex) => ex switch
    {
        ArgumentException argEx when argEx.Message.Contains("subscription") =>
            "Invalid subscription. Verify the subscription ID or name is correct and you have access.",
        Azure.RequestFailedException reqEx when reqEx.Status == 404 =>
            "No budgets found or subscription not found. Verify the subscription exists and you have access.",
        Azure.RequestFailedException reqEx when reqEx.Status == 403 =>
            $"Authorization failed accessing budgets. Ensure you have Cost Management Reader role. Details: {reqEx.Message}",
        Azure.RequestFailedException reqEx => reqEx.Message,
        _ => base.GetErrorMessage(ex)
    };

    protected override int GetStatusCode(Exception ex) => ex switch
    {
        Azure.RequestFailedException reqEx => reqEx.Status,
        _ => base.GetStatusCode(ex)
    };

    // Strongly-typed result record
    internal record BudgetsListCommandResult(List<BudgetSummary> Budgets);
}
