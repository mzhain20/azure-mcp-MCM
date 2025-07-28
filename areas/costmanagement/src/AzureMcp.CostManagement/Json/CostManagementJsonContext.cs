using System.Text.Json.Serialization;
using AzureMcp.CostManagement.Commands.Budgets;
using AzureMcp.CostManagement.Models;
using AzureMcp.CostManagement.Options.Budgets;

namespace AzureMcp.CostManagement.Json;

[JsonSerializable(typeof(BudgetsListOptions))]
[JsonSerializable(typeof(BudgetSummary))]
[JsonSerializable(typeof(List<BudgetSummary>))]
[JsonSerializable(typeof(BudgetsListCommand.BudgetsListCommandResult))]
public partial class CostManagementJsonContext : JsonSerializerContext
{
}
