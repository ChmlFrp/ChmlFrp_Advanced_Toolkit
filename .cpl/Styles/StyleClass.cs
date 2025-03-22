using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ChmlFrp_Professional_Launcher
{
    public class CornerIconRadioButton : RadioButton
    {
        //Data
        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data",
            typeof(object),
            typeof(CornerIconRadioButton)
        );
    }

    public class TransparentIconRadioButton : CornerIconRadioButton { }

    public class CornerButten : Button
    {
        //CornerRadius
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(CornerButten));

        //IsSelected
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected",
            typeof(bool),
            typeof(CornerButten)
        );
    }

    public class CornerTextBox : TextBox, INotifyPropertyChanged
    {
        private string TempText;

        public CornerTextBox()
        {
            GotFocus += TextBox_GotFocus;
            LostFocus += TextBox_LostFocus;
        }

        //CornerRadius
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                "CornerRadius",
                typeof(CornerRadius),
                typeof(CornerTextBox)
            );

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        //Postscript
        public static readonly DependencyProperty PostscriptProperty = DependencyProperty.Register(
            "Postscript",
            typeof(string),
            typeof(CornerTextBox)
        );

        public string Postscript
        {
            get { return (string)GetValue(PostscriptProperty); }
            set { SetValue(PostscriptProperty, value); }
        }

        //Text
        public static new readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(CornerTextBox),
            new PropertyMetadata(string.Empty, OnTextChanged)
        );

        public new string Text
        {
            get { return (string)GetValue(TextProperty); }
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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Focus
        int i;

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            i++;
            if (i == 1)
                TempText = Postscript;
            Postscript = "";
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Text == "")
                Postscript = TempText;
        }
    }

    public class CornerComboBox : ComboBox
    {
        //CornerRadius
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                "CornerRadius",
                typeof(CornerRadius),
                typeof(CornerComboBox)
            );
    }
}
