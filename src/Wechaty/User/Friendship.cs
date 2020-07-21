using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Wechaty.Schemas;

namespace Wechaty.User
{
    public class Friendship : Accessory<Friendship>, IAcceptable
    {
        protected FriendshipPayload? Payload { get; set; }
        public string Id { get; }

        public Friendship([DisallowNull] string id,
            [DisallowNull] Wechaty wechaty,
            [DisallowNull] Puppet puppet,
            [DisallowNull] ILogger<Friendship> logger,
            [AllowNull] string? name = default) : base(wechaty, puppet, logger, name)
        {
            Id = id;
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"constructor{id}");
            }
            if (puppet == null)
            {
                throw new ArgumentNullException(nameof(puppet), "Friendship class can not be instanciated without a puppet!");
            }
        }

        public override string ToString() => $"Friendship#{Payload?.Type}<{Payload?.ContactId}>";

        public bool IsReady => Payload != null;

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

        public FriendshipType Type => Payload?.Type ?? FriendshipType.Unknown;

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

        public override Friendship ToImplement() => this;
    }
}
