using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Wordbook.Controls
{
    public class CustomToolTipSlider : Slider
    {
        private ToolTip _autoToolTip;
        public ToolTip AutoToolTip
        {
            get
            {
                if (this._autoToolTip == null)
                {
                    var field = typeof(Slider).GetField("_autoToolTip", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null)
                    {
                        _autoToolTip = field.GetValue(this) as ToolTip;

                        if (this._autoToolTip != null)
                        {
                            _autoToolTip.VerticalOffset = this.AutoTooltipVerticalOffset;
                        }
                    }
                }

                return this._autoToolTip;
            }
        }

        public static readonly DependencyProperty AutoToolTipContentProperty = DependencyProperty.Register(
            "AutoToolTipContent", typeof(string), typeof(CustomToolTipSlider), new PropertyMetadata(default(string)));
        
        public object AutoToolTipContent
        {
            get { return (string)GetValue(AutoToolTipContentProperty); }
            set { SetValue(AutoToolTipContentProperty, value); }
        }

        public static readonly DependencyProperty AutoTooltipVerticalOffsetProperty = DependencyProperty.Register(
            "AutoTooltipVerticalOffset", typeof (int), typeof (CustomToolTipSlider), new PropertyMetadata(default(int)));

        public int AutoTooltipVerticalOffset
        {
            get { return (int) GetValue(AutoTooltipVerticalOffsetProperty); }
            set { SetValue(AutoTooltipVerticalOffsetProperty, value); }
        }

        protected override void OnThumbDragStarted(DragStartedEventArgs e)
        {
            base.OnThumbDragStarted(e);
            this.AutoToolTip.Content = this.AutoToolTipContent;
        }

        protected override void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            base.OnThumbDragDelta(e);
            this.AutoToolTip.Content = this.AutoToolTipContent;
        }


    }
}