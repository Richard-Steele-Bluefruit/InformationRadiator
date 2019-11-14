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
using PresenterCommon.Configuration;

namespace InformationRadiator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PresenterCommon.PresenterResources _presenterResources;
        PresenterCommon.MainWindowPresenter _presenter;
        List<MainWindow> _windows;

        public MainWindow()
        {
            InitializeComponent();
            _presenterResources = new PresenterCommon.PresenterResources();
            _presenter = new PresenterCommon.MainWindowPresenter();
            _presenter.CreateView += _presenter_CreateView;

            _windows = new List<MainWindow>();

            foreach(var screen in System.Windows.Forms.Screen.AllScreens)
            {
                MainWindow window;

                if(screen.Primary)
                {
                    window = this;
                }
                else
                {
                    window = new MainWindow(_presenterResources, _windows);
                }
                _windows.Add(window);

                window.Show();
                window.Top = screen.WorkingArea.Top;
                window.Left = screen.WorkingArea.Left;
                window.WindowState = System.Windows.WindowState.Maximized;
                window.Topmost = true;
                break;
            }

            CommonConstruction();

            _presenter.ParseConfiguration(_presenterResources.Configuration, _presenterResources.PresenterFactory);

        }

        public MainWindow(PresenterCommon.PresenterResources presenterResources, List<MainWindow> windows)
        {
            InitializeComponent();
            _presenterResources = presenterResources;
            _windows = windows;

            this.ShowInTaskbar = false;

            CommonConstruction();
        }

        private void CommonConstruction()
        {
#if DEBUG
            Topmost = false;
            WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
            Cursor = Cursors.Arrow;
#endif
        }

        void _presenter_CreateView(object sender, PresenterCommon.MainWindowPresenter.CreateViewEventArgs e)
        {
            if (e.Screen >= 0 && e.Screen < _windows.Count)
            {
                _windows[e.Screen].AddInformationRadiatorView(e);
            }
        }

        public void AddInformationRadiatorView(PresenterCommon.MainWindowPresenter.CreateViewEventArgs e)
        {
            var view = _presenterResources.ViewFactory.CreateObject(e.ItemType, e.Presenter) as Control;

            if (!string.IsNullOrEmpty(e.Title))
                view = new ItemContainer(e.Title, view);

            view.Width = e.Width;
            view.Height = e.Height;

            if (e.Left == null || e.Top == null)
            {
                layout.Children.Add(view);
            }
            else
            {
                absoluteLayout.Children.Add(view);
                Canvas.SetLeft(view, e.Left ?? 0);
                Canvas.SetTop(view, e.Top ?? 0);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_windows != null)
            {
                foreach (var window in _windows)
                {
                    if (window != this && window.IsVisible)
                    {
                        window.Close();
                    }
                }
            }
        }
    }
}
