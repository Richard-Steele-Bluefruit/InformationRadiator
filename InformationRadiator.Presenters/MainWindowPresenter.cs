using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresenterCommon
{
    public class MainWindowPresenter
    {

        public class CreateViewEventArgs : EventArgs
        {
            public string ItemType { get; private set; }
            public object Presenter { get; private set; }
            public int Width { get; private set; }
            public int Height { get; private set; }
            public int? Left { get; private set; }
            public int? Top { get; private set; }
            public int Screen { get; private set; }
            public string Title { get; private set; }
            public CreateViewEventArgs(string itemType, object presenter, int width, int height, int? left, int? top, int screen, string title)
            {
                ItemType = itemType;
                Presenter = presenter;
                Width = width;
                Height = height;
                Left = left;
                Top = top;
                Screen = screen;
                Title = title;
            }
        }

        public event EventHandler<CreateViewEventArgs> CreateView;

        protected void OnCreateView(string itemType, object presenter, int width, int height, int? left, int? top, int screen, string title)
        {
            EventHandler<CreateViewEventArgs> ev = CreateView;
            if (ev != null)
                ev(this, new CreateViewEventArgs(itemType, presenter, width, height, left, top, screen, title));
        }

        public void ParseConfiguration(Configuration.InformationRadiatorConfiguration configuration, ItemFactory.IItemFactory presenterFactory)
        {
            foreach(var item in configuration.Items)
            {
                string itemType = item.ItemType;
                object presenter = presenterFactory.CreateObject(itemType, item.Configuration);

                int? left = null;
                int? top = null;
                int screen = 0;

                int value;
                if(int.TryParse(item.Left, out value))
                {
                    left = value;
                }

                if (int.TryParse(item.Top, out value))
                {
                    top = value;
                }

                if(int.TryParse(item.Screen, out value))
                {
                    screen = value;
                }

                OnCreateView(itemType, presenter, item.Width, item.Height, left, top, screen, item.Title);
            }
        }
    }
}
