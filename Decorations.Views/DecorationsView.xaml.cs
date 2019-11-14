using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Decorations.Views
{
    /// <summary>
    /// Interaction logic for DecorationsView.xaml
    /// </summary>
    public partial class DecorationsView : UserControl
    {
        private Decorations.Presenters.DecorationsPresenter _presenter;

        private double bunnySteveX = 0.0;
        private double bunnySteveY = 0.0;
        private double bunnyPaulX = 0.0;
        private double bunnyPaulY = 0.0;
        private Random bunnyRandom;


        public DecorationsView(Decorations.Presenters.DecorationsPresenter presenter)
        {
            InitializeComponent();

            bunnyRandom = new Random();

            _presenter = presenter;

            _presenter.Update += presenter_Update;
            _presenter.SteveQuoteUpdate += presenter_SteveQuoteUpdate;
            UpdateDecorations();
            UpdateSteveQuote(false);

            BunnySteveEndAnimation(this, EventArgs.Empty);
        }

        private void presenter_Update(object sender, EventArgs e)
        {
            var x = new Action(() => { UpdateDecorations(); });
            Dispatcher.Invoke(x);
        }

        private void UpdateDecorations()
        {
            canvasChristmas.Visibility = _presenter.IsChristmas ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            canvasEaster.Visibility = _presenter.IsEaster ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        private void UpdateSteveQuote(bool visible, string quote = "")
        {
            canvasSteveAngel.Visibility = visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            textblockSteveQuote.Text = quote;
        }

        private void presenter_SteveQuoteUpdate(object sender, Presenters.DecorationsPresenter.SteveQuoteEventArgs e)
        {
            var x = new Action(() => { UpdateSteveQuote(e.ShowSteveAngel, e.Quote); });
            Dispatcher.Invoke(x);
        }

        private void BunnySteveEndAnimation(object sender, EventArgs eventArgs)
        {
            double endX = bunnyRandom.NextDouble() * (1920.0 - canvasBunnyPaul.Width);
            double endY = bunnyRandom.NextDouble() * (1080.0 - canvasBunnyPaul.Height);

            Duration tX = new Duration(TimeSpan.FromSeconds(2));
            Duration tY = new Duration(TimeSpan.FromSeconds(2));

            var moveAnimationX = new DoubleAnimation(bunnyPaulX, endX, tX);
            var moveAnimationY = new DoubleAnimation(bunnyPaulY, endY, tY);

            moveAnimationY.Completed += BunnyPaulEndAnimation;

            var transform = new ScaleTransform()
            {
                ScaleX = endX > bunnyPaulX ? -1 : 1
            };
            canvasBunnyPaul.RenderTransform = transform;

            canvasBunnyPaul.BeginAnimation(Canvas.LeftProperty, moveAnimationX);
            canvasBunnyPaul.BeginAnimation(Canvas.TopProperty, moveAnimationY);

            bunnyPaulX = endX;
            bunnyPaulY = endY;
        }

        private void BunnyPaulEndAnimation(object sender, EventArgs eventArgs)
        {
            double endX = bunnyRandom.NextDouble() * (1920.0 - canvasBunnySteve.Width);
            double endY = bunnyRandom.NextDouble() * (1080.0 - canvasBunnySteve.Height);

            Duration tX = new Duration(TimeSpan.FromSeconds(2));
            Duration tY = new Duration(TimeSpan.FromSeconds(2));

            var moveAnimationX = new DoubleAnimation(bunnySteveX, endX, tX);
            var moveAnimationY = new DoubleAnimation(bunnySteveY, endY, tY);

            moveAnimationY.Completed += BunnySteveEndAnimation;

            var transform = new ScaleTransform()
            {
                ScaleX = endX > bunnySteveX ? -1 : 1
            };
            canvasBunnySteve.RenderTransform = transform;

            canvasBunnySteve.BeginAnimation(Canvas.LeftProperty, moveAnimationX);
            canvasBunnySteve.BeginAnimation(Canvas.TopProperty, moveAnimationY);

            bunnySteveX = endX;
            bunnySteveY = endY;
        }
    }
}
