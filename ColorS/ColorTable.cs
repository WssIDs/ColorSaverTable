using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ColorS
{
    /// <summary>
    /// Color Table class
    /// </summary>
    public class ColorTable
    {
        /// <summary>
        /// index of color
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// color table column 
        /// </summary>
        public int Column  { get; set; }
        /// <summary>
        /// color table row
        /// </summary>
        public int Row { get; set; }
        /// <summary>
        /// color of color table
        /// </summary>
        public Color color { get; set; }
    }
}
