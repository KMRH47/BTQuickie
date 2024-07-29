using System.Windows;

namespace BTQuickie.Helpers;

public static class DependencyPropertyUtils
{
  public static DependencyProperty Register<TProp, TOwner>(
    string propertyName,
    FrameworkPropertyMetadata? metadata = null) {
    return DependencyProperty.Register(
      propertyName,
      typeof(TProp),
      typeof(TOwner),
      metadata ?? new FrameworkPropertyMetadata());
  }

  public static DependencyProperty RegisterAttached<TProp, TOwner>(
    string propertyName,
    PropertyMetadata? metadata = null) {
    return DependencyProperty.RegisterAttached(
      propertyName,
      typeof(TProp),
      typeof(TOwner),
      metadata ?? new PropertyMetadata());
  }
}