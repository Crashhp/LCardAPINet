using System;
using System.Windows;

namespace LCard.E2010GUI.Controls
{
    public interface IViewLocator
    {
        UIElement GetOrCreateViewType(Type viewType);
    }
}