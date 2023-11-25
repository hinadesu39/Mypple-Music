using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Mypple_Music.Extensions
{
    public class PopUpExtension
    {
        public static DependencyObject GetPopUpPlacementTarget(DependencyObject obj)
        {
            return (DependencyObject)obj.GetValue(PopUpPlacementTargetProperty);
        }

        public static void SetPopUpPlacementTarget(DependencyObject obj, DependencyObject value)
        {
            obj.SetValue(PopUpPlacementTargetProperty, value);
        }

        // Using a DependencyProperty as the backing store for PopUpPlacementTarget.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PopUpPlacementTargetProperty =
            DependencyProperty.RegisterAttached("PopUpPlacementTarget", typeof(DependencyObject), typeof(PopUpExtension), new PropertyMetadata(null, OnWindowPositionChanged));

        private static void OnWindowPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                DependencyObject popupPopupPlacementTarget = e.NewValue as DependencyObject;
                Popup pop = d as Popup;

                Window w = Window.GetWindow(popupPopupPlacementTarget);
                if (null != w)
                {
                    //让Popup随着窗体的移动而移动
                    w.LocationChanged += delegate
                    {
                        var mi = typeof(Popup).GetMethod("UpdatePosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        mi.Invoke(pop, null);
                    };
                    //让Popup随着窗体的Size改变而移动位置
                    w.SizeChanged += delegate
                    {
                        var mi = typeof(Popup).GetMethod("UpdatePosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        mi.Invoke(pop, null);
                    };
                }
            }

        }
    }
}
