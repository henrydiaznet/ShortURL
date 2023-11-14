using System;
using System.Threading;
using System.Threading.Tasks;
using ShortUrl.Core.Interfaces;
using ShortUrl.Core.Models;

namespace ShortUrl.Core
{
    public class ShortenUrlService : IShortenUrlService
    {
        private static readonly string Root = "https://zip.ly{0}";

        private readonly IInputValidator _validator;
        private readonly IRepository _repository;

        public ShortenUrlService(IInputValidator validator, IRepository repository)
        {
            _validator = validator;
            _repository = repository;
        }
        
        public async Task<Maybe<Uri>> RetrieveUriAsync(string shortUri, CancellationToken cancellationToken)
        {
            if (!_validator.ValidateUrl(shortUri, out var uri))
            {
                throw new ArgumentException($"{shortUri} is not a valid URL", shortUri);
            }

            var result = await _repository.GetByIdAsync(uri.AbsolutePath, cancellationToken);
            if (result.HasNoValue)
            {
                return Maybe<Uri>.From(null);
            }
            
            _repository.IncCounterAsync(uri.AbsolutePath, cancellationToken);
            return Maybe<Uri>.From(new Uri(result.GetValueOrThrow(), UriKind.Absolute));
        }

        public async Task<Maybe<Uri>> TryAddShortUriAsync(string maybeShortPath, string longMaybeUri, CancellationToken cancellationToken)
        {
            if (!_validator.ValidateUrl(longMaybeUri, out var longUri))
            {
                throw new ArgumentException($"{longMaybeUri} is not a valid URL", longMaybeUri);
            }

            if (!_validator.ValidatePath(maybeShortPath, out var shortPath))
            {
                return Maybe<Uri>.From(null);
            }
            
            if (await _repository.IsExistsAsync(shortPath, cancellationToken))
            {
                return Maybe<Uri>.From(null);
            }

            await _repository.AddAsync(shortPath, longUri.ToString(), cancellationToken);
            return Maybe<Uri>.From(ToUrl(shortPath));
        }

        public async Task<Uri> CreateUriAsync(string longUri, CancellationToken cancellationToken)
        {
            if (!_validator.ValidateUrl(longUri, out var uri))
            {
                throw new ArgumentException($"{longUri} is not a valid URL", longUri);
            }

            var result = await _repository.CreateAsync(uri.ToString(), cancellationToken);
            
            return ToUrl(result);
        }

        public async Task DeleteUriAsync(string shortUri, CancellationToken cancellationToken)
        {
            if (!_validator.ValidateUrl(shortUri, out var uri))
            {
                throw new ArgumentException($"{shortUri} is not a valid URL", shortUri);
            }
            
            await _repository.DeleteAsync(uri.AbsolutePath, cancellationToken);
        }

        private static Uri ToUrl(string uri)
        {
            return new Uri(string.Format(Root, uri), UriKind.Absolute);
        }
    }
}