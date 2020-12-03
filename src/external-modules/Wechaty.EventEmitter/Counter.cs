using System.Threading;

namespace EventEmitter

{
    internal class Counter
    {
        private int _count;

        public int Increment() => Interlocked.Increment(ref _count);

        public int Decrement() => Interlocked.Decrement(ref _count);
    }
}
