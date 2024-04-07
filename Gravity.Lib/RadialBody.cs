using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravity.Lib
{
    /// <summary>
    /// A circular body
    /// </summary>
    public class RadialBody : Body
    {
        /// <summary>
        /// The radius of the circular body (m)
        /// </summary>
        public float Radius
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public RadialBody()
        {
            Radius = 0;
        }
    }
}
