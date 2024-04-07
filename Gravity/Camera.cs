using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Gravity
{
    internal class Camera
    {
        public Point Position
        {
            get;
            set;
        }
        public float Zoom
        {
            get;
            set;
        }
        public static float ZoomSpeed
        {
            get
            {
                return 1.1f;
            }
        }
        public int MovementSpeed
        {
            get
            {
                return Math.Min((int)(40 / Zoom), 100);
            }
        }

        public Camera()
        {
            Position = new Point(0, 0);
            Zoom = 7f / 10000;
        }
    }
}
