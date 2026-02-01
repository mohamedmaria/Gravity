using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using Numerics = System.Numerics;

using Gravity.Lib;

namespace Gravity
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager Graphics
        {
            get;
            set;
        }
        private SpriteBatch? SpriteBatch
        {
            get;
            set;
        }

        public SpriteFont? MainFont
        {
            get;
            set;
        }
        public Dictionary<Body, Texture2D> BodyTextures
        {
            get;
            set;
        }

        private float TimeFactor
        {
            get;
            set;
        }
        private float PositionFactor
        {
            get;
            set;
        }
        private float MoonSizeFactor
        {
            get;
            set;
        }
        private float PlanetSizeFactor
        {
            get;
            set;
        }
        private float SunSizeFactor
        {
            get;
            set;
        }

        private BodyCollection Bodies
        {
            get;
            set;
        }
        private Vector2[] Stars
        {
            get;
            set;
        }
        private Camera Camera
        {
            get;
            set;
        }
        private string Selection = "Sol";
        private int previousMouseWheel = int.MaxValue;
        
        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            BodyTextures = [];

            TimeFactor = 1000;
            MoonSizeFactor = 150;
            PlanetSizeFactor = 150;
            SunSizeFactor = 150;
            PositionFactor = 5000;

            Camera = new Camera();

            var sol = new RadialBody(
                BodyType.Sun,
                "Sol",
                (float)(1.9885 * Math.Pow(10, 33)),
                696000000,
                System.Drawing.Color.FromArgb(255, 200, 0)
                );
            var mercury = new RadialBody(
                BodyType.Planet,
                "Mercury",
                (float)(3.302 * Math.Pow(10, 26)),
                2439700,
                System.Drawing.Color.FromArgb(180, 150, 130),
                sol,
                57910000000,
                0
            );
            var venus = new RadialBody(
                BodyType.Planet,
                "Venus",
                (float)(4.868 * Math.Pow(10, 27)),
                6051800,
                System.Drawing.Color.FromArgb(255, 180, 100),
                sol,
                108210000000,
                0
            );
            var earth = new RadialBody(
                BodyType.Planet,
                "Earth",
                (float)(5.972 * Math.Pow(10, 27)),
                6371000,
                System.Drawing.Color.FromArgb(100, 150, 200),
                sol,
                149597870000,
                0
            );
            var luna = new RadialBody(
                BodyType.Moon,
                "Luna",
                (float)(7.35 * Math.Pow(10, 22)),
                1737400,
                System.Drawing.Color.FromArgb(200, 200, 200),
                earth,
                385000600,
                0
            );
            var mars = new RadialBody(
                BodyType.Planet,
                "Mars",
                (float)(6.4191 * Math.Pow(10, 26)),
                3396200,
                System.Drawing.Color.FromArgb(200, 100, 50),
                sol,
                227940000000,
                0
            );
            var jupiter = new RadialBody(
                BodyType.Planet,
                "Jupiter",
                (float)(1.8987 * Math.Pow(10, 30)),
                71492000,
                System.Drawing.Color.FromArgb(200, 150, 100),
                sol,
                778410000000,
                0
            );
            var saturn = new RadialBody(
                BodyType.Planet,
                "Saturn",
                (float)(5.6851 * Math.Pow(10, 29)),
                60268000,
                System.Drawing.Color.FromArgb(220, 190, 140),
                sol,
                1430000000000,
                0
            );
            var uranus = new RadialBody(
                BodyType.Planet,
                "Uranus",
                (float)(8.6849 * Math.Pow(10, 28)),
                25559000,
                System.Drawing.Color.FromArgb(100, 180, 220),
                sol,
                2870000000000,
                0
            );
            var neptune = new RadialBody(
                BodyType.Planet,
                "Neptune",
                (float)(1.0244 * Math.Pow(10, 29)),
                24764000,
                System.Drawing.Color.FromArgb(50, 100, 200),
                sol,
                4500000000000,
                0
            );

            Bodies = new BodyCollection()
            {
                StartDate = new DateTime(2024, 4, 14),
                Date = new DateTime(2024, 4, 14),
                Bodies =
                [
                    sol,
                    mercury,
                    venus,
                    earth,
                    luna,
                    mars,
                    jupiter,
                    saturn,
                    uranus,
                    neptune
                ]
            };
            Bodies.StartUpdate(TimeFactor);

            var starCount = 500;
            Stars = new Vector2[starCount];           
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Graphics.PreferredBackBufferWidth = 1920;
            Graphics.PreferredBackBufferHeight = 1080;
            Graphics.ApplyChanges();

            var rand = new Random(12345);
            for (int i = 0; i < Stars.Length; i++)
            {
                Stars[i] = new Vector2(rand.Next(0, Graphics.PreferredBackBufferWidth), rand.Next(0, Graphics.PreferredBackBufferHeight));
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Load fonts
            MainFont = Content.Load<SpriteFont>("File");

            // Load textures
            Texture2D createPlanetTexture(int diameter, Body body)
            {
                var texture = new Texture2D(GraphicsDevice, diameter, diameter);
                var colorData = new Color[diameter * diameter];

                var radius = diameter / 2f;
                var radiussq = radius * radius;

                for (int x = 0; x < diameter; x++)
                {
                    for (int y = 0; y < diameter; y++)
                    {
                        var index = x * diameter + y;
                        var pos = new Vector2(x - radius, y - radius);
                        if (pos.LengthSquared() <= radiussq)
                        {
                            // Add some procedural variation
                            var noise = SimplexNoise(x, y, body.Name.GetHashCode()) * 0.3f;

                            colorData[index] = new Color(
                                (int)Math.Clamp(body.BaseColor.R + noise * 30, 0, 255),
                                (int)Math.Clamp(body.BaseColor.G + noise * 30, 0, 255),
                                (int)Math.Clamp(body.BaseColor.B + noise * 30, 0, 255),
                                255
                            );
                        }
                        else
                        {
                            colorData[index] = Color.Transparent;
                        }
                    }
                }

                texture.SetData(colorData);
                return texture;
            }

            foreach (var body in Bodies.Bodies)
            {
                BodyTextures[body] = createPlanetTexture(1000, body);
            }
        }

        private static float SimplexNoise(int x, int y, int seed)
        {
            var n = (float)Math.Sin((x + seed) * 12.9898f + (y + seed) * 78.233f) * 43758.5453f;
            return n - (float)Math.Floor(n);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Camera.Position = new Point(Camera.Position.X, Camera.Position.Y + Camera.MovementSpeed);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Camera.Position = new Point(Camera.Position.X, Camera.Position.Y - Camera.MovementSpeed);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                Camera.Position = new Point(Camera.Position.X + Camera.MovementSpeed, Camera.Position.Y);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                Camera.Position = new Point(Camera.Position.X - Camera.MovementSpeed, Camera.Position.Y);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.NumPad0))
            {
                Selection = "Sol";
                Camera.Position = new Point(0, 0);
                Camera.Zoom = SetCameraZoomForBody(Bodies.Bodies.FirstOrDefault(temp => temp.Name == Selection)) ?? Camera.Zoom;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.NumPad1))
            {
                Selection = "Mercury";
                Camera.Position = new Point(0, 0);
                Camera.Zoom = SetCameraZoomForBody(Bodies.Bodies.FirstOrDefault(temp => temp.Name == Selection)) ?? Camera.Zoom;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.NumPad2))
            {
                Selection = "Venus";
                Camera.Position = new Point(0, 0);
                Camera.Zoom = SetCameraZoomForBody(Bodies.Bodies.FirstOrDefault(temp => temp.Name == Selection)) ?? Camera.Zoom;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.NumPad3))
            {
                Selection = "Earth";
                Camera.Position = new Point(0, 0);
                Camera.Zoom = SetCameraZoomForBody(Bodies.Bodies.FirstOrDefault(temp => temp.Name == Selection)) ?? Camera.Zoom;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.NumPad4))
            {
                Selection = "Mars";
                Camera.Position = new Point(0, 0);
                Camera.Zoom = SetCameraZoomForBody(Bodies.Bodies.FirstOrDefault(temp => temp.Name == Selection)) ?? Camera.Zoom;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.NumPad5))
            {
                Selection = "Jupiter";
                Camera.Position = new Point(0, 0);
                Camera.Zoom = SetCameraZoomForBody(Bodies.Bodies.FirstOrDefault(temp => temp.Name == Selection)) ?? Camera.Zoom;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.NumPad6))
            {
                Selection = "Saturn";
                Camera.Position = new Point(0, 0);
                Camera.Zoom = SetCameraZoomForBody(Bodies.Bodies.FirstOrDefault(temp => temp.Name == Selection)) ?? Camera.Zoom;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.NumPad7))
            {
                Selection = "Uranus";
                Camera.Position = new Point(0, 0);
                Camera.Zoom = SetCameraZoomForBody(Bodies.Bodies.FirstOrDefault(temp => temp.Name == Selection)) ?? Camera.Zoom;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.NumPad8))
            {
                Selection = "Neptune";
                Camera.Position = new Point(0, 0);
                Camera.Zoom = SetCameraZoomForBody(Bodies.Bodies.FirstOrDefault(temp => temp.Name == Selection)) ?? Camera.Zoom;
            }

            // Mouse wheel zoom
            {
                var mouseState = Mouse.GetState();
                if (previousMouseWheel == int.MaxValue)
                    previousMouseWheel = mouseState.ScrollWheelValue;
                if (mouseState.ScrollWheelValue > previousMouseWheel)
                    Camera.Zoom *= Camera.ZoomSpeed;
                if (mouseState.ScrollWheelValue < previousMouseWheel)
                    Camera.Zoom /= Camera.ZoomSpeed;
                previousMouseWheel = mouseState.ScrollWheelValue;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(5, 5, 20));

            // TODO: Add your drawing code here
            if (SpriteBatch == null)
                return;
            if (MainFont == null)
                return;

            // Get camera position relative to earth
            var referenceBodyName = Selection;
            var referencePosition = new Point(0, 0);
            var referenceCameraPosition = new Point(0, 0);
            foreach (var body in Bodies.Bodies)
            {
                if (body.Name == referenceBodyName)
                {
                    var positionFactor = PositionFactor / Camera.Zoom;

                    referencePosition = GetCenterPosition(body, 1000000);
                    referenceCameraPosition = GetCenterPosition(body, positionFactor);
                }
            }

            // Set camera position
            var cameraPosition = referenceCameraPosition - Camera.Position - new Point(Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferHeight / 2);

            static string getPositionText(string title, Point position, Point referencePosition)
            {
                var arrow = "";
                var angle = Math.Round(Math.Atan2((referencePosition - position).Y, (referencePosition - position).X) * 180 / Math.PI, 2);
                if (angle >= 0 && angle <= 45)
                {
                    arrow = "=>";
                }
                else if (angle >= -45 && angle <= 0)
                {
                    arrow = "=>";
                }
                else if (angle >= 135 && angle <= 180)
                {
                    arrow = "<=";
                }
                else if (angle >= -180 && angle <= -135)
                {
                    arrow = "<=";
                }
                else if (angle >= 45 && angle <= 135)
                {
                    arrow = "A";
                }
                else if (angle >= -135 && angle <= -45)
                {
                    arrow = "V";
                }

                return title + ": " + (referencePosition - position).ToVector2().Length() + " <" + angle + " " + arrow;
            }

            // Get position text
            var positions = "";
            {
                var positionFactor = PositionFactor / Camera.Zoom;
                positions += getPositionText("Camera", new Point(0, 0), new Point((int)(Camera.Position.X * positionFactor / 1000000), (int)(Camera.Position.Y * positionFactor / 1000000))) + "\n";
            }
            foreach (var body in Bodies.Bodies)
            {
                if (body.Name != referenceBodyName)
                {
                    var position = GetCenterPosition(body, 1000000);
                    positions += getPositionText(body.Name, position, referencePosition) + "\n";
                }
            }

            SpriteBatch.Begin();

            // Draw starfield
            foreach (var star in Stars)
            {
                SpriteBatch.Draw(
                    BodyTextures[Bodies.Bodies[0]], // Use any small texture
                    new Rectangle((int)star.X, (int)star.Y, 2, 2),
                    Color.White * 0.8f
                    );
            }

            // Draw all bodies
            foreach (var body in Bodies.Bodies)
            {
                var radialBody = (body as RadialBody);
                if (radialBody != null)
                {
                    var sizeFactor = (
                        radialBody.BodyType == BodyType.Sun ? SunSizeFactor :
                        radialBody.BodyType == BodyType.Planet ? PlanetSizeFactor :
                        MoonSizeFactor
                        ) / Camera.Zoom;
                    var positionFactor = PositionFactor / Camera.Zoom;

                    var radius = GetRadius(radialBody, sizeFactor);
                    var position = GetTopLeftPosition(radialBody, sizeFactor, positionFactor);
                    var relativePosition = (position - cameraPosition);

                    SpriteBatch.Draw(
                        BodyTextures[body],
                        new Rectangle(relativePosition, new Point(radius * 2, radius * 2)),
                        Color.White
                        );

                    if (Camera.Zoom > (1f / 10000) || radialBody.BodyType != BodyType.Moon)
                    {
                        SpriteBatch.DrawString(MainFont, radialBody.Name, new Vector2(relativePosition.X, relativePosition.Y), Color.White);
                    }
                }
            }

            // Draw position text
            SpriteBatch.DrawString(MainFont, positions, new Vector2(10, 10), Color.White);

            SpriteBatch.End();


            base.Draw(gameTime);
        }

        // Helpers
        private static float? SetCameraZoomForBody(Body? body)
        {
            var radialBody = body as RadialBody;
            if (radialBody != null)
            {
                var solRadius = 696000000f;
                var zoomRatio = solRadius / radialBody.Radius;
                return (1f / 100000) * zoomRatio;
            }
            return null;
        }

        static int GetRadius(RadialBody? body, float sizeFactor)
        {
            if (body == null)
                return 0;

            return (int)(body.Radius / sizeFactor);
        }
        static Point GetCenterPosition(Body body, float positionFactor)
        {
            return new Point((int)(body.Position.X / positionFactor), (int)(body.Position.Y / positionFactor));
        }
        static Point GetTopLeftPosition(RadialBody body, float sizeFactor, float positionFactor)
        {
            var radius = GetRadius(body, sizeFactor);
            return GetCenterPosition(body, positionFactor) - new Point(radius, radius);
        }
    }
}
