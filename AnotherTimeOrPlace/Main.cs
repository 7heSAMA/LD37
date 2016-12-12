using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using AnotherTimeOrPlace.Theater;
using AnotherTimeOrPlace.Util;

namespace AnotherTimeOrPlace
{
    /* TODO 
     * State manager: buggy but done 
     * Events manager: not started 
     * Component based entities: partial 
     * Input manager: untested but done 
     * Presorted rendering: not done
     * Tile engine: not ported 
     * Generalized collisions: not done 
     * Voronoi resource control: not done 
     * Animation system: not done 
     * Actual game: not done 
     */

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Main : Game
    {
        private GraphicsDeviceManager Graphics;
        private SpriteBatch Batch;
        private Viewport View;

        public Main()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            View = new Viewport(new Rectangle(0, 0, 960, 540));
            Graphics.PreferredBackBufferWidth = View.Width;
            Graphics.PreferredBackBufferHeight = View.Height;
            Graphics.IsFullScreen = false;
            Graphics.ApplyChanges();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            Batch = new SpriteBatch(GraphicsDevice);
            Registry.LoadGameServices(this, Graphics.GraphicsDevice, Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Registry.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Batch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null,
                null,
                null,
                Registry.Scale);

            Registry.Draw(Batch);
            Batch.End();
            base.Draw(gameTime);
        }
    }
}
