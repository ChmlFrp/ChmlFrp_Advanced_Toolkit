using System.Windows.Controls;
using System.Windows.Media;

namespace CAT.Views.Windows.Styles;

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

    //CornerRadius
    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
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

    // HasSelection
    public static readonly DependencyProperty HasSelectionProperty =
        DependencyProperty.Register(nameof(HasSelection), typeof(bool), typeof(CornerComboBox));

    // CornerRadius
    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public bool HasSelection
    {
        get => (bool)GetValue(HasSelectionProperty);
        private set => SetValue(HasSelectionProperty, value);
    }

    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        base.OnSelectionChanged(e);
        HasSelection = SelectedIndex >= 0;
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

public class UserCard : TextBox;

public class Card : Control
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(Card), new PropertyMetadata(""));

    public static readonly DependencyProperty TagTextProperty =
        DependencyProperty.Register(nameof(TagText), typeof(string), typeof(Card), new PropertyMetadata(""));

    public static readonly DependencyProperty ImageSourceProperty =
        DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(Card), new PropertyMetadata(null));

    static Card()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Card), new FrameworkPropertyMetadata(typeof(Card)));
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string TagText
    {
        get => (string)GetValue(TagTextProperty);
        set => SetValue(TagTextProperty, value);
    }

    public ImageSource ImageSource
    {
        get => (ImageSource)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }
}