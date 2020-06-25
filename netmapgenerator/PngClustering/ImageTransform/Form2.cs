using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows.Forms;
using Voronoi2;

namespace ImageTransform
{
    public partial class Form2 : Form
    {
        private List<Node> FogNodes { get; set; }
        private List<Node> EdgeNodes { get; set; }
        private List<Cluster> Clusters { get; set; }
        private Dictionary<Node,Cluster> IndexedClusters { get; set; }


        public Bitmap DensityMap { get; set; }
        //public double[][] PDensities { get; set; }
        public List<double> FPPixels { get; set; }
        public List<double> EPPixels { get; set; }

        public List<System.Drawing.Point> Pixels { get; set; }

        public Form2()
        {
            InitializeComponent();
        }

        #region Generate

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            Clusters = null;

            var numFognodes = int.Parse(txtFognodes.Text);
            var numEdgenodes = int.Parse(txtEdgenodes.Text);

            var rd = new Random((int)DateTime.Now.Ticks);
            //1200 -> 750 minus some edges, sooooo 200 + rnd(800), 150 + rnd(450)
            FogNodes = new List<Node>();
            for (int i = 0; i < numFognodes; i++)
            {
                var chance = rd.NextDouble();

                var pFound = BinaryFind(FPPixels, chance);
                FogNodes.Add(new Node() { X = pFound.X, Y = pFound.Y });
            }

            var maxRelDiff = int.Parse(txtPingDistDiff.Text) / 100d;

            EdgeNodes = new List<Node>();

            //edge nodes are a bit shiftier, not sure how to make "realistic" clusters yet
            for (int i = 0; i < numEdgenodes; i++)
            {
                var chance = rd.NextDouble();

                var pFound = BinaryFind(EPPixels, chance);

                var fogNodePings = new Dictionary<Node, int>();
                //var fnSortedPings = new List<PingNode>();
                foreach (var fn in FogNodes)
                {
                    var dist = Math.Sqrt(Math.Pow(fn.X - pFound.X, 2) + Math.Pow(fn.Y - pFound.Y, 2));
                    var ping = ((1 - maxRelDiff) + rd.NextDouble() * (2 * maxRelDiff)) * dist;
                    //Console.WriteLine("New node dist to fog node " + dist + " ping to fog node " + ping);
                    fogNodePings[fn] = (int)ping;
                    //fnSortedPings.Add(new PingNode() { Node = fn, Ping = (int)ping });
                }

                EdgeNodes.Add(new Node() { X = pFound.X, Y = pFound.Y, Pings = fogNodePings, SortedPings = fogNodePings.OrderBy(p => p.Value).Select(p => p.Key).ToList() });
            }

            UpdateDraw();
        }

        private System.Drawing.Point BinaryFind(List<double> pPixels, double chance)
        {
            var pFound = new System.Drawing.Point(0, 0);
            bool found = false;
            var curIdx = (int)(pPixels.Count / 2);
            var step = curIdx;
            while (!found)
            {
                var current = pPixels[curIdx];
                if (current == chance)
                {
                    found = true;
                    pFound = Pixels[curIdx];
                }
                else
                {
                    step /= 2;
                    if (step == 1)
                    {
                        found = true;
                        if (chance > current)
                        {
                            pFound = Pixels[curIdx];
                        }
                        else
                        {
                            pFound = Pixels[curIdx - 1];
                        }
                    }
                    else
                    {
                        if (chance > current)
                        {
                            curIdx += step;
                        }
                        else
                        {
                            curIdx -= step;
                        }
                    }
                }
            }
            return pFound;
        }

