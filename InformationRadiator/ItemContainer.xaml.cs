using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InformationRadiator
{
    /// <summary>
    /// Interaction logic for ItemContainer.xaml
    /// </summary>
    public partial class ItemContainer : UserControl
    {
        public ItemContainer(string title, Control view)
        {
            InitializeComponent();
            
            titleText.Text = title;

            mainGrid.Children.Add(view);
            Grid.SetRow(view, 2);
            Grid.SetColumn(view, 1);
        }
    }
}
