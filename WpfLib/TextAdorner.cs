using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfLib;

public interface ITextAdornable
{
    string AdornmentText { get; }
}

public class TextAdorner<TShapeTag> : Adorner
    where TShapeTag : ITextAdornable
{
    public TextAdorner(UIElement adornedElement) : base(adornedElement)
    { }

    public static Typeface Tf = new ("Arial");
    protected override void OnRender(DrawingContext drawingContext)
    {
        if (AdornedElement is Shape shape /*&& shape.IsLabelUsed*/)
        {
            Rect segmentBounds = new Rect(shape.DesiredSize);
            if (shape.Tag is TShapeTag shapeTag)
            {
                FormattedText ft = new FormattedText(shapeTag.AdornmentText, Thread.CurrentThread.CurrentCulture, 
                    FlowDirection.LeftToRight, Tf, 10, Brushes.White);
                segmentBounds.Offset(segmentBounds.Width / 2 - ft.Width / 2, 0);
                drawingContext.DrawText(ft, segmentBounds.TopLeft);
            }
        }
    }
}
