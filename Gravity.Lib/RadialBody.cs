﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Numerics;

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
        public RadialBody(BodyType bodyType, string name, float mass, float radius)
        {
            BodyType = bodyType;
            Name = name;
            Mass = mass;
            Radius = radius;

            Position = new Vector2(0, 0);
            Velocity = new Vector2(0, 0);
            Acceleration = new Vector2(0, 0);
            Jerk = new Vector2(0, 0);
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public RadialBody(BodyType bodyType, string name, float mass, float radius, Body parent, float orbitLength, float orbitDegrees) : this(bodyType, name, mass, radius)
        {
            InitBasedOnParent(parent, orbitLength, orbitDegrees);
        }
    }
}
