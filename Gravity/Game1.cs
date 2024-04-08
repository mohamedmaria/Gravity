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
        private Camera Camera
        {
            get;
            set;
        }
        private int previousMouseWheel = int.MaxValue;

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
                        
            BodyTextures = [];

            TimeFactor = 1000;
            MoonSizeFactor = 100;
            PlanetSizeFactor = 150;
            SunSizeFactor = 500;
            PositionFactor = 5000;

            Camera = new Camera();

            var sol = new RadialBody()
            {
                BodyType = BodyType.Sun,
                Name = "Sol",
                Mass = (float)(1.9885 * Math.Pow(10, 33)),
                Radius = 696000000
            };
            var mercury = new RadialBody()
            {
                BodyType = BodyType.Planet,
                Name = "Mercury",
                Mass = (float)(3.302 * Math.Pow(10, 26)),
                Radius = 2439700
            };
            var venus = new RadialBody()
            {
                BodyType = BodyType.Planet,
                Name = "Venus",
                Mass = (float)(4.868 * Math.Pow(10, 27)),
                Radius = 6051800
            };
            var earth = new RadialBody()
            {
                BodyType = BodyType.Planet,
                Name = "Earth",
                Mass = (float)(5.972 * Math.Pow(10, 27)),
                Radius = 6371000
            };
            var luna = new RadialBody()
            {
                BodyType = BodyType.Moon,
                Name = "Luna",
                Mass = (float)(7.35 * Math.Pow(10, 22)),
                Radius = 1737400
            };

            mercury.InitBasedOnParent(sol, 
                new Numerics.Vector2(0, 57910000000));
            venus.InitBasedOnParent(sol, 
                new Numerics.Vector2(0, 108210000000));
            earth.InitBasedOnParent(sol, 
                new Numerics.Vector2(0, 149597870000));
            luna.InitBasedOnParent(earth, 
                new Numerics.Vector2(0, 385000600));

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
                    luna
                ]
            };
            Bodies.StartUpdate(TimeFactor);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Graphics.PreferredBackBufferWidth = 1920;
            Graphics.PreferredBackBufferHeight = 1080;
            Graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Load fonts
            MainFont = Content.Load<SpriteFont>("File");

            // Load textures
            Texture2D createCircleText(int diameter)
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
                            colorData[index] = Color.White;
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
                BodyTextures[body] = createCircleText(1000);
            }
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            if (SpriteBatch == null)
                return;
            if (MainFont == null)
                return;

            // Get camera position relative to earth
            var referenceBodyName = "Sol";
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
                        Color.Brown
                        );

                    SpriteBatch.DrawString(MainFont, radialBody.Name, new Vector2(relativePosition.X, relativePosition.Y), Color.White);
                }
            }

            // Draw position text
            SpriteBatch.DrawString(MainFont, positions, new Vector2(10, 10), Color.White);

            SpriteBatch.End();


            base.Draw(gameTime);
        }

        // Helpers
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
