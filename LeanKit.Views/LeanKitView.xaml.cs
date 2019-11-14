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

namespace LeanKit.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class LeanKitView : UserControl
    {
        private LeanKit.Presenters.LeanKitPresenter _presenter;

        public LeanKitView(LeanKit.Presenters.LeanKitPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            _presenter.ProgressUpdate += _presenter_ProgressUpdate;
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback((o) => { _presenter.ManualUpdate(); }));
        }

        void _presenter_ProgressUpdate(object sender, Presenters.ProgressUpdateEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                unknownColumn.Width = new GridLength(e.UntypedPoints, GridUnitType.Star);
                unknownText.Text = e.UntypedPoints.ToString();

                readyColumn.Width = new GridLength(e.ReadyPoints, GridUnitType.Star);
                readyText.Text = e.ReadyPoints.ToString();
                
                inProgressColumn.Width = new GridLength(e.InProgressPoints, GridUnitType.Star);
                inProgressText.Text = e.InProgressPoints.ToString();
                
                completeColumn.Width = new GridLength(e.CompletePoints, GridUnitType.Star);
                completeText.Text = e.CompletePoints.ToString();
            }));
        }
    }
}
