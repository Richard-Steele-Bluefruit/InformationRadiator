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

namespace General.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SprintDaysView : UserControl
    {
        private General.Presenters.SprintDaysPresenter _presenter;

        public SprintDaysView(General.Presenters.SprintDaysPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            sprintDay.Text = _presenter.SprintDayText;

            _presenter.SprintDayUpdated += _presenter_SprintDayUpdated;
        }

        void _presenter_SprintDayUpdated(object sender, General.Presenters.SprintDaysPresenter.SprintDayUpdatedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() => { sprintDay.Text = e.SprintDayText; }));
        }
    }
}
