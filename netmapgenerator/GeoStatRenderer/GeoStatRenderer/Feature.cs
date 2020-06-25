using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStatRenderer
{
    class Feature
    {
        public string Type { get; set; }
        public Dictionary<string,string> Properties { get; set; }
        public Geometry Geometry { get; set; }

        public string PKUID
        {
            get
            {
                return Properties["PKUID"];
            }
        }

        public string ObjectID
        {
            get
            {
                return Properties["OBJECTID"];
            }
        }

        public string SectorID
        {
            get
            {
                return Properties["CD_SECTOR"];
            }
        }
    }
}
