using System;
using System.Windows;
using PresenterCommon.Configuration;
using System.Collections.Generic;
using System.Windows.Media;

using GitVisualiser.Model;

namespace GitVisualiser.Presenters
{
    public class GitVisualiserPresenter
    {
        public event EventHandler<DisplayAddDrawableDataEventArgs> AddNewIslandToView;
        public event EventHandler<DisplayRemoveDrawableDataEventArgs> RemoveIslandFromView;
        public event EventHandler<DisplayTextualDataEventArgs> DisplayTextualDataUpdate;

        public readonly string ReleaseArchipelagoBranchName = "Release Archipelago";

        private string _distanceScriptDirectory;
        private string _pythonExeLocation;
        private double _configFileMinPixelSizeIslandAdditiveModifier;
        private double _configFileMaxPixelSizeIslandAdditiveModifier;
        private double _pixelsPerArchipelagoUnitDistanceAdditiveModifier;
        private double _textualInformationDisplayTickTimeInMilliseconds;
        private double _pythonScriptUpdateTickTimeInMilliseconds;
        private System.Windows.Threading.Dispatcher _mainThreadDispatcher;
        private PresenterCommon.ITimer _archipelagoInformationDisplayTimer;
        private IIslands _islands;
        private Random _randomColourPicker;
        private List<Color> _colourList;
        private List<string> _branchNames;
        private int _nextBranchDisplayedIndex;
        private ViewControlData _viewControlData;
        private PresenterCommon.ITimer _pythonScriptUpdateTimer;
        private IPythonScriptRunner _pythonScript;

        public GitVisualiserPresenter(InformationRadiatorItemConfiguration configuration)
        {
            _branchNames = new List<string>();
            _nextBranchDisplayedIndex = 0;

            ParseConfiguration(configuration);

            _mainThreadDispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
        }

        private void ParseConfiguration(InformationRadiatorItemConfiguration configuration)
        {
            foreach (var item in configuration)
            {
                switch (item.ID.ToLower())
                {
                    case "scriptdirectory":
                        _distanceScriptDirectory = item.Value;
                        break;
                    case "pythonexelocation":
                        _pythonExeLocation = item.Value;
                        break;
                    case "minpixelsizeislandadditivemodifier":
                        _configFileMinPixelSizeIslandAdditiveModifier = ParseDouble(item.Value);
                        break;
                    case "maxpixelsizeislandadditivemodifier":
                        _configFileMaxPixelSizeIslandAdditiveModifier = ParseDouble(item.Value);
                        break;
                    case "pixelsperarchipelagounitdistanceadditivemodifier":
                        _pixelsPerArchipelagoUnitDistanceAdditiveModifier = ParseDouble(item.Value);
                        break;
                    case "textualinformationdisplayticktimeinseconds":
                        _textualInformationDisplayTickTimeInMilliseconds = ParseDouble(item.Value) * 1000;
                        break;
                    case "pythonscriptupdateticktimeinhours":
                        _pythonScriptUpdateTickTimeInMilliseconds= ((ParseDouble(item.Value) * 60) * 60) * 1000;
                        break;
                }
            }

            var didNotExistInConfigurationOrFailedParsing = (_textualInformationDisplayTickTimeInMilliseconds == 0);
            if (didNotExistInConfigurationOrFailedParsing)
            {
                var DefaultTimeoutInMilliseconds = 6 * 1000;
                _textualInformationDisplayTickTimeInMilliseconds = DefaultTimeoutInMilliseconds;
            }

            didNotExistInConfigurationOrFailedParsing = (_pythonScriptUpdateTickTimeInMilliseconds == 0);
            if (didNotExistInConfigurationOrFailedParsing)
            {
                var hours = 0.5;
                var DefaultTimeoutInMilliseconds = ((hours * 60) * 60) * 1000;
                _pythonScriptUpdateTickTimeInMilliseconds = DefaultTimeoutInMilliseconds;
            }
        }

