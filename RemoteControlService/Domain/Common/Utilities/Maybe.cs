using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Domain.Common.Utilities
{
    public class Maybe<T> : IEnumerable<T>
    {
        private IEnumerable<T> Content { get; }

        private Maybe(IEnumerable<T> content)
        {
            this.Content = content;
        }

        public static Maybe<T> Some(T value) => new Maybe<T>(new[] { value });

        public static Maybe<T> None() => new Maybe<T>(new T[0]);

        public IEnumerator<T> GetEnumerator() => this.Content.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
