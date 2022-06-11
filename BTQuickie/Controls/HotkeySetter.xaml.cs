using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BTQuickie.Extensions;
using BTQuickie.Models.Hotkey;

namespace BTQuickie.Controls;

/// <summary>
/// A user-control for setting hotkeys.<br/>
/// </summary>
public partial class HotkeySetter
{
    private const string Separator = " + ";
    private readonly Duration loadDuration = new(TimeSpan.FromSeconds(1));
    private readonly Duration resetDuration = new(TimeSpan.FromMilliseconds(200));
    private readonly DoubleAnimation progressBarToZeroDoubleAnimation;
    private readonly DoubleAnimation progressBarToMaxDoubleAnimation;
    private readonly DoubleAnimation textGridFadeToZeroDoubleAnimation;
    private readonly DoubleAnimation textGridFadeToOneDoubleAnimation;
    private readonly ColorAnimation toGreenColorAnimation;
    private readonly ColorAnimation toDefaultColorAnimation;
    private CancellationTokenSource cancellationTokenSource = new();
    private string[] pressedKeys = new string[8];
    private Key key;
    private ModifierKeys modifierKeys;

    public HotkeySetter()
    {
        InitializeComponent();

        // Create animations
        this.progressBarToZeroDoubleAnimation = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(80)));
        this.progressBarToMaxDoubleAnimation = new DoubleAnimation(0, 100, this.loadDuration);
        this.textGridFadeToZeroDoubleAnimation = new DoubleAnimation(0, this.resetDuration);
        this.textGridFadeToOneDoubleAnimation = new DoubleAnimation(1, this.resetDuration);
        this.toGreenColorAnimation = new ColorAnimation(Colors.Green, this.resetDuration);
        this.toDefaultColorAnimation =
            new ColorAnimation(((SolidColorBrush) ProgressBar.Foreground).Color, this.resetDuration);

        // Subscribe to events
        this.HotkeyTextBox.PreviewKeyDown += OnPreviewKeyDown;
        this.HotkeyTextBox.PreviewKeyUp += OnPreviewKeyUp;
        this.HotkeyTextBox.IsKeyboardFocusedChanged += OnIsKeyboardFocusedChanged;
        this.HotkeyTextBox.TextChanged += OnHotkeyTextChanged;
        ProgressBar.ValueChanged += OnProgressBarValueChanged;
        this.textGridFadeToZeroDoubleAnimation.Completed += TextGridFadeToZeroDoubleAnimationOnCompleted;

        // Override initial brush to circumvent object freeze/locking
        SolidColorBrush brush = new(((SolidColorBrush) ProgressBar.Foreground).Color);
        ProgressBar.Foreground = brush;
    }

    public Color ProgressBarColor
    {
        get => (Color) GetValue(ProgressBarColorProperty);
        set => SetValue(ProgressBarColorProperty, value);
    }

    public Hotkey Hotkey
    {
        get => (Hotkey) GetValue(HotkeyProperty);
        set => SetValue(HotkeyProperty, value);
    }

    public ModifierKeys ModifierKeys
    {
        get => (ModifierKeys) GetValue(ModifierKeysProperty);
        set => SetValue(ModifierKeysProperty, value);
    }

    public Key Key
    {
        get => (Key) GetValue(KeyProperty);
        set => SetValue(KeyProperty, value);
    }
    
    public static readonly DependencyProperty ProgressBarColorProperty
        = DependencyProperty.Register(name: nameof(ProgressBarColor),
                                      propertyType: typeof(Color),
                                      ownerType: typeof(HotkeySetter),
                                      typeMetadata: new FrameworkPropertyMetadata());

    public static readonly DependencyProperty ModifierKeysProperty
        = DependencyProperty.Register(name: nameof(ModifierKeys),
                                      propertyType: typeof(ModifierKeys),
                                      ownerType: typeof(HotkeySetter),
                                      typeMetadata: new FrameworkPropertyMetadata());

    public static readonly DependencyProperty KeyProperty
        = DependencyProperty.Register(name: nameof(Key),
                                      propertyType: typeof(Key),
                                      ownerType: typeof(HotkeySetter),
                                      typeMetadata: new FrameworkPropertyMetadata());

    public static readonly DependencyProperty HotkeyProperty
        = DependencyProperty.Register(name: nameof(Hotkey),
                                      propertyType: typeof(Hotkey),
                                      ownerType: typeof(HotkeySetter),
                                      typeMetadata: new FrameworkPropertyMetadata());

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        try
        {
            if (e.IsRepeat)
            {
                return;
            }

            if (e.IdenticalModifierKeyStroke())
            {
                return;
            }

            string realKey = e.RealKey().ToString();

            if (!e.IsModifierKey())
            {
                bool containsKey = this.pressedKeys.Contains(this.key.ToString());

                if (containsKey)
                {
                    return;
                }

                this.key = e.RealKey();

                if (this.HotkeyTextBox.Text == realKey)
                {
                    return;
                }

                this.HotkeyTextBox.Text += this.HotkeyTextBox.Text.Length is 0
                                               ? realKey
                                               : $"{Separator}{realKey}";

                StartProgressBar();
            }
            else if (this.HotkeyTextBox.Text != realKey)
            {
                if (this.pressedKeys.Contains(realKey))
                {
                    return;
                }

                this.modifierKeys = e.ModifierKeys();
                Debug.WriteLine($"Modifiers: {e.ModifierKeys()}");

                this.HotkeyTextBox.Text = this.HotkeyTextBox.Text.Length is 0
                                              ? realKey
                                              : $"{realKey}{Separator}{this.HotkeyTextBox.Text}";
            }
        }
        finally
        {
            e.Handled = true;
        }
    }

    private void OnPreviewKeyUp(object sender, KeyEventArgs e)
    {
        try
        {
            ResetProgressBar();

            if (!e.IsModifierKey())
            {
                if (e.RealKey() != this.key)
                {
                    return;
                }

                this.key = Key.None;
            }

            string realKey = e.RealKey().ToString();

            if (!this.pressedKeys.Contains(realKey))
            {
                return;
            }

            string keyToRemove = this.pressedKeys.First(key => key == realKey);
            string[] formattedKeys = this.pressedKeys.Where(key => key != keyToRemove).ToArray();

            this.modifierKeys = e.ModifierKeys();
            this.HotkeyTextBox.Text = string.Join(Separator, formattedKeys);
        }
        finally
        {
            e.Handled = true;
        }
    }

    private void OnHotkeyTextChanged(object sender, TextChangedEventArgs e)
    {
        this.pressedKeys = this.HotkeyTextBox.Text.Split(Separator);
    }

    private void OnIsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        ResetProgressBar();
        this.HotkeyTextBox.Text = string.Empty;
    }

    private void OnProgressBarValueChanged(object _, RoutedPropertyChangedEventArgs<double> propertyEventArgs)
    {
        if (propertyEventArgs.NewValue is not 100)
        {
            return;
        }

        Key = this.key;
        ModifierKeys = this.modifierKeys;
        string modiferKeysFormatted = new ModifierKeysConverter().ConvertToString(ModifierKeys)!;
        Hotkey = new Hotkey(Key.ToString(), modiferKeysFormatted);
        ProgressBar.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, this.toGreenColorAnimation);
        PopupHotkey.Content = $"'{this.HotkeyTextBox.Text}'";
        _ = ShowHotkeyBoundText(this.cancellationTokenSource.Token);
    }

    private void StartProgressBar()
    {
        this.cancellationTokenSource.Cancel();
        this.cancellationTokenSource = new CancellationTokenSource();
        ProgressBar.BeginAnimation(RangeBase.ValueProperty, this.progressBarToMaxDoubleAnimation);
    }

    private void ResetProgressBar()
    {
        if (ProgressBar.Value is 100)
        {
            ProgressBar.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, this.toDefaultColorAnimation);
        }

        ProgressBar.BeginAnimation(RangeBase.ValueProperty, this.progressBarToZeroDoubleAnimation);
    }

    private void TextGridFadeToZeroDoubleAnimationOnCompleted(object? sender, EventArgs e)
    {
        HotkeyBoundTextGrid.Visibility = Visibility.Hidden;
    }

    private async Task ShowHotkeyBoundText(CancellationToken cancellationToken)
    {
        HotkeyBoundTextGrid.Visibility = Visibility.Visible;
        this.HotkeyBoundTextGrid.BeginAnimation(OpacityProperty, this.textGridFadeToOneDoubleAnimation);
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        this.HotkeyBoundTextGrid.BeginAnimation(OpacityProperty, this.textGridFadeToZeroDoubleAnimation);
    }

    private void UIElement_OnGotFocus(object sender, RoutedEventArgs e)
    {
        HotkeyGuideLabel.Visibility = Visibility.Collapsed;
    }

    private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
    {
        HotkeyGuideLabel.Visibility = Visibility.Visible;
    }

    protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        base.OnLostKeyboardFocus(e);
        this.cancellationTokenSource.Cancel();
        this.HotkeyBoundTextGrid.BeginAnimation(OpacityProperty, this.textGridFadeToZeroDoubleAnimation);
    }
}