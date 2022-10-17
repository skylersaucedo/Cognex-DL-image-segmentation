using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSI_DL_cognexIntegration
{
    public class defect
    {
        public string filePath { get; set; }
        public double x { get; set; }

        public double y { get; set; }

        public double height { get; set; }

        public double width { get; set; }

        public string bestTagName { get; set; }
        public double bestTagScore { get; set; }

        public double Redscore { get; set; }

        public List<string> tags { get; set; }
        public List<double> GreenScores { get; set; }

    }
}
