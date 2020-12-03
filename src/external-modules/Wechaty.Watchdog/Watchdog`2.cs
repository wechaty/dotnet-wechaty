using System;
using System.Diagnostics.CodeAnalysis;
using System.Timers;
using EventEmitter;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Wechaty
{
    public class Watchdog<TFoodType, TData> : EventEmitter<Watchdog<TFoodType, TData>>
    {
        private Optional<Timer> _timer;
        private Optional<long> _lastFeed;
        private Optional<WatchdogFood<TFoodType, TData>> _lastFood;
        private readonly ILogger<Watchdog<TFoodType, TData>> _logger;

        public long DefaultTimeout { get; set; }
        public string Name { get; set; }
        public Watchdog([DisallowNull] ILogger<Watchdog<TFoodType, TData>> logger)
            : this(60 * 1000, "Bark", logger)
        {
        }
        public Watchdog([DisallowNull] string name,
                        [DisallowNull] ILogger<Watchdog<TFoodType, TData>> logger)
            : this(60 * 1000, name, logger)
        {
        }
        public Watchdog([DisallowNull] long defaultTimeout,
                        [DisallowNull] ILogger<Watchdog<TFoodType, TData>> logger)
            : this(defaultTimeout, "Bark", logger)
        {
        }
        public Watchdog([DisallowNull] long defaultTimeout,
                        [DisallowNull] string name,
                        [DisallowNull] ILogger<Watchdog<TFoodType, TData>> logger)
        {
            DefaultTimeout = defaultTimeout;
            Name = name;
            _logger = logger;
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace($"{name}: constructor(name={name}, defaultTimeout={defaultTimeout})");
            }
        }

        public Watchdog<TFoodType, TData> On(WatchdogEvent @event, WatchdogListener<TFoodType, TData> listener)
            => this.On(@event.ToString(), listener);

        public Watchdog<TFoodType, TData> RemoveListener(WatchdogEvent @event, WatchdogListener<TFoodType, TData> listener)
            => this.RemoveListener(@event.ToString(), listener);

        private void StartTimer(long timeout)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace($"<{Name}> startTimer()");
            }
            if (_timer.HasValue)
            {
                throw new InvalidOperationException("timer already exist!");
            }
            _timer = new Timer
            {
                //elapsed once
                AutoReset = false,
                Interval = timeout
            };
            _timer.Value.Elapsed += (sender, e) =>
            {
                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    _logger.LogTrace($"<{Name}> startTimer() setTimeout() after {timeout}");
                }
                _ = Emit("reset", _lastFood.Value, (_lastFood.Value?.Timeout).GetValueOrDefault(DefaultTimeout));
            };
            _timer.Value.Start();
        }

        private void StopTimer(bool sleep = false)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace($"<{Name}> stopTimer()");
            }
            if (!_timer.HasValue)
            {
                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    _logger.LogTrace($"<{Name}> first run(or after sleep)");
                }
                return;
            }
            if (_timer.Value.Enabled)
            {
                _timer.Value.Stop();
            }
            else if (!sleep)
            {
                throw new InvalidOperationException("timer is already stoped!");
            }
        }

        public long Left()
        {
            long left;
            if (_lastFeed.HasValue)
            {
                left = _lastFeed.Value + DefaultTimeout - DateTime.Now.GetUnixEpochMillisecond();
                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    _logger.LogTrace($"<{Name}> timerLeft() = {left}");
                }
            }
            else
            {
                left = 0;
                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    _logger.LogTrace($"<{Name}> timerLeft() first feed, left = {left}");
                }
            }
            return left;
        }

        public long Feed(TData data) => Feed(new WatchdogFood<TFoodType, TData> { Data = data });

        public long Feed(WatchdogFood<TFoodType, TData> food)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace($"<{Name}> feed({JsonConvert.SerializeObject(food)})");
            }

            if (food.Timeout <= 0)
            {
                food.Timeout = DefaultTimeout;
            }

            var left = Left();

            StopTimer();
            StartTimer(food.Timeout);

            _lastFeed = DateTime.Now.GetUnixEpochMillisecond();
            _lastFood = food;

            _ = Emit("feed", food, left);

            return left;
        }

        public void Sleep()
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace($"<{Name}> sleep()");
            }
            StopTimer(true);
            _timer = default;
            _ = Emit("sleep", _lastFood, Left());
        }

        public override Watchdog<TFoodType, TData> ToImplement => this;
    }
}
