using System;

namespace fed.cloud.eventbus.EventBus.Abstraction
{
    public class SubscribtionInfo
    {
        private SubscribtionInfo(Type type)
        {
            HandlerType = type;
        }

        public Type HandlerType { get; }

        public static SubscribtionInfo Create(Type type)
        {
            return new SubscribtionInfo(type);
        }
    }
}
