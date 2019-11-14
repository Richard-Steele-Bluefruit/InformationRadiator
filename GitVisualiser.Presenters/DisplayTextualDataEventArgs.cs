using System;

namespace GitVisualiser.Presenters
{
    public class DisplayTextualDataEventArgs : EventArgs
    {
        public string TextToDisplay { get; private set; }
        public System.Windows.Point Location { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }

        public DisplayTextualDataEventArgs(string newTextToDisplay, System.Windows.Point newLocation, double newWidth, double newHeight)
        {
            TextToDisplay = newTextToDisplay;
            Location = newLocation;
            Width = newWidth;
            Height = newHeight;
        }
    }
}
