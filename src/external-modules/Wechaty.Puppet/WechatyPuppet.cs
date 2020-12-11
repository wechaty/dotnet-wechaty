using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wechaty.EventEmitter;
using Wechaty.Filebox;
using Wechaty.Memorycard;
using Wechaty.Reactivequeue;
using Wechaty.Schemas;
using Wechaty.Watchdog;
using Wechaty.Watchdog;

namespace Wechaty
{
    public abstract class WechatyPuppet : EventEmitter<WechatyPuppet>
    {
        public StateSwitch State { get; }

        protected LRUCache<string, ContactPayload> CacheContactPayload { get; }
        protected LRUCache<string, FriendshipPayload> CacheFriendshipPayload { get; }
        protected LRUCache<string, MessagePayload> CacheMessagePayload { get; }
        protected LRUCache<string, RoomPayload> CacheRoomPayload { get; }
        protected LRUCache<string, RoomMemberPayload> CacheRoomMemberPayload { get; }
        protected LRUCache<string, RoomInvitationPayload> CacheRoomInvitationPayload { get; }

        protected int Counter { get; }

        protected MemoryCard Memory { get; set; }

        protected PuppetOptions Options { get; }

        protected ILogger<WechatyPuppet> Logger { get; }

        public string? SelfId
        {
            get
            {
                if (Logger.IsEnabled(LogLevel.Trace))
                {
                    Logger.LogTrace("selfId()");
                }
                if (string.IsNullOrEmpty(_id))
                {
                    throw new InvalidOperationException("not logged in, no this.id yet.");
                }
                return _id;
            }
            protected set
            {
                if (!string.IsNullOrEmpty(_id))
                {
                    throw new InvalidOperationException("must logout first before login again!");
                }
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }
                _id = value;
                _ = Emit(new EventLoginPayload { ContactId = value });
            }
        }

        protected WatchDog<object, EventHeartbeatPayload> Watchdog { get; }

        private string? _id;
        private readonly ThrottleQueue<string> _resetThrottleQueue;

        protected WechatyPuppet(PuppetOptions options, ILogger<WechatyPuppet> logger, ILoggerFactory loggerFactory)
        {
            Options = options;
            Logger = logger;
            State = new StateSwitch(GetType().Name, loggerFactory.CreateLogger<StateSwitch>());
            Memory = new MemoryCard((MemoryCardOptions?)null, loggerFactory.CreateLogger<MemoryCard>(), loggerFactory);
            //load here is for testing only
            Memory.Load()
                .ContinueWith(task =>
                {
                    if (!task.IsFaulted)
                    {
                        if (Logger.IsEnabled(LogLevel.Trace))
                        {
                            Logger.LogTrace("constructor() memory.load() done");
                        }
                    }
                    else
                    {
                        Logger.LogWarning("constructor() memory.load() rejection", task.Exception);
                    }
                });
            // 1. Setup Watchdog
            // puppet implementation class only need to do one thing:
            // feed the watchdog by `this.emit('heartbeat', ...)`

            // TODO  Darren 注释
            //Watchdog = new Watchdog<object, EventHeartbeatPayload>(loggerFactory.CreateLogger<Watchdog<object, EventHeartbeatPayload>>());
            //_ = this.On<WechatyPuppet, EventHeartbeatPayload>("heartbeat", payload => _ = Watchdog.Feed(payload));
            //_ = Watchdog
            //    .On<Watchdog<object, EventHeartbeatPayload>, EventHeartbeatPayload>("reset", lastFood =>
            //    {
            //        Logger.LogWarning($"constructor() watchdog.on(reset) reason: {JsonConvert.SerializeObject(lastFood)}");
            //        _ = Emit("reset", lastFood);
            //    });


            // 2. Setup `reset` Event via a 1 second Throttle Queue:
            _resetThrottleQueue = new ThrottleQueue<string>(TimeSpan.FromSeconds(1));

            // 2.2. handle all `reset` events via the resetThrottleQueue
            _ = this.On<WechatyPuppet, EventHeartbeatPayload>("reset", payload =>
            {
                if (Logger.IsEnabled(LogLevel.Trace))
                {
                    Logger.LogTrace($"constructor() this.on(reset) payload: {JsonConvert.SerializeObject(payload)}");
                }
                _resetThrottleQueue.OnNext(payload.Data);
            });

            // 2.3. call reset() and then ignore the following `reset` event for 1 second
            _resetThrottleQueue.Subscribe(reason =>
            {
                if (Logger.IsEnabled(LogLevel.Trace))
                {
                    Logger.LogTrace($"constructor() resetThrottleQueue.subscribe() reason: \"{reason}\"");
                }
                Reset(reason);
            });

            // 3. Setup LRU Caches
            CacheContactPayload = new LRUCache<string, ContactPayload>(3000);
            CacheFriendshipPayload = new LRUCache<string, FriendshipPayload>(3000);
            CacheMessagePayload = new LRUCache<string, MessagePayload>(3000);
            CacheRoomPayload = new LRUCache<string, RoomPayload>(3000);
            CacheRoomMemberPayload = new LRUCache<string, RoomMemberPayload>(3000);
            CacheRoomInvitationPayload = new LRUCache<string, RoomInvitationPayload>(3000);
        }

