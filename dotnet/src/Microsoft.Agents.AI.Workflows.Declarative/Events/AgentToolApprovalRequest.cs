// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI;

namespace Microsoft.Agents.AI.Workflows.Declarative.Events;

/// <summary>
/// Represents a tool approval request.
/// </summary>
public sealed class AgentToolApprovalRequest
{
    /// <summary>
    /// The name of the agent associated with the tool request.
    /// </summary>
    public string AgentName { get; }

    /// <summary>
    /// A list of tool requests.
    /// </summary>
    public IList<UserInputResponseContent> ApprovalRequests { get; }

    [JsonConstructor]
    internal AgentToolApprovalRequest(string agentName, IList<UserInputResponseContent> approvalRequests)
    {
        this.AgentName = agentName;
        this.ApprovalRequests = approvalRequests;
    }
}
