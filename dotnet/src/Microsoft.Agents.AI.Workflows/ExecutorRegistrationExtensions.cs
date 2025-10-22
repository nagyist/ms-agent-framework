﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Shared.Diagnostics;

namespace Microsoft.Agents.AI.Workflows;

/// <summary>
/// Extension methods for configuring executors and functions as <see cref="ExecutorRegistration"/> instances.
/// </summary>
public static class ExecutorRegistrationExtensions
{
    /// <summary>
    /// Configures an <see cref="Executor"/> instance for use in a workflow.
    /// </summary>
    /// <remarks>
    /// Note that Executor Ids must be unique within a workflow.
    /// </remarks>
    /// <param name="executor">The executor instance.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance wrapping the specified <see cref="Executor"/>.</returns>
    public static ExecutorInstanceRegistration RegisterExecutor(this Executor executor)
        => new(executor);

    /// <summary>
    /// Configures a factory method for creating an <see cref="Executor"/> of type <typeparamref name="TExecutor"/>, using the
    /// type name as the id.
    /// </summary>
    /// <remarks>
    /// Note that Executor Ids must be unique within a workflow.
    ///
    /// Although this will generally result in a delay-instantiated <see cref="Executor"/> once messages are available
    /// for it, it will be instantiated if a <see cref="ProtocolDescriptor"/> for the <see cref="Workflow"/> is requested,
    /// and it is the starting executor.
    /// </remarks>
    /// <typeparam name="TExecutor">The type of the resulting executor</typeparam>
    /// <param name="factoryAsync">The factory method.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that resolves to the result of the factory call when messages get sent to it.</returns>
    public static ExecutorRegistration RegisterExecutor<TExecutor>(this Func<string, string, ValueTask<TExecutor>> factoryAsync)
        where TExecutor : Executor
        => RegisterExecutor<TExecutor, ExecutorOptions>((config, runId) => factoryAsync(config.Id, runId), id: typeof(TExecutor).Name, options: null);

    /// <summary>
    /// Configures a factory method for creating an <see cref="Executor"/> of type <typeparamref name="TExecutor"/>, using the
    /// type name as the id.
    /// </summary>
    /// <remarks>
    /// Note that Executor Ids must be unique within a workflow.
    ///
    /// Although this will generally result in a delay-instantiated <see cref="Executor"/> once messages are available
    /// for it, it will be instantiated if a <see cref="ProtocolDescriptor"/> for the <see cref="Workflow"/> is requested,
    /// and it is the starting executor.
    /// </remarks>
    /// <typeparam name="TExecutor">The type of the resulting executor</typeparam>
    /// <param name="factoryAsync">The factory method.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that resolves to the result of the factory call when messages get sent to it.</returns>
    [Obsolete("Use RegisterExecutor() instead.")]
    public static ExecutorRegistration ConfigureFactory<TExecutor>(this Func<string, string, ValueTask<TExecutor>> factoryAsync)
        where TExecutor : Executor
        => factoryAsync.RegisterExecutor();

    /// <summary>
    /// Configures a factory method for creating an <see cref="Executor"/> of type <typeparamref name="TExecutor"/>, with
    /// the specified id.
    /// </summary>
    /// <remarks>
    /// Although this will generally result in a delay-instantiated <see cref="Executor"/> once messages are available
    /// for it, it will be instantiated if a <see cref="ProtocolDescriptor"/> for the <see cref="Workflow"/> is requested,
    /// and it is the starting executor.
    /// </remarks>
    /// <typeparam name="TExecutor">The type of the resulting executor</typeparam>
    /// <param name="factoryAsync">The factory method.</param>
    /// <param name="id">An id for the executor to be instantiated.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that resolves to the result of the factory call when messages get sent to it.</returns>
    public static ExecutorRegistration RegisterExecutor<TExecutor>(this Func<string, string, ValueTask<TExecutor>> factoryAsync, string id)
        where TExecutor : Executor
        => RegisterExecutor<TExecutor, ExecutorOptions>((_, runId) => factoryAsync(id, runId), id, options: null);

