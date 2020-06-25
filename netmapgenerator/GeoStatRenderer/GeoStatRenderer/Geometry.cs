using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStatRenderer
{
    class Geometry
    {
        public string Type { get; set; }
        public double[][][][] Coordinates { get; set; }

        public List<PointF[]> ReadableCoordinates
        {
            get
            {
                var coords = new List<PointF[]>();

                foreach (var poly in Coordinates[0])
                {
                    var points = new List<PointF>();
                    foreach (var point in poly)
                    {
                        points.Add(new PointF((float)point[0], (float)point[1]));
                    }
                    coords.Add(points.ToArray());
                }

                return coords;
            }
        }

        public List<PointF[]> ScaleCoordinates(float scale)
        {
            var coords = new List<PointF[]>();

            foreach (var poly in Coordinates[0])
            {
                var points = new List<PointF>();
                foreach (var point in poly)
                {
                    points.Add(new PointF((float)point[0] * scale, (float)point[1] * scale));
                }
                coords.Add(points.ToArray());
            }

            return coords;
        }

        public List<PointF[]> ScaleAndFlipCoordinates(float scale, float offsetX, float offsetY, float maxY)
        {
            var coords = new List<PointF[]>();

            foreach (var poly in Coordinates[0])
            {
                var points = new List<PointF>();
                foreach (var point in poly)
                {
                    points.Add(new PointF(((float)point[0] - offsetX) * scale, ((maxY - (float)point[1]) - offsetY) * scale));
                }
                coords.Add(points.ToArray());
            }

            return coords;
        }
    }
}