        public override string ToString() => string.Concat("Puppet#", Counter, "<", GetType().Name, ">", "(", Memory.Name ?? "", ")");

        public void SetMemory(MemoryCard memory)
        {
            if (!string.IsNullOrEmpty(Memory.Name))
            {
                throw new InvalidOperationException("puppet has already had a named memory: " + Memory.Name);
            }
            Memory = memory;
        }

        public virtual bool Emit(EventDongPayload payload) => Emit("dong", payload);
        public virtual bool Emit(EventErrorPayload payload) => Emit("error", payload);
        public virtual bool Emit(EventFriendshipPayload payload) => Emit("friendship", payload);
        public virtual bool Emit(EventLoginPayload payload) => Emit("login", payload);
        public virtual bool Emit(EventLogoutPayload payload) => Emit("logout", payload);
        public virtual bool Emit(EventMessagePayload payload) => Emit("message", payload);
        public virtual bool Emit(EventResetPayload payload) => Emit("reset", payload);
        public virtual bool Emit(EventRoomInvitePayload payload) => Emit("room-invite", payload);
        public virtual bool Emit(EventRoomJoinPayload payload) => Emit("room-join", payload);
        public virtual bool Emit(EventRoomLeavePayload payload) => Emit("room-leave", payload);
        public virtual bool Emit(EventRoomTopicPayload payload) => Emit("room-topic", payload);
        public virtual bool Emit(EventReadyPayload payload) => Emit("ready", payload);
        public virtual bool Emit(EventScanPayload payload) => Emit("scan", payload);
        public virtual bool Emit(EventHeartbeatPayload payload) => Emit("heartbeat", payload);

        public virtual WechatyPuppet OnDong(Action<EventDongPayload> listener) => this.On("dong", listener);
        public virtual WechatyPuppet OnError(Action<EventErrorPayload> listener) => this.On("error", listener);
        public virtual WechatyPuppet OnFriendship(Action<EventFriendshipPayload> listener) => this.On("friendship", listener);
        public virtual WechatyPuppet OnLogin(Action<EventLoginPayload> listener) => this.On("login", listener);
        public virtual WechatyPuppet OnLogout(Action<EventLogoutPayload> listener) => this.On("logout", listener);
        public virtual WechatyPuppet OnMessage(Action<EventMessagePayload> listener) => this.On("message", listener);
        public virtual WechatyPuppet OnReset(Action<EventResetPayload> listener) => this.On("reset", listener);
        public virtual WechatyPuppet OnRoomJoin(Action<EventRoomJoinPayload> listener) => this.On("room-join", listener);
        public virtual WechatyPuppet OnRoomLeave(Action<EventRoomLeavePayload> listener) => this.On("room-leave", listener);
        public virtual WechatyPuppet OnRoomTopic(Action<EventRoomTopicPayload> listener) => this.On("room-topic", listener);
        public virtual WechatyPuppet OnRoomInvite(Action<EventRoomInvitePayload> listener) => this.On("room-invite", listener);
        public virtual WechatyPuppet OnScan(Action<EventScanPayload> listener) => this.On("scan", listener);
        public virtual WechatyPuppet OnReady(Action<EventReadyPayload> listener) => this.On("ready", listener);
        public virtual WechatyPuppet OnHeartbeat(Action<EventHeartbeatPayload> listener) => this.On("heartbeat", listener);

