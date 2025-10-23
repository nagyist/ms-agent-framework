# Invoke Azure Agent

### Overview

The `InvokeAzureAgent` action is used to invoke an Azure Agent within a workflow.

This update enhances the existing `InvokeAzureAgent` action to support additional features available in the V2 Agents API,
including structured inputs and outputs, as well as response formatting options.

### Definition

#### Current

The current version of `InvokeAzureAgent` was pared down to only support basic message input/output when the V2 Agents API was introduced.

```yaml
- kind: InvokeAzureAgent
  id: invoke_agent_1
  conversationId: =System.ConversationId
  agent:
    name: asst_abc123
  input:
    additionalInstructions: |-
      The user is located in Seattle, Washington, USA.
    messages: =UserMessage("Hi!")
  output:
    autoSend: true
    messages: =Local.AgentResponse
```

#### Proposed

In addition to removing `additionalInstructions` (which are not supported by the V2 Agents API),
we are adding support for V2 features _Structured Inputs_ & _Structured Outputs__ 
as well as re-adding support for _Response Format_ that is supported in existing APIs.

```yaml
- kind: InvokeAzureAgent
  id: invoke_agent_1
  conversationId: =System.ConversationId
  agent:
    name: DemoAgent
    version: 5
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

Property|Type|Description|Required|Default
--|--|--|--|--
`kind`|`string`|The kind of action. Must be `InvokeAzureAgent`.|Required
`id`|`string`|The unique identifier for the action.|Required
`conversationId`|`StringExpression`|Identifies the conversation in which agent will participate.|Optional
`agent.name`|`StringExpression`|Identifies the Azure Agent to invoke.|Required
`agent.version`|`StringExpression`|Specifies the version of the agent to invoke.|Optional|Latest
`input`|`Node`|Defines aspects related to the agent inputs.|Optional
`input.messages`|`Value`|The messages to send to the agent.|Optional
`input.parameters`|`CollectionProperty<StructuredInput>`|Structured inputs being provided to the agent.|Optional
`output`|`Node`|Defines aspects related to the agent response/output.|Optional
`output.autoSend`|`BoolExpression`|Indicates whether to automatically include the agent response as part of the workflow (external) conversation.  If `conversationId` property identifies the workflow conversation, setting this to `false` has no impact.|Optional|true
`output.messages`|`PropertyPath`|The scoped variable to store the agent response messages.|Optional
`output.structured`|`Node`|Defines the response format|Optional
`output.structured.schema`|`StringExpression`|The response schema when format is `json_schema`.|Required for `json_schema`
`output.structured.object`|`PropertyPath`|The scoped variable to store the structured response object.  Only assigned when for `json_object` or `json_schema`|Optional
`output.parameters`|`CollectionProperty<StructuredOutput>`|Mapping of structured output parameters from the agent to scoped variables.|Optional
`user`|`Node`|Defines if human input is needed.|Optional
`user.when`|`BoolExpression`|Expression that evaluates to true when user input is needed.|Required
`user.variable`|`PropertyPath`|The scoped variable to store user input.|Optional
`user.maxIterations`|`IntExpression`|Maximum number of iterations to allow.|Optional

**StructuredInput**:

Property|Type|Description|Required|Default
--|--|--|--
`name`|`String`|The name of the structured input parameter.|Required
`value`|`ValueExpression`|The value of the structured input parameter.|Required

**StructuredOutput**:

Property|Type|Description|Required|Default
--|--|--|--
`name`|`String`|The name of the structured output parameter.|Required
`value`|`PropertyPath`|The variable to assign the value of the structured output parameter.|Required


### Usage

#### 1. Response Format - JSON Schema

JSON schema can be provided to define the expected structure of the agent response.
The response is provided as an object (record) in a scoped variable.
The raw message is still available via the `messages` output property.

```yaml
- kind: InvokeAzureAgent
  id: invoke_agent_1
  agent:
    name: DemoAgent
  output:
    messages: Local.AgentResponse
    format:
      object: Local.AgentResponseObject
      schema: |-
        {
          "type": "object",
          "properties": {
            "result": {
              "type": "string",
              "description": "The main output or conclusion."
            },
            "confidence": {
              "type": "number",
              "description": "A numeric confidence score representing the model's certainty."
            },
            "justification": {
              "type": "string",
              "description": "Explanation or reasoning behind the result."
            }
          },
          "required": ["result", "confidence", "justification"],
          "additionalProperties": false
        }

- kind: ConditionGroup
  id: check_completion
  conditions:

    - condition: =!IsBlank(Local.BestResult) And Local.AgentResponseObject.confidence > Local.BestResult.confidence
      id: check_best_result
      actions:
    
        - kind: SetVariable
          id: set_best_result
          variable: Local.BestResult
          value: =Local.AgentResponseObject
```

#### 2. Structured Inputs

Values for structured inputs can be provided to the agent as literal values or expressions.

```yaml
- kind: InvokeAzureAgent
  id: invoke_agent_1
  agent:
    name: DemoAgent
  input:
    messages: =UserMessage("Where a good place to hike?")
    parameters:
      - name: location
        value: Seattle, WA, USA
      - name: now
        value: =System.CurrentDateTime    
  output:
    messages: Local.AgentResponse
```

#### 3. Structured Outputs

Values for structued outputs can be captured to scoped variables.

```yaml
- kind: InvokeAzureAgent
  id: invoke_agent_1
  conversationId: =System.ConversationId
  agent:
    name: DemoAgent
  output:
    parameters:
      - name: confidence
        value: Local.IntentConfidence
      - name: intent
        value: Local.UserIntent
```

#### 4. Human in the Loop: Final message from agent

When the criteria for soliciting user input is based on the agent response (e.g. low confidence),
then the agent response will be the final message in the invocation loop.

```yaml
- kind: InvokeAzureAgent
  id: invoke_agent_1
  agent:
    name: DemoAgent
  output:
    parameters:
      - name: confidence
        value: Local.IntentConfidence
      - name: intent
        value: Local.UserIntent
  user:
    when: =Local.IntentConfidence < 0.8
```

#### 5. Human in the Loop: Final message from user

When the criteria for soliciting user input is based on the user input,
then the user response will be the final message in the invocation loop.

```yaml
- kind: InvokeAzureAgent
  id: invoke_agent_1
  agent:
    name: DemoAgent
  user:
    variable: Local.UserInput
    when: =!IsBlank(Find("APPROVED", Upper(Local.UserInput.Text)))
```
