using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProgGame
{
	public class MovingPlatform
	{
		public const float movingSpeed = 2f;

		private bool isMoving = false;
		private Texture2D texture;
		public Vector2 position;
		private GameLevel level;
		private Vector2 startPosition;
		private Rectangle localBounds;
		public Rectangle Bounds
		{
			get
			{
				int left = (int)Math.Round(position.X - (texture.Width / 2)) + localBounds.X;
				int top = (int)Math.Round(position.Y - texture.Height) + localBounds.Y;

				return new Rectangle(left, top, localBounds.Width, localBounds.Height);
			}
		}

		public MovingPlatform(GameLevel level, Vector2 position)
		{
			this.level = level;
			startPosition = position;
			this.position = startPosition;
			texture = level.content.Load<Texture2D>("tiles/movingplatform");
			int width = (int)texture.Width;
			int left = (texture.Width - width) / 2;
			int height = 1;
			int top = texture.Height - height;
			localBounds = new Rectangle(left, top, width, height);
		}

		public void Update()
		{
			if (isMoving)
				position = new Vector2(position.X, position.Y - movingSpeed);
		}

		public void TriggerMove()
		{
			isMoving = true;
		}
		public void UnTriggerMove()
		{
			isMoving = false;
		}
		public void ResetPosition()
		{
			position = startPosition;
		}
		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, position, null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 4), 1.0f, SpriteEffects.None, 0.0f);
		//	Extentions.DrawRectangle(Bounds, spriteBatch);
		}
	}
}