        public void RemoveListener(Action<EventDongPayload> listener) => this.RemoveListener("dong", listener);
        public void RemoveListener(Action<EventErrorPayload> listener) => this.RemoveListener("error", listener);
        public void RemoveListener(Action<EventFriendshipPayload> listener) => this.RemoveListener("friendship", listener);
        public void RemoveListener(Action<EventLoginPayload> listener) => this.RemoveListener("login", listener);
        public void RemoveListener(Action<EventLogoutPayload> listener) => this.RemoveListener("logout", listener);
        public void RemoveListener(Action<EventMessagePayload> listener) => this.RemoveListener("message", listener);
        public void RemoveListener(Action<EventResetPayload> listener) => this.RemoveListener("reset", listener);
        public void RemoveListener(Action<EventRoomJoinPayload> listener) => this.RemoveListener("room-join", listener);
        public void RemoveListener(Action<EventRoomLeavePayload> listener) => this.RemoveListener("room-leave", listener);
        public void RemoveListener(Action<EventRoomTopicPayload> listener) => this.RemoveListener("room-topic", listener);
        public void RemoveListener(Action<EventRoomInvitePayload> listener) => this.RemoveListener("room-invite", listener);
        public void RemoveListener(Action<EventScanPayload> listener) => this.RemoveListener("scan", listener);
        public void RemoveListener(Action<EventReadyPayload> listener) => this.RemoveListener("ready", listener);
        public void RemoveListener(Action<EventHeartbeatPayload> listener) => this.RemoveListener("heartbeat", listener);


        public abstract Task StartGrpc();

        public async Task Start()
        {
            await StartGrpc();
            //OnHeartbeat(FeedDog);
            //Watchdog.On(WatchdogEvent.Reset, DogReset);
            OnReset(ThrottleReset);
        }

        public async Task Stop()
        {
            //RemoveListener(FeedDog);
            //Watchdog.RemoveListener(WatchdogEvent.Reset, DogReset);
            RemoveListener(ThrottleReset);

            //Watchdog.Sleep();
        }

        private void FeedDog(EventHeartbeatPayload payload) => Watchdog.Feed(payload);

        private void DogReset(WatchDogFood<object, EventHeartbeatPayload> lastFood, long time) => Emit(lastFood.Data);

        private void ThrottleReset(EventResetPayload payload)
        {
            if (_resetThrottleQueue != null)
            {
                _resetThrottleQueue.OnNext(payload.Data);
            }
        }

        private void Reset(string reason)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"reset({reason}");
            }
            Task.Run(async () =>
            {
                await Stop();
                await Start();
            }).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Logger.LogWarning($"reset() failed.", task.Exception);
                    _ = Emit(new EventErrorPayload
                    {
#pragma warning disable CS8602 // 取消引用可能出现的空引用。
                        Data = task.Exception.Message,
                        Exception = task.Exception
#pragma warning restore CS8602 // 取消引用可能出现的空引用。
                    });
                }
            })
            .GetAwaiter()
            .GetResult();
        }

#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        protected async Task Login(string userId)
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"login({userId})");
            }
            SelfId = userId;

            Emit(new EventLoginPayload { ContactId = userId });
        }

        public async Task Logout(string reason = "logout()")
        {
            if (string.IsNullOrWhiteSpace(_id))
            {
                throw new InvalidOperationException("must login first before logout!");
            }
            Emit(new EventLogoutPayload
            {
                ContactId = _id,
                Data = reason
            });
            _id = null;
        }

        public bool Logonoff() => !string.IsNullOrEmpty(_id);

        /// <summary>
        /// Check whether the puppet is work property.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>`false` if something went wrong, else if everything is OK</returns>
        public abstract void Ding(string? data);

        #region ContactSelf
        public abstract Task ContactSelfName(string name);
        public abstract Task<string> ContactSelfQRCode();
        public abstract Task ContactSelfSignature(string signature);
        #endregion

        #region Tag
        /// <summary>
        /// add a tag for a Contact. Create it first if it not exist.
        /// </summary>
        /// <param name="tagId"></param>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public abstract Task TagContactAdd(string tagId, string contactId);
        /// <summary>
        /// delete a tag from Wechat
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public abstract Task TagContactDelete(string tagId);
        /// <summary>
        /// get tags from a specific Contact
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public abstract Task<List<string>> TagContactList(string contactId);
        /// <summary>
        /// get tags from all Contacts
        /// </summary>
        /// <returns></returns>
        public abstract Task<List<string>> TagContactList();
        /// <summary>
        /// remove a tag from the Contact
        /// </summary>
        /// <param name="tagId"></param>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public abstract Task TagContactRemove(string tagId, string contactId);
        #endregion

        #region Contact
        public abstract Task<string> ContactAlias(string contactId);
        public abstract Task ContactAlias(string contactId, string? alias);
        public abstract Task<FileBox> ContactAvatar(string contactId);
        public abstract Task ContactAvatar(string contactId, FileBox file);
        public abstract Task<List<string>> ContactList();
        protected abstract Task<ContactPayload> ContactRawPayload(string contactId);
        protected abstract Task<ContactPayload> ContactRawPayloadParser(ContactPayload rawPayload);

        public async Task<List<string>> ContactRoomList(string contactId)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"contactRoomList({contactId})");
            }
            var roomIdList = await RoomList();

            var result = new List<string>();

            foreach (var roomId in roomIdList)
            {
                var roomPayload = await RoomPayload(roomId);
                if (roomPayload.MemberIdList.Contains(contactId))
                {
                    result.Add(roomPayload.Id);
                }
            }

            return result;
        }

