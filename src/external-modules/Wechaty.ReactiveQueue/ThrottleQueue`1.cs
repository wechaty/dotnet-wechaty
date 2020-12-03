using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveQueue
{
    /// <summary>
    /// Throttle Queue
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    public class ThrottleQueue<TElement> : ObservableBase<TElement>
    {
        private readonly Subject<TElement> _subject;
        private readonly TimeSpan _period;
        private readonly IObservable<TElement> _observable;

        /// <summary>
        /// Throttle Queue ctor.
        /// </summary>
        /// <param name="period">delay</param>
        public ThrottleQueue(TimeSpan period)
        {
            _subject = new Subject<TElement>();
            _period = period;
            var delay = Observable.Empty<TElement>().Delay(_period);
            _observable = _subject.Take(1).Concat(delay);
        }

        ///<inheritdoc/>
        protected override IDisposable SubscribeCore(IObserver<TElement> observer) =>
            _observable.Subscribe(observer);

        /// <summary>
        /// Notifies all subscribed observers about the arrival of the specified element in the sequence.
        /// </summary>
        /// <param name="element"></param>
        public void OnNext(TElement element) => _subject.OnNext(element);

        /// <summary>
        /// Notifies all subscribed observers about the end of the sequence.
        /// </summary>
        public void OnCompleted() => _subject.OnCompleted();
    }
}
