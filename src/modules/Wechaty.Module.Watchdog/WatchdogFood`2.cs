namespace Wechaty.Module.Watchdog
{
    public class WatchDogFood<TFoodType, TData>
    {
        public TData Data { get; set; }
        /// <summary>
        /// millisecond
        /// </summary>
        public long Timeout { get; set; }
        public TFoodType Type { get; set; }
    }
}