#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        public async Task ContactPayloadDirty(string contactId)
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"contactPayloadDirty({contactId})");
            }
            _ = CacheContactPayload.Delete(contactId);
        }

        public async Task<IReadOnlyList<string>> ContactSearch(ContactQueryFilter? query, params string[]? searchIdList)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"contactSearch(query={JsonConvert.SerializeObject(query)}, searchIdList.length{searchIdList?.Length})");
            }
            if (searchIdList.Count() == 0)
            {
                searchIdList = (await ContactList()).ToArray();
            }
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"contactSearch(), searchIdList.length{searchIdList.Length})");
            }
            if (query == null)
            {
                return searchIdList;
            }
            var filterFuncion = query.Every<ContactQueryFilter, ContactPayload>();

            const int BATCH_SIZE = 16;
            var batchIndex = 0;

            var resultIdList = new List<string>();
            while (BATCH_SIZE * batchIndex < searchIdList.Length)
            {
                var batchSearchIdList = searchIdList.Skip(
                  BATCH_SIZE * batchIndex
                ).Take(BATCH_SIZE);

                var matchBatchIdFutureList = batchSearchIdList.Select(Matcher);
                var matchBatchIdList = await Task.WhenAll(matchBatchIdFutureList.ToArray());

                var batchSearchIdResultList = matchBatchIdList.Where(t => !string.IsNullOrWhiteSpace(t));

#pragma warning disable CS8620 // 由于引用类型的可为 null 性差异，实参不能用于形参。
                resultIdList.AddRange(batchSearchIdResultList);
#pragma warning restore CS8620 // 由于引用类型的可为 null 性差异，实参不能用于形参。
                batchIndex++;
            }
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"contactSearch() searchContactPayloadList.length = {resultIdList.Count})");
            }

            return resultIdList;

            async Task<string?> Matcher(string id)
            {
                try
                {
                    // Does LRU cache matter at here?
                    // const rawPayload = await this.contactRawPayload(id)
                    // const payload    = await this.contactRawPayloadParser(rawPayload)
                    var payload = await ContactPayload(id);
                    if (payload != null && filterFuncion(payload))
                    {
                        return id;
                    }
                }
                catch (Exception exception)
                {
                    if (Logger.IsEnabled(LogLevel.Trace))
                    {
                        Logger.LogTrace(exception, $"contactSearch() contactPayload failed.");
                    }
                    await ContactPayloadDirty(id);
                }
                return null;
            }
        }

        /// <summary>
        /// Check a Contact Id if it's still valid.
        /// For example: talk to the server, and see if it should be deleted in the local cache.
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        public async Task<bool> ContactValidate(string contactId)
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"contactValidate({contactId}) base class just return `true`");
            }
            return true;
        }

        protected ContactPayload? ContactPayloadCache(string contactId)
        {
            if (string.IsNullOrWhiteSpace(contactId))
            {
                throw new ArgumentException("no id");
            }
            var cachedPayload = CacheContactPayload.Get(contactId);

            if (cachedPayload == null)
            {
                if (Logger.IsEnabled(LogLevel.Trace))
                {
                    Logger.LogTrace($"contactPayload({contactId}) cache MISS");
                }
                // log.silly('Puppet', 'contactPayload(%s) cache HIT', contactId)
            }
            return cachedPayload;
        }

        public async Task<ContactPayload> ContactPayload(string contactId)
        {

            if (string.IsNullOrWhiteSpace(contactId))
            {
                throw new ArgumentException("no id");
            }

            //1. Try to get from cache first
            var cachedPayload = ContactPayloadCache(contactId);
            if (cachedPayload != null)
            {
                return cachedPayload;
            }

            //2. Cache not found
            var rawPayload = await ContactRawPayload(contactId);
            var payload = await ContactRawPayloadParser(rawPayload);

            CacheContactPayload.Set(contactId, payload);
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"contactPayload({contactId}) cache SET");
            }

            return payload;
        }
        #endregion

        #region Friendship
        public abstract Task FriendshipAccept(string friendshipId);
        public abstract Task FriendshipAdd(string contactId, string? hello);
        public abstract Task<string?> FriendshipSearchPhone(string phone);
        public abstract Task<string?> FriendshipSearchWeixin(string weixin);
        protected abstract Task<FriendshipPayload> FriendshipRawPayload(string friendshipId);
        protected abstract Task<FriendshipPayload> FriendshipRawPayloadParser(FriendshipPayload rawPayload);

        public async Task<string?> FriendshipSearch(FriendshipSearchCondition searchQueryFilter)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"friendshipSearch({JsonConvert.SerializeObject(searchQueryFilter)})");
            }

            if (!string.IsNullOrWhiteSpace(searchQueryFilter.Phone))
            {
                return await FriendshipSearchPhone(searchQueryFilter.Phone);
            }
            else if (!string.IsNullOrWhiteSpace(searchQueryFilter.Weixin))
            {
                return await FriendshipSearchWeixin(searchQueryFilter.Weixin);
            }
            throw new ArgumentException($"unknown key from searchQueryFilter: {JsonConvert.SerializeObject(searchQueryFilter) }");
        }

        protected FriendshipPayload? FriendshipPayloadCache(string friendshipId)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"friendshipPayloadCache(id={friendshipId}) @ {this}");
            }

            if (string.IsNullOrWhiteSpace(friendshipId))
            {
                throw new ArgumentException("no id");
            }
            var cachedPayload = CacheFriendshipPayload.Get(friendshipId);

            if (cachedPayload == null)
            {
                if (Logger.IsEnabled(LogLevel.Trace))
                {
                    Logger.LogTrace($"friendshipPayloadCache({friendshipId}) cache MISS");
                }
            }

            return cachedPayload;
        }

