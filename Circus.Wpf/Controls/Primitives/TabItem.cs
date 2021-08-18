#pragma warning disable IDE0002

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace Circus.Wpf.Controls.Primitives {
    public abstract class TabItem : System.Windows.Controls.TabItem {
		public static readonly DependencyProperty FocusContentProperty;
		[Bindable(true)]
		public bool FocusContent { get => (bool)base.GetValue(TabItem.FocusContentProperty); set => base.SetValue(TabItem.FocusContentProperty, Boxes.Box(value)); }
		private new Tab Parent => (Tab)ItemsControl.ItemsControlFromItemContainer(this);
		static TabItem() {
			TabItem.FocusContentProperty = DependencyProperty.Register("FocusContent", typeof(bool), typeof(TabItem), new FrameworkPropertyMetadata(Boxes.False));
		}
		/// <summary>Constructs a TabItem.</summary>
		protected TabItem() : base() {
		}
        protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
            base.OnPreviewGotKeyboardFocus(e);
            if (this.FocusContent && Keyboard.FocusedElement != base.Content && Assert.NotNull(this.Parent) && Assert.NotNull(this.Parent.Presenter)) {
				this.Parent.UpdateLayout();
				if (this.Parent.Presenter.MoveFocus(new TraversalRequest(FocusNavigationDirection.First)) && !e.Handled) {
					e.Handled = true;
				}
            }
        }
	}
}
