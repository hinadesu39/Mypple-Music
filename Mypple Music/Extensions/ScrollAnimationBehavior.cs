using System;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Mypple_Music.Extensions
{
    public static class ScrollAnimationBehavior
    {
        public static double intendedLocation = 0;

        #region Private ScrollViewer for ListBox

        private static ScrollViewer listBoxScroller;

        #endregion

        #region Private ScrollViewer for DataGrid

        private static ScrollViewer dataGridScroller;

        #endregion

        #region VerticalOffset Property

        public static DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached("VerticalOffset",
                                                typeof(double),
                                                typeof(ScrollAnimationBehavior),
                                                new UIPropertyMetadata(0.0, OnVerticalOffsetChanged));

        public static void SetVerticalOffset(FrameworkElement target, double value)
        {
            target.SetValue(VerticalOffsetProperty, value);
        }

        public static double GetVerticalOffset(FrameworkElement target)
        {
            return (double)target.GetValue(VerticalOffsetProperty);
        }

        #endregion

        #region OnVerticalOffset Changed

        private static void OnVerticalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = target as ScrollViewer;
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
            }
        }

        #endregion

        #region HorizontalOffset Property

        // 定义一个附加依赖属性 HorizontalOffsetProperty
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.RegisterAttached(
                "HorizontalOffset",
                typeof(double),
                typeof(ScrollAnimationBehavior),
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

        #endregion

        #region OnHorizontalOffset Changed
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
        }
        #endregion

        #region IsEnabled Property

        public static DependencyProperty IsEnabledProperty =
                                                DependencyProperty.RegisterAttached("IsEnabled",
                                                typeof(bool),
                                                typeof(ScrollAnimationBehavior),
                                                new UIPropertyMetadata(false, OnIsEnabledChanged));

        public static void SetIsEnabled(FrameworkElement target, bool value)
        {
            target.SetValue(IsEnabledProperty, value);
        }

        public static bool GetIsEnabled(FrameworkElement target)
        {
            return (bool)target.GetValue(IsEnabledProperty);
        }

        #endregion

        #region OnIsEnabledChanged Changed

        private static void OnIsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var target = sender;

            //if (target != null && target is ScrollViewer)
            //{
            //    ScrollViewer scroller = target as ScrollViewer;
            //    scroller.Loaded += new RoutedEventHandler(scrollerLoaded);
            //}

            if (target != null && target is ListBox)
            {
                ListBox listbox = target as ListBox;
                listbox.Loaded += new RoutedEventHandler(listboxLoaded);
            }

            if (target != null && target is DataGrid)
            {
                DataGrid datagrid = target as DataGrid;
                datagrid.Loaded += new RoutedEventHandler(dataGridLoaded);
            }
        }

        #endregion

        #region IsScrollToSelection

        public static bool GetIsScrollToSelection(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsScrollToSelectionProperty);
        }

        public static void SetIsScrollToSelection(DependencyObject obj, bool value)
        {
            obj.SetValue(IsScrollToSelectionProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsScrollToSelection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsScrollToSelectionProperty =
            DependencyProperty.RegisterAttached("IsScrollToSelection", typeof(bool), typeof(ScrollAnimationBehavior), new PropertyMetadata(false, OnIsScrollToSelectionChanged));


        #endregion

        #region OnIsScrollToSelectionChanged Changed

        private static void OnIsScrollToSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = d;
            if (target != null && target is ListBox)
            {
                ListBox listbox = target as ListBox;
                listbox.SelectionChanged += new SelectionChangedEventHandler(ListBoxSelectionChanged);
            }
        }

        #endregion

        #region TimeDuration Property

        public static DependencyProperty TimeDurationProperty =
            DependencyProperty.RegisterAttached("TimeDuration",
                                                typeof(TimeSpan),
                                                typeof(ScrollAnimationBehavior),
                                                new PropertyMetadata(new TimeSpan(0, 0, 0, 0, 0)));

        public static void SetTimeDuration(FrameworkElement target, TimeSpan value)
        {
            target.SetValue(TimeDurationProperty, value);
        }

        public static TimeSpan GetTimeDuration(FrameworkElement target)
        {
            return (TimeSpan)target.GetValue(TimeDurationProperty);
        }

        #endregion

        #region SetEventHandlersForScrollViewer Helper

        private static void SetEventHandlersForScrollViewer(ScrollViewer scroller)
        {
            scroller.PreviewMouseWheel += new MouseWheelEventHandler(ScrollViewerPreviewMouseWheel);
            //scroller.PreviewKeyDown += new KeyEventHandler(ScrollViewerPreviewKeyDown);
            scroller.PreviewMouseLeftButtonUp += Scroller_PreviewMouseLeftButtonUp;

        }

        private static void Scroller_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            intendedLocation = ((ScrollViewer)sender).VerticalOffset;
            //AnimateScroll((ScrollViewer)sender, intendedLocation);
        }

        #endregion

        #region ScrollViewerPreviewMouseWheel Event Handler

        private static void ScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double mouseWheelChange = (double)e.Delta;
            ScrollViewer scroller = (ScrollViewer)sender;
            double newVOffset = intendedLocation - (mouseWheelChange * 2);
            //We got hit by the mouse again. jump to the offset.
            scroller.ScrollToVerticalOffset(intendedLocation);
            if (newVOffset < 0)
            {
                newVOffset = 0;
            }
            if (newVOffset > scroller.ScrollableHeight)
            {
                newVOffset = scroller.ScrollableHeight;
            }

            AnimateVerticalScroll(scroller, newVOffset);
            intendedLocation = newVOffset;
            e.Handled = true;
        }

        #endregion

        #region AnimateVerticalScroll Helper

        private static void AnimateVerticalScroll(ScrollViewer scrollViewer, double ToValue)
        {
            scrollViewer.BeginAnimation(VerticalOffsetProperty, null);
            DoubleAnimation verticalAnimation = new DoubleAnimation();
            verticalAnimation.From = scrollViewer.VerticalOffset;
            verticalAnimation.To = ToValue;
            verticalAnimation.Duration = new Duration(GetTimeDuration(scrollViewer));
            verticalAnimation.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
            scrollViewer.BeginAnimation(VerticalOffsetProperty, verticalAnimation);
        }

        public static void AnimateVerticalScrollWithCustomBehavior(this ScrollViewer scrollViewer, double ToValue, TimeSpan duration, IEasingFunction easingFunction)
        {
            scrollViewer.BeginAnimation(VerticalOffsetProperty, null);
            DoubleAnimation verticalAnimation = new DoubleAnimation();
            verticalAnimation.From = scrollViewer.VerticalOffset;
            verticalAnimation.To = ToValue;
            verticalAnimation.Duration = duration;
            verticalAnimation.EasingFunction = easingFunction;
            scrollViewer.BeginAnimation(VerticalOffsetProperty, verticalAnimation);
        }

        #endregion

        #region AnimateHorizontalScroll Helper

        private static void AnimateHorizontalScroll(ScrollViewer scrollViewer, double ToValue)
        {
            scrollViewer.BeginAnimation(HorizontalOffsetProperty, null);
            DoubleAnimation horizontalAnimation = new DoubleAnimation();
            horizontalAnimation.From = scrollViewer.HorizontalOffset;
            horizontalAnimation.To = ToValue;
            horizontalAnimation.Duration = new Duration(GetTimeDuration(scrollViewer));
            horizontalAnimation.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
            scrollViewer.BeginAnimation(HorizontalOffsetProperty, horizontalAnimation);
        }

        #endregion

        #region listboxLoaded Event Handler

        private static void listboxLoaded(object sender, RoutedEventArgs e)
        {
            ListBox listbox = sender as ListBox;

            listBoxScroller = FindVisualChild<ScrollViewer>(listbox);
            SetEventHandlersForScrollViewer(listBoxScroller);

            SetTimeDuration(listBoxScroller, new TimeSpan(0, 0, 0, 0, 300));

            //listbox.SelectionChanged += new SelectionChangedEventHandler(ListBoxSelectionChanged);
            //listbox.Loaded += new RoutedEventHandler(ListBoxLoaded);
            //listbox.LayoutUpdated += new EventHandler(ListBoxLayoutUpdated);
        }

        #endregion

        #region dataGridLoaded Event Handler

        private static void dataGridLoaded(object sender, RoutedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;

            dataGridScroller = FindVisualChild<ScrollViewer>(dataGrid);
            SetEventHandlersForScrollViewer(dataGridScroller);

            SetTimeDuration(dataGridScroller, new TimeSpan(0, 0, 0, 0, 300));

            //dataGrid.SelectionChanged += new SelectionChangedEventHandler(dataGridSelectionChanged);
            //dataGrid.Loaded += new RoutedEventHandler(dataGridLoaded);
            //dataGrid.LayoutUpdated += new EventHandler(dataGridLayoutUpdated);
        }

        #endregion

        #region ListBox Event Handlers

        private static void ListBoxLayoutUpdated(object sender, EventArgs e)
        {
            UpdateScrollPosition(sender);
        }

        private static void ListBoxLoaded(object sender, RoutedEventArgs e)
        {
            UpdateScrollPosition(sender);
        }

        private static void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = e.Source as ListBox;
            if (listBox!.IsMouseOver)
            {
                return;
            }
            UpdateScrollPosition(sender);
        }

        #endregion

        #region UpdateScrollPosition Helper
        /// <summary>
        /// to Locate the selected item's Position
        /// </summary>
        /// <param name="sender"></param>
        private static void UpdateScrollPosition(object sender)
        {
            ListBox listbox = sender as ListBox;

            if (listbox != null)
            {
                double scrollTo = 0;

                for (int i = 0; i < (listbox.SelectedIndex); i++)
                {
                    ListBoxItem tempItem = listbox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;

                    if (tempItem != null)
                    {
                        scrollTo += tempItem.RenderSize.Height;
                    }
                }
                var listBoxScroller = FindVisualChild<ScrollViewer>(listbox);
                if (listBoxScroller != null)
                {
                    AnimateVerticalScroll(listBoxScroller, scrollTo);
                }

            }
        }

        #endregion

        #region ScrollLeftButton Helper

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
                typeof(ScrollAnimationBehavior),
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

                        AnimateHorizontalScroll(scrollViewer, scrollViewer.HorizontalOffset + firstItemWidth);

                    }
                }
            }
        }

        #endregion

        # region ScrollRightButton Helper

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
                typeof(ScrollAnimationBehavior),
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

                        AnimateHorizontalScroll(scrollViewer, scrollViewer.HorizontalOffset - firstItemWidth);

                    }
                }
            }
        }

        #endregion

        #region FindChild&Parent

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