        private void UpdateDraw()
        {
            var time = DateTime.Now;
            var maxPing = int.Parse(txtSLAMaxPing.Text);

            var capacity = 0;
            int.TryParse(txtFogCapacity.Text, out capacity);

            var g = pnlCluster.CreateGraphics();
            g.Clear(Color.White);

            var brshRed = new SolidBrush(Color.Red);
            var brshGreen = new SolidBrush(Color.Green);
            var pGreen = new Pen(brshGreen, 2);
            var pYellow = new Pen(new SolidBrush(Color.Yellow), 2);
            var pBlue = new Pen(new SolidBrush(Color.Blue), 2);

            Random rd = new Random();
            if (DensityMap != null)
            {
                g.DrawImage(DensityMap, new System.Drawing.Point(0, 0));
            }
            if (Clusters != null)
            {
                foreach (var cluster in Clusters)
                {
                    var brshCluster = new SolidBrush(Color.FromArgb(rd.Next(250), rd.Next(250), rd.Next(250)));
                    foreach (var node in cluster.Nodes)
                    {
                        g.FillEllipse(brshCluster, new Rectangle(node.X - 1, node.Y - 1, 3, 3));
                    }
                }
                foreach (var fogNode in FogNodes)
                {
                    g.FillEllipse(brshRed, new Rectangle(fogNode.X - 4, fogNode.Y - 4, 8, 8));
                }
                foreach (var cluster in Clusters)
                {
                    g.FillEllipse(brshGreen, new Rectangle(cluster.FogNode.X - 4, cluster.FogNode.Y - 4, 8, 8));
                    if (chkRp.Checked) 
                        g.DrawEllipse(pGreen, new Rectangle(cluster.FogNode.X - maxPing, cluster.FogNode.Y - maxPing, maxPing * 2, maxPing * 2));
                    if (chkRe.Checked && capacity > 0)
                    {
                        //hmm.. fuck
                        var limit = EdgeNodes.OrderBy(en => Math.Sqrt(en.Distance(cluster.FogNode))).Skip(capacity - 1).First();
                        var dist = (int)limit.Distance(cluster.FogNode);
                        g.DrawEllipse(pBlue, new Rectangle(cluster.FogNode.X - dist, cluster.FogNode.Y - dist, dist * 2, dist * 2));
                    }
                }
                if (Clusters.Count > 0 && chkRf.Checked)
                {
                    var xVal = Clusters.Select(c => (double)c.FogNode.X).ToArray();
                    var yVal = Clusters.Select(c => (double)c.FogNode.Y).ToArray();
                    Voronoi voroObject = new Voronoi(0);
                    // Generate the diagram
                    List<GraphEdge> ge = voroObject.generateVoronoi(xVal, yVal, 0, 1200, 0, 750);
                    foreach (var edge in ge)
                    {
                        g.DrawLine(pYellow, new PointF((float)edge.x1, (float)edge.y1), new PointF((float)edge.x2, (float)edge.y2));
                    }
                }
            }
            else
            {
                if (EdgeNodes != null)
                {
                    var brshBlue = new SolidBrush(Color.Blue);

                    foreach (var edgeNode in EdgeNodes)
                    {
                        g.FillEllipse(brshBlue, new Rectangle(edgeNode.X - 1, edgeNode.Y - 1, 3, 3));
                    }
                    foreach (var fogNode in FogNodes)
                    {
                        g.FillEllipse(brshRed, new Rectangle(fogNode.X - 4, fogNode.Y - 4, 8, 8));
                    }
                    brshBlue.Dispose();
                }
            }

            brshRed.Dispose();
            brshGreen.Dispose();

            g.Dispose();
            Console.WriteLine("Redraw took " + (DateTime.Now - time).TotalMilliseconds + "ms");
        }

        private void UpdatePingStats(int? timeTaken = null)
        {
            var pings = Clusters.SelectMany(cl => cl.Nodes.Select(n => n.Pings[cl.FogNode])).OrderBy(p => p).ToList();
            txtMaxPing.Text = ((int)pings.Max()).ToString();
            txtMinPing.Text = ((int)pings.Min()).ToString();
            txtAvgMedPing.Text = ((int)pings.Average()).ToString() + "/" + ((int)pings.Skip(pings.Count() / 2).First()).ToString();
            txtClusters.Text = Clusters.Count.ToString();
            txtTime.Text = timeTaken == null ? "" : timeTaken.ToString();
        }

        #endregion

        #region Handlers 

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void btnSolveDist_Click(object sender, EventArgs e)
        {
            var start = DateTime.Now;
            var fogClusters = FogNodes.ToDictionary(fn => fn, fn => new Cluster() { FogNode = fn });

            foreach (var edgeNode in EdgeNodes)
            {
                var minDist = 10000d;
                Node closest = null;
                foreach (var fc in fogClusters)
                {
                    var dist = Math.Sqrt(Math.Pow(fc.Key.X - edgeNode.X, 2) + Math.Pow(fc.Key.Y - edgeNode.Y, 2));
                    if (dist < minDist)
                    {
                        closest = fc.Key;
                        minDist = dist;
                    }
                }
                fogClusters[closest].Nodes.Add(edgeNode);
            }
            Clusters = fogClusters.Values.ToList();
            var time = (int)(DateTime.Now - start).TotalMilliseconds;

            UpdatePingStats(time);
            UpdateDraw();
        }

