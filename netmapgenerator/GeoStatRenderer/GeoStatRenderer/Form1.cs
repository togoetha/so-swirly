using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeoStatRenderer
{
    public partial class Form1 : Form
    {
        internal FeatureCollection Geomap { get; private set; }
        public Dictionary<string, StatRegValue> RegionValues { get; private set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //pnlMap.Paint += PnlMap_Paint;
        }

        /*private void PnlMap_Paint(object sender, PaintEventArgs e)
        {
            throw new NotImplementedException();
        }*/

        private void btnLoadJson_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var sr = new StreamReader(dialog.FileName))
                    {
                        ParseGeoJSON(sr.ReadToEnd());
                        //SetText(sr.ReadToEnd());
                    }
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void ParseGeoJSON(string json)
        {
            Geomap = JsonConvert.DeserializeObject<FeatureCollection>(json);
        }

        private void btnFindBounds_Click(object sender, EventArgs e)
        {
            var minX = float.MaxValue;
            var minY = float.MaxValue;
            var maxX = float.MinValue;
            var maxY = float.MinValue;

            var features = Geomap.Features;
            foreach (Feature f in features)
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
        }

        private void btnDrawBasic_Click(object sender, EventArgs e)
        {
            var bounds = Geomap.GetBounds();

            var g = pnlMap.CreateGraphics();
            g.Clear(Color.White);
            var rd = new Random();

            var features = Geomap.Features;
            var scale = float.Parse(txtScale.Text);
            var offsetX = float.Parse(txtOffsetX.Text);
            var offsetY = float.Parse(txtOffsetY.Text);

            foreach (Feature f in features)
            {
                var polys = f.Geometry.ScaleAndFlipCoordinates(scale, offsetX, offsetY, bounds.Bottom);
                foreach (var poly in polys)
                {
                    var pen = new Pen(Color.FromArgb(rd.Next(250), rd.Next(250), rd.Next(250)));
                    g.DrawPolygon(pen, poly);
                }
            }
        }

        private void btnLoadStat_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sr = new StreamReader(dialog.FileName);
                    ParseStatCsv(sr);
                    //SetText(sr.ReadToEnd());
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void ParseStatCsv(StreamReader r)
        {
            var confg = new CsvConfiguration(CultureInfo.CurrentCulture);
            confg.Delimiter = ";";
            using (var csv = new CsvReader(r, confg))
            {
                RegionValues = csv.GetRecords<StatRegValue>().ToDictionary(rv => rv.SectorId, rv => rv);
            }
        }

        private void btnDrawR_Click(object sender, EventArgs e)
        {
            var bounds = Geomap.GetBounds();

            var g = pnlMap.CreateGraphics();
            g.Clear(Color.White);

            var rd = new Random();

            var minStatValue = RegionValues.Values.Min(rv => rv.Value);
            var maxStatValue = RegionValues.Values.Max(rv => rv.Value);
            var statGradient = (maxStatValue - minStatValue) / 255;

            var scale = float.Parse(txtScale.Text);
            var offsetX = float.Parse(txtOffsetX.Text);
            var offsetY = float.Parse(txtOffsetY.Text);

            var features = Geomap.Features;
            foreach (Feature f in features)
            {
                var statvalue = 0f;
                if (RegionValues.ContainsKey(f.SectorID))
                    statvalue = RegionValues[f.SectorID].Value;

                var red = 0;
                var green = 0;
                var blue = 0;
                var fillValue = (statvalue - minStatValue) / statGradient;

                HsvToRgb(180+ fillValue / 255 *360, 1, 1, out red, out green, out blue);



                var polys = f.Geometry.ScaleAndFlipCoordinates(scale, offsetX, offsetY, bounds.Bottom);
                foreach (var poly in polys)
                {
                    var brush = new SolidBrush(Color.FromArgb(red, green, blue));
                    g.FillPolygon(brush, poly);
                }
            }
        }

        private void btnDrawRGB_Click(object sender, EventArgs e)
        {
            PaintRGB(pnlMap.CreateGraphics());
        }

        void PaintRGB(Graphics g)
        {
            var bounds = Geomap.GetBounds();

            //var g = pnlMap.CreateGraphics();
            g.Clear(Color.White);

            var rd = new Random();

            var minStatValue = RegionValues.Values.Min(rv => rv.Value);
            var maxStatValue = RegionValues.Values.Max(rv => rv.Value);
            var statGradient = (maxStatValue - minStatValue) / 255;

            var scale = float.Parse(txtScale.Text);
            var offsetX = float.Parse(txtOffsetX.Text);
            var offsetY = float.Parse(txtOffsetY.Text);

            var features = Geomap.Features;
            foreach (Feature f in features)
            {
                var statvalue = 0f;
                if (RegionValues.ContainsKey(f.SectorID))
                    statvalue = RegionValues[f.SectorID].Value;

                var red = (int)statvalue % 256;
                var green = (int)((statvalue % 65536) / 256);
                var blue = (int)(statvalue / 65536);

                var fillValue = (statvalue - minStatValue) / statGradient;

                var polys = f.Geometry.ScaleAndFlipCoordinates(scale, offsetX, offsetY, bounds.Bottom);
                foreach (var poly in polys)
                {
                    var brush = new SolidBrush(Color.FromArgb(red, green, blue));
                    g.FillPolygon(brush, poly);
                }
            }
        }

        void HsvToRgb(double h, double S, double V, out int r, out int g, out int b)
        {
            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }
            r = Clamp((int)(R * 255.0));
            g = Clamp((int)(G * 255.0));
            b = Clamp((int)(B * 255.0));
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int width = pnlMap.Size.Width;
            int height = pnlMap.Size.Height;

            Bitmap bm = new Bitmap(width, height);
            PaintRGB(Graphics.FromImage(bm));
            //pnlMap.DrawToBitmap(bm, new Rectangle(0, 0, width, height));

            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Png Image (.png)|*.png";
            sf.ShowDialog();
            var path = sf.FileName;

            bm.Save(path, ImageFormat.Png);
        }
    }
}
