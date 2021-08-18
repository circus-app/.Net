#pragma warning disable IDE0002

using System.Windows;
namespace Circus.Wpf.Controls {
    public class ContextMenu : System.Windows.Controls.ContextMenu {
        static ContextMenu() {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ContextMenu), new FrameworkPropertyMetadata(typeof(ContextMenu)));
        }
        public ContextMenu() {
        }
    }
}