        private void btnIncrSolve_click(object sender, EventArgs e)
        {
            var slaMaxPing = int.Parse(txtSLAMaxPing.Text);
            var fogNodeCapacity = int.Parse(txtFogCapacity.Text);
            var start = DateTime.Now;
            ClusterIncremental(slaMaxPing, fogNodeCapacity == 0 ? (int?)null : fogNodeCapacity);
            var time = (int)(DateTime.Now - start).TotalMilliseconds;

            UpdatePingStats(time);
            UpdateDraw();
        }

        #endregion

        #region Solvers 

        private void OptimizeMergingFast(int slaMaxPing)
        {
            var start = DateTime.Now;
            var merged = true;

            //while (Clusters.Count > 1 && merged)
            {
                //Console.WriteLine("Attempting cluster merge");
                //find 2 closest clusters to attempt to merge
                TryMergeClustersFast(slaMaxPing);
            }
            //Console.WriteLine("Optimization took " + (DateTime.Now - start).TotalMilliseconds + "ms, " + Clusters.Count + " clusters left");

        }

        private void TryMergeClustersFast(int slaMaxPing)
        {
            if (Clusters.Count < 2)
                return;

            //perhaps work with average ping from one's members to the other's's? 
            //remove doubles that are reversed
            //and try to implement the "geo-near" bit here to reduce all the possible combos
            var ignoreList = Clusters.ToDictionary(c => c, c => false);
            var closestOrdered = Clusters.SelectMany(cl1 => Clusters.Select(cl2 => new Tuple<double, Cluster, Cluster>(cl1.FogNode.Distance(cl2.FogNode), cl1, cl2))).Where(t => t.Item2 != t.Item3 && t.Item2.FogNode.Distance(t.Item3.FogNode) < slaMaxPing).OrderBy(t => t.Item1).ToList();

            var idx = 0;
            while (idx < closestOrdered.Count())
            {
                //Console.WriteLine("Trying to merge cluster " + closestOrdered[idx].Item2 + " and " + closestOrdered[idx].Item3);
                var cluster1 = closestOrdered[idx].Item2;
                var cluster2 = closestOrdered[idx].Item3;
                if (!ignoreList[cluster1] && !ignoreList[cluster2])
                {
                    var mergedCluster1 = SimulateMerge(cluster1, cluster2, slaMaxPing);
                    var mergedCluster2 = SimulateMerge(cluster2, cluster1, slaMaxPing);

                    //determine best avg ping between the 2
                    if (mergedCluster1 != null && mergedCluster2 != null)
                    {
                        if (mergedCluster1.Nodes.Count == 0)
                        {
                            Clusters.Remove(cluster2);
                            ignoreList[cluster2] = true;
                        }
                        else
                        {
                            //Console.WriteLine("Both recombinations possible, finding best avg ping");
                            var avgCluster1Ping = mergedCluster1.Nodes.Average(n => n.Pings[mergedCluster1.FogNode]);
                            var avgCluster2Ping = mergedCluster2.Nodes.Average(n => n.Pings[mergedCluster2.FogNode]);
                            //Console.WriteLine("Avg ping combo 1 " + avgCluster1Ping + " Avg ping combo 2 " + avgCluster2Ping);
                            if (avgCluster1Ping < avgCluster2Ping)
                            {
                                cluster1.Nodes.AddRange(cluster2.Nodes);
                                Clusters.Remove(cluster2);
                                ignoreList[cluster2] = true;
                            }
                            else
                            {
                                cluster2.Nodes.AddRange(cluster1.Nodes);
                                Clusters.Remove(cluster1);
                                ignoreList[cluster1] = true;
                            }
                        }
                    }
                    else
                    {
                        if (mergedCluster1 != null)
                        {
                            cluster1.Nodes.AddRange(cluster2.Nodes);
                            Clusters.Remove(cluster2);
                            ignoreList[cluster2] = true;
                        }
                        else if (mergedCluster2 != null)
                        {
                            cluster2.Nodes.AddRange(cluster1.Nodes);
                            Clusters.Remove(cluster1);
                            ignoreList[cluster1] = true;
                        }
                    }
                }
                else
                {
                    //Console.WriteLine("One of the clusters to merge is on ignore list, skipping");
                }
                idx++;
            }
        }

