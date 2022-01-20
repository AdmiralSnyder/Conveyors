using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1;

class TextAdorner : Adorner
{
    public TextAdorner(UIElement adornedElement) : base(adornedElement)
    { }

    public static Typeface Tf = new Typeface("Arial");
    protected override void OnRender(DrawingContext drawingContext)
    {
        if (AdornedElement is Shape shape /*&& shape.IsLabelUsed*/)
        {
            Rect segmentBounds = new Rect(shape.DesiredSize);
            if (shape.Tag is Item item)
            {
                FormattedText ft = new FormattedText(item.Number.ToString(), Thread.CurrentThread.CurrentCulture, 
                    FlowDirection.LeftToRight, Tf, 10, Brushes.White);
                segmentBounds.Offset(segmentBounds.Width / 2 - ft.Width / 2, 0);
                drawingContext.DrawText(ft, segmentBounds.TopLeft);
            }
        }
    }
}
