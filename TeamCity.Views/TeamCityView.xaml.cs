using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TeamCity.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class TeamCityView : UserControl
    {
        private TeamCity.Presenters.TeamCityPresenter _presenter;
        private Dictionary<string, TextBlock> _configurations;

        private readonly Brush SuccessBrush = Brushes.Green;
        private readonly Brush FailedBrush = Brushes.Red;
        private readonly Brush UnknownBrush = Brushes.Yellow;

        public TeamCityView(TeamCity.Presenters.TeamCityPresenter presenter)
        {
            InitializeComponent();

            _presenter = presenter;
            _configurations = new Dictionary<string, TextBlock>();
            AddControls();
            _presenter.BuildStatusUpdate += _presenter_BuildStatusUpdate;
            _presenter.ManualUpdate();
        }

        private void AddControls()
        {
            foreach(string configuration in _presenter.Configurations)
            {
                var grid = new Grid()
                {
                    AllowDrop = true
                };
                var holder = new Viewbox()
                {
                    Margin = new Thickness(5.0)
                };

                var text = new TextBlock()
                {
                    AllowDrop = true,
                    FontFamily = new FontFamily("Comic Sans MS") //https://stackoverflow.com/questions/6993736/list-of-built-in-wpf-fonts
                    
                };
                text.Text = configuration;
                text.Foreground = Brushes.Black;
                grid.Background = UnknownBrush;
                
                grid.Children.Add(holder);
                holder.Child = text;
                Grid.SetRow(grid, mainGrid.RowDefinitions.Count);
                
                mainGrid.RowDefinitions.Add(new RowDefinition());
                mainGrid.Children.Add(grid);

                _configurations.Add(configuration, text);
            }
        }

        private void _presenter_BuildStatusUpdate(object sender, Presenters.BuildStatusUpdateEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                var text = _configurations[e.BuildConfiguration];
                text.Text = e.Name;
                var viewBox = text.Parent as Viewbox;
                var grid = viewBox.Parent as Grid;
                Brush background = Brushes.Black;
                switch(e.Status)
                {
                    case Presenters.BuildStatus.Success:
                        background = SuccessBrush;
                        break;
                    case Presenters.BuildStatus.Failed:
                        background = FailedBrush;
                        break;
                    case Presenters.BuildStatus.Unknown:
                        background = UnknownBrush;
                        break;
                }
                grid.Background = background;
            }));
        }
    }
}
