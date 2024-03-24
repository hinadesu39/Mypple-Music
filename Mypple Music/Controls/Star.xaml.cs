using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mypple_Music.Controls
{
    /// <summary>
    /// Star.xaml 的交互逻辑
    /// </summary>
    public partial class Star : UserControl
    {
        public Star()
        {
            InitializeComponent();
        }
        public Thickness CanvasMargin
        {
            get { return (Thickness)GetValue(CanvasMarginProperty); }
            set { SetValue(CanvasMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanvasMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanvasMarginProperty =
            DependencyProperty.Register("CanvasMargin", typeof(Thickness), typeof(Star), new PropertyMetadata(null));



        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(Star), new PropertyMetadata(null));




        public double OffSetPlus
        {
            get { return (double)GetValue(OffSetPlusProperty); }
            set { SetValue(OffSetPlusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OffSetPlus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffSetPlusProperty =
            DependencyProperty.Register("OffSetPlus", typeof(double), typeof(Star), new PropertyMetadata(0.0));




        public double OffSetMinus
        {
            get { return (double)GetValue(OffSetMinusProperty); }
            set { SetValue(OffSetMinusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OffSetMinu.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffSetMinusProperty =
            DependencyProperty.Register("OffSetMinus", typeof(double), typeof(Star), new PropertyMetadata(0.0));



    }
}
