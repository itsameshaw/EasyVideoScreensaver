using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace EasyVideoScreensaver
{
    public class DVDAnimation
    {
        private UIElement _element;
        private Window _window;
        private TranslateTransform _transform;
        private double _dx = 1;
        private double _dy = 1;
        private double _elementWidth; 
        private double _elementHeight;

        public DVDAnimation(UIElement elementToAnimate, Window windowToAnimateIn)
        {
            _element = elementToAnimate;
            _window = windowToAnimateIn;

            _window.Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)_element).Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity)); 
            ((FrameworkElement)_element).Arrange(new Rect(((FrameworkElement)_element).DesiredSize)); 
            _elementWidth = Math.Ceiling(((FrameworkElement)_element).ActualWidth); 
            _elementHeight = Math.Ceiling(((FrameworkElement)_element).ActualHeight);

            var random = new Random();
            double initialX = random.NextDouble() * (_window.ActualWidth - _elementWidth);
            double initialY = random.NextDouble() * (_window.ActualHeight - _elementHeight);

            _transform = new TranslateTransform(initialX, initialY);
            _element.RenderTransform = _transform;

            CompositionTarget.Rendering += OnRender;
        }

        private void OnRender(object sender, EventArgs e)
        {
            double newX = _transform.X + _dx;
            double newY = _transform.Y + _dy;

            if (newX < 0 || newX + _elementWidth > _window.ActualWidth)
            {
                _dx = -_dx;
                newX = _transform.X + _dx;
            }

            if (newY < 0 || newY + _elementHeight > _window.ActualHeight)
            {
                _dy = -_dy;
                newY = _transform.Y + _dy;
            }

            _transform.X = newX;
            _transform.Y = newY;
        }
    }
}
