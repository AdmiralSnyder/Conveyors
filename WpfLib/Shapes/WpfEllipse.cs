using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UILib.Shapes;

namespace WpfLib.Shapes;

public class WpfEllipse : WpfShape<Ellipse>, IEllipse
{
    // anscheinend muss der radius +1 gerechnet werden...
    public WpfEllipse() : base(new Ellipse()) { }
    public new double Height
    {
        get => base.Height - 1;
        set => base.Height = value + 1;
    }

    public new double Width
    {
        get => base.Width - 1;
        set => base.Width = value + 1;
    }
}