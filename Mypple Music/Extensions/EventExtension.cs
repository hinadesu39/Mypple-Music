using Mypple_Music.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Extensions
{
    public static class EventExtension
    {
        /// <summary>
        /// 注册提示消息以及配置过滤器
        /// </summary>
        /// <param name="aggregator"></param>
        /// <param name="action"></param>
        public static void RegisterMessage(this IEventAggregator aggregator, Action<MessageModel> action, string filterName = "MainView")
        {
            aggregator.GetEvent<MessageEvent>().Subscribe(action, ThreadOption.PublisherThread, true, (m) =>
            {
                return m.Filter.Equals(filterName);
            });
        }
        /// <summary>
        /// 发送提示消息
        /// </summary>
        /// <param name="aggregator"></param>
        /// <param name="message"></param>
        public static void SendMessage(this IEventAggregator aggregator, string message, string filterName = "MainView")
        {
            aggregator.GetEvent<MessageEvent>().Publish(new MessageModel(message, filterName));
        }

        public static void UpdateLoading(this IEventAggregator aggregator,bool isLoading, string filterName = "MainView")
        {
            aggregator.GetEvent<LoadingEvent>().Publish(new LoadingModel(isLoading, filterName));
        }
    }
}
