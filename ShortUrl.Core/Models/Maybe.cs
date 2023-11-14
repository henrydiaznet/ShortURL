using System;

namespace ShortUrl.Core.Models
{
    public class Maybe<T> where T: class
    {
        private readonly T _value;
        private readonly bool _isValueSet;

        public T Value => GetValueOrThrow();
        
        public bool HasValue => _isValueSet;
        public bool HasNoValue => !HasValue;
        
        private Maybe(T value)
        {
            if (value == null)
            {
                _isValueSet = false;
                _value = default;
                return;
            }

            _isValueSet = true;
            _value = value;
        }

        public T GetValueOrThrow()
        {
            if (HasNoValue)
            {
                throw new InvalidOperationException("Attempting to retrieve value when no value is set");
            }

            return _value;
        }

        public static Maybe<T> From(T value)
        {
            return new Maybe<T>(value);
        }
    }
}