#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        protected async Task FriendshipPayloadDirty(string friendshipId)
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"friendshipPayloadDirty({friendshipId})");
            }
            _ = CacheFriendshipPayload.Delete(friendshipId);
        }

        /// <summary>
        /// Get and Set
        /// </summary>
        /// <param name="friendshipId"></param>
        /// <returns></returns>
        public async Task<FriendshipPayload> GetFriendshipPayload(string friendshipId)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"friendshipPayload({friendshipId})");
            }
            //1. Try to get from cache first
            var cachedPayload = FriendshipPayloadCache(friendshipId);
            if (cachedPayload != null)
            {
                return cachedPayload;
            }

            //2. Cache not found
            var rawPayload = await FriendshipRawPayload(friendshipId);
            var payload = await FriendshipRawPayloadParser(rawPayload);

            CacheFriendshipPayload.Set(friendshipId, payload);

            return payload;
        }
#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        public async Task SetFriendshipPayload(string friendshipId, FriendshipPayload newPayload)
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"friendshipPayload({friendshipId}){JsonConvert.SerializeObject(newPayload)}");
            }
            _ = CacheFriendshipPayload.Set(friendshipId, newPayload);
        }
        #endregion

        #region Message
        public abstract Task<string> MessageContact(string messageId);
        public abstract Task<FileBox> MessageFile(string messageId);
        public abstract Task<FileBox> MessageImage(string messageId, ImageType imageType);
        public abstract Task<MiniProgramPayload> MessageMiniProgram(string messageId);
        public abstract Task<UrlLinkPayload> MessageUrl(string messageId);

        public abstract Task<string?> MessageSendContact(string conversationId, string contactId);
        public abstract Task<string?> MessageSendFile(string conversationId, FileBox file);
        public abstract Task<string?> MessageSendMiniProgram(string conversationId, MiniProgramPayload miniProgramPayload);
        public abstract Task<string?> MessageSendText(string conversationId, string text, params string[]? mentionIdList);
        public abstract Task<string?> MessageSendText(string conversationId, string text, IEnumerable<string>? mentionIdList);
        public abstract Task<string?> MessageSendUrl(string conversationId, UrlLinkPayload urlLinkPayload);

        public abstract Task<bool> MessageRecall(string messageId);
        protected abstract Task<MessagePayload> MessageRawPayload(string messageId);
        protected abstract MessagePayload MessageRawPayloadParser(MessagePayload rawPayload);

        protected MessagePayload? MessagePayloadCache(string messageId)
        {
            if (string.IsNullOrWhiteSpace(messageId))
            {
                throw new ArgumentException("no id");
            }
            var cachedPayload = CacheMessagePayload.Get(messageId);
            if (cachedPayload == null)
            {
                if (Logger.IsEnabled(LogLevel.Trace))
                {
                    Logger.LogTrace($"messagePayloadCache({messageId}) cache MISS");
                }
            }

            return cachedPayload;
        }

