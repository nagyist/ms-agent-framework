# Invoke Inline Agent

### Overview

The `InvokeInlineAgent` action is intended to support the ability to explicitly prompt the user for input during a workflow execution. 

Note, the definition of the action corresponds direclty with `InvokeAzureAgent` in all ways other than how the agent is specified:

1. For `InvokeAzureAgent`, the agent is specified by name (and optionally version).
1. For `InvokeInlineAgent`, the agent definition is specified inline and not persisted.

The definition for the inline agent corresponds directly with the `PromptAgent` definition.
The `PromptAgent` definition is currently in flight and can be viewed in the active PR 
[Final State of Foundry Alignment for Prompt Agent](https://msazure.visualstudio.com/CCI/_git/ObjectModel/pullrequest/13793226).

Due to the alignment between `InvokeInlineAgent` and `InvokeAzureAgent`, a shared base type `InvokeAgentBase` will be defined to reduce duplication.

### Definition

```yaml
- kind: InvokeInlineAgent
  id: invoke_inline_1
  conversationId: conv_123xyz
  agent:
    model: 
      id: gpt-5
      options:
        temperature: 0
    instructions: Be awesome!
    capabilities:
      codeInterpeter: true      
  input:
    messages: =UserMessage("Hi!")
    parameters:
      - name: location
        value: Seattle, WA, USA
      - name: now
        value: =System.CurrentDateTime
  output:
    autoSend: true
    messages: =Local.AgentResponse
    format:
      object: Local.AgentResponseObject
      schema: |-
        {
          "type": "object",
          "properties": {
            ...
          },
          "required": [...],
          "additionalProperties": false
        }    
    parameters:
      - name: isComplete
        variable: Local.IsComplete       
  user:
    variable: Local.UserInput
    when: =!Local.IsComplete
    maxIterations: 4
```

Since the `InvokeInlineAgent` action inherits from `InvokeAgentBase`, please refer to the `InvokeAzureAgent` action for details on all other properties.
The following table summarizes the properties specific to `PromptAgent`:

Property|Type|Description|Required|Default
--|--|--|--
`model`|`Model`|
`instructions`|`Template`|
`additionalInstructions`|`Template`|
`tools`|`CollectionProperty<AgentTool>`|
`capabilities`|`GptCapabilities`|
`settings`|`AISettings`|
`...`|`TBD`
