using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ChmlFrp_Professional_Launcher.Styles
{
    public class CornerIconRadioButton : RadioButton
    {
        //Data
        public object Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            nameof(Data),
            typeof(object),
            typeof(CornerIconRadioButton)
        );
    }

    public class TransparentIconRadioButton : CornerIconRadioButton
    {
    }

    public class CornerButton : Button
    {
        //CornerRadius
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(CornerButton));

        //IsSelected
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            nameof(IsSelected),
            typeof(bool),
            typeof(CornerButton)
        );
    }

    public sealed class CornerTextBox : TextBox, INotifyPropertyChanged
    {
        private string _tempText;

        public CornerTextBox()
        {
            Loaded += CornerTextBox_Loaded;

            TextChanged += (s, e) => { Postscript = string.IsNullOrWhiteSpace(Text) ? _tempText : ""; };
        }

        private void CornerTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            _tempText = Postscript;
        }

        //CornerRadius
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                typeof(CornerTextBox)
            );

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        //Postscript
        public static readonly DependencyProperty PostscriptProperty = DependencyProperty.Register(
            nameof(Postscript),
            typeof(string),
            typeof(CornerTextBox)
        );

        public string Postscript
        {
            get => (string)GetValue(PostscriptProperty);
            set => SetValue(PostscriptProperty, value);
        }

        //Text
        public new static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(CornerTextBox),
            new PropertyMetadata(string.Empty, OnTextChanged)
        );

        public new string Text
        {
            get => (string)GetValue(TextProperty);
            set
            {
                SetValue(TextProperty, value);
                OnPropertyChanged(nameof(Text));
            }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CornerTextBox;
            control?.OnPropertyChanged(nameof(Text));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CornerComboBox : ComboBox
    {
        //CornerRadius
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                typeof(CornerComboBox)
            );
    }

    public class CornerTextBlock : TextBox
    {
        //CornerRadius
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                typeof(CornerTextBlock)
            );
    }

    public class CornerTunnelTextBox : CornerTextBlock
    {
        //IsTrue
        public bool IsTrue
        {
            get => (bool)GetValue(IsTrueProperty);
            set => SetValue(IsTrueProperty, value);
        }

        public static readonly DependencyProperty IsTrueProperty = DependencyProperty.Register(
            nameof(IsTrue),
            typeof(bool),
            typeof(CornerTunnelTextBox)
        );
    }
}