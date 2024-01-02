using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;

namespace Mypple_Music.Extensions
{
    /// <summary>
    /// 通过添加动画使得item切换的时候更加线性
    /// </summary>
    public static class ScrollViewerExtension
    {
        #region LastHorizontalOffset

        public static double GetLastHorizontalOffset(DependencyObject obj)
        {
            return (double)obj.GetValue(LastHorizontalOffsetProperty);
        }

        public static void SetLastHorizontalOffset(DependencyObject obj, double value)
        {
            obj.SetValue(LastHorizontalOffsetProperty, value);
        }

        // Using a DependencyProperty as the backing store for LastHorizontalOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastHorizontalOffsetProperty =
            DependencyProperty.RegisterAttached(
                "LastHorizontalOffset",
                typeof(double),
                typeof(ScrollViewerExtension),
                new PropertyMetadata(0.0)
            );

        #endregion

        #region HorizontalOffset
        // 定义一个附加依赖属性 HorizontalOffsetProperty
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.RegisterAttached(
                "HorizontalOffset",
                typeof(double),
                typeof(ScrollViewerExtension),
                new UIPropertyMetadata(0.0, OnHorizontalOffsetChanged)
            );

        // 获取 HorizontalOffsetProperty 的值
        public static double GetHorizontalOffset(DependencyObject obj)
        {
            return (double)obj.GetValue(HorizontalOffsetProperty);
        }

        // 设置 HorizontalOffsetProperty 的值
        public static void SetHorizontalOffset(DependencyObject obj, double value)
        {
            obj.SetValue(HorizontalOffsetProperty, value);
        }

        // 当 HorizontalOffsetProperty 的值发生变化时，更新 ScrollViewer 的水平偏移量
        private static void OnHorizontalOffsetChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            var viewer = d as ScrollViewer;
            if (viewer != null)
            {
                viewer.ScrollToHorizontalOffset((double)e.NewValue);
            }
            Debug.WriteLine((double)e.NewValue);
        }

        // 定义一个扩展方法 ScrollToHorizontalOffsetWithAnimation
        public static void ScrollToHorizontalOffsetWithAnimation(
            this ScrollViewer viewer,
            double offset,
            TimeSpan duration,
            IEasingFunction easingFunction
        )
        {
            // 创建一个 DoubleAnimation 对象
            var animation = new DoubleAnimation();
            animation.To = offset; // 设置目标的水平偏移量
            animation.Duration = duration; // 设置持续时间
            animation.EasingFunction = easingFunction; // 设置缓动函数
            // 将动画应用到 HorizontalOffsetProperty 上
            viewer.BeginAnimation(HorizontalOffsetProperty, animation);
        }
        #endregion

