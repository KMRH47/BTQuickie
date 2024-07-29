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
///   A user-control for setting hotkeys.<br />
/// </summary>
public partial class HotkeySetter
{
  private const string Separator = " + ";

  public static readonly DependencyProperty ProgressBarColorProperty =
    RegisterDependencyProperty<Color>(nameof(ProgressBarColor));

  public static readonly DependencyProperty HotkeyProperty =
    RegisterDependencyProperty<Hotkey>(nameof(Hotkey));

  public static readonly DependencyProperty ModifierKeysProperty =
    RegisterDependencyProperty<ModifierKeys>(nameof(ModifierKeys));

  public static readonly DependencyProperty KeyProperty =
    RegisterDependencyProperty<Key>(nameof(Key));

  private static DependencyProperty RegisterDependencyProperty<T>(string propertyName) =>
    DependencyProperty.Register(propertyName, typeof(T), typeof(HotkeySetter), new FrameworkPropertyMetadata());

  private readonly Duration loadDuration = new(TimeSpan.FromSeconds(1));
  private readonly DoubleAnimation progressBarToMaxDoubleAnimation;
  private readonly DoubleAnimation progressBarToZeroDoubleAnimation;
  private readonly Duration resetDuration = new(TimeSpan.FromMilliseconds(200));
  private readonly DoubleAnimation textGridFadeToOneDoubleAnimation;
  private readonly DoubleAnimation textGridFadeToZeroDoubleAnimation;
  private readonly ColorAnimation toDefaultColorAnimation;
  private readonly ColorAnimation toGreenColorAnimation;
  private CancellationTokenSource cancellationTokenSource = new();
  private Key key;
  private ModifierKeys modifierKeys;
  private string[] pressedKeys = new string[8];

