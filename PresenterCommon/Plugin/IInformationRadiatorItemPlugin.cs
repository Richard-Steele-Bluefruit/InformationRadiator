using System;

namespace PresenterCommon.Plugin
{
    public interface IInformationRadiatorItemPlugin
    {
        string ItemType { get; }
        Type ViewType { get; }
        Type PresenterType { get; }
    }
}