    /// <summary>
    /// Configures a factory method for creating an <see cref="Executor"/> of type <typeparamref name="TExecutor"/>, with
    /// the specified id.
    /// </summary>
    /// <remarks>
    /// Although this will generally result in a delay-instantiated <see cref="Executor"/> once messages are available
    /// for it, it will be instantiated if a <see cref="ProtocolDescriptor"/> for the <see cref="Workflow"/> is requested,
    /// and it is the starting executor.
    /// </remarks>
    /// <typeparam name="TExecutor">The type of the resulting executor</typeparam>
    /// <param name="factoryAsync">The factory method.</param>
    /// <param name="id">An id for the executor to be instantiated.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that resolves to the result of the factory call when messages get sent to it.</returns>
    [Obsolete("Use RegisterExecutor() instead.")]
    public static ExecutorRegistration ConfigureFactory<TExecutor>(this Func<string, string, ValueTask<TExecutor>> factoryAsync, string id)
        where TExecutor : Executor
        => factoryAsync.RegisterExecutor(id);

    /// <summary>
    /// Configures a factory method for creating an <see cref="Executor"/> of type <typeparamref name="TExecutor"/>, with
    /// the specified id and options.
    /// </summary>
    /// <remarks>
    /// Although this will generally result in a delay-instantiated <see cref="Executor"/> once messages are available
    /// for it, it will be instantiated if a <see cref="ProtocolDescriptor"/> for the <see cref="Workflow"/> is requested,
    /// and it is the starting executor.
    /// </remarks>
    /// <typeparam name="TExecutor">The type of the resulting executor</typeparam>
    /// <typeparam name="TOptions">The type of options object to be passed to the factory method.</typeparam>
    /// <param name="factoryAsync">The factory method.</param>
    /// <param name="id">An id for the executor to be instantiated.</param>
    /// <param name="options">An optional parameter specifying the options.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that resolves to the result of the factory call when messages get sent to it.</returns>
    public static ExecutorRegistration RegisterExecutor<TExecutor, TOptions>(this Func<Config<TOptions>, string, ValueTask<TExecutor>> factoryAsync, string id, TOptions? options = null)
        where TExecutor : Executor
        where TOptions : ExecutorOptions
    {
        Configured<TExecutor, TOptions> configured = new(factoryAsync, id, options);

        return new ConfiguredExecutorRegistration(configured.Super<TExecutor, Executor, TOptions>(), typeof(TExecutor));
    }

    /// <summary>
    /// Configures a factory method for creating an <see cref="Executor"/> of type <typeparamref name="TExecutor"/>, with
    /// the specified id and options.
    /// </summary>
    /// <remarks>
    /// Although this will generally result in a delay-instantiated <see cref="Executor"/> once messages are available
    /// for it, it will be instantiated if a <see cref="ProtocolDescriptor"/> for the <see cref="Workflow"/> is requested,
    /// and it is the starting executor.
    /// </remarks>
    /// <typeparam name="TExecutor">The type of the resulting executor</typeparam>
    /// <typeparam name="TOptions">The type of options object to be passed to the factory method.</typeparam>
    /// <param name="factoryAsync">The factory method.</param>
    /// <param name="id">An id for the executor to be instantiated.</param>
    /// <param name="options">An optional parameter specifying the options.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that resolves to the result of the factory call when messages get sent to it.</returns>
    [Obsolete("Use RegisterExecutor() instead")]
    public static ExecutorRegistration ConfigureFactory<TExecutor, TOptions>(this Func<Config<TOptions>, string, ValueTask<TExecutor>> factoryAsync, string id, TOptions? options = null)
        where TExecutor : Executor
        where TOptions : ExecutorOptions
        => factoryAsync.RegisterExecutor(id, options);

    private static ConfiguredExecutorRegistration ToRegistration<TInput>(this FunctionExecutor<TInput> executor, Delegate raw)
        => new(Configured.FromInstance(executor, raw: raw)
                         .Super<FunctionExecutor<TInput>, Executor>(),
            typeof(FunctionExecutor<TInput>));

    private static ConfiguredExecutorRegistration ToRegistration<TInput, TOutput>(this FunctionExecutor<TInput, TOutput> executor, Delegate raw)
        => new(Configured.FromInstance(executor, raw: raw)
                         .Super<FunctionExecutor<TInput, TOutput>, Executor>(),
            typeof(FunctionExecutor<TInput, TOutput>));