        private void OptimizeMerging(int slaMaxPing)
        {
            //var start = DateTime.Now;
            var merged = true;

            while (Clusters.Count > 1 && merged)
            {
                //Console.WriteLine("Attempting cluster merge");
                //find 2 closest clusters to attempt to merge
                merged = TryMergeClusters(slaMaxPing);
            }
            //Console.WriteLine("Optimization took " + (DateTime.Now - start).TotalMilliseconds + "ms, " + Clusters.Count + " clusters left");
        }

        private bool TryMergeClusters(int slaMaxPing)
        {
            if (Clusters.Count < 2)
                return false;

            //perhaps work with average ping from one's members to the other's's? 
            //remove doubles that are reversed
            //and try to implement the "geo-near" bit here to reduce all the possible combos
            var closestOrdered = Clusters.SelectMany(cl1 => Clusters.Select(cl2 => new Tuple<double, Cluster, Cluster>(cl1.FogNode.Distance(cl2.FogNode), cl1, cl2))).Where(t => t.Item2 != t.Item3 && t.Item1 < slaMaxPing).OrderBy(t => t.Item1).ToList();

            var idx = 0;
            var merged = false;
            while (!merged && idx < closestOrdered.Count())
            {
                //Console.WriteLine("Trying to merge cluster " + closestOrdered[idx].Item2 + " and " + closestOrdered[idx].Item3);
                var mergedCluster1 = SimulateMerge(closestOrdered[idx].Item2, closestOrdered[idx].Item3, slaMaxPing);
                var mergedCluster2 = SimulateMerge(closestOrdered[idx].Item3, closestOrdered[idx].Item2, slaMaxPing);

                Cluster mergedCluster = null;
                //determine best avg ping between the 2
                if (mergedCluster1 != null && mergedCluster2 != null)
                {
                    if (mergedCluster1.Nodes.Count == 0)
                    {
                        mergedCluster = mergedCluster1;
                    }
                    else
                    {
                        //Console.WriteLine("Both recombinations possible, finding best avg ping");
                        var avgCluster1Ping = mergedCluster1.Nodes.Average(n => n.Pings[mergedCluster1.FogNode]);
                        var avgCluster2Ping = mergedCluster2.Nodes.Average(n => n.Pings[mergedCluster2.FogNode]);
                        //Console.WriteLine("Avg ping combo 1 " + avgCluster1Ping + " Avg ping combo 2 " + avgCluster2Ping);
                        if (avgCluster1Ping < avgCluster2Ping)
                        {
                            mergedCluster = mergedCluster1;
                        }
                        else
                        {
                            mergedCluster = mergedCluster2;
                        }
                    }
                }
                else
                {
                    //otherwise take the one not null, or either one if both null
                    mergedCluster = mergedCluster1 == null ? mergedCluster2 : mergedCluster1;
                }

                if (mergedCluster != null)
                {
                    //Console.WriteLine("Merge successful, merging into " + mergedCluster);
                    merged = true;
                    Clusters.Remove(closestOrdered[idx].Item2);
                    Clusters.Remove(closestOrdered[idx].Item3);
                    Clusters.Add(mergedCluster);
                }
                idx++;
            }
            return merged;
        }

        private Cluster SimulateMerge(Cluster cl1, Cluster cl2, int slaMaxPing)
        {
            var alreadyOverSLAPing = cl1.Nodes.Count(n => n.Pings[cl1.FogNode] > slaMaxPing) + cl2.Nodes.Count(n => n.Pings[cl2.FogNode] > slaMaxPing);
            var cluster = new Cluster() { FogNode = cl1.FogNode, Nodes = new List<Node>() };
            cluster.Nodes.AddRange(cl1.Nodes);
            cluster.Nodes.AddRange(cl2.Nodes);

            var newOverSLAPing = cluster.Nodes.Count(n => n.Pings[cluster.FogNode] > slaMaxPing);
            //Console.WriteLine("Constituent cluster over SLA ping " + alreadyOverSLAPing + " new over SLA ping " + newOverSLAPing);
            if (alreadyOverSLAPing < newOverSLAPing)
            {
                //Console.WriteLine("Too many nodes over SLA ping, ignoring this combo");
                return null;
            }

            return cluster;
        }