#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        protected async Task MessagePayloadDirty(string messageId)
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"messagePayloadDirty({messageId})");
            }
            CacheMessagePayload.Delete(messageId);
        }

        public async Task<MessagePayload?> MessagePayload(string messageId)
        {
            try
            {
                if (Logger.IsEnabled(LogLevel.Trace))
                {
                    Logger.LogTrace($"messagePayload({messageId})");
                }

                if (string.IsNullOrWhiteSpace(messageId))
                {
                    throw new ArgumentException("no id");
                }

                //1. Try to get from cache first
                var cachedPayload = MessagePayloadCache(messageId);
                if (cachedPayload != null)
                {
                    return cachedPayload;
                }

                //2. Cache not found
                var rawPayload = await MessageRawPayload(messageId);
                var payload = MessageRawPayloadParser(rawPayload);

                CacheMessagePayload.Set(messageId, payload);
                if (Logger.IsEnabled(LogLevel.Trace))
                {
                    Logger.LogTrace($"messagePayload({messageId}) cache SET");
                }

                return payload;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"messagePayload() exception,messageId:{messageId}");
                return null;
            }
        }

        public IReadOnlyList<string> MessageList()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"messageList()");
            }
            return CacheMessagePayload.Keys;
        }

        public async Task<IReadOnlyList<string>> MessageSearch(MessageQueryFilter? query)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"messageSearch({JsonConvert.SerializeObject(query)})");
            }

            var allMessageIdList = MessageList();
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"messageSearch() allMessageIdList.length={allMessageIdList.Count}");
            }

            if (query == null)
            {
                return allMessageIdList;
            }

            var result = new List<string>();

            var filterFunction = query.Every<MessageQueryFilter, MessagePayload>();

            foreach (var item in allMessageIdList)
            {
                var payload = await MessagePayload(item);
                if (payload != null && filterFunction(payload))
                {
                    result.Add(payload.Id);
                }
            }

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"messageSearch() messageIdList filtered. result length={result.Count}");
            }

            return result;
        }

        public async Task<string?> MessageForward(string conversationId, string messageId)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"messageForward({conversationId}, {messageId})");
            }

            var payload = await MessagePayload(messageId);

            if (payload == null)
            {
                return null;
            }

            string? newMsgId = null;

            switch (payload.Type)
            {
                case MessageType.Attachment:     // Attach(6),
                case MessageType.Audio:          // Audio(1), Voice(34)
                case MessageType.Image:          // Img(2), Image(3)
                case MessageType.Video:          // Video(4), Video(43)
                    var fileBox = await MessageFile(messageId);
                    newMsgId = await MessageSendFile(conversationId, fileBox);
                    break;


                case MessageType.Text:           // Text(1)
                    if (payload.Text != null)
                    {
                        newMsgId = await MessageSendText(conversationId, payload.Text);
                    }
                    else
                    {
                        Logger.LogWarning($"messageForward() payload.text is undefined.");
                    }
                    break;
                case MessageType.MiniProgram:    // MiniProgram(33)
                    newMsgId = await MessageSendMiniProgram(conversationId, await MessageMiniProgram(messageId));
                    break;
                case MessageType.Url:            // Url(5)
                    await MessageSendUrl(conversationId, await MessageUrl(messageId));
                    break;
                case MessageType.Contact:        // ShareCard(42)
                    newMsgId = await MessageSendContact(conversationId, await MessageContact(messageId));
                    break;
                case MessageType.ChatHistory:    // ChatHistory(19)
                case MessageType.Location:       // Location(48)
                case MessageType.Emoticon:       // Sticker: Emoticon(15), Emoticon(47)
                case MessageType.Transfer:
                case MessageType.RedEnvelope:
                case MessageType.Recalled:       // Recalled(10002)
                    throw new NotSupportedException("Wechaty Puppet Unsupported API Error.Learn More At https://github.com/wechaty/wechaty-puppet/wiki/Compatibility .");
                case MessageType.Unknown:
                default:
                    throw new InvalidOperationException("Unsupported forward message type:" + payload.Type);
            }
            return newMsgId;
        }
        #endregion

        #region Room Invitation
        protected RoomInvitationPayload? RoomInvitationPayloadCache(string roomInvitationId)
        {
            if (string.IsNullOrWhiteSpace(roomInvitationId))
            {
                throw new ArgumentException("no id");
            }
            var cachedPayload = CacheRoomInvitationPayload.Get(roomInvitationId);

            if (cachedPayload == null)
            {
                if (Logger.IsEnabled(LogLevel.Trace))
                {
                    Logger.LogTrace($"roomInvitationPayloadCache({roomInvitationId}) cache MISS");
                }
            }

            return cachedPayload;
        }

        public abstract Task RoomInvitationAccept(string roomInvitationId);

        protected abstract Task<RoomInvitationPayload> RoomInvitationRawPayload(string roomInvitationId);
        protected abstract Task<RoomInvitationPayload> RoomInvitationRawPayloadParser(RoomInvitationPayload rawPayload);

        /// <summary>
        /// get room inviatation payload 
        /// </summary>
        /// <param name="roomInvitationId"></param>
        /// <returns></returns>
        public async Task<RoomInvitationPayload> GetRoomInvitationPayload(string roomInvitationId)
        {
            //1. Try to get from cache first
            var cachedPayload = RoomInvitationPayloadCache(roomInvitationId);
            if (cachedPayload != null)
            {
                return cachedPayload;
            }

            //2. Cache not found
            var rawPayload = await RoomInvitationRawPayload(roomInvitationId);
            var payload = await RoomInvitationRawPayloadParser(rawPayload);

            return payload;
        }

        /// <summary>
        /// set room inviatation payload
        /// </summary>
        /// <param name="roomInvitationId"></param>
        /// <param name="newPayload"></param>
        /// <returns></returns>