    /// <summary>
    /// Configures a sub-workflow executor for the specified workflow, using the provided identifier and options.
    /// </summary>
    /// <param name="workflow">The workflow instance to be executed as a sub-workflow. Cannot be null.</param>
    /// <param name="id">A unique identifier for the sub-workflow execution. Used to distinguish this sub-workflow instance.</param>
    /// <param name="options">Optional configuration options for the sub-workflow executor. If null, default options are used.</param>
    /// <returns>An ExecutorRegistration instance representing the configured sub-workflow executor.</returns>
    [Obsolete("Use AsExecutor() instead")]
    public static SubworkflowRegistration ConfigureSubWorkflow(this Workflow workflow, string id, ExecutorOptions? options = null)
        => workflow.AsExecutor(id, options);

    /// <summary>
    /// Configures a sub-workflow executor for the specified workflow, using the provided identifier and options.
    /// </summary>
    /// <param name="workflow">The workflow instance to be executed as a sub-workflow. Cannot be null.</param>
    /// <param name="id">A unique identifier for the sub-workflow execution. Used to distinguish this sub-workflow instance.</param>
    /// <param name="options">Optional configuration options for the sub-workflow executor. If null, default options are used.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance representing the configured sub-workflow executor.</returns>
    public static SubworkflowRegistration AsExecutor(this Workflow workflow, string id, ExecutorOptions? options = null)
        => new(workflow, id, options);

    /// <summary>
    /// Configures a function-based asynchronous message handler as an executor with the specified identifier and
    /// options.
    /// </summary>
    /// <typeparam name="TInput">The type of input message.</typeparam>
    /// <param name="messageHandlerAsync">A delegate that defines the asynchronous function to execute for each input message.</param>
    /// <param name="id">An optional unique identifier for the executor. If <c>null</c>, will use the function argument as an id.</param>
    /// <param name="options">Configuration options for the executor. If <c>null</c>, default options will be used.</param>
    /// <param name="threadsafe">Declare that the message handler may be used simultaneously by multiple runs concurrently.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that wraps the provided asynchronous message handler and configuration.</returns>
    public static ExecutorRegistration AsExecutor<TInput>(this Func<TInput, IWorkflowContext, CancellationToken, ValueTask> messageHandlerAsync, string id, ExecutorOptions? options = null, bool threadsafe = false)
        => new FunctionExecutor<TInput>(id, messageHandlerAsync, options, declareCrossRunShareable: threadsafe).ToRegistration(messageHandlerAsync);

    /// <summary>
    /// Configures a function-based asynchronous message handler as an executor with the specified identifier and
    /// options.
    /// </summary>
    /// <typeparam name="TInput">The type of input message.</typeparam>
    /// <param name="messageHandlerAsync">A delegate that defines the asynchronous function to execute for each input message.</param>
    /// <param name="id">An optional unique identifier for the executor. If <c>null</c>, will use the function argument as an id.</param>
    /// <param name="options">Configuration options for the executor. If <c>null</c>, default options will be used.</param>
    /// <param name="threadsafe">Declare that the message handler may be used simultaneously by multiple runs concurrently.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that wraps the provided asynchronous message handler and configuration.</returns>
    public static ExecutorRegistration AsExecutor<TInput>(this Func<TInput, ValueTask> messageHandlerAsync, string id, ExecutorOptions? options = null, bool threadsafe = false)
        => ((Func<TInput, IWorkflowContext, CancellationToken, ValueTask>)((input, _, __) => messageHandlerAsync(input)))
                .AsExecutor(id, options, threadsafe);

    /// <summary>
    /// Configures a function-based asynchronous message handler as an executor with the specified identifier and
    /// options.
    /// </summary>
    /// <typeparam name="TInput">The type of input message.</typeparam>
    /// <param name="messageHandlerAsync">A delegate that defines the asynchronous function to execute for each input message.</param>
    /// <param name="id">An optional unique identifier for the executor. If <c>null</c>, will use the function argument as an id.</param>
    /// <param name="options">Configuration options for the executor. If <c>null</c>, default options will be used.</param>
    /// <param name="threadsafe">Declare that the message handler may be used simultaneously by multiple runs concurrently.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that wraps the provided asynchronous message handler and configuration.</returns>
    public static ExecutorRegistration AsExecutor<TInput>(this Func<TInput, IWorkflowContext, ValueTask> messageHandlerAsync, string id, ExecutorOptions? options = null, bool threadsafe = false)
        => ((Func<TInput, IWorkflowContext, CancellationToken, ValueTask>)((input, ctx, __) => messageHandlerAsync(input, ctx)))
                .AsExecutor(id, options, threadsafe);

