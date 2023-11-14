using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShortUrl.Core.Interfaces;
using ShortUrl.Core.Models;

namespace ShortUrl.Core;

//If key is generated outside persistence layer, Redis should do great for actual implementation
//Alternatively, relational database can return an INT id which we can convert into unique string, which might work even better than random generation
public class InMemoryRepo : IRepository
{
    private readonly IKeyGenerator _keyGenerator;
    private readonly Dictionary<string, (string, int)> _storage = new();

    public InMemoryRepo(IKeyGenerator keyGenerator)
    {
        _keyGenerator = keyGenerator;
    }
    
    public Task<bool> IsExistsAsync(string key, CancellationToken cancellationToken)
    {
        return Task.FromResult(_storage.ContainsKey(key));
    }

    public Task AddAsync(string key, string longUri, CancellationToken cancellationToken)
    {
        _storage.Add(key, (longUri, 0));
        return Task.CompletedTask;
    }

    public Task<string> CreateAsync(string longUrl, CancellationToken cancellationToken)
    {
        for (var i = 0; i < 100; i++)
        {
            var key = _keyGenerator.GetKey();
            if (_storage.TryAdd(key, (longUrl, 0)))
            {
                return Task.FromResult(key);
            }
        }

        throw new Exception("If you got here, you should probably increase the key length in IKeyGenerator");
    }

    public Task DeleteAsync(string key, CancellationToken cancellationToken)
    {
        _storage.Remove(key);
        return Task.CompletedTask;
    }

    public Task IncCounterAsync(string key, CancellationToken cancellationToken)
    {
        var value = _storage[key];
        _storage[key] = (value.Item1, value.Item2 + 1);
        return Task.CompletedTask;
    }

    public Task<int> GetCounterAsync(string key, CancellationToken cancellationToken)
    {
        return Task.FromResult(_storage[key].Item2);
    }

    public Task<Maybe<string>> GetByIdAsync(string key, CancellationToken cancellationToken)
    {
        if (_storage.TryGetValue(key, out var value))
        {
            return Task.FromResult(Maybe<string>.From(value.Item1));
        }

        return Task.FromResult(Maybe<string>.From(null));
    }
}