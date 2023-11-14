using System;
using System.Threading;
using System.Threading.Tasks;
using ShortUrl.Core.Models;

namespace ShortUrl.Core.Interfaces
{
    public interface IShortenUrlService
    {
        Task<Maybe<Uri>> RetrieveUriAsync(string shortUri, CancellationToken cancellationToken);
        Task<Maybe<Uri>> TryAddShortUriAsync(string shortPath, string longMaybeUri, CancellationToken cancellationToken);
        Task<Uri> CreateUriAsync(string longUri, CancellationToken cancellationToken);
        Task DeleteUriAsync(string shortUri, CancellationToken cancellationToken);
    }
}