  public HotkeySetter() {
    InitializeComponent();

    // Create animations
    progressBarToZeroDoubleAnimation = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(80)));
    progressBarToMaxDoubleAnimation = new DoubleAnimation(0, 100, loadDuration);
    textGridFadeToZeroDoubleAnimation = new DoubleAnimation(0, resetDuration);
    textGridFadeToOneDoubleAnimation = new DoubleAnimation(1, resetDuration);
    toGreenColorAnimation = new ColorAnimation(Colors.Green, resetDuration);
    toDefaultColorAnimation =
      new ColorAnimation(((SolidColorBrush)ProgressBar.Foreground).Color, resetDuration);

    // Subscribe to events
    HotkeyTextBox.PreviewKeyDown += OnPreviewKeyDown;
    HotkeyTextBox.PreviewKeyUp += OnPreviewKeyUp;
    HotkeyTextBox.IsKeyboardFocusedChanged += OnIsKeyboardFocusedChanged;
    HotkeyTextBox.TextChanged += OnHotkeyTextChanged;
    ProgressBar.ValueChanged += OnProgressBarValueChanged;
    textGridFadeToZeroDoubleAnimation.Completed += TextGridFadeToZeroDoubleAnimationOnCompleted;

    // Override initial brush to circumvent object freeze/locking
    SolidColorBrush brush = new(((SolidColorBrush)ProgressBar.Foreground).Color);
    ProgressBar.Foreground = brush;
  }

  public Color ProgressBarColor {
    get => (Color)GetValue(ProgressBarColorProperty);
    set => SetValue(ProgressBarColorProperty, value);
  }

  public Hotkey Hotkey {
    get => (Hotkey)GetValue(HotkeyProperty);
    set => SetValue(HotkeyProperty, value);
  }

  public ModifierKeys ModifierKeys {
    get => (ModifierKeys)GetValue(ModifierKeysProperty);
    set => SetValue(ModifierKeysProperty, value);
  }

  public Key Key {
    get => (Key)GetValue(KeyProperty);
    set => SetValue(KeyProperty, value);
  }

  private void OnPreviewKeyDown(object sender, KeyEventArgs e) {
    try {
      if (e.IsRepeat) {
        return;
      }

      if (e.IdenticalModifierKeyStroke()) {
        return;
      }

      var realKey = e.RealKey().ToString();

      if (!e.IsModifierKey()) {
        bool containsKey = pressedKeys.Contains(key.ToString());

        if (containsKey) {
          return;
        }

        key = e.RealKey();

        if (HotkeyTextBox.Text == realKey) {
          return;
        }

        HotkeyTextBox.Text += HotkeyTextBox.Text.Length is 0
          ? realKey
          : $"{Separator}{realKey}";

        StartProgressBar();
      }
      else if (HotkeyTextBox.Text != realKey) {
        if (pressedKeys.Contains(realKey)) {
          return;
        }

        modifierKeys = e.ModifierKeys();
        Debug.WriteLine($"Modifiers: {e.ModifierKeys()}");

        HotkeyTextBox.Text = HotkeyTextBox.Text.Length is 0
          ? realKey
          : $"{realKey}{Separator}{HotkeyTextBox.Text}";
      }
    }
    finally {
      e.Handled = true;
    }
  }

  private void OnPreviewKeyUp(object sender, KeyEventArgs e) {
    try {
      ResetProgressBar();

      if (!e.IsModifierKey()) {
        if (e.RealKey() != key) {
          return;
        }

        key = Key.None;
      }

      var realKey = e.RealKey().ToString();

      if (!pressedKeys.Contains(realKey)) {
        return;
      }

      string keyToRemove = pressedKeys.First(key => key == realKey);
      string[] formattedKeys = pressedKeys.Where(key => key != keyToRemove).ToArray();

      modifierKeys = e.ModifierKeys();
      HotkeyTextBox.Text = string.Join(Separator, formattedKeys);
    }
    finally {
      e.Handled = true;
    }
  }

  private void OnHotkeyTextChanged(object sender, TextChangedEventArgs e) {
    pressedKeys = HotkeyTextBox.Text.Split(Separator);
  }

  private void OnIsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e) {
    ResetProgressBar();
    HotkeyTextBox.Text = string.Empty;
  }

  private void OnProgressBarValueChanged(object _, RoutedPropertyChangedEventArgs<double> propertyEventArgs) {
    if (propertyEventArgs.NewValue is not 100) {
      return;
    }

    Key = key;
    ModifierKeys = modifierKeys;
    string modiferKeysFormatted = new ModifierKeysConverter().ConvertToString(ModifierKeys)!;
    Hotkey = new Hotkey(Key.ToString(), modiferKeysFormatted);
    ProgressBar.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, toGreenColorAnimation);
    PopupHotkey.Content = $"'{HotkeyTextBox.Text}'";
    _ = ShowHotkeyBoundText(cancellationTokenSource.Token);
  }

  private void StartProgressBar() {
    cancellationTokenSource.Cancel();
    cancellationTokenSource = new CancellationTokenSource();
    ProgressBar.BeginAnimation(RangeBase.ValueProperty, progressBarToMaxDoubleAnimation);
  }

  private void ResetProgressBar() {
    if (ProgressBar.Value is 100) {
      ProgressBar.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, toDefaultColorAnimation);
    }

    ProgressBar.BeginAnimation(RangeBase.ValueProperty, progressBarToZeroDoubleAnimation);
  }

  private void TextGridFadeToZeroDoubleAnimationOnCompleted(object? sender, EventArgs e) {
    HotkeyBoundTextGrid.Visibility = Visibility.Hidden;
  }

  private async Task ShowHotkeyBoundText(CancellationToken cancellationToken) {
    HotkeyBoundTextGrid.Visibility = Visibility.Visible;
    HotkeyBoundTextGrid.BeginAnimation(OpacityProperty, textGridFadeToOneDoubleAnimation);
    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
    HotkeyBoundTextGrid.BeginAnimation(OpacityProperty, textGridFadeToZeroDoubleAnimation);
  }

  private void UIElement_OnGotFocus(object sender, RoutedEventArgs e) {
    HotkeyGuideLabel.Visibility = Visibility.Collapsed;
  }

  private void UIElement_OnLostFocus(object sender, RoutedEventArgs e) {
    HotkeyGuideLabel.Visibility = Visibility.Visible;
  }

  protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
    base.OnLostKeyboardFocus(e);
    cancellationTokenSource.Cancel();
    HotkeyBoundTextGrid.BeginAnimation(OpacityProperty, textGridFadeToZeroDoubleAnimation);
  }
}