        private double ParseDouble(string toParse)
        {
            double parseResult;
            bool parsingSuccessful = Double.TryParse(toParse, out parseResult);
            if (parsingSuccessful)
            {
                return parseResult;
            }

            return 0;
        }

        public void FinishInitialisation(double controlWidth, double controlHeight)
        {
            _mainThreadDispatcher.Invoke(new Action(() =>
            {
                _viewControlData = new ViewControlData(_configFileMinPixelSizeIslandAdditiveModifier,
                    _configFileMaxPixelSizeIslandAdditiveModifier,
                    _pixelsPerArchipelagoUnitDistanceAdditiveModifier,
                    controlWidth,
                    controlHeight);

                var releaseArchipelago = CreateBranch(ReleaseArchipelagoBranchName);
                _islands = GitVisualiserPresenterFactory.Instance.CreateIslands(_viewControlData, releaseArchipelago);
                AddNewIslandToView(this, new DisplayAddDrawableDataEventArgs(releaseArchipelago.Name, releaseArchipelago.Shape.Drawables()));

                _randomColourPicker = new Random();
                _colourList = new List<Color>()
                {
                    Color.FromRgb(220, 20, 60),
                    Color.FromRgb(255, 192, 203),
                    Color.FromRgb(128, 0, 128),
                    Color.FromRgb(0, 0, 255),
                    Color.FromRgb(0, 191, 255),
                    Color.FromRgb(0, 245, 255),
                    Color.FromRgb(0, 199, 140),
                    Color.FromRgb(0, 128, 128),
                    Color.FromRgb(0, 255, 127),
                    Color.FromRgb(192, 255, 62),
                    Color.FromRgb(255, 255, 0),
                    Color.FromRgb(255, 215, 0),
                    Color.FromRgb(255, 128, 0),
                    Color.FromRgb(255, 218, 185),
                    Color.FromRgb(173, 255, 47),
                    Color.FromRgb(193, 255, 193),
                    Color.FromRgb(0, 255, 255),
                };

                _pythonScript = GitVisualiserPresenterFactory.Instance.CreatePythonScriptRunner(
                    _distanceScriptDirectory, 
                    _pythonExeLocation);
                _pythonScript.FinishedEventHandler += NewDataReceivedFromPythonScript;
                _pythonScript.Go();

                _pythonScriptUpdateTimer = GitVisualiserPresenterFactory.Instance.CreateTimer(_pythonScriptUpdateTickTimeInMilliseconds);
                _pythonScriptUpdateTimer.Tick += __PythonScriptUpdateTimer_Tick;

                _archipelagoInformationDisplayTimer = GitVisualiserPresenterFactory.Instance.CreateTimer(_textualInformationDisplayTickTimeInMilliseconds);
                _archipelagoInformationDisplayTimer.Tick += __DisplayTextualDataUpdateTimer_Tick;
                
                //DEBUG_ADD_ARCHIS();
                __DisplayTextualDataUpdateTimer_Tick(this, new EventArgs());    // manual tick
            }));
        }

        private Branch CreateBranch(string branchName)
        {
            Branch newBranch;

            var isReleaseArchipelago = (branchName == ReleaseArchipelagoBranchName);
            if (isReleaseArchipelago)
            {
                var white = Color.FromRgb(255, 255, 255);
                newBranch = new Branch(branchName, white);
            }
            else
            {
                var colour = GetUniqueColour();
                newBranch = new Branch(branchName, colour);
                _islands.Add(newBranch);
                _branchNames.Add(branchName);
            }

            return newBranch;
        }

        private Color GetUniqueColour()
        {
            if (_colourList.Count != 0)
            {
                var chosenColourIndex = _randomColourPicker.Next(_colourList.Count);
                var chosenColour = _colourList[chosenColourIndex];
                _colourList.RemoveAt(chosenColourIndex);

                return chosenColour;
            }

            var randomBytes = new byte[3];
            _randomColourPicker.NextBytes(randomBytes);
            var randomColour = Color.FromArgb(100, randomBytes[0], randomBytes[1], randomBytes[2]);
            return randomColour;
        }

