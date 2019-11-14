using System;
using System.Collections.Generic;

using PresenterCommon.Plugin;
using PresenterCommon.Configuration;

namespace PresenterCommon
{
    public class PresenterResources
    {
        private IDayUpdateMonitor _dayUpdateMonitor = null;

        private ItemFactory.Factory _viewFactory = null;
        private ItemFactory.Factory _presenterFactory = null;

        private InformationRadiatorConfiguration _configuration = null;

        private void CreateResources()
        {
            _dayUpdateMonitor = new DayUpdateMonitor();
        }

        private void BuildFactories()
        {
            _viewFactory = new ItemFactory.Factory();
            _presenterFactory = new ItemFactory.Factory();

            _presenterFactory.AddParameter(typeof(IDayUpdateMonitor), _dayUpdateMonitor);
        }

        private void LoadPlugins()
        {
            var plugins = Plugin.PluginFinder<Plugin.IInformationRadiatorItemPlugin>.SearchPath(System.AppDomain.CurrentDomain.BaseDirectory);
            foreach (var plugin in plugins)
            {
                IInformationRadiatorItemPlugin info = Activator.CreateInstance(plugin) as IInformationRadiatorItemPlugin;

                _presenterFactory.AddItemType(info.ItemType, info.PresenterType);
                _viewFactory.AddItemType(info.ItemType, info.ViewType);
            }
        }

        private void LoadConfiguration()
        {
            _configuration = InformationRadiatorConfiguration.Load(System.AppDomain.CurrentDomain.BaseDirectory + "Configuration.xml");
        }

        public PresenterResources()
        {
            CreateResources();
            BuildFactories();
            LoadPlugins();
            LoadConfiguration();
        }

        public ItemFactory.IItemFactory PresenterFactory
        {
            get
            {
                return _presenterFactory;
            }
        }

        public ItemFactory.IItemFactory ViewFactory
        {
            get
            {
                return _viewFactory;
            }
        }

        public InformationRadiatorConfiguration Configuration
        {
            get { return _configuration; }
        }
    }
}
