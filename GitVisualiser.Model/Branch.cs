using System.Windows.Media;

namespace GitVisualiser.Model
{
    public class Branch
    {
        public string Name { get; private set; }
        public IslandShape Shape { get; private set; }
        public double Distance;

        // and other tings from the python script

        public Branch(string newName, Color newColour)
        {
            Name = newName;
            Shape = new IslandShape(newColour);
        }
    }
}