        private void ClusterIncremental(int slaMaxPing, int? fogCapacity)
        {
            //var start = DateTime.Now;
            Clusters = new List<Cluster>();
            IndexedClusters = new Dictionary<Node, Cluster>();
            foreach (var fogNode in FogNodes)
            {
                fogNode.Active = false;
            }

            foreach (var edgeNode in EdgeNodes)
            {
                IncrementalClusterNode(slaMaxPing, edgeNode, fogCapacity);
            }

            Clusters = IndexedClusters.Values.ToList();

            //Console.WriteLine("Incremental cluster took " + (DateTime.Now - start).TotalMilliseconds + "ms, " + Clusters.Count + " clusters created");
        }

        private void IncrementalClusterNode(int slaMaxPing, Node edgeNode, int? fogCapacity)
        {
            if (IndexedClusters.Count == 0)
            {
                CreateNewClusterFor(edgeNode);
            }
            else
            {
                Cluster closest = GetClosestCluster(edgeNode, fogCapacity);

                if (edgeNode.Pings[closest.FogNode] > slaMaxPing)
                {
                    TryCreateNewClusterFor(edgeNode, closest);
                }
                else
                {
                    closest.Nodes.Add(edgeNode);
                }
            }
        }

        private void TryCreateNewClusterFor(Node edgeNode, Cluster closestCluster)
        {
            Node closestFogNode = GetClosestFogNode(edgeNode);
            if (closestCluster.FogNode == closestFogNode)
            {
                //seems it's the closest anyway, add there
                closestCluster.Nodes.Add(edgeNode);
            }
            else
            {
                CreateNewClusterFor(edgeNode, closestFogNode);
            }
        }

        private void CreateNewClusterFor(Node edgeNode, Node closestFogNode = null)
        {
            if (closestFogNode == null)
                closestFogNode = GetClosestFogNode(edgeNode);

            Cluster cluster = null;
            var anyCluster = IndexedClusters.TryGetValue(closestFogNode, out cluster);//Clusters.FirstOrDefault(cl => cl.FogNode == closestFogNode);
            if (anyCluster)
            {
                cluster.Nodes.Add(edgeNode);
            }
            else
            {
                closestFogNode.Active = true;
                IndexedClusters[closestFogNode] = new Cluster() { FogNode = closestFogNode, Nodes = new List<Node>() { edgeNode } };
            }
        }

        private Cluster GetClosestCluster(Node edgeNode, int? fogCapacity)
        {
            Cluster closest = null;
            var closestPing = double.MaxValue;

            Node closestActiveNode = null;
            var idx = 0;
            while (closestActiveNode == null)
            {
                closestActiveNode = edgeNode.SortedPings[idx].Active ? edgeNode.SortedPings[idx] : null;
                if (closestActiveNode != null && fogCapacity.HasValue && IndexedClusters[closestActiveNode].Nodes.Count >= fogCapacity)
                {
                    closestActiveNode = null;
                }
                idx++;
            }
            
            closest = IndexedClusters[closestActiveNode];

            return closest;
        }

        private Node GetClosestFogNode(Node edgeNode)
        {
            Node closestFogNode = null;
            var closestPing = double.MaxValue;

            closestFogNode = edgeNode.SortedPings[0];

            return closestFogNode;
        }




        #endregion