    /// <summary>
    /// Configures a function-based asynchronous message handler as an executor with the specified identifier and
    /// options.
    /// </summary>
    /// <typeparam name="TInput">The type of input message.</typeparam>
    /// <param name="messageHandlerAsync">A delegate that defines the asynchronous function to execute for each input message.</param>
    /// <param name="id">An optional unique identifier for the executor. If <c>null</c>, will use the function argument as an id.</param>
    /// <param name="options">Configuration options for the executor. If <c>null</c>, default options will be used.</param>
    /// <param name="threadsafe">Declare that the message handler may be used simultaneously by multiple runs concurrently.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that wraps the provided asynchronous message handler and configuration.</returns>
    public static ExecutorRegistration AsExecutor<TInput>(this Func<TInput, CancellationToken, ValueTask> messageHandlerAsync, string id, ExecutorOptions? options = null, bool threadsafe = false)
        => ((Func<TInput, IWorkflowContext, CancellationToken, ValueTask>)((input, _, ct) => messageHandlerAsync(input, ct)))
                .AsExecutor(id, options, threadsafe);

    /// <summary>
    /// Configures a function-based message handler as an executor with the specified identifier and
    /// options.
    /// </summary>
    /// <typeparam name="TInput">The type of input message.</typeparam>
    /// <param name="messageHandler">A delegate that defines the function to execute for each input message.</param>
    /// <param name="id">An optional unique identifier for the executor. If <c>null</c>, will use the function argument as an id.</param>
    /// <param name="options">Configuration options for the executor. If <c>null</c>, default options will be used.</param>
    /// <param name="threadsafe">Declare that the message handler may be used simultaneously by multiple runs concurrently.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that wraps the provided asynchronous message handler and configuration.</returns>
    public static ExecutorRegistration AsExecutor<TInput>(this Action<TInput, IWorkflowContext, CancellationToken> messageHandler, string id, ExecutorOptions? options = null, bool threadsafe = false)
        => new FunctionExecutor<TInput>(id, messageHandler, options, declareCrossRunShareable: threadsafe).ToRegistration(messageHandler);

    /// <summary>
    /// Configures a function-based message handler as an executor with the specified identifier and
    /// options.
    /// </summary>
    /// <typeparam name="TInput">The type of input message.</typeparam>
    /// <param name="messageHandler">A delegate that defines the function to execute for each input message.</param>
    /// <param name="id">An optional unique identifier for the executor. If <c>null</c>, will use the function argument as an id.</param>
    /// <param name="options">Configuration options for the executor. If <c>null</c>, default options will be used.</param>
    /// <param name="threadsafe">Declare that the message handler may be used simultaneously by multiple runs concurrently.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that wraps the provided asynchronous message handler and configuration.</returns>
    public static ExecutorRegistration AsExecutor<TInput>(this Action<TInput> messageHandler, string id, ExecutorOptions? options = null, bool threadsafe = false)
        => ((Action<TInput, IWorkflowContext, CancellationToken>)((input, _, __) => messageHandler(input)))
            .AsExecutor(id, options, threadsafe);

    /// <summary>
    /// Configures a function-based message handler as an executor with the specified identifier and
    /// options.
    /// </summary>
    /// <typeparam name="TInput">The type of input message.</typeparam>
    /// <param name="messageHandler">A delegate that defines the function to execute for each input message.</param>
    /// <param name="id">An optional unique identifier for the executor. If <c>null</c>, will use the function argument as an id.</param>
    /// <param name="options">Configuration options for the executor. If <c>null</c>, default options will be used.</param>
    /// <param name="threadsafe">Declare that the message handler may be used simultaneously by multiple runs concurrently.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that wraps the provided asynchronous message handler and configuration.</returns>
    public static ExecutorRegistration AsExecutor<TInput>(this Action<TInput, IWorkflowContext> messageHandler, string id, ExecutorOptions? options = null, bool threadsafe = false)
        => ((Action<TInput, IWorkflowContext, CancellationToken>)((input, ctx, __) => messageHandler(input, ctx)))
            .AsExecutor(id, options, threadsafe);

