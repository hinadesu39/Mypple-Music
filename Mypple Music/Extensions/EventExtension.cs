using Mypple_Music.Events;
using Prism.Events;
using Prism.Services.Dialogs;
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
        /// 询问窗口
        /// </summary>
        /// <param name="dialogHost">指定的DialogHost会话主机</param>
        /// <param name="title">标题</param>
        /// <param name="content">询问内容</param>
        /// <param name="dialogHostName">会话主机名称(唯一)</param>
        /// <returns></returns>
        public static async Task<IDialogResult> Question(this IDialogHostService dialogHost,
            string title, string content
            )
        {
            DialogParameters param = new DialogParameters();
            param.Add("Title", title);
            param.Add("Content", content);
            var dialogResult = await dialogHost.ShowDialog("QuestionView", param);
            return dialogResult;
        }

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
