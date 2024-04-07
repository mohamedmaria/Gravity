using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Numerics;

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

        private void UpdateBodies(float elapsedTime)
        {
            // Update acceleration
            foreach (var body in Bodies)
            {
                var acceleration = new Vector2(0, 0);
                foreach (var otherBody in Bodies)
                {
                    if (body != otherBody)
                    {
                        var unitVector = (otherBody.Position - body.Position) / (otherBody.Position - body.Position).Length();
                        var magnitude = (Constants.Gravity * (otherBody.Mass / 1000) / (otherBody.Position - body.Position).LengthSquared());

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
        /// Starts the position update routine
        /// </summary>
        public void StartUpdate(float timeFactor)
        {
            int elapsedTime = 1;

            var bw = new BackgroundWorker();
            bw.DoWork += (sender, e) =>
            {
                while(true)
                {
                    try 
                    { 
                        UpdateBodies(timeFactor * elapsedTime / 1000); 
                    }
                    catch 
                    { 
                    
                    }
                    Thread.Sleep(elapsedTime);
                }
            };
            bw.RunWorkerAsync();
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
