using Bet.BuildingBlocks.Messaging.Abstractions;
using Bet.BuildingBlocks.Messaging.Abstractions.Consumer;

namespace Bet.Azure.Messaging;

public interface IAzureConsumerPool
{
    string Named { get; }

    /// <summary>
    /// Start listening only on specify message handler.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="THandler"></typeparam>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task StartAsync<TModel, THandler>(CancellationToken cancellationToken = default)
        where TModel : EventMessage
        where THandler : IMessageConsumerHandler<TModel>;

    /// <summary>
    /// Stop listening only on a specify message handler.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="THandler"></typeparam>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task StopAsync<TModel, THandler>(CancellationToken cancellationToken = default)
          where TModel : EventMessage
          where THandler : IMessageConsumerHandler<TModel>;

    /// <summary>
    ///  Start listening only on specify message handler.
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task StartAsync<THandler>(CancellationToken cancellationToken = default)
        where THandler : IDynamicMessageConsumerHandler;

    /// <summary>
    /// Stop listening only on a specify message handler.
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task StopAsync<THandler>(CancellationToken cancellationToken = default)
        where THandler : IDynamicMessageConsumerHandler;

    /// <summary>
    /// Starts all of the registered handlers.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops all of the registers message handlers.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task StopAsync(CancellationToken cancellationToken = default);
}
