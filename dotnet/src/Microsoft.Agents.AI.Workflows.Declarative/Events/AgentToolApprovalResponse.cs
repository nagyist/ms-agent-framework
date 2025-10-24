// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI;

namespace Microsoft.Agents.AI.Workflows.Declarative.Events;

/// <summary>
/// Represents a tool approval response.
/// </summary>
public sealed class AgentToolApprovalResponse
{
    /// <summary>
    /// The name of the agent associated with the tool request.
    /// </summary>
    public string AgentName { get; }

    /// <summary>
    /// A list of approval responses.
    /// </summary>
    public IList<FunctionResultContent> ApprovalResponses { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InputResponse"/> class.
    /// </summary>
    [JsonConstructor]
    internal AgentToolApprovalResponse(string agentName, IList<FunctionResultContent> approvalResponses)
    {
        this.AgentName = agentName;
        this.ApprovalResponses = approvalResponses;
    }
}
