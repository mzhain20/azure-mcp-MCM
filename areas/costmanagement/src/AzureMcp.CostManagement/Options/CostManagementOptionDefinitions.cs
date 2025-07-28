// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace AzureMcp.CostManagement.Options;

public static class CostManagementOptionDefinitions
{
    public const string BudgetName = "budget-name";
    public const string TopCountName = "top";
    public const string FilterName = "filter";

    public static readonly Option<string> BudgetNameOption = new(
        $"--{BudgetName}",
        "The name of the budget to retrieve."
    )
    {
        IsRequired = false
    };

    public static readonly Option<int> TopCount = new(
        $"--{TopCountName}",
        () => 100,
        "The maximum number of budgets to return. Defaults to 100."
    )
    {
        IsRequired = false
    };

    public static readonly Option<string> Filter = new(
        $"--{FilterName}",
        "OData filter to apply to the budgets query (e.g., properties/category eq 'Cost')."
    )
    {
        IsRequired = false
    };
}
