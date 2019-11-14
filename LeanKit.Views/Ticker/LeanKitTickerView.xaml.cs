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
using System.Windows.Navigation;
using System.Windows.Shapes;

using LeanKit.Presenters.Ticker;

namespace LeanKit.Views.Ticker
{
    /// <summary>
    /// Interaction logic for LeanKitTicker.xaml
    /// </summary>
    public partial class LeanKitTickerView : UserControl
    {
        public LeanKitTickerView(LeanKitTickerPresenter presenter)
        {
            InitializeComponent();

            presenter.TickerUpdate += presenter_TickerUpdate;
            presenter.Update();
        }

        void presenter_TickerUpdate(object sender, TickerUpdateEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                messageText.Text = e.Message;
            }));
        }
    }
}
