using System;
using PresenterCommon.Plugin;

namespace GitVisualiser.Views
{
    public class GitVisualiserPlugin : IInformationRadiatorItemPlugin
    {
        public string ItemType
        {
            get { return "GitVisualiser"; }
        }

        public Type PresenterType
        {
            get { return typeof(GitVisualiser.Presenters.GitVisualiserPresenter); }
        }

        public Type ViewType
        {
            get { return typeof(GitVisualiserView); }
        }
    }
}
