using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Расчет_значений_трубопровода_WPF
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="X">координата X</param>
        /// <param name="Y">координата Y</param>
        public Point(double X, double Y)
        {
            this.X = X;
            this.Y = Y; 
        }
    }
}
