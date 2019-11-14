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

namespace HolidayCalendar.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class HolidayCalendarView : UserControl
    {
        public HolidayCalendarView(HolidayCalendar.Presenters.IHolidayCalendarPresenter presenter)
        {
            InitializeComponent();

            presenter.HolidayCalendarUpdate += presenter_HolidayCalendarUpdate;
            presenter.ErrorLoadingHolidayInformation += presenter_ErrorLoadingHolidayInformation;

            presenter.UpdateHolidayCalendar();
        }

        private int currentRow;

        TextBlock AddAutoSizeTextBlockToNewRow(SolidColorBrush background = null, SolidColorBrush foreground = null)
        {
            if (currentRow >= gridMain.RowDefinitions.Count)
            {
                gridMain.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Star)});
            }

            if (background != null)
            {
                var label = new Label
                {
                    Background = background,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };
                gridMain.Children.Add(label);
                Grid.SetRow(label, currentRow);
                Grid.SetColumn(label, gridMain.ColumnDefinitions.Count - 1);
            }

            var textBlock = new TextBlock { Foreground = foreground ?? Brushes.White };
            var viewBox = new Viewbox { Child = textBlock, Margin = new Thickness(5,0,5,0)};
            gridMain.Children.Add(viewBox);
            Grid.SetRow(viewBox, currentRow);
            Grid.SetColumn(viewBox, gridMain.ColumnDefinitions.Count - 1);

            currentRow++;
            return textBlock;
        }

        private void ResetGrid()
        {
            gridMain.Children.Clear();
            gridMain.RowDefinitions.Clear();
            gridMain.ColumnDefinitions.Clear();
            currentRow = 0;
        }

        void AddColumn()
        {
            gridMain.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)});
            currentRow = 0;
        }

        private void presenter_ErrorLoadingHolidayInformation(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                ResetGrid();
                AddColumn();
                AddAutoSizeTextBlockToNewRow().Text = "No Data";
            });
        }

        private void presenter_HolidayCalendarUpdate(object sender, Presenters.HolidayCalendarUpdateEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                ResetGrid();
                DateTime date = DateTime.Now.Date;
                DateTime endDate = date.AddDays(14).Date;

                while (date.Date < endDate)
                {
                    bool isWeekend = (date.DayOfWeek == DayOfWeek.Saturday) || (date.DayOfWeek == DayOfWeek.Sunday);
                    var backgroundBrush = isWeekend ? Brushes.Gray : Brushes.DarkGray;
                    AddColumn();
                    AddAutoSizeTextBlockToNewRow(background: backgroundBrush, foreground: Brushes.Black).Text =
                        date.Date.ToLongDateString();

                    var day = e.Days.FirstOrDefault(d => d.Date == date);
                    if (day != null)
                    {
                        foreach (var person in day.PeopleOnLeave)
                        {
                            AddAutoSizeTextBlockToNewRow().Text = person;
                        }
                    }

                    date = date.AddDays(1);
                }

                AddAutoSizeTextBlockToNewRow(background: Brushes.DarkSlateGray).Text =
                    e.DownloadedTime.ToLongDateString() + " " + e.DownloadedTime.ToShortTimeString();
            });
        }
    }
}
