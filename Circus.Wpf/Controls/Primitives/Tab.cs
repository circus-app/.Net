#pragma warning disable IDE0002

using System.Windows;
using System.Windows.Controls;
using Circus.Wpf.Data;
namespace Circus.Wpf.Controls.Primitives {
    public abstract class Tab : TabControl {
        private bool flag;
        public static readonly RoutedEvent RenderCompletedEvent;
        protected DataSource DataSource { get; private set; }
        internal ContentPresenter Presenter { get; private set; }
        static Tab() {
            Tab.DataContextProperty.OverrideMetadata(typeof(Tab), new FrameworkPropertyMetadata(Boxes.Null, new PropertyChangedCallback(Tab.OnDataContextChanged)));
            Tab.ItemsSourceProperty.OverrideMetadata(typeof(Tab), new FrameworkPropertyMetadata(Boxes.Null, new PropertyChangedCallback(Tab.OnItemsSourceChanged)));
            Tab.RenderCompletedEvent = EventManager.RegisterRoutedEvent("RenderCompleted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Tab));
            Tab.VisibilityProperty.OverrideMetadata(typeof(Tab), new FrameworkPropertyMetadata(Visibility.Visible, new PropertyChangedCallback(Tab.OnVisibilityChanged)));
        }
        protected Tab() {
            this.DataSource = null;
            this.flag = false;
        }
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            if (TemplateItems.FindName(this, "PART_SelectedContentHost", out object value) && Assert.As(value, out ContentPresenter presenter)) {
                this.Presenter = presenter;
            }
        }
        protected virtual void OnDataContextChanged(object value) {
            this.DataSource = Assert.Is(value, out DataSource data) ? data : null;
        }
        private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (Assert.As(d, out Tab tab)) {
                tab.OnDataContextChanged(e.NewValue);
            }
        }
        protected virtual void OnItemsSourceChanged(object value) {
        }
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (Assert.As(d, out Tab tab)) {
                tab.OnItemsSourceChanged(e.NewValue);
            }
        }
		protected virtual void OnRenderCompleted(SizeChangedInfo info) {
            base.RaiseEvent(new RoutedEventArgs(Tab.RenderCompletedEvent, this));
            this.flag = true;
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo info) {
            base.OnRenderSizeChanged(info);
            if (!this.flag) {
                this.OnRenderCompleted(info);
            }
        }
        protected virtual void OnIsVisibleChanged(bool value) {
        }
        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (Assert.As(d, out Tab tab)) {
                tab.OnIsVisibleChanged(tab.IsVisible);
            }
        }
    }
}