        private void btnLoadDensity_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DensityMap = new Bitmap(Bitmap.FromFile(dialog.FileName));
                    //var bmp = new Bitmap(DensityMap);
                    Pixels = new List<System.Drawing.Point>();
                    var tempMap = new Bitmap(Bitmap.FromFile(dialog.FileName));
                    BuildEDensity(tempMap);
                    BuildFDensity(tempMap);
                    UpdateDraw();
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        void BuildEDensity(Bitmap densMap)
        {
            var maxPing = float.Parse(txtSLAMaxPing.Text);
            var capacity = float.Parse(txtFogCapacity.Text);

            var epDensities = new double[1200][];

            var etotal = 0d;
            var max = 0d;
            for (int x = 0; x < densMap.Width && x < 1200; x++)
            {
                epDensities[x] = new double[750];
                for (int y = 0; y < densMap.Height && y < 750; y++)
                {
                    var pix = densMap.GetPixel(x, y);
                    //white is "unknown, so leave at 0"
                    if (!(pix.R == 255 && pix.G == 255 && pix.B == 255))
                    {
                        epDensities[x][y] = pix.B * 255 * 255 + pix.G * 255 + pix.R;

                        etotal += epDensities[x][y];
                        max = Math.Max(epDensities[x][y], max);
                    }
                }
            }

            EPPixels = new List<double>();

            var pTotal = 0d;
            var statGradient = max / 255;
            for (int x = 0; x < densMap.Width && x < 1200; x++)
            {
                for (int y = 0; y < densMap.Height && y < 750; y++)
                {
                    pTotal += epDensities[x][y] / etotal;
                    EPPixels.Add(pTotal);
                    Pixels.Add(new System.Drawing.Point(x, y));

                    //cough
                    var pix = densMap.GetPixel(x, y);
                    var red = 0;
                    var green = 0;
                    var blue = 0;
                    var fillValue = epDensities[x][y] / statGradient;

                    HsvToRgb(180 + fillValue / 255 * 360, 1, 1, out red, out green, out blue);

                    if (!(pix.R == 255 && pix.G == 255 && pix.B == 255))
                    {
                        pix = Color.FromArgb(150, red, green, blue);
                        DensityMap.SetPixel(x, y, pix);
                    }
                }
            }
        }

        void BuildFDensity(Bitmap densMap)
        {
            var maxPing = float.Parse(txtSLAMaxPing.Text);
            var capacity = float.Parse(txtFogCapacity.Text);

            var pingVar = 1 + float.Parse(txtPingDistDiff.Text) / 100;
            var fpDensities = new double[1200][];

            var ftotal = 0d;
            var minDensity = 1 / (Math.PI * maxPing * maxPing * pingVar * pingVar);
            for (int x = 0; x < densMap.Width && x < 1200; x++)
            {
                fpDensities[x] = new double[750];
                for (int y = 0; y < densMap.Height && y < 750; y++)
                {
                    var pix = densMap.GetPixel(x, y);
                    //white is "unknown, so leave at 0"
                    if (!(pix.R == 255 && pix.G == 255 && pix.B == 255))
                    {
                        fpDensities[x][y] = pix.B * 255 * 255 + pix.G * 255 + pix.R;

                        ftotal += fpDensities[x][y];
                    }
                }
            }

            var estPixelCapacity = 0.025 * 0.025 / capacity;
            for (int x = 0; x < densMap.Width && x < 1200; x++)
            {
                for (int y = 0; y < densMap.Height && y < 750; y++)
                {
                    var pix = densMap.GetPixel(x, y);
                    //white is "unknown, so leave at 0"
                    if (!(pix.R == 255 && pix.G == 255 && pix.B == 255))
                    {
                        fpDensities[x][y] = fpDensities[x][y] * estPixelCapacity;
                        fpDensities[x][y] = Math.Max(fpDensities[x][y], minDensity);
                    }
                }
            }

            ftotal = 0;
            //var max = 0d;
            for (int x = 0; x < densMap.Width && x < 1200; x++)
            {
                for (int y = 0; y < densMap.Height && y < 750; y++)
                {
                    var pix = densMap.GetPixel(x, y);
                    //white is "unknown, so leave at 0"
                    if (!(pix.R == 255 && pix.G == 255 && pix.B == 255))
                    {
                        ftotal += fpDensities[x][y];
                        //max = Math.Max(fpDensities[x][y], max);
                    }
                }
            }

            FPPixels = new List<double>();
            var pTotal = 0d;
            //var statGradient = max / 255;
            for (int x = 0; x < densMap.Width && x < 1200; x++)
            {
                for (int y = 0; y < densMap.Height && y < 750; y++)
                {
                    pTotal += fpDensities[x][y] / ftotal;
                    FPPixels.Add(pTotal);
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
    }


}
