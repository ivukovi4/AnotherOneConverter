using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace AnotherOneConverter.WPF.Controls {
    public static class ProgressBarExtension {
        public static double GetAnimatedValue(DependencyObject obj) {
            return (double)obj.GetValue(AnimatedValueProperty);
        }

        public static void SetAnimatedValue(DependencyObject obj, double value) {
            obj.SetValue(AnimatedValueProperty, value);
        }

        public static readonly DependencyProperty AnimatedValueProperty =
            DependencyProperty.RegisterAttached("AnimatedValue", typeof(double), typeof(ProgressBarExtension), new PropertyMetadata(0.0, OnAnimatedValueChanged));

        private static void OnAnimatedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var animmation = new DoubleAnimation((double)e.OldValue, (double)e.NewValue, new TimeSpan(0, 0, 0, 0, 250));
            ((ProgressBar)d).BeginAnimation(RangeBase.ValueProperty, animmation, HandoffBehavior.Compose);
        }
    }
}
