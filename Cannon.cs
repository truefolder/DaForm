using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgGame
{
	public class Cannon
	{
		private GameLevel level;
		private float moveSpeed = 3;
		private Vector2 position;
		public Vector2 projectilePosition;
		private Texture2D texture;
		private Texture2D projectileTexture;
		private Direction direction;
		private bool projectileAlive = false;
		public Rectangle projectileLocalBounds;
	
		public Rectangle ProjectileBoundingRectangle
		{
			get
			{
				int left = (int)Math.Round(projectilePosition.X - (projectileTexture.Width / 2)) + projectileLocalBounds.X;
				int top = (int)Math.Round(projectilePosition.Y - projectileTexture.Height) + projectileLocalBounds.Y - 5;

				return new Rectangle(left, top, projectileLocalBounds.Width, projectileTexture.Height / 2);
			}
		}

		public Cannon(GameLevel level, Direction direction, Vector2 position)
		{
			this.direction = direction;
			this.level = level;
			this.position = position;
			texture = level.content.Load<Texture2D>("tiles/cannon");
			projectileTexture = level.content.Load<Texture2D>("tiles/cannon_projectile");
			int width = (int)(projectileTexture.Width * 0.6f);
			int left = (projectileTexture.Width - width) / 2;
			int height = (int)(projectileTexture.Width * 0.6f);
			int top = projectileTexture.Height - height;
			projectileLocalBounds = new Rectangle(left, top, width, height);
			SpawnNewProjectile();
		}

		public void Update()
		{
			if (projectileAlive)
			{
				projectilePosition = MoveProjectile();
				if (level.CheckForSolidCollision(ProjectileBoundingRectangle))
					projectileAlive = false;
				return;
			}
			SpawnNewProjectile();
		}

		public void SpawnNewProjectile()
		{
			projectilePosition = position;
			projectileAlive = true;
		}

		public Vector2 MoveProjectile()
		{
			switch (direction)
			{
				case Direction.Up:
					return new Vector2(projectilePosition.X, projectilePosition.Y - moveSpeed);
				case Direction.Down:
					return new Vector2(projectilePosition.X, projectilePosition.Y + moveSpeed);
				case Direction.Left:
					return new Vector2(projectilePosition.X - moveSpeed, projectilePosition.Y);
				case Direction.Right:
					return new Vector2(projectilePosition.X + moveSpeed, projectilePosition.Y);
			}
			return projectilePosition;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(projectileTexture, projectilePosition, null, Color.White, 0, new Vector2(texture.Height / 2, texture.Width), 1.0f, SpriteEffects.None, 0.0f);
			spriteBatch.Draw(texture, position, null, Color.White, 0, new Vector2(texture.Height / 2, texture.Width), 1.0f, SpriteEffects.None, 0.0f);
		}
	}
}
