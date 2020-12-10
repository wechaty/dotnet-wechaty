using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Threading;

namespace Wechaty.Memorycard
{
    public static class EnumerableAsyncExtensions
    {
        public static IAsyncEnumerable<T> AsAsyncEnumerable<T>(this IEnumerable<T> source) => new AsyncEnumerable<T>(source);

        private struct AsyncEnumerable<T> : IAsyncEnumerable<T>
        {
            private readonly IEnumerable<T> _source;

            public AsyncEnumerable([DisallowNull] IEnumerable<T> source) => _source = source;

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new AsyncEnumerator<T>(_source.GetEnumerator(), cancellationToken);
        }

        private struct AsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _enumerator;
            private readonly CancellationToken _cancellationToken;

            public AsyncEnumerator([DisallowNull] IEnumerator<T> enumerator, CancellationToken cancellationToken = default)
            {
                _enumerator = enumerator;
                _cancellationToken = cancellationToken;
            }

            public T Current => _enumerator.Current;

#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
            public async ValueTask DisposeAsync()
            {
                _cancellationToken.ThrowIfCancellationRequested();
                _enumerator.Dispose();
            }
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行

#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
            public async ValueTask<bool> MoveNextAsync()
            {
                _cancellationToken.ThrowIfCancellationRequested();
                return _enumerator.MoveNext();
            }
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        }
    }
}