    /// <summary>
    /// Configures a function-based message handler as an executor with the specified identifier and
    /// options.
    /// </summary>
    /// <typeparam name="TInput">The type of input message.</typeparam>
    /// <param name="messageHandler">A delegate that defines the function to execute for each input message.</param>
    /// <param name="id">An optional unique identifier for the executor. If <c>null</c>, will use the function argument as an id.</param>
    /// <param name="options">Configuration options for the executor. If <c>null</c>, default options will be used.</param>
    /// <param name="threadsafe">Declare that the message handler may be used simultaneously by multiple runs concurrently.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that wraps the provided asynchronous message handler and configuration.</returns>
    public static ExecutorRegistration AsExecutor<TInput>(this Action<TInput, CancellationToken> messageHandler, string id, ExecutorOptions? options = null, bool threadsafe = false)
        => ((Action<TInput, IWorkflowContext, CancellationToken>)((input, _, ct) => messageHandler(input, ct)))
            .AsExecutor(id, options, threadsafe);

    /// <summary>
    /// Configures a function-based asynchronous message handler as an executor with the specified identifier and
    /// options.
    /// </summary>
    /// <typeparam name="TInput">The type of input message.</typeparam>
    /// <typeparam name="TOutput">The type of output message.</typeparam>
    /// <param name="messageHandlerAsync">A delegate that defines the asynchronous function to execute for each input message.</param>
    /// <param name="id">A unique identifier for the executor.</param>
    /// <param name="options">Configuration options for the executor. If <c>null</c>, default options will be used.</param>
    /// <param name="threadsafe">Declare that the message handler may be used simultaneously by multiple runs concurrently.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that wraps the provided asynchronous message handler and configuration.</returns>
    public static ExecutorRegistration AsExecutor<TInput, TOutput>(this Func<TInput, IWorkflowContext, CancellationToken, ValueTask<TOutput>> messageHandlerAsync, string id, ExecutorOptions? options = null, bool threadsafe = false)
        => new FunctionExecutor<TInput, TOutput>(Throw.IfNull(id), messageHandlerAsync, options, declareCrossRunShareable: threadsafe).ToRegistration(messageHandlerAsync);

    /// <summary>
    /// Configures a function-based asynchronous message handler as an executor with the specified identifier and
    /// options.
    /// </summary>
    /// <typeparam name="TInput">The type of input message.</typeparam>
    /// <typeparam name="TOutput">The type of output message.</typeparam>
    /// <param name="messageHandlerAsync">A delegate that defines the asynchronous function to execute for each input message.</param>
    /// <param name="id">An optional unique identifier for the executor. If <c>null</c>, will use the function argument as an id.</param>
    /// <param name="options">Configuration options for the executor. If <c>null</c>, default options will be used.</param>
    /// <param name="threadsafe">Declare that the message handler may be used simultaneously by multiple runs concurrently.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that wraps the provided asynchronous message handler and configuration.</returns>
    public static ExecutorRegistration AsExecutor<TInput, TOutput>(this Func<TInput, ValueTask<TOutput>> messageHandlerAsync, string id, ExecutorOptions? options = null, bool threadsafe = false)
        => ((Func<TInput, IWorkflowContext, CancellationToken, ValueTask<TOutput>>)((input, _, __) => messageHandlerAsync(input)))
                .AsExecutor(id, options, threadsafe);

