using System;
using System.Net.WebSockets;
using System.Runtime.Serialization;
using System.Timers;
using Wechaty.EventEmitter;
using Wechaty.Schemas;

namespace Wechaty
{
    public class IoEventBuffer
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public enum IoEventName
    {
        
        [EnumMember(Value = "botie")] Botie,
        [EnumMember(Value = "error")] Error,
        [EnumMember(Value = "heartbeat")] Heartbeat,
        [EnumMember(Value = "jsonrpc")] Jsonrpc,
        [EnumMember(Value = "login")] Login,
        [EnumMember(Value = "logout")] Logout,
        [EnumMember(Value = "message")] Message,
        [EnumMember(Value = "raw")] Raw,
        [EnumMember(Value = "reset")] Reset,
        [EnumMember(Value = "scan")] Scan,
        [EnumMember(Value = "shutdown")] Shutdown,
        [EnumMember(Value = "sys")] Sys,
        [EnumMember(Value = "update")] Update
    }

    public class Io
    {
        private readonly string _id;
        private readonly string _protocol;
        private object[] _eventBuffer;
        private WebSocket? _ws;

        private readonly StateSwitch _state;

        private Timer? _reconnectTimer;
        private long? _reconnectTimeout;

        private Timer? _lifeTimer;

        private Func<object[], object> _anyFunction;

        private EventScanPayload? _scanPayload;

    }
}
