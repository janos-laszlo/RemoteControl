#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Domain.Common.Utilities
{
    public struct Maybe<T> : IEnumerable<T>
    {
        private IEnumerable<T> Content { get; }

        private Maybe(IEnumerable<T> content)
        {
            this.Content = content;
        }

        public static Maybe<T> Some(T value) => new(new[] { value });

        public static Maybe<T> None() => new(new T[0]);

        public IEnumerator<T> GetEnumerator() => this.Content.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public static implicit operator Maybe<T>(T value) =>
            value is null ? Maybe<T>.None() : Maybe<T>.Some(value);

        public TResult Match<TResult>(
            Func<TResult> none, Func<T, TResult> some)
        {
            return this.Any() ? some(this.First()) : none();

        }

        public void Match(
            Action none, Action<T> some)
        {
            if (this.Any())
            {
                some(this.First());
            }
            else
            {
                none();
            }
        }
    }
}
