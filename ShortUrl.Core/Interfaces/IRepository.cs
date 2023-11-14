using System.Threading;
using System.Threading.Tasks;
using ShortUrl.Core.Models;

namespace ShortUrl.Core.Interfaces
{
    public interface IRepository
    {
        Task<bool> IsExistsAsync(string key, CancellationToken cancellationToken);
        Task AddAsync(string key, string longUri, CancellationToken cancellationToken);
        Task<string> CreateAsync(string longUrl, CancellationToken cancellationToken);
        Task DeleteAsync(string key, CancellationToken cancellationToken);
        Task IncCounterAsync(string key, CancellationToken cancellationToken);
        Task<int> GetCounterAsync(string key, CancellationToken cancellationToken);        
        Task<Maybe<string>> GetByIdAsync(string key, CancellationToken cancellationToken);
    }
}