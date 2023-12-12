using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Mypple_Music.Extensions
{
    public class PasswordExtension 
    {

        public static string GetPasswordContent(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordProperty);
        }

        public static void SetPasswordContent(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordProperty, value);
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("PasswordContent", typeof(string), typeof(PasswordExtension), new PropertyMetadata(string.Empty,OnPasswordContentChanged));

        private static void OnPasswordContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as PasswordBox;
            if (box == null)
            {
                return;
            }
            box.PasswordChanged -= OnPasswordChanged;
            var password = e.NewValue as string;
            if (password != box.Password)
                box.Password = password;
            box.PasswordChanged += OnPasswordChanged;

        }

        private static void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            var box = sender as PasswordBox;
            SetPasswordContent(box, box.Password);
        }
    }
}
