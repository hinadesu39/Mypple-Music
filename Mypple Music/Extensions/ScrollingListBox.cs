using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace Mypple_Music.Extensions
{
    public class ScrollingListBox : ListBox
    {
        private static ScrollViewer scrollViewer;


        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            ScrollingListBox listBox = e.Source as ScrollingListBox;

            // var item = FindVisualChild<ListBoxItem>(listBox);
            if (scrollViewer == null)
            {
                scrollViewer = FindVisualChild<ScrollViewer>(listBox);
                return;
            }

            if (scrollViewer != null)
            {
                // 获取 ListBoxItem 的引用
                ListBoxItem firstItem =
                    listBox.ItemContainerGenerator.ContainerFromIndex(listBox.SelectedIndex) as ListBoxItem;
                if (firstItem != null)
                {
                    // 获取 ListBoxItem 的 RenderSize 属性
                    Size firstItemSize = firstItem.RenderSize;
                    // 获取 ListBoxItem 的高度
                    double firstItemHeight = firstItemSize.Height;
                    Debug.WriteLine(firstItemHeight);
                    scrollViewer.ScrollToVerticalOffsetWithAnimation(
                        firstItemHeight * (listBox.SelectedIndex - 2),
                        TimeSpan.FromSeconds(1),
                        new ElasticEase() { Oscillations = 1, Springiness = 6}
                    );
                }

            }
            //告诉viewmodel点击事件后播放进度需要发生改变(待做)
        }

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
    }
}
