using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConveyorLib.Shapes;

namespace ConveyorLib
{

    public static class ShapeProviderProvider
    {
        public static IConveyorShapeProvider ShapeProvider { get; set; }
    }
}
