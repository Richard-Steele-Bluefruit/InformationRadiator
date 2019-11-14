using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GitVisualiser.Views
{
    /// <summary>
    /// Interaction logic for GitVisualiserView.xaml
    /// </summary>
    public partial class GitVisualiserView : UserControl
    {
        private class DrawData
        {
            public readonly string IslandName;
            public readonly int NumberOfDrawables;
            public readonly int IndexOfFirstUIElementInCanvas;
            public readonly List<System.Windows.Shapes.Shape> Drawables;

            public DrawData(string islandName, int numberOfDrawables, int indexOfFirstUIElementInCanvas, List<System.Windows.Shapes.Shape> drawables)
            {
                IslandName = islandName;
                NumberOfDrawables = numberOfDrawables;
                IndexOfFirstUIElementInCanvas = indexOfFirstUIElementInCanvas;
                Drawables = drawables;
            }
        }

        private bool _hasBeenFullyInitialised;
        private GitVisualiser.Presenters.GitVisualiserPresenter _presenter;
        private Label _archipelagoInformationLabel;
        private List<DrawData> _drawDataList;

        private const int InformationLabelZOrderIndex = 3;
        private const int ReleaseIslandFocusZOrderIndex = 2;
        private const int IslandFocusZOrderIndex = 1;
        private const int IslandNonFocusZOrderIndex = 0;

        public GitVisualiserView(GitVisualiser.Presenters.GitVisualiserPresenter presenter)
        {
            InitializeComponent();

            _presenter = presenter;

            // Due to the way that WPF works, programatically created UserControls (like GitVisualiserView) will not have this.ActualHeight or this.ActualWidth
            //      populated with values until an event is raised on this.SizeChanged event handler. We need these values for creating the release archipelago 
            //      in the centre. Hence why we have late initialisation.
            _hasBeenFullyInitialised = false;
            this.SizeChanged += new SizeChangedEventHandler(GitVisualiserView_SizeChanged);
        }

        void GitVisualiserView_SizeChanged(object sender, EventArgs e)
        {
            if (!_hasBeenFullyInitialised)
            {
                FinishInitialisation();

                _hasBeenFullyInitialised = true;
            }
        }

        private void FinishInitialisation()
        {
            var blue = Color.FromRgb(30, 144, 255);
            mainCanvas.Background = new SolidColorBrush(blue);

            _drawDataList = new List<DrawData>();

            _archipelagoInformationLabel = new Label();
            _archipelagoInformationLabel.Background = System.Windows.Media.Brushes.White;
            _archipelagoInformationLabel.Visibility = System.Windows.Visibility.Hidden;
            mainCanvas.Children.Add(_archipelagoInformationLabel);
            var labelIndex = (mainCanvas.Children.Count - 1);
            Canvas.SetZIndex(mainCanvas.Children[0], InformationLabelZOrderIndex);

            _presenter.AddNewIslandToView += _presenter_AddIslandToCanvas;
            _presenter.RemoveIslandFromView += _presenter_RemoveIslandFromCanvas;
            _presenter.DisplayTextualDataUpdate += _presenter_InformationLabelDisplayText;

            var controlWidth = this.ActualWidth;
            var controlHeight = this.ActualHeight;

            _presenter.FinishInitialisation(controlWidth, controlHeight);
        }

        private void _presenter_AddIslandToCanvas(object sender, Presenters.DisplayAddDrawableDataEventArgs displayAddDrawableDataEventArgs)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                var drawables = displayAddDrawableDataEventArgs.Drawables;

                // we will use _drawData later when displaying textual information
                var indexAddingDrawablesTo = mainCanvas.Children.Count;
                var newDrawData = new DrawData(displayAddDrawableDataEventArgs.Name, drawables.Count, indexAddingDrawablesTo, drawables);
                _drawDataList.Add(newDrawData);

                foreach (var polygonUIElement in drawables)
                {
                    mainCanvas.Children.Add(polygonUIElement);
                }
            }));
        }

        private void _presenter_RemoveIslandFromCanvas(object sender, Presenters.DisplayRemoveDrawableDataEventArgs displayRemoveDrawableDataEventArgs)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                var deletedIslandName = displayRemoveDrawableDataEventArgs.Name;
                var deletedIslandDrawData = _drawDataList.Find(f => f.IslandName == deletedIslandName);

                // remove the old one
                _drawDataList.Remove(deletedIslandDrawData);
                foreach (var polygonUIElement in deletedIslandDrawData.Drawables)
                {
                    mainCanvas.Children.Remove(polygonUIElement);
                }

                // reorder _drawDataList indices
                var newDrawDataItems = new List<DrawData>();
                var releaseArchipelagoDrawData = _drawDataList[0];
                newDrawDataItems.Add(releaseArchipelagoDrawData);

                var deletedIslandLastDrawableIndex = deletedIslandDrawData.IndexOfFirstUIElementInCanvas + deletedIslandDrawData.NumberOfDrawables - 1;
                foreach (var drawable in _drawDataList)
                {
                    if (drawable.IndexOfFirstUIElementInCanvas > deletedIslandLastDrawableIndex)
                    {
                        var numberOfIndicesToDecrementBy = deletedIslandDrawData.NumberOfDrawables;
                        var decrementedIndexOfFirstUIElementInCanvas = drawable.IndexOfFirstUIElementInCanvas - numberOfIndicesToDecrementBy;

                        var newDrawable = new DrawData(drawable.IslandName, drawable.NumberOfDrawables, decrementedIndexOfFirstUIElementInCanvas, drawable.Drawables);
                        newDrawDataItems.Add(newDrawable);
                    }
                    else
                    {
                        newDrawDataItems.Add(drawable);
                    }
                }
                _drawDataList = newDrawDataItems;
            }));
        }

        private void _presenter_InformationLabelDisplayText(object sender, Presenters.DisplayTextualDataEventArgs informationLabelEventArgs)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                var labelText = informationLabelEventArgs.TextToDisplay;
                _archipelagoInformationLabel.Content = labelText;

                _archipelagoInformationLabel.Width = informationLabelEventArgs.Width;
                _archipelagoInformationLabel.Height = informationLabelEventArgs.Height;

                var loc = informationLabelEventArgs.Location;
                _archipelagoInformationLabel.Margin = new Thickness(loc.X, loc.Y, 0, 0);
                _archipelagoInformationLabel.Visibility = System.Windows.Visibility.Visible;

                char[] delimiters = new char[] { '\r', '\n' };
                var islandName = labelText.Split(delimiters, StringSplitOptions.RemoveEmptyEntries)[0];

                ReorderUIElementsForTextualDisplay(islandName);
            }));
        }

        private void ReorderUIElementsForTextualDisplay(string islandName)
        {
            // reorder all of the islands
            var drawData = _drawDataList.Find(f => f.IslandName == islandName);
            var startOfSelectedIslandUIElements = drawData.IndexOfFirstUIElementInCanvas;
            var endOfSelectedIslandUIElements = drawData.IndexOfFirstUIElementInCanvas + (drawData.NumberOfDrawables);
            for (int i = 1; i < mainCanvas.Children.Count; i++)
            {
                int zOrderIndex = IslandNonFocusZOrderIndex;
                if (i == startOfSelectedIslandUIElements || i < endOfSelectedIslandUIElements)
                {
                    zOrderIndex = IslandFocusZOrderIndex;
                }
                Canvas.SetZIndex(mainCanvas.Children[i], zOrderIndex);
            }

            // make sure that the release island is visible
            drawData = _drawDataList.Find(f => f.IslandName == _presenter.ReleaseArchipelagoBranchName);
            startOfSelectedIslandUIElements = drawData.IndexOfFirstUIElementInCanvas;
            endOfSelectedIslandUIElements = drawData.IndexOfFirstUIElementInCanvas + (drawData.NumberOfDrawables);
            for (int i = startOfSelectedIslandUIElements; i < endOfSelectedIslandUIElements; i++)
            {
                Canvas.SetZIndex(mainCanvas.Children[i], ReleaseIslandFocusZOrderIndex);
            }
        }
    }
}