    /// <summary>
    /// Configures a function-based asynchronous message handler as an executor with the specified identifier and
    /// options.
    /// </summary>
    /// <typeparam name="TInput">The type of input message.</typeparam>
    /// <typeparam name="TOutput">The type of output message.</typeparam>
    /// <param name="messageHandlerAsync">A delegate that defines the asynchronous function to execute for each input message.</param>
    /// <param name="id">An optional unique identifier for the executor. If <c>null</c>, will use the function argument as an id.</param>
    /// <param name="options">Configuration options for the executor. If <c>null</c>, default options will be used.</param>
    /// <param name="threadsafe">Declare that the message handler may be used simultaneously by multiple runs concurrently.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that wraps the provided asynchronous message handler and configuration.</returns>
    public static ExecutorRegistration AsExecutor<TInput, TOutput>(this Func<TInput, IWorkflowContext, ValueTask<TOutput>> messageHandlerAsync, string id, ExecutorOptions? options = null, bool threadsafe = false)
        => ((Func<TInput, IWorkflowContext, CancellationToken, ValueTask<TOutput>>)((input, ctx, __) => messageHandlerAsync(input, ctx)))
                .AsExecutor(id, options, threadsafe);

    /// <summary>
    /// Configures a function-based asynchronous message handler as an executor with the specified identifier and
    /// options.
    /// </summary>
    /// <typeparam name="TInput">The type of input message.</typeparam>
    /// <typeparam name="TOutput">The type of output message.</typeparam>
    /// <param name="messageHandlerAsync">A delegate that defines the asynchronous function to execute for each input message.</param>
    /// <param name="id">An optional unique identifier for the executor. If <c>null</c>, will use the function argument as an id.</param>
    /// <param name="options">Configuration options for the executor. If <c>null</c>, default options will be used.</param>
    /// <param name="threadsafe">Declare that the message handler may be used simultaneously by multiple runs concurrently.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that wraps the provided asynchronous message handler and configuration.</returns>
    public static ExecutorRegistration AsExecutor<TInput, TOutput>(this Func<TInput, CancellationToken, ValueTask<TOutput>> messageHandlerAsync, string id, ExecutorOptions? options = null, bool threadsafe = false)
        => ((Func<TInput, IWorkflowContext, CancellationToken, ValueTask<TOutput>>)((input, _, ct) => messageHandlerAsync(input, ct)))
                .AsExecutor(id, options, threadsafe);

    /// <summary>
    /// Configures a function-based message handler as an executor with the specified identifier and options.
    /// </summary>
    /// <typeparam name="TInput">The type of input message.</typeparam>
    /// <typeparam name="TOutput">The type of output message.</typeparam>
    /// <param name="messageHandler">A delegate that defines the function to execute for each input message.</param>
    /// <param name="id">An optional unique identifier for the executor. If <c>null</c>, will use the function argument as an id.</param>
    /// <param name="options">Configuration options for the executor. If <c>null</c>, default options will be used.</param>
    /// <param name="threadsafe">Declare that the message handler may be used simultaneously by multiple runs concurrently.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that wraps the provided asynchronous message handler and configuration.</returns>
    public static ExecutorRegistration AsExecutor<TInput, TOutput>(this Func<TInput, IWorkflowContext, CancellationToken, TOutput> messageHandler, string id, ExecutorOptions? options = null, bool threadsafe = false)
        => new FunctionExecutor<TInput, TOutput>(id, messageHandler, options, declareCrossRunShareable: threadsafe).ToRegistration(messageHandler);

    /// <summary>
    /// Configures a function-based message handler as an executor with the specified identifier and options.
    /// </summary>
    /// <typeparam name="TInput">The type of input message.</typeparam>
    /// <typeparam name="TOutput">The type of output message.</typeparam>
    /// <param name="messageHandler">A delegate that defines the function to execute for each input message.</param>
    /// <param name="id">An optional unique identifier for the executor. If <c>null</c>, will use the function argument as an id.</param>
    /// <param name="options">Configuration options for the executor. If <c>null</c>, default options will be used.</param>
    /// <param name="threadsafe">Declare that the message handler may be used simultaneously by multiple runs concurrently.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that wraps the provided asynchronous message handler and configuration.</returns>
    public static ExecutorRegistration AsExecutor<TInput, TOutput>(this Func<TInput, TOutput> messageHandler, string id, ExecutorOptions? options = null, bool threadsafe = false)
        => ((Func<TInput, IWorkflowContext, CancellationToken, TOutput>)((input, _, __) => messageHandler(input)))
                .AsExecutor(id, options, threadsafe);

