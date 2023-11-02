using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mypple_Music.Extensions
{
    public class TextBoxExtension
    {
        public static ICommand GetTextEmptyCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(TextEmptyCommandProperty);
        }

        public static void SetTextEmptyCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(TextEmptyCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextEmptyCommandProperty =
            DependencyProperty.RegisterAttached(
                "TextEmptyCommand",
                typeof(ICommand),
                typeof(TextBoxExtension),
                new PropertyMetadata(default(ICommand), OnTextEmptyCommandChanged)
            );

        private static void OnTextEmptyCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = d as TextBox;
            if (textBox == null)
            {
                return;
            }
            if (e.NewValue is ICommand)
            {
                textBox.Loaded += OnTextLoaded;
            }
        }

        private static void OnTextLoaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null)
            {
                return;
            }
            textBox.TextChanged += (textEmptySender, textEmptyArgs) =>
            {
                if (textBox.Text == string.Empty)
                {
                    ICommand command = GetTextEmptyCommand(textBox);
                    command.Execute(null);
                }

            };
        }
    }
}
