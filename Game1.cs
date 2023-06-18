using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Comora;
using System.IO;
using System;

namespace ProgGame
{
	public class Game1 : Game
	{
		private const int defaultLives = 30;
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private GameLevel level;
		private SpriteFont spriteFont;
		private SpriteFont boldFont;
		private bool isTest = false;
		private int livesCount = defaultLives;
		private int levelIndex = -1;
		private int maxLevelIndex = 4;
		private bool gameEnded = false;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			_graphics.PreferredBackBufferWidth = 1920;
			_graphics.PreferredBackBufferHeight = 1080;
			_graphics.HardwareModeSwitch = false;
			_graphics.ToggleFullScreen();
			this._graphics.ApplyChanges();
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			LoadNextLevel();
			base.Initialize();
		}

		private void LoadTestLevel()
		{
			string path = "Content/levels/testlevel.txt";
			using (Stream fileStream = TitleContainer.OpenStream(path))
				level = new GameLevel(Services, fileStream, GraphicsDevice);
		}

		private void LoadNextLevel()
		{
			if (isTest)
			{
				LoadTestLevel();
				return;
			}
			if (level != null)
				level.content.Unload();
			if (levelIndex != maxLevelIndex)
				++levelIndex;
			else
				gameEnded = true;
			string path = $"Content/levels/{levelIndex}.txt";
			using (Stream fileStream = TitleContainer.OpenStream(path))
				level = new GameLevel(Services, fileStream, GraphicsDevice);
		}

		protected override void LoadContent()
		{
			spriteFont = Content.Load<SpriteFont>("font");
			boldFont = Content.Load<SpriteFont>("boldfont");
		}

		protected override void Update(GameTime gameTime)
		{
			if (gameEnded)
				return;

			KeyboardState state = Keyboard.GetState();
			if (!level.player.isAlive && state.IsKeyDown(Keys.R))
			{
				levelIndex -= 1;
				if (--livesCount <= 0)
				{
					livesCount = defaultLives;
					levelIndex = -1;
				}
				LoadNextLevel();
			}
			if (level.levelEnded)
				LoadNextLevel();
			if (state.IsKeyDown(Keys.Escape))
				Exit();
			level.Update(gameTime);
			base.Update(gameTime);
		}
		private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
		{
			_spriteBatch.DrawString(font, value, position + new Vector2(2, 2), Color.Black);
			_spriteBatch.DrawString(font, value, position, color);
		}
		private void DrawUI()
		{
			_spriteBatch.Begin();

			string s = $"Жизней осталось: {livesCount}";
			Vector2 size = spriteFont.MeasureString(s);
			DrawShadowedString(spriteFont, s, new Vector2((1920 / 2) - (size.X / 2), 10), Color.White);

			if (gameEnded)
			{
				string a = "Игра пройдена!";
				Vector2 strSize = spriteFont.MeasureString(a);
				DrawShadowedString(spriteFont, a, new Vector2((GraphicsDevice.Viewport.Width / 2) - (strSize.X / 2),
					(GraphicsDevice.Viewport.Height / 2) - (strSize.Y / 2)), Color.White);
			}

			if (!level.player.isAlive)
			{
				if (livesCount <= 1)
				{
					string a = "Вы проиграли\nНажмите \"R\" чтобы перезапустить игру";
					Vector2 strSize = spriteFont.MeasureString(a);
					DrawShadowedString(spriteFont, a, new Vector2((GraphicsDevice.Viewport.Width / 2) - (strSize.X / 2), 
						(GraphicsDevice.Viewport.Height / 2) - (strSize.Y / 2)), Color.White);
				}
				else
				{
					string a = "Вы умерли\nНажмите \"R\" чтобы перезапустить уровень";
					Vector2 strSize = spriteFont.MeasureString(a);
					DrawShadowedString(spriteFont, a, new Vector2((GraphicsDevice.Viewport.Width / 2) - (strSize.X / 2),
						(GraphicsDevice.Viewport.Height / 2) - (strSize.Y / 2)), Color.White);
				}
			}
			_spriteBatch.End();
		}
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(new Color(80, 187, 255));

			_spriteBatch.Begin(level.camera); // Рисуем объекты в вьюпорте камеры
			level.Draw(_spriteBatch);
			_spriteBatch.End();
			DrawUI();
			base.Draw(gameTime);
		}
	}
}