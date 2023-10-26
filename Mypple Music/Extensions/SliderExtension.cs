using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Mypple_Music.Extensions
{
    public class SliderExtension
    {
        /// <summary>
        /// 拖拽定位
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ICommand GetDragCompletedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DragCompletedCommandProperty);
        }

        public static void SetDragCompletedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DragCompletedCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DragCompletedCommandProperty =
            DependencyProperty.RegisterAttached("DragCompletedCommand",
                                                typeof(ICommand),
                                                typeof(SliderExtension),
                                                new PropertyMetadata(default(ICommand), OnDragCompletedCommandChanged));

        private static void OnDragCompletedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Slider slider = d as Slider;
            if (slider == null)
            {
                return;
            }

            if (e.NewValue is ICommand)
            {
                slider.Loaded += SliderOnLoaded;
            }
        }

        private static void SliderOnLoaded(object sender, RoutedEventArgs e)
        {
            Slider slider = sender as Slider;
            if (slider == null)
            {
                return;
            }
            slider.Loaded -= SliderOnLoaded;

            Track track = slider.Template.FindName("PART_Track", slider) as Track;
            if (track == null)
            {
                return;
            }
            track.Thumb.DragCompleted += (dragCompletedSender, dragCompletedArgs) =>
            {
                ICommand command = GetDragCompletedCommand(slider);
                command.Execute(null);
            };
        }



        /// <summary>
        /// 点击定位
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ICommand GetPreviewMouseDownCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(PreviewMouseDownCommandProperty);
        }

        public static void SetPreviewMouseDownCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(PreviewMouseDownCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for PreviewMouseDownCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviewMouseDownCommandProperty =
            DependencyProperty.RegisterAttached("PreviewMouseDownCommand",
                                                typeof(ICommand),
                                                typeof(SliderExtension),
                                                new PropertyMetadata(default(ICommand), OnPreviewMouseDownCommandChanged));

        private static void OnPreviewMouseDownCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Slider slider = d as Slider;
            if (slider == null)
            {
                return;
            }

            if (e.NewValue is ICommand)
            {
                slider.Loaded += Control_PreviewMouseDown;
            }
        }

        private static void Control_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
            Slider slider = sender as Slider;
            if (slider == null)
            {
                return;
            }
            slider.Loaded -= SliderOnLoaded;
            slider.PreviewMouseDown += (sender, e) =>
            {

                var value = (e.GetPosition(slider).X / slider.ActualWidth) * (slider.Maximum - slider.Minimum);
                slider.Value = value; 
                ICommand command = GetPreviewMouseDownCommand(slider);
                command.Execute(null);
            };

        }


    }
}
