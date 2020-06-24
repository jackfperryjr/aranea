using System.Threading;
using System.Threading.Tasks;

namespace Aranea.Api.Core.Abstractions
{
    public interface IStore<T> : IStoreBase
    {
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = new CancellationToken());

        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = new CancellationToken());
    }
}