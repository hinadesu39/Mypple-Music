using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;

namespace Mypple_Music.Extensions
{
    /// <summary>
    /// 通过添加动画使得item切换的时候更加线性
    /// </summary>
    public static class ScrollViewerExtension
    {
        // 定义一个附加依赖属性 VerticalOffsetProperty
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached("VerticalOffset",
                                                typeof(double),
                                                typeof(ScrollViewerExtension),
                                                new UIPropertyMetadata(0.0, OnVerticalOffsetChanged));

        // 获取 VerticalOffsetProperty 的值
        public static double GetVerticalOffset(DependencyObject obj)
        {
            return (double)obj.GetValue(VerticalOffsetProperty);
        }

        // 设置 VerticalOffsetProperty 的值
        public static void SetVerticalOffset(DependencyObject obj, double value)
        {
            obj.SetValue(VerticalOffsetProperty, value);
        }

        // 当 VerticalOffsetProperty 的值发生变化时，更新 ScrollViewer 的垂直偏移量
        private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewer = d as ScrollViewer;
            if (viewer != null)
            {
                viewer.ScrollToVerticalOffset((double)e.NewValue);
            }
        }

        // 定义一个扩展方法 ScrollToVerticalOffsetWithAnimation
        public static void ScrollToVerticalOffsetWithAnimation(this ScrollViewer viewer, double offset, TimeSpan duration, IEasingFunction easingFunction)
        {
            // 创建一个 DoubleAnimation 对象
            var animation = new DoubleAnimation();
            animation.To = offset; // 设置目标的垂直偏移量
            animation.Duration = duration; // 设置持续时间
            animation.EasingFunction = easingFunction; // 设置缓动函数
                                                       // 将动画应用到 VerticalOffsetProperty 上
            viewer.BeginAnimation(VerticalOffsetProperty, animation);
        }
    }
}
