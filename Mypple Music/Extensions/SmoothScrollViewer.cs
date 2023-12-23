using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Mypple_Music.Extensions
{
    public class SmoothScrollViewer : ScrollViewer
    {
        //记录上一次的滚动位置
        private double LastLocation = 0;

        //重写鼠标滚动事件
        //protected override void OnMouseWheel(MouseWheelEventArgs e)
        //{
        //    //this.ScrollToVerticalOffset(LastLocation);
        //    double WheelChange = e.Delta;
        //    //可以更改一次滚动的距离倍数 (WheelChange可能为正负数!)
        //    double newOffset = LastLocation - (WheelChange * 1.5);          
        //    //碰到底部和顶部时的处理
        //    if (newOffset < 0)
        //        newOffset = 0;
        //    if (newOffset > ScrollableHeight)
        //        newOffset = ScrollableHeight;

        //    this.ScrollToVerticalOffsetWithAnimation(
        //        newOffset,
        //        TimeSpan.FromMilliseconds(800),
        //        new CubicEase { EasingMode = EasingMode.EaseOut }
        //    );                 
        //    LastLocation = newOffset;
        //    //告诉ScrollViewer我们已经完成了滚动
        //    e.Handled = true;
        //}
    }
}
