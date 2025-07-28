// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using AzureMcp.CostManagement.Commands.Budgets;

namespace AzureMcp.CostManagement.Commands;

[JsonSerializable(typeof(BudgetsListCommand.BudgetsListCommandResult))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal sealed partial class CostManagementJsonContext : JsonSerializerContext
{
}
