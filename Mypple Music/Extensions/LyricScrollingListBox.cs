﻿using System;
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
    public class LyricScrollingListBox : ListBox
    {
        private static ScrollViewer scrollViewer;


        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            LyricScrollingListBox listBox = e.Source as LyricScrollingListBox;

            // var item = FindVisualChild<ListBoxItem>(listBox);
            if (scrollViewer == null)
            {
                scrollViewer = ScrollViewerExtension.FindVisualChild<ScrollViewer>(listBox);
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
    }
}