        #region VerticalOffset
        // 定义一个附加依赖属性 VerticalOffsetProperty
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached(
                "VerticalOffset",
                typeof(double),
                typeof(ScrollViewerExtension),
                new UIPropertyMetadata(0.0, OnVerticalOffsetChanged)
            );

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
        private static void OnVerticalOffsetChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            var viewer = d as ScrollViewer;
            if (viewer != null)
            {
                viewer.ScrollToVerticalOffset((double)e.NewValue);
            }
        }

        // 定义一个扩展方法 ScrollToVerticalOffsetWithAnimation
        public static void ScrollToVerticalOffsetWithAnimation(
            this ScrollViewer viewer,
            double offset,
            TimeSpan duration,
            IEasingFunction easingFunction
        )
        {
            //viewer.BeginAnimation(VerticalOffsetProperty, null);
            // 创建一个 DoubleAnimation 对象
            var animation = new DoubleAnimation();
            animation.To = offset; // 设置目标的垂直偏移量
            animation.Duration = duration; // 设置持续时间
            animation.EasingFunction = easingFunction; // 设置缓动函数
            // 将动画应用到 VerticalOffsetProperty 上
            viewer.BeginAnimation(VerticalOffsetProperty, animation);
        }
        #endregion

        # region ScrollLeft

        public static bool GetScrollLeft(DependencyObject obj)
        {
            return (bool)obj.GetValue(ScrollLeftProperty);
        }

        public static void SetScrollLeft(DependencyObject obj, bool value)
        {
            obj.SetValue(ScrollLeftProperty, value);
        }

        // Using a DependencyProperty as the backing store for ScrollLeft.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollLeftProperty =
            DependencyProperty.RegisterAttached(
                "ScrollLeft",
                typeof(bool),
                typeof(ScrollViewerExtension),
                new PropertyMetadata(false, OnScrollLeftChanged)
            );

        private static void OnScrollLeftChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            if (d != null && d is Button)
            {
                var button = d as Button;
                button.Click += ScrollLeftClick;
            }
        }

        private static void ScrollLeftClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            ListBox listBox = GetParentObject<ListBox>(button!);
            if (listBox != null)
            {
                ScrollViewer scrollViewer = FindVisualChild<ScrollViewer>(listBox);
                if (scrollViewer != null)
                {
                    // 获取 ListBoxItem 的引用
                    ListBoxItem firstItem =
                        listBox.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                    if (firstItem != null)
                    {
                        // 获取 ListBoxItem 的 RenderSize 属性
                        Size firstItemSize = firstItem.RenderSize;
                        // 获取 ListBoxItem 的宽度
                        double firstItemWidth = firstItemSize.Width;
                        Debug.WriteLine(GetLastHorizontalOffset(scrollViewer));
                        Debug.WriteLine(scrollViewer.ContentHorizontalOffset);
                        Debug.WriteLine(scrollViewer.ActualWidth);
                        //if (firstItemWidth + GetLastHorizontalOffset(scrollViewer) > scrollViewer.ContentHorizontalOffset)
                        //    return;
                        if (
                            GetLastHorizontalOffset(scrollViewer)
                            != scrollViewer.ContentHorizontalOffset
                        )
                        {
                            scrollViewer.ScrollToHorizontalOffsetWithAnimation(
                                scrollViewer.ContentHorizontalOffset,
                            TimeSpan.FromSeconds(0.8),
                            new CircleEase()
                            );
                            return;
                        }

                        scrollViewer.ScrollToHorizontalOffsetWithAnimation(
                            firstItemWidth + GetLastHorizontalOffset(scrollViewer),
                            TimeSpan.FromSeconds(0.8),
                            new CircleEase()
                        );
                        //记录上一次的位置
                        SetLastHorizontalOffset(
                            scrollViewer,
                            GetLastHorizontalOffset(scrollViewer) + firstItemWidth
                        );
                    }
                }
            }
        }

        #endregion

        # region ScrollRight

        public static bool GetScrollRight(DependencyObject obj)
        {
            return (bool)obj.GetValue(ScrollRightProperty);
        }

        public static void SetScrollRight(DependencyObject obj, bool value)
        {
            obj.SetValue(ScrollRightProperty, value);
        }

        // Using a DependencyProperty as the backing store for ScrollRight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollRightProperty =
            DependencyProperty.RegisterAttached(
                "ScrollRight",
                typeof(bool),
                typeof(ScrollViewerExtension),
                new PropertyMetadata(false, OnScrollRightChanged)
            );

        private static void OnScrollRightChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            if (d != null && d is Button)
            {
                var button = d as Button;
                button.Click += ScrollRightClick;
            }
        }

        private static void ScrollRightClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            ListBox listBox = GetParentObject<ListBox>(button!);
            if (listBox != null)
            {
                ScrollViewer scrollViewer = FindVisualChild<ScrollViewer>(listBox);
                if (scrollViewer != null)
                {
                    // 获取 ListBoxItem 的引用
                    ListBoxItem firstItem =
                        listBox.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                    if (firstItem != null)
                    {
                        // 获取 ListBoxItem 的 RenderSize 属性
                        Size firstItemSize = firstItem.RenderSize;
                        // 获取 ListBoxItem 的宽度
                        double firstItemWidth = firstItemSize.Width;

                        if (
                            GetLastHorizontalOffset(scrollViewer)
                            != scrollViewer.ContentHorizontalOffset
                        )
                        {
                            scrollViewer.ScrollToHorizontalOffset(
                                scrollViewer.ContentHorizontalOffset
                            );
                            SetLastHorizontalOffset(
                                scrollViewer,
                                scrollViewer.ContentHorizontalOffset
                            );
                        }
                        if (GetLastHorizontalOffset(scrollViewer) - firstItemWidth < 0)
                        {
                            scrollViewer.ScrollToHorizontalOffsetWithAnimation(
                                0,
                                TimeSpan.FromSeconds(0.8),
                                new CircleEase()
                            );
                            SetLastHorizontalOffset(scrollViewer, 0);
                            return;
                        }

                        scrollViewer.ScrollToHorizontalOffsetWithAnimation(
                            GetLastHorizontalOffset(scrollViewer) - firstItemWidth,
                            TimeSpan.FromSeconds(0.8),
                            new CircleEase()
                        );
                        SetLastHorizontalOffset(
                            scrollViewer,
                            GetLastHorizontalOffset(scrollViewer) - firstItemWidth
                        );
                    }
                }
            }
        }

        #endregion

        #region FindChild&Parent
        /// <summary>
        /// 通过遍历可视化树获取到Listbox中的ScrollViewer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static T FindVisualChild<T>(DependencyObject parent)
            where T : DependencyObject
        {
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }
                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null)
                    {
                        return childItem;
                    }
                }
            }
            return null;
        }

        public static T GetParentObject<T>(DependencyObject child)
            where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            while (parent != null)
            {
                if (parent is T && parent != null)
                {
                    return (T)parent;
                }
                // 在上一级父控件中没有找到指定的控件，就再往上一级找
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }
        #endregion
    }
}
