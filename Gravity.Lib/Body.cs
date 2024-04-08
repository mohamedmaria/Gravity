using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Numerics;

namespace Gravity.Lib
{
    public enum BodyType
    {
        Planet,
        Moon,
        Sun
    }

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
        /// The type of body
        /// </summary>
        public BodyType BodyType
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
        /// Acceleration of the body m/s^2
        /// </summary>
        public Vector2 Acceleration
        {
            get;
            set;
        }
        /// <summary>
        /// Jerk of the body m/s^3
        /// </summary>
        public Vector2 Jerk
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the velocity to ensure stable orbit around parent
        /// </summary>
        internal void InitBasedOnParent(Body parent, Vector2 distance)
        {
            Position = parent.Position + distance;

            var unit = (Position - parent.Position) / (Position - parent.Position).Length();
            var orthogonal = new Vector2(unit.Y, -1 * unit.X);
            var magnitude = (float)Math.Sqrt(Constants.Gravity * (parent.Mass / 1000) / (Position - parent.Position).Length());

            Velocity = new Vector2(magnitude * orthogonal.X, magnitude * orthogonal.Y) + parent.Velocity;
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
            Jerk = new Vector2(0, 0);
        }
    }
}
