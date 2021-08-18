#pragma warning disable IDE0002

using System.Windows;
namespace Circus.Wpf.Controls {
    public sealed class MenuItemCloseAll : MenuItem {
        static MenuItemCloseAll() {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuItemCloseAll), new FrameworkPropertyMetadata(typeof(MenuItemCloseAll)));
        }
        public MenuItemCloseAll() { 
        }
    }
}
