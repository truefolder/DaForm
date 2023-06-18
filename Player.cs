using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProgGame
{
	public class Player
	{
		private const float MaxJumpTime = 0.4f;
		private const float JumpForce = -1500.0f;
		private const float GravityAcceleration = 3400.0f;
		private const float MaxVerticalSpeed = 400.0f;
		private const float HorizontalPowerInJump = 0.15f;

		private const float MoveAcceleration = 10000.0f;
		private const float MaxHorizontalSpeed = 1750.0f;
		private const float GroundDrag = 0.5f;
		private const float AirDrag = 0.57f;

		private Texture2D textureToDraw;
		private Texture2D normalTexture;
		private Texture2D deadTexture;
		public Vector2 position;
		public Vector2 velocity;
		private bool wantsJump;
		private bool wasJumping;
		public bool grounded;
		private float jumpTime;
		private float movement;
		private float previousBottom;
		public bool isAlive;
		public bool onPlatform;
		private GameLevel level;
		private Rectangle localBounds;
		private SpriteEffects spriteEffects;
		public Rectangle BoundingRectangle
		{
			get
			{
				int left = (int)Math.Round(position.X - (normalTexture.Height / 2)) + localBounds.X;
				int top = (int)Math.Round(position.Y - normalTexture.Height) + localBounds.Y;

				return new Rectangle(left, top, localBounds.Width, localBounds.Height);
			}
		}

		public Player(GameLevel level, Vector2 position)
		{
			this.position = position;
			this.level = level;
			normalTexture = level.content.Load<Texture2D>("player");
			deadTexture = level.content.Load<Texture2D>("player_dead");
			int width = (int)(normalTexture.Width * 0.4);
			int left = (normalTexture.Width - width) / 2;
			int height = (int)(normalTexture.Width * 0.8);
			int top = normalTexture.Height - height;
			localBounds = new Rectangle(left, top, width, height);
			wantsJump = false;
			wasJumping = false;
			isAlive = true;
			grounded = false;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (velocity.X > 0)
				spriteEffects = SpriteEffects.None;
			else if (velocity.X < 0)
				spriteEffects = SpriteEffects.FlipHorizontally;
			textureToDraw = isAlive ? normalTexture : deadTexture;
			spriteBatch.Draw(textureToDraw, position, null, Color.White, 0.0f, new Vector2(textureToDraw.Width / 2, textureToDraw.Height), 1.0f, spriteEffects, 0.0f);
		}

		public void Update(GameTime gameTime)
		{
			ProcessInput();

			DoPhysics(gameTime);

			movement = 0;
			wantsJump = false;
		}

		public void ProcessInput()
		{
			if (!isAlive)
				return;
			KeyboardState state = Keyboard.GetState();
			if (state.IsKeyDown(Keys.A) || state.IsKeyDown(Keys.Left))
				movement = -1.0f;
			if (state.IsKeyDown(Keys.D) || state.IsKeyDown(Keys.Right))
				movement = 1.0f;
			if (state.IsKeyDown(Keys.W) || state.IsKeyDown(Keys.Space) || state.IsKeyDown(Keys.Up))
				wantsJump = true;
		}

		public void OnKilled()
		{
			isAlive = false;
		}

		public void Respawn(Vector2 position)
		{
			isAlive = true;
			this.position = position;
			velocity = Vector2.Zero;
		}

		public void DoPhysics(GameTime gameTime)
		{
			Vector2 positionBefore = position;
			float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

			velocity.X += movement * MoveAcceleration * elapsed;
			velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxVerticalSpeed, MaxVerticalSpeed);
			velocity.Y = Jump(velocity.Y, gameTime);

			if (grounded)
				velocity.X *= GroundDrag;
			else
				velocity.X *= AirDrag;

			velocity.X = MathHelper.Clamp(velocity.X, -MaxHorizontalSpeed, MaxHorizontalSpeed);

			if (onPlatform)
			{
				position = new Vector2(position.X, position.Y - MovingPlatform.movingSpeed);
				if (!wantsJump || jumpTime == 0)
					velocity.Y = 0;
			}
			position += velocity * elapsed;
			position = new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));

			ProcessCollision();

			if (position.X == positionBefore.X)
				velocity.X = 0;

			if (position.Y == positionBefore.Y)
				velocity.Y = 0;
		}

		public float Jump(float velocity, GameTime gameTime)
		{
			if (wantsJump)
			{
				if ((!wasJumping && (grounded || onPlatform)) || jumpTime > 0.0f)
				{
					if (jumpTime == 0.0f)
					{
						// TODO: звук прыжка
					}
					jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
				}
				if (jumpTime > 0.0f && jumpTime <= MaxJumpTime)
				{
					velocity = JumpForce * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, HorizontalPowerInJump));
				}
				else
					jumpTime = 0;
			}
			else
				jumpTime = 0;
			wasJumping = wantsJump;
			return velocity;
		}

		private void ProcessCollision()
		{
			Rectangle bounds = BoundingRectangle;
			int leftTile = (int)Math.Floor((float)bounds.Left / Tile.width);
			int rightTile = (int)Math.Ceiling((float)bounds.Right / Tile.width) - 1;
			int topTile = (int)Math.Floor((float)bounds.Top / Tile.height);
			int bottomTile = (int)Math.Ceiling((float)bounds.Bottom / Tile.height) - 1;

			grounded = false;
			
			for (int y = topTile; y <= bottomTile; ++y)
			{
				for (int x = leftTile; x <= rightTile; ++x)
				{
					CollisionType collision = level.GetCollisionType(x, y);
					if (collision != CollisionType.Air)
					{
						Rectangle tileBounds = level.GetTileBounds(x, y);
						Vector2 depth = Extentions.GetIntersectionDepth(bounds, tileBounds);
						if (depth != Vector2.Zero)
						{
							if (collision == CollisionType.FakeSolid)
							{
								level.tiles[x, y].texture = null;
								continue;
							}
							float absDepthX = Math.Abs(depth.X);
							float absDepthY = Math.Abs(depth.Y);
							if (absDepthY < absDepthX)
							{
								if (previousBottom <= tileBounds.Top)
									grounded = true;

								if (collision == CollisionType.Solid || grounded)
								{
									position = new Vector2(position.X, position.Y + depth.Y);
									bounds = BoundingRectangle;
								}
							}
							else if (collision == CollisionType.Solid)
							{
								position = new Vector2(position.X + depth.X, position.Y);
								bounds = BoundingRectangle;
							}
						}
					}
				}
			}
			previousBottom = bounds.Bottom;
		}
	}
}
