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
        /// The current date
        /// </summary>
        public DateTime Date
        {
            get;
            set;
        }
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

                body.Jerk = (acceleration - body.Acceleration) / elapsedTime;
                body.Acceleration = acceleration;
            }

            // Update position
            foreach (var body in Bodies)
            {
                body.Position =
                    (1.0f / 6.0f) * body.Jerk * ((float)Math.Pow(elapsedTime, 3)) +
                    (1.0f / 2.0f) * body.Acceleration * ((float)Math.Pow(elapsedTime, 2)) +
                    (1.0f / 1.0f) * body.Velocity * elapsedTime +
                    (1.0f / 1.0f) * body.Position;
            }

            // Update velocity
            foreach (var body in Bodies)
            {
                body.Velocity =
                    (1.0f / 2.0f) * body.Jerk * ((float)Math.Pow(elapsedTime, 2)) +
                    (1.0f / 1.0f) * body.Acceleration * elapsedTime +
                    (1.0f / 1.0f) * body.Velocity;
            }
        }
        /// <summary>
        /// Starts the position update routine
        /// </summary>
        public void StartUpdate(float timeFactor)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += (sender, e) =>
            {
                var previousTime = DateTime.Now;
                while(true)
                {
                    try 
                    {
                        var currentTime = DateTime.Now;
                        Date = Date.AddSeconds((currentTime - previousTime).TotalMilliseconds * timeFactor);
                        UpdateBodies(timeFactor);
                        previousTime = currentTime;
                    }
                    catch 
                    { 
                    
                    }
                    Thread.Sleep(1);
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
