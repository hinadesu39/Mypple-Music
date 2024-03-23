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
using System.Collections;
using System.Collections.Specialized;
using ImTools;
using System.Collections.ObjectModel;
using Mypple_Music.Models;

namespace Mypple_Music.Extensions
{
    public class LyricScrollingListBox : ListBox
    {
        private static ScrollViewer scrollViewer;
        //private static double[] verticalOffSet;

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            LyricScrollingListBox listBox = e.Source as LyricScrollingListBox;

            if (scrollViewer == null)
            {
                scrollViewer = ScrollAnimationBehavior.FindVisualChild<ScrollViewer>(listBox);
                //verticalOffSet = new double[listBox.Items.Count];
                
            }
            //if (listBox.SelectedIndex == -1)
            //{
            //    //切歌了滚动偏移量需要重新计算
            //    verticalOffSet = new double[listBox.Items.Count];
            //}
            //if (scrollViewer != null && listBox.Items != null)
            //{
            //    //如果偏移量没有初始化就遍历所有itme计算一次
            //    if (verticalOffSet[listBox.Items.Count - 1] == 0)
            //        for (int i = 1; i < listBox.Items.Count; i++)
            //        {
            //            ListBoxItem item =
            //                listBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
            //            if (item != null)
            //            {
            //                // 获取 ListBoxItem 的 RenderSize 属性
            //                Size itemSize = item.RenderSize;
            //                // 获取 ListBoxItem 的高度
            //                double itemHeight = itemSize.Height;
            //                verticalOffSet[i] = itemHeight + verticalOffSet[i - 1];
            //            }
            //        }

            //    scrollViewer.AnimateVerticalScrollWithCustomBehavior(
            //        verticalOffSet[listBox.SelectedIndex == -1 ? 0 : listBox.SelectedIndex] - 200,
            //        TimeSpan.FromSeconds(1),
            //        new ElasticEase() { Oscillations = 1, Springiness = 6 }
            //    );
            //}
            if (listBox != null && scrollViewer != null)
            {
                double scrollTo = 0;

                for (int i = 0; i < (listBox.SelectedIndex); i++)
                {
                    ListBoxItem tempItem = listBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;

                    if (tempItem != null)
                    {
                        scrollTo += tempItem.RenderSize.Height;
                    }
                }
                //var listBoxScroller = FindVisualChild<ScrollViewer>(listbox);
                //if (listBoxScroller != null)
                //{
                //    AnimateVerticalScroll(listBoxScroller, scrollTo);
                //}
                scrollViewer.AnimateVerticalScrollWithCustomBehavior(scrollTo - 200, TimeSpan.FromSeconds(1), new ElasticEase() { Oscillations = 1, Springiness = 6 });
            }
        }
    }
}
