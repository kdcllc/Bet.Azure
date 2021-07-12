using System.Threading;
using System.Threading.Tasks;

using Bet.BuildingBlocks.Messaging.Abstractions;
using Bet.BuildingBlocks.Messaging.Abstractions.Consumer;

namespace Bet.Azure.Messaging
{
    public interface IAzureConsumerPool
    {
        string Named { get; }

        Task StartAsync<TModel, THandler>(CancellationToken cancellationToken = default)
            where TModel : EventMessage
            where THandler : IMessageConsumerHandler<TModel>;

        Task StartAsync<THandler>(CancellationToken cancellationToken = default)
            where THandler : IDynamicMessageConsumerHandler;
    }
}
