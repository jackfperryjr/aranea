using System.Threading;
using System.Threading.Tasks;

namespace Aranea.Api.Core.Abstractions
{

    public interface IFactory<T, in TKey> : IFactoryBase
    {
        Task<T> GetAsync(TKey key, CancellationToken cancellationToken = new CancellationToken());
    }
}