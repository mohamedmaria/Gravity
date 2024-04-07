using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Numerics;

namespace Gravity.Lib
{
    /// <summary>
    /// A body
    /// </summary>
    public class Body
    {
        /// <summary>
        /// Name of the body
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// Mass of the body (g)
        /// </summary>
        public float Mass
        {
            get;
            set;
        }

        /// <summary>
        /// Position of the body m
        /// </summary>
        public Vector2 Position
        {
            get;
            set;
        }
        /// <summary>
        /// Velocity of the body m/s
        /// </summary>
        public Vector2 Velocity
        {
            get;
            set;
        }
        /// <summary>
        /// Velocity of the body m/s^2
        /// </summary>
        public Vector2 Acceleration
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Body()
        {
            Name = "";
            Mass = 0;
            Position = new Vector2(0, 0);
            Velocity = new Vector2(0, 0);
            Acceleration = new Vector2(0, 0);
        }
    }
}
