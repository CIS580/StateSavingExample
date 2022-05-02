using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Text;

namespace StateSavingExample
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private string _saveFilePath;
        private Texture2D _slime;
        private Vector2 _position;
        private SpriteFont _font;
        private KeyboardState _oldState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Determine a platform-independent save file path
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appName = "StateSavingExample.save";
            _saveFilePath = Path.Combine(appDataPath, appName, "SaveFile");

            // Ensure the game data directory exists
            string directoryPath = Path.Combine(appDataPath, appName);
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            // Write the file path to the debug output
            System.Diagnostics.Debug.WriteLine($"Save file path: {_saveFilePath}");
        }

        protected override void Initialize()
        {
            // Create the player's initial position
            _position = new Vector2(300, 300);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // Load the player sprite
            _slime = Content.Load<Texture2D>("slime");
            _font = Content.Load<SpriteFont>("font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            KeyboardState ks = Keyboard.GetState();

            // Move the player
            if (ks.IsKeyDown(Keys.Left)) _position.X -= 1;
            if (ks.IsKeyDown(Keys.Right)) _position.X += 1;
            if (ks.IsKeyDown(Keys.Up)) _position.Y -= 1;
            if (ks.IsKeyDown(Keys.Down)) _position.Y += 1;

            // Save or load the game
            if (ks.IsKeyDown(Keys.S) && !_oldState.IsKeyDown(Keys.S)) SaveGame();
            if (ks.IsKeyDown(Keys.L) && !_oldState.IsKeyDown(Keys.L)) LoadGame();

            _oldState = ks;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            
            _spriteBatch.Begin();
            // Draw the player
            _spriteBatch.Draw(_slime, _position, Color.White);
            // Draw instructions 
            _spriteBatch.DrawString(_font, "Use arrow keys to move, [s] to save, and [l] to load.", new Vector2(50, 50), Color.Yellow);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Saves the game, saving the current position of the player
        /// </summary>
        private void SaveGame()
        {
            // Open the save file for writing, overwriting if necessary
            using (var stream = File.Open(_saveFilePath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    // Write the player's location to the file
                    writer.Write(_position.X);
                    writer.Write(_position.Y);
                }
            }
        }

        /// <summary>
        /// Loads the game, moving the player to the loaded position
        /// </summary>
        private void LoadGame() 
        {
            // Open the save file fo reading
            using (var stream = File.Open(_saveFilePath, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    // Read the player's position from the file
                    _position.X = reader.ReadSingle();
                    _position.Y = reader.ReadSingle();
                }
            }
        }
    }
}
