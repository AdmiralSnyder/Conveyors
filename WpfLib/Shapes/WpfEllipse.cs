using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UILib.Shapes;

namespace WpfLib.Shapes;

public class WpfEllipse : WpfShape<Ellipse>, IEllipse
{
    public WpfEllipse(Ellipse wpfShape) : base(wpfShape) { }

    public Color? FillColor { get => BackingObject.Fill is SolidColorBrush scb ? Color.FromArgb scb.Color ; set => ; }
    public Color? StrokeColor { get => ; set => ; }
    public double StrokeThickness { get => ; set => ; }
}