        private void DEBUG_ADD_ARCHIS()
        {
            // debug start
            var debugArchiList = new List<Tuple<string, double>>();

            var branchName = "bob"; // debug
            var distance = 500.0;   // debug
            debugArchiList.Add(new Tuple<string, double>(branchName, distance));
            
            branchName = "ben"; // debug
            distance = 500.0;   // debug
            debugArchiList.Add(new Tuple<string, double>(branchName, distance));
            
            branchName = "bill"; // debug
            distance = 500.0;   // debug
            debugArchiList.Add(new Tuple<string, double>(branchName, distance));
            
            branchName = "jane"; // debug
            distance = 500.0;   // debug
            debugArchiList.Add(new Tuple<string, double>(branchName, distance));
            
            branchName = "jen"; // debug
            distance = 500.0;   // debug
            debugArchiList.Add(new Tuple<string, double>(branchName, distance));
            
            branchName = "jill"; // debug
            distance = 500.0;   // debug
            debugArchiList.Add(new Tuple<string, double>(branchName, distance));
            
            branchName = "will"; // debug
            distance = 500.0;   // debug
            debugArchiList.Add(new Tuple<string, double>(branchName, distance));
            
            
            //branchName = "bob"; // debug
            //distance = 10.0;   // debug
            //NewDataReceivedFromPythonScript(this, new PythonDataEventArgs(branchName, distance));
            //
            //branchName = "ben"; // debug
            //distance = 50.0;   // debug
            //NewDataReceivedFromPythonScript(this, new PythonDataEventArgs(branchName, distance));
            //
            //branchName = "bill"; // debug
            //distance = 20.0;   // debug
            //NewDataReceivedFromPythonScript(this, new PythonDataEventArgs(branchName, distance));
            //
            //branchName = "jane"; // debug
            //distance = 50.0;   // debug
            //NewDataReceivedFromPythonScript(this, new PythonDataEventArgs(branchName, distance));
            //
            //branchName = "jen"; // debug
            //distance = 200.0;   // debug
            //NewDataReceivedFromPythonScript(this, new PythonDataEventArgs(branchName, distance));
            //
            //branchName = "jill"; // debug
            //distance = 200.0;   // debug
            //NewDataReceivedFromPythonScript(this, new PythonDataEventArgs(branchName, distance));
            //
            //branchName = "will"; // debug
            //distance = 200.0;   // debug
            //NewDataReceivedFromPythonScript(this, new PythonDataEventArgs(branchName, distance));

            NewDataReceivedFromPythonScript(this, new PythonDataEventArgs(debugArchiList));

            // debug end
        }

        private void NewDataReceivedFromPythonScript(object sender, PythonDataEventArgs pythonData)
        {
            AddNewIslandsToView(pythonData);
            RemoveOldIslandsFromView(pythonData);

            _islands.MoveAll();

            // manual tick, so that the label isn't left hanging in the middle of nowhere if the new islands were added or existing ones moved
            __DisplayTextualDataUpdateTimer_Tick(this, new EventArgs());
        }

        private void AddNewIslandsToView(PythonDataEventArgs pythonData)
        {
            if (AddNewIslandToView == null)
            {
                return;
            }

            foreach (var data in pythonData.ArchipelagoData)
            {
                var branchName = data.Item1;
                var branchExists = _islands.Contains(branchName);
                var branchHasBeenDisplayedBefore = branchExists;
                if (!branchHasBeenDisplayedBefore)
                {
                    var branch = CreateBranch(branchName);
                    AddNewIslandToView(this, new DisplayAddDrawableDataEventArgs(branch.Name, branch.Shape.Drawables()));
                }

                var distance = data.Item2;
                _islands.SetBranchDistance(branchName, distance);
            }
        }

