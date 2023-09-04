using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UILib.Shapes;

namespace WpfLib.Shapes;

public class WpfEllipse : WpfShape<Ellipse>, IEllipse
{
    public WpfEllipse(Ellipse wpfShape) : base(wpfShape) { }
}