#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        public async Task SetRoomInvitationPayload(string roomInvitationId, RoomInvitationPayload newPayload) => CacheRoomInvitationPayload.Set(roomInvitationId, newPayload);
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        #endregion

        #region Room
        public abstract Task RoomAdd(string roomId, string contactId);
        public abstract Task<FileBox> RoomAvatar(string roomId);
        public abstract Task<string> RoomCreate(IEnumerable<string> contactIdList, string? topic);
        public abstract Task<string> RoomCreate(string[] contactIdList, string? topic);
        public abstract Task RoomDel(string roomId, string contactId);
        public abstract Task<IReadOnlyList<string>> RoomList();
        public abstract Task<string> RoomQRCode(string roomId);
        public abstract Task RoomQuit(string roomId);
        public abstract Task<string> RoomTopic(string roomId);
        public abstract Task RoomTopic(string roomId, string topic);

        protected abstract Task<RoomPayload> RoomRawPayload(string roomId);
        protected abstract Task<RoomPayload> RoomRawPayloadParser(RoomPayload rawPayload);
        #endregion

        #region RoomMember
        public abstract Task<string> RoomAnnounce(string roomId);
        public abstract Task RoomAnnounce(string roomId, string text);
        public abstract Task<string[]> RoomMemberList(string roomId);

        protected abstract Task<RoomMemberPayload> RoomMemberRawPayload(string roomId, string contactId);
        protected abstract Task<RoomMemberPayload> RoomMemberRawPayloadParser(RoomMemberPayload rawPayload);
        #endregion

        public async Task<IReadOnlyList<string>> RoomMemberSearch(string roomId, string query)
        {
            var byRoomAlias = await RoomMemberSearch(roomId, new RoomMemberQueryFilter { RoomAlias = query });
            var byName = await RoomMemberSearch(roomId, new RoomMemberQueryFilter { Name = query });
            var byContactAlias = await RoomMemberSearch(roomId, new RoomMemberQueryFilter { ContactAlias = query });

            var builder = ImmutableList.CreateBuilder<string>();
            builder.AddRange(byRoomAlias);
            builder.AddRange(byName);
            builder.AddRange(byContactAlias);
            return builder.ToImmutableList();
        }

        public async Task<IReadOnlyList<string>> RoomMemberSearch(string roomId, RoomMemberQueryFilter query)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"roomMemberSearch({roomId}, {JsonConvert.SerializeObject(query)})");
            }
            if (string.IsNullOrWhiteSpace(SelfId))
            {
                throw new InvalidOperationException("no puppet.id. need puppet to be login-ed for a search");
            }
            if (query == null)
            {
                throw new InvalidOperationException("no query");
            }
            var memberIds = await RoomMemberList(roomId);
            var ids = new List<string>();
            if (query.ContactAlias != null || query.Name != null)
            {
                ids.AddRange(await ContactSearch(new ContactQueryFilter { Alias = query.ContactAlias, Name = query.Name }));
            }
            if (query.RoomAlias != null)
            {
                var memberPayloads = await Task.WhenAll(memberIds.Select(async m => await RoomMemberPayload(roomId, m)));
                ids.AddRange(memberPayloads.Where(payload => payload.RoomAlias == query.RoomAlias).Select(p => p.Id));
            }
            return ids;
        }

        public async Task<IReadOnlyList<string>> RoomSearch(RoomQueryFilter? query)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"roomSearch({(query != null ? JsonConvert.SerializeObject(query) : "")})");
            }
            var allRoomIds = await RoomList();
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"roomSearch() allRoomIdList.length={allRoomIds.Count}");
            }
            if (query == null)
            {
                return allRoomIds;
            }
            if (!string.IsNullOrEmpty(query.Id))
            {
                var result = allRoomIds.Where(x => x == query.Id);
                return result.ToImmutableList();
            }

            var filter = query.Every<RoomQueryFilter, RoomPayload>();
            var allRoomPayloads = (await Task.WhenAll(allRoomIds.Select(async id =>
            {
                try
                {
                    return await RoomPayload(id);
                }
                catch (Exception exception)
                {
                    if (Logger.IsEnabled(LogLevel.Trace))
                    {
                        Logger.LogTrace(exception, $"roomSearch() roomPayload failed.");
                    }
                    await RoomPayloadDirty(id);
                    await RoomMemberPayloadDirty(id);
                    return null;
                }
            })))
            .Where(r => r != null)
            .OfType<RoomPayload>()
            //.Select(r => r.Id)
            .ToImmutableList();

            IReadOnlyList<string> filterResult = null;


            if (!string.IsNullOrEmpty(query.Topic.Value))
            {
                filterResult = allRoomPayloads
                .Where(x => query.Topic.Value == x.Topic)
                .Select(x => x.Id)
                .ToImmutableList();
            }
            else
            {
                allRoomPayloads.Select(x => x.Id);
            }
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"roomSearch() roomIdList filtered. result length={filterResult.Count}");
            }
            return filterResult;
        }

        /// <summary>
        /// Check a Room Id if it's still valid.
        /// For example: talk to the server, and see if it should be deleted in the local cache.
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        public async Task<bool> RoomValidate(string roomId)
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"roomValidate({roomId}) base class just return `true`");
            }
            return true;
        }

        protected RoomPayload? RoomPayloadCache(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId))
            {
                throw new ArgumentException("no id");
            }
            var cachePlayload = CacheRoomPayload.Get(roomId);
            if (cachePlayload == null)
            {
                if (Logger.IsEnabled(LogLevel.Trace))
                {
                    Logger.LogTrace($"roomPayloadCache({roomId}) cache MISS");
                }
            }
            return cachePlayload;
        }