        private void RemoveOldIslandsFromView(PythonDataEventArgs pythonData)
        {
            if (RemoveIslandFromView == null)
            {
                return;
            }

            var namesOfIslandsCurrentlyDisplayed = _islands.GetAllNames();

            var newIslandNames = new HashSet<string>();
            foreach (var data in pythonData.ArchipelagoData)
            {
                newIslandNames.Add(data.Item1);
            }

            newIslandNames.SymmetricExceptWith(namesOfIslandsCurrentlyDisplayed);
            var oldIslandNames = newIslandNames;

            foreach (var old in oldIslandNames)
            {
                _islands.Delete(old);
                _branchNames.Remove(old);
                RemoveIslandFromView(this, new DisplayRemoveDrawableDataEventArgs(old));
            }
        }

        private void __DisplayTextualDataUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (DisplayTextualDataUpdate == null)
            {
                return;
            }

            var noBranchesToDisplayDataFor = (_branchNames.Count < 1);
            if (noBranchesToDisplayDataFor)
            {
                return;
            }

            var branchIndexToDisplay = _nextBranchDisplayedIndex;
            if (branchIndexToDisplay >= _branchNames.Count)
            {
                branchIndexToDisplay = 0;
            }

            string nextBranchToDisplayName = _branchNames[branchIndexToDisplay];
            var proposedLocationRelativeToCentre = _islands.GetLocation(nextBranchToDisplayName);
            var distance = _islands.GetDistance(nextBranchToDisplayName);

            string textToDisplay = nextBranchToDisplayName + "\r\n" + distance;
            System.Windows.Point labelWidthAndHeight = GetLabelWidthAndHeight(textToDisplay);
            var locationRelativeToCentre = GetTextualLabelLocationWithinControl(labelWidthAndHeight, 
                proposedLocationRelativeToCentre);

            DisplayTextualDataUpdate(this, new DisplayTextualDataEventArgs(textToDisplay, 
                locationRelativeToCentre,
                labelWidthAndHeight.X,
                labelWidthAndHeight.Y));
            _mainThreadDispatcher.Invoke(new Action(() =>
            {
                _islands.Highlight(nextBranchToDisplayName);
            }));

            _nextBranchDisplayedIndex = branchIndexToDisplay + 1;
        }

        private System.Windows.Point GetLabelWidthAndHeight(string text)
        {
            var defaultFontFamily = new System.Windows.Media.FontFamily();
            var defaultFontStyle = new System.Windows.FontStyle();
            var defaultFontStretch = new System.Windows.FontStretch();
            var defaultFontSizeEm = 12.0;

            var typeFace = new Typeface(defaultFontFamily,
                defaultFontStyle,
                System.Windows.FontWeights.Normal,
                defaultFontStretch);

            var formattedText = new FormattedText(
                    text,
                    System.Globalization.CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    typeFace,
                    defaultFontSizeEm,
                    Brushes.Black);

            var Padding = 15;   // empirical
            var width = formattedText.Width + Padding;
            var height = formattedText.Height + Padding;

            return new System.Windows.Point(width, height);
        }

        private System.Windows.Point GetTextualLabelLocationWithinControl(System.Windows.Point widthAndHeight, 
            System.Windows.Point proposedLocation)
        {
            var rightHandSideOfTheLabelLocation = proposedLocation.X + widthAndHeight.X;
            if (rightHandSideOfTheLabelLocation > _viewControlData.Width)
            {
                var difference = (rightHandSideOfTheLabelLocation - _viewControlData.Width);
                var newX = proposedLocation.X - difference;
                proposedLocation = new System.Windows.Point(newX, proposedLocation.Y);
            }

            return proposedLocation;
        }

        private void __PythonScriptUpdateTimer_Tick(object sender, EventArgs e)
        {
            _mainThreadDispatcher.Invoke(new Action(() =>
            {
                if (_pythonScript == null)
                {
                    return;
                }

                _pythonScript.Go();
            }));
        }
    }
}
