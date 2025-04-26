namespace CPL.Styles;

public class CornerIconRadioButton : RadioButton
{
    public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
        nameof(Data),
        typeof(object),
        typeof(CornerIconRadioButton)
    );

    //Data
    public object Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }
}

public class TransparentIconRadioButton : CornerIconRadioButton;

public class CornerButton : Button
{
    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(CornerButton));

    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
        nameof(IsSelected),
        typeof(bool),
        typeof(CornerButton)
    );

    //CornerRadius
    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    //IsSelected
    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }
}

public sealed class CornerTextBox : TextBox
{
    //CornerRadius
    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(CornerTextBox)
        );

    //Postscript
    public static readonly DependencyProperty PostscriptProperty = DependencyProperty.Register(
        nameof(Postscript),
        typeof(string),
        typeof(CornerTextBox)
    );

    //IsPassword
    public static readonly DependencyProperty IsPasswordProperty = DependencyProperty.Register(
        nameof(IsPassword),
        typeof(bool),
        typeof(CornerTextBox)
    );

    private string _tempText;

    public CornerTextBox()
    {
        TextChanged += OnTextChanged;
    }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public string Postscript
    {
        get => (string)GetValue(PostscriptProperty);
        set => SetValue(PostscriptProperty, value);
    }

    public bool IsPassword
    {
        get => (bool)GetValue(IsPasswordProperty);
        set => SetValue(IsPasswordProperty, value);
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_tempText)) _tempText = Postscript;
        Postscript = string.IsNullOrWhiteSpace(Text) ? _tempText : "";
    }
}

public class CornerComboBox : ComboBox
{
    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(CornerComboBox)
        );

    // CornerRadius
    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }
}

public class CornerTextBlock : TextBox
{
    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(CornerTextBlock)
        );

    //CornerRadius
    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }
}

public class CornerTunnelTextBox : CornerTextBlock
{
    public static readonly DependencyProperty IsTrueProperty = DependencyProperty.Register(
        nameof(IsTrue),
        typeof(bool),
        typeof(CornerTunnelTextBox)
    );

    //IsTrue
    public bool IsTrue
    {
        get => (bool)GetValue(IsTrueProperty);
        set => SetValue(IsTrueProperty, value);
    }
}