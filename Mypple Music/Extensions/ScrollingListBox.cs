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
                scrollViewer = ScrollViewerExtension.FindVisualChild<ScrollViewer>(listBox);
            }

            if (scrollViewer != null)
            {
                // 获取 ListBoxItem 的引用
                ListBoxItem firstItem =
                    listBox.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                if (firstItem != null)
                {
                    // 获取 ListBoxItem 的 RenderSize 属性
                    Size firstItemSize = firstItem.RenderSize;
                    // 获取 ListBoxItem 的高度
                    double firstItemHeight = firstItemSize.Height;
                    Debug.WriteLine(firstItemHeight);
                    scrollViewer.ScrollToVerticalOffsetWithAnimation(
                        firstItemHeight * (listBox.SelectedIndex - 1),
                        TimeSpan.FromSeconds(1),
                        new CircleEase()
                    );
                }
            }
        }
    }
}

