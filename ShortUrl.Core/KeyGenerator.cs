using System;
using System.Linq;
using ShortUrl.Core.Interfaces;

namespace ShortUrl.Core
{
    //as a service this should be singleton, to keep Random instance around,
    //because it doesn't guarantee randomness otherwise if called concurrently
    public class KeyGenerator: IKeyGenerator
    {
        private static readonly string Symbols = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        private readonly int _length = 6; //~5 * 10^10 combinations
        
        private readonly Random _random = new();

        public KeyGenerator()
        {
        }

        public KeyGenerator(int keyLength)
        {
            _length = keyLength;
        }

        public string GetKey()
        {
            return "/" + string.Join(string.Empty, Enumerable.Range(0, _length).Select(x => Symbols[Next()]));
        }

        private int Next()
        {
            return _random.Next(0, Symbols.Length); //could have used hash function instead, or a different random generator, but it comes down to much the same thing
        }
        
    }
}