using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgGame
{
	public enum Direction
	{
		Left = -90,
		Right = 90,
		Up = 0,
		Down = 180
	}

	public class Spike
	{
		private GameLevel level;
		private Vector2 position;
		private Texture2D texture;
		private Direction direction;

		public Rectangle localBounds;
		public Rectangle BoundingRectangle
		{
			get
			{
				int left = (int)Math.Round(position.X - (texture.Width / 2)) + localBounds.X;
				int top = (int)Math.Round(position.Y - texture.Height) + localBounds.Y;

				return new Rectangle(left, top, localBounds.Width, localBounds.Height);
			}
		}

		public Spike(GameLevel level, Direction direction, Vector2 position)
		{
			this.direction = direction;
			this.level = level;
			this.position = position;
			texture = level.content.Load<Texture2D>("tiles/spike");
			int width = (int)(texture.Width * 0.8f);
			int left = (texture.Width - width) / 2;
			int height = (int)(texture.Width * 0.8f);
			int top = texture.Height - height;
			localBounds = new Rectangle(left, top, width, height);
		}
		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, position, null, Color.White, MathHelper.ToRadians((float)direction), Extentions.GetOriginByRotation(direction, texture), 1.0f, SpriteEffects.None, 0.0f);
		}
	}
}
