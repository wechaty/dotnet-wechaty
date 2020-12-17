using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Wechaty.Module.Puppet.Schemas;

namespace Wechaty.User
{
    /// <summary>
    /// friendship
    /// </summary>
    public class Friendship : Accessory<Friendship>, IAcceptable
    {
        /// <summary>
        /// payload
        /// </summary>
        protected FriendshipPayload? Payload { get; set; }

        /// <summary>
        /// id
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// init <see cref="Friendship"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="wechaty"></param>
        /// <param name="logger"></param>
        /// <param name="name"></param>
        public Friendship([DisallowNull] string id,
            [DisallowNull] Wechaty wechaty,
            [DisallowNull] ILogger<Friendship> logger,
            [AllowNull] string? name = default) : base(wechaty, logger, name)
        {
            Id = id;
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"constructor{id}");
            }
        }

        ///<inheritdoc/>
        public override string ToString() => $"Friendship#{Payload?.Type}<{Payload?.ContactId}>";

        /// <summary>
        /// check payload is ready?
        /// </summary>
        public bool IsReady => Payload != null;

        /// <summary>
        /// make sure payload is ready
        /// </summary>
        /// <returns></returns>
        public async Task Ready()
        {
            if (IsReady)
            {
                return;
            }
            Payload = await Puppet.GetFriendshipPayload(Id);
            if (Payload == null)
            {
                throw new InvalidOperationException("no payload");
            }
            await Contact.Ready();
        }

        /// <summary>
        /// accept friendship
        /// </summary>
        /// <returns></returns>
        public async Task Accept()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"accept()");
            }
            if (Payload == null)
            {
                throw new InvalidOperationException("no payload");
            }
            if (Payload.Type != FriendshipType.Receive)
            {
                throw new InvalidOperationException($"accept() need type to be FriendshipType.Receive, but it got a {Payload.Type}");
            }
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"accept() to {Payload.ContactId}");
            }
            await Puppet.FriendshipAccept(Id);
            var contact = Contact;
            await Policy.Handle<Exception>()
                 .WaitAndRetryAsync(10, (attempt) => TimeSpan.FromSeconds((1d - new Random().NextDouble()) * Math.Pow(2, attempt)), (exception, attempt) =>
                    {
                        if (Logger.IsEnabled(LogLevel.Trace))
                        {
                            Logger.LogTrace($"accept() retry() ready() attempt {attempt}");
                        }
                    }).ExecuteAsync(async () =>
                    {
                        await contact.Ready();
                        if (IsReady)
                        {
                            if (Logger.IsEnabled(LogLevel.Trace))
                            {
                                Logger.LogTrace($"accept() with contact {contact.Name} ready()");
                            }
                            return;
                        }
                        throw new InvalidOperationException("Friendship.accept() contact.ready() not ready");
                    }).ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Logger.LogWarning($"accept() contact {contact} not ready because of {task.Exception?.Message}");
                        }
                    });
            await contact.Sync();
        }

        /// <summary>
        /// hello
        /// </summary>
        public string Hello
        {
            get
            {
                if (Payload == null)
                {
                    throw new InvalidOperationException("no payload");
                }
                return Payload.Hello ?? "";
            }
        }

        /// <summary>
        /// contact
        /// </summary>
        public Contact Contact
        {
            get
            {
                if (Payload == null)
                {
                    throw new InvalidOperationException("no payload");
                }
                return WechatyInstance.Contact.Load(Payload.ContactId);
            }
        }

        /// <summary>
        /// firendship 
        /// </summary>
        public FriendshipType Type => Payload?.Type ?? FriendshipType.Unknown;

        /// <summary>
        /// to json
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"toJSON()");
            }
            if (IsReady)
            {
                throw new InvalidOperationException($"Friendship<{Id}> needs to be ready. Please call ready() before toJSON()");
            }
            return JsonConvert.SerializeObject(Payload);
        }

        ///<inheritdoc/>
        public override Friendship ToImplement => this;
    }
}
