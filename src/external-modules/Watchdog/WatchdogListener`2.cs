namespace Wechaty
{

    public delegate void WatchdogListener<TFoodType, TData>(WatchdogFood<TFoodType, TData> food, long time);
}
