using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgGame
{
	public class MovingSpike
	{
		private float moveSpeed = 7f;
		private bool isMoving = false;
		public bool isAlive = true;
		private Vector2 startTileIndex;
		private GameLevel level;
		private Vector2 position;
		private Texture2D texture;
		private Direction direction;
		public Rectangle TriggerRect
		{
			get
			{
				int left;
				int top;
				switch (direction)
				{
					case Direction.Left:
						left = 0;
						top = (int)Math.Round(position.Y - texture.Height) + localBounds.Y;
						return new Rectangle(left, top, (int)startTileIndex.X * Tile.width, localBounds.Height);

					case Direction.Right:
						left = (int)startTileIndex.X * Tile.width;
						top = (int)Math.Round(position.Y - texture.Height) + localBounds.Y;
						return new Rectangle(left, top, (level.Width - (int)startTileIndex.X) * Tile.width, localBounds.Height);

					case Direction.Up:
						left = (int)Math.Round(position.X - (texture.Width / 2)) + localBounds.X;
						top = 0;
						return new Rectangle(left, top, localBounds.Width, (int)startTileIndex.Y * Tile.height);

					case Direction.Down:
						left = (int)Math.Round(position.X - (texture.Width / 2)) + localBounds.X;
						top = (int)startTileIndex.Y * Tile.height;
						return new Rectangle(left, top, localBounds.Width, (level.Height - (int)startTileIndex.Y) * Tile.height);
				}
				throw new Exception();
			}
		}

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

		public MovingSpike(GameLevel level, Direction direction, Vector2 position, Vector2 startIndex)
		{
			this.direction = direction;
			this.level = level;
			this.position = position;
			texture = level.content.Load<Texture2D>("tiles/spike");
			startTileIndex = startIndex;
			int width = (int)(texture.Width * 0.8f);
			int left = (texture.Width - width) / 2;
			int height = (int)(texture.Width * 0.8f);
			int top = texture.Height - height;
			localBounds = new Rectangle(left, top, width, height);
		}

		public void TriggerMove()
		{
			isMoving = true;
		}

		public Vector2 Move()
		{
			if (!isAlive)
				return position;
			switch (direction)
			{
				case Direction.Up:
					return new Vector2(position.X, position.Y - moveSpeed);
				case Direction.Down:
					return new Vector2(position.X, position.Y + moveSpeed);
				case Direction.Left:
					return new Vector2(position.X - moveSpeed, position.Y);
				case Direction.Right:
					return new Vector2(position.X + moveSpeed, position.Y);
			}
			return position;
		}

		public void Update()
		{
			if (isMoving)
				position = Move();

			if (level.CheckForSolidCollision(BoundingRectangle))
				isAlive = false;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (isAlive)
				spriteBatch.Draw(texture, position, null, Color.White, MathHelper.ToRadians((float)direction), Extentions.GetOriginByRotation(direction, texture), 1.0f, SpriteEffects.None, 0.0f);
		}
	}
}