    /// <summary>
    /// Configures a function-based message handler as an executor with the specified identifier and options.
    /// </summary>
    /// <typeparam name="TInput">The type of input message.</typeparam>
    /// <typeparam name="TOutput">The type of output message.</typeparam>
    /// <param name="messageHandler">A delegate that defines the function to execute for each input message.</param>
    /// <param name="id">An optional unique identifier for the executor. If <c>null</c>, will use the function argument as an id.</param>
    /// <param name="options">Configuration options for the executor. If <c>null</c>, default options will be used.</param>
    /// <param name="threadsafe">Declare that the message handler may be used simultaneously by multiple runs concurrently.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that wraps the provided asynchronous message handler and configuration.</returns>
    public static ExecutorRegistration AsExecutor<TInput, TOutput>(this Func<TInput, IWorkflowContext, TOutput> messageHandler, string id, ExecutorOptions? options = null, bool threadsafe = false)
        => ((Func<TInput, IWorkflowContext, CancellationToken, TOutput>)((input, ctx, __) => messageHandler(input, ctx)))
                .AsExecutor(id, options, threadsafe);

    /// <summary>
    /// Configures a function-based message handler as an executor with the specified identifier and options.
    /// </summary>
    /// <typeparam name="TInput">The type of input message.</typeparam>
    /// <typeparam name="TOutput">The type of output message.</typeparam>
    /// <param name="messageHandler">A delegate that defines the function to execute for each input message.</param>
    /// <param name="id">An optional unique identifier for the executor. If <c>null</c>, will use the function argument as an id.</param>
    /// <param name="options">Configuration options for the executor. If <c>null</c>, default options will be used.</param>
    /// <param name="threadsafe">Declare that the message handler may be used simultaneously by multiple runs concurrently.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that wraps the provided asynchronous message handler and configuration.</returns>
    public static ExecutorRegistration AsExecutor<TInput, TOutput>(this Func<TInput, CancellationToken, TOutput> messageHandler, string id, ExecutorOptions? options = null, bool threadsafe = false)
        => ((Func<TInput, IWorkflowContext, CancellationToken, TOutput>)((input, _, ct) => messageHandler(input, ct)))
                .AsExecutor(id, options, threadsafe);

    /// <summary>
    /// Configures a function-based aggregating executor with the specified identifier and options.
    /// </summary>
    /// <typeparam name="TInput">The type of input message.</typeparam>
    /// <typeparam name="TAccumulate">The type of the accumulating object.</typeparam>
    /// <param name="aggregatorFunc">A delegate the defines the aggregation procedure</param>
    /// <param name="id">A unique identifier for the executor.</param>
    /// <param name="options">Configuration options for the executor. If <c>null</c>, default options will be used.</param>
    /// <param name="threadsafe">Declare that the message handler may be used simultaneously by multiple runs concurrently.</param>
    /// <returns>An <see cref="ExecutorRegistration"/> instance that wraps the provided asynchronous message handler and configuration.</returns>
    public static ExecutorRegistration AsExecutor<TInput, TAccumulate>(this Func<TAccumulate?, TInput, TAccumulate?> aggregatorFunc, string id, ExecutorOptions? options = null, bool threadsafe = false)
        => new AggregatingExecutor<TInput, TAccumulate>(id, aggregatorFunc, options, declareCrossRunShareable: threadsafe);

    /// <summary>
    /// Configure an <see cref="AIAgent"/> as an executor for use in a workflow.
    /// </summary>
    /// <param name="agent">The agent instance.</param>
    /// <param name="emitEvents">Specifies whether the agent should emit streaming events.</param>
    /// <returns>An <see cref="AIAgentRegistration"/> instance that wraps the provided agent.</returns>
    public static AIAgentRegistration AsExecutor(this AIAgent agent, bool emitEvents = false)
        => new(agent, emitEvents);

    /// <summary>
    /// Configure a <see cref="RequestPort"/> as an executor for use in a workflow.
    /// </summary>
    /// <param name="port">The port configuration.</param>
    /// <param name="allowWrappedRequests">Specifies whether the port should accept requests already wrapped in
    /// <see cref="ExternalRequest"/>.</param>
    /// <returns>A <see cref="RequestPortRegistration"/> instance that wraps the provided port.</returns>
    public static RequestPortRegistration AsExecutor(this RequestPort port, bool allowWrappedRequests = true)
        => new(port, allowWrappedRequests);
}
