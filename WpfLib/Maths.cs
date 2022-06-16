using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreMaths = CoreLib.Maths.Maths;

namespace WpfLib.Maths;

internal static class Maths
{
    public static double Length(this Line line) => CoreMaths.Distance(new(line.X1, line.Y1), new(line.X2, line.Y2));

}