#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        public async Task RoomPayloadDirty(string roomId)
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"roomPayloadDirty({roomId})");
            }
            _ = CacheRoomPayload.Delete(roomId);
        }

        public async Task<RoomPayload> RoomPayload(string roomId)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"roomPayload({roomId})");
            }
            if (string.IsNullOrWhiteSpace(roomId))
            {
                throw new ArgumentException("no id", nameof(roomId));
            }

            //1. Try to get from cache first
            var cached = RoomPayloadCache(roomId);
            if (cached != null)
            {
                return cached;
            }

            //2. Cache not found
            var rawPayload = await RoomRawPayload(roomId);
            var payload = await RoomRawPayloadParser(rawPayload);

            CacheRoomPayload.Set(roomId, payload);
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"roomPayload({roomId}) cache SET");
            }

            return payload;

        }
        private string CacheKeyRoomMember(string roomId, string contactId) => contactId + "@@@" + roomId;

        public async Task RoomMemberPayloadDirty(string roomId)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"roomMemberPayloadDirty({roomId})");
            }
            var contactIdList = await RoomMemberList(roomId);
            foreach (var contactId in contactIdList)
            {
                var cacheKey = CacheKeyRoomMember(roomId, contactId);
                _ = CacheRoomMemberPayload.Delete(cacheKey);
            }
        }

        public async Task<RoomMemberPayload> RoomMemberPayload(string roomId, string memberId)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"roomMemberPayload(roomId={roomId}, memberId={memberId})");
            }
            if (string.IsNullOrWhiteSpace(roomId) || string.IsNullOrWhiteSpace(memberId))
            {
                throw new ArgumentException("no id");
            }

            //1. Try to get from cache
            var cacheKey = CacheKeyRoomMember(roomId, memberId);
            var cachedPayload = CacheRoomMemberPayload.Get(cacheKey);

            if (cachedPayload != null)
            {
                return cachedPayload;
            }

            //2. Cache not found
            var rawPayload = await RoomMemberRawPayload(roomId, memberId);
            if (rawPayload == null)
            {
                throw new InvalidOperationException($"contact({memberId}) is not in the Room({roomId})");
            }
            var payload = await RoomMemberRawPayloadParser(rawPayload);

            CacheRoomMemberPayload.Set(cacheKey, payload);
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"roomMemberPayload({roomId}) cache SET");
            }

            return payload;
        }
    }
}
