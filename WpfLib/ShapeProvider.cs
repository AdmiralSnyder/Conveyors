using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using UILib.Shapes;
using WpfLib.Shapes;

namespace WpfLib;

public class ShapeProvider : IShapeProvider
{
    public static Action<IShape> SelectBehaviour { get; private set; }
    public void RegisterSelectBehavior(Action<IShape> selectBehaviour) => SelectBehaviour = selectBehaviour;

    protected ILine PrepareLine(TwoPoints points) => new WpfLine(new Line())
        .SetLocation(points)
        .WithSelectBehaviour();
}
