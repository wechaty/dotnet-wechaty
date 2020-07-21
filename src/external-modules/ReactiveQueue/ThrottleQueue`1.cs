using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveQueue
{
    public class ThrottleQueue<TElement> : ObservableBase<TElement>
    {
        private readonly Subject<TElement> _subject;
        private readonly TimeSpan _period;
        private readonly IObservable<TElement> _observable;

        public ThrottleQueue(TimeSpan period)
        {
            _subject = new Subject<TElement>();
            _period = period;
            var delay = Observable.Empty<TElement>().Delay(_period);
            _observable = _subject.Take(1).Concat(delay);
        }

        protected override IDisposable SubscribeCore(IObserver<TElement> observer) =>
            _observable.Subscribe(observer);

        public void OnNext(TElement element) => _subject.OnNext(element);

        public void OnCompleted() => _subject.OnCompleted();
    }
}
