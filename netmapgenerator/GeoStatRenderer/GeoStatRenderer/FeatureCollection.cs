using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStatRenderer
{
    class FeatureCollection
    {
        public string Type { get; set; }
        public List<Feature> Features { get; set; }

        public RectangleF GetBounds()
        {
            var minX = float.MaxValue;
            var minY = float.MaxValue;
            var maxX = float.MinValue;
            var maxY = float.MinValue;

            foreach (Feature f in Features)
            {
                var polys = f.Geometry.ReadableCoordinates;
                foreach (var poly in polys)
                {
                    foreach (var point in poly)
                    {
                        if (point.X > maxX)
                            maxX = point.X;
                        if (point.X < minX)
                            minX = point.X;
                        if (point.Y > maxY)
                            maxY = point.Y;
                        if (point.Y < minY)
                            minY = point.Y;
                    }
                }
            }

            var rangeX = maxX - minX;
            var rangeY = maxY - minY;
            return new RectangleF(minX, minY, rangeX, rangeY);
        }
    }
}
