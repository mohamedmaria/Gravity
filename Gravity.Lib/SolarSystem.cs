using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Gravity.Lib
{
    /// <summary>
    /// The system of bodies
    /// </summary>
    public class SolarSystem
    {
        /// <summary>
        /// List of bodies in the system
        /// </summary>
        public List<Body> Bodies
        {
            get;
            set;
        }

        /// <summary>
        /// Updates the acceleration/velocity/position of all bodies in the system
        /// </summary>
        public void UpdateBodies(float elapsedTime)
        {
            var constG = (float)(6.674 * Math.Pow(10, -11));

            // Update acceleration
            foreach (var body in Bodies)
            {
                var acceleration = new Vector2(0, 0);
                foreach (var otherBody in Bodies)
                {
                    if (body != otherBody)
                    {
                        var unitVector = (otherBody.Position - body.Position) / (otherBody.Position - body.Position).Length();
                        var magnitude = (constG * (otherBody.Mass / 1000) / (otherBody.Position - body.Position).LengthSquared());

                        acceleration += new Vector2(unitVector.X * magnitude, unitVector.Y * magnitude);
                    }
                }
                body.Acceleration = acceleration;
            }

            // Update position
            foreach (var body in Bodies)
            {
                body.Position =
                    0.5f * body.Acceleration * ((float)Math.Pow(elapsedTime, 2)) +
                    body.Velocity * elapsedTime +
                    body.Position;
            }

            // Update velocity
            foreach (var body in Bodies)
            {
                body.Velocity = 
                    body.Acceleration * elapsedTime +
                    body.Velocity;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SolarSystem()
        {
            Bodies = [];
        }
    }
}
