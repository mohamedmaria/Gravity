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
        private float SizeFactor
        {
            get;
            set;
        }

        private SolarSystem SolarSystem
        {
            get;
            set;
        }
        private Point CameraPosition
        {
            get;
            set;
        }

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            var earth = new RadialBody()
            {
                Name = "Earth",
                Mass = (float)(5.972 * Math.Pow(10, 27)),
                Radius = 6371000,
                Position = new Numerics.Vector2(0, 0)
            };
            var luna = new RadialBody()
            {
                Name = "Luna",
                Mass = (float)(7.35 * Math.Pow(10, 22)),
                Radius = 1737400,
                Position = new Numerics.Vector2(0, 385000600)
            };
            luna.SetStableVelocity(earth);

            SolarSystem = new SolarSystem()
            {
                Bodies = new List<Body>()
                {
                    earth,
                    luna
                }
            };

            TimeFactor = 1000000;
            SizeFactor = 250000;
            PositionFactor = 2500000;
            CameraPosition = new Point(0, 0);
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
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            SolarSystem.UpdateBodies((float)gameTime.ElapsedGameTime.TotalSeconds * TimeFactor);

            int movement = 10;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                CameraPosition = new Point(CameraPosition.X, CameraPosition.Y + movement);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                CameraPosition = new Point(CameraPosition.X, CameraPosition.Y - movement);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                CameraPosition = new Point(CameraPosition.X + movement, CameraPosition.Y);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                CameraPosition = new Point(CameraPosition.X - movement, CameraPosition.Y);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
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

            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            if (SpriteBatch == null)
                return;
            if (MainFont == null)
                return;

            static int getRadius(RadialBody body, float sizeFactor)
            {
                return (int)(body.Radius / sizeFactor);
            }
            static Point getCenterPosition(Body body, float positionFactor)
            {
                return new Point((int)(body.Position.X / positionFactor), (int)(body.Position.Y / positionFactor));
            }
            static Point getTopLeftPosition(RadialBody body, float sizeFactor, float positionFactor)
            {
                var radius = getRadius(body, sizeFactor);
                return getCenterPosition(body, positionFactor) - new Point(radius, radius);
            }

            // Get camera position relative to earth
            var cameraPosition = CameraPosition;
            foreach (var body in SolarSystem.Bodies)
            {
                if (body.Name == "Earth")
                {
                    var position = getCenterPosition(body, PositionFactor);
                    cameraPosition = position - CameraPosition - new Point(Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferHeight / 2);
                }
            }

            // Get position text
            var positions = "";
            foreach (var body in SolarSystem.Bodies)
            {
                var position = getCenterPosition(body, PositionFactor);
                var relativePosition = position - CameraPosition;
                positions += body.Name + ": " + relativePosition.X + ", " + relativePosition.Y + "\n";
            }

            SpriteBatch.Begin();

            // Draw all bodies
            foreach (var body in SolarSystem.Bodies)
            {
                var radialBody = (body as RadialBody);
                if (radialBody != null)
                {
                    var radius = getRadius(radialBody, SizeFactor);
                    var position = getTopLeftPosition(radialBody, SizeFactor, PositionFactor);
                    var relativePosition = position - cameraPosition;

                    SpriteBatch.Draw(
                        createCircleText(radius * 2),
                        new Rectangle(relativePosition, new Point(radius * 2, radius * 2)),
                        Color.Brown
                        );
                }
            }

            // Draw position text
            SpriteBatch.DrawString(MainFont, positions, new Vector2(10, 10), Color.White);

            SpriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
