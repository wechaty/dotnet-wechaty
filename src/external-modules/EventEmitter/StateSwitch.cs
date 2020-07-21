using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Wechaty
{
    /// <summary>
    /// 状态开关
    /// </summary>
    public class StateSwitch : EventEmitter
    {
        private static readonly object VoidResult = new object();
        private static int _counter;

        private static readonly Action NOP = () => { };

        private ILogger<StateSwitch> _logger;

        private Task _onTask;
        private Task _offTask;

        private Action _onResolver;
        private Action _offResolver;

        private bool _onoff;
        private bool _pending;

        public StateSwitch([AllowNull]string? name, [AllowNull]ILogger<StateSwitch>? logger = null)
        {
            Name = name ?? $"#{_counter++}";
            _logger = logger ?? NullLogger<StateSwitch>.Instance;
            _offTask = Task.CompletedTask;
            var source = new TaskCompletionSource<object>();
            _onTask = source.Task;
            _onResolver = () => source.SetResult(VoidResult);
            _offResolver = NOP;
        }

        public string Name { get; }

        public bool Pending
        {
            get
            {
                _logger.LogTrace($"{Name} Pending is {_pending}");
                return _pending;
            }
        }

        public void SetLog(ILogger<StateSwitch> logger) => _logger = logger ?? NullLogger<StateSwitch>.Instance;

        public State? On(State? state = null)
        {
            if (state.HasValue && !state.Value)
            {
                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    _logger.LogTrace($"{Name} On({state}) <- ({On()})");
                }
                _onoff = true;
                _pending = state == State.Pending;
                _ = Emit("on", state.Value);
                /**
                 * for ready()
                 */
                if (_offResolver == NOP)
                {
                    var source = new TaskCompletionSource<object>();
                    _offTask = source.Task;
                    _offResolver = () => source.SetResult(VoidResult);
                }
                if (state.Value == State.On && _onResolver != NOP)
                {
                    _onResolver();
                    _onResolver = NOP;
                }
                return null;
            }
            /**
             * Get
             */
            var on = _onoff ? (_pending ? State.Pending : State.On) : State.Off;
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace($"{Name} On() is {on}");
            }
            return on;
        }

        public State? Off(State? state = null)
        {
            if (state.HasValue && !state.Value)
            {
                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    _logger.LogTrace($"{Name} On({state}) <- ({Off()})");
                }
                _onoff = false;
                _pending = state.Value == State.Pending;

                Emit("off", state);
                /**
                 * for ready()
                 */
                if (_onResolver == NOP)
                {
                    var source = new TaskCompletionSource<object>();
                    _onTask = source.Task;
                    _onResolver = () => source.SetResult(VoidResult);
                }
                if (state == true && _offResolver != NOP)
                {
                    _offResolver();
                    _offResolver = NOP;
                }
                return null;
            }

            /**
             * Get
             */
            var off = !_onoff ? (_pending ? State.Pending : State.On) : State.Off;
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace($"{Name} On() is {off}");
            }
            return off;
        }

        public Task Ready(bool noCross = false) => Ready(State.On, noCross);

        public async Task Ready(State state, bool noCross = false)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace($"{Name} ready({state}, {noCross})");
            }
            if (state == State.On)
            {
                if (_onoff == false && noCross == true)
                {
                    throw new InvalidOperationException($"Ready(State.On) but the state is off. call Ready(on, true) to force crossWait");
                }
                await _onTask;
            }
            else if (state == State.Off)
            {
                if (_onoff == true && noCross == true)
                {
                    throw new InvalidOperationException($"Ready(State.Off) but the state is on. call Ready(State.Off, true) to force crossWait");
                }
                await _offTask;
            }
            else
            {
                throw new ArgumentException($"should not go here. ${state} should be of type 'never'");
            }
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace($"{Name} ready({state}, {noCross}) resolved.");
            }
        }
    }
}
