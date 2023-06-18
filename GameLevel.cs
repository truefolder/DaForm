using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgGame
{
	public class GameLevel
	{
		public Camera camera;
		public Tile[,] tiles;
		public int Width
		{
			get { return tiles.GetLength(0); }
		}

		public int Height
		{
			get { return tiles.GetLength(1); }
		}

		public Player player;
		private List<Spike> spikes = new();
		private List<MovingPlatform> movingPlatforms = new();
		private List<Cannon> cannons = new();
		private List<MovingSpike> movingSpikes = new();
		private List<BackgroundLayer> bgLayers = new();

		public Vector2 startPosition;
		private Point endPoint;
		public bool levelEnded = false;
		public ContentManager content;

		public GameLevel(IServiceProvider serviceProvider, Stream fileStream, GraphicsDevice device)
		{
			content = new ContentManager(serviceProvider, "Content");
			camera = new Camera(device);
			camera.Zoom = 2f;
			LoadLevel(fileStream);

			bgLayers.Add(new BackgroundLayer(content, "bg/hills", 0.02f));
			bgLayers.Add(new BackgroundLayer(content, "bg/clouds1", 0.03f));
			bgLayers.Add(new BackgroundLayer(content, "bg/clouds2", 0.04f));
			bgLayers.Add(new BackgroundLayer(content, "bg/foreground", 0.1f));
		}

		public void ProcessCamera(GameTime gameTime)
		{
			camera.Position = Vector2.Lerp(camera.Position, player.position, 10 * (float)gameTime.ElapsedGameTime.TotalSeconds);
		}

		public void LoadLevel(Stream fileStream)
		{
			List<string> lines = new();
			int width;
			using (StreamReader reader = new StreamReader(fileStream))
			{
				string line = reader.ReadLine();
				width = line.Length;
				while (line != null)
				{
					lines.Add(line);
					line = reader.ReadLine();
				}
			}
			tiles = new Tile[width, lines.Count];

			for (int y = 0; y < Height; ++y)
			{
				for (int x = 0; x < Width; ++x)
				{
					char tile = lines[y][x];
					tiles[x, y] = ParseTileFromChar(tile, x, y);
				}
			}
			SetWallTextures();
		}

		public Tile ParseTileFromChar(char tile, int x, int y)
		{
			switch (tile)
			{
				case '.':
					return new Tile(null, CollisionType.Air, TileType.None);
				case '#':
					return new Tile(content.Load<Texture2D>("tiles/wall"), CollisionType.Solid, TileType.Static);
				case 'X':
					return new Tile(content.Load<Texture2D>("tiles/wall"), CollisionType.FakeSolid, TileType.Static);
				case 'S':
					return InstantiateStartTile(x, y);
				case '^':
					return InstantiateSpike(x, y, Direction.Up);
				case '>':
					return InstantiateSpike(x, y, Direction.Right);
				case '<':
					return InstantiateSpike(x, y, Direction.Left);
				case 'v':
					return InstantiateSpike(x, y, Direction.Down);
				case '-':
					return InstantiateMovingPlatform(x, y);
				case 'E':
					return InstantiateEndTile(x, y);
				case '{':
					return InstantiateCannonTile(x, y, Direction.Left);
				case '}':
					return InstantiateCannonTile(x, y, Direction.Right);
				case '!':
					return InstantiateCannonTile(x, y, Direction.Down);
				case 'i':
					return InstantiateCannonTile(x, y, Direction.Up);
				case '(':
					return InstantiateMovingSpike(x, y, Direction.Left);
				case ')':
					return InstantiateMovingSpike(x, y, Direction.Right);
				case 'q':
					return InstantiateMovingSpike(x, y, Direction.Down);
				case '`':
					return InstantiateMovingSpike(x, y, Direction.Up);
			}
			throw new Exception("unknown tile");
		}

		public void SetWallTextures()
		{
			for (int y = 0; y < Height; ++y)
			{
				for (int x = 0; x < Width; ++x)
				{
					if (tiles[x, y].texture == null)
						continue;
					if (!tiles[x, y].texture.Name.Contains("wall"))
						continue;
					if (y - 1 < 0)
						continue;

					if (tiles[x, y - 1].texture == null)
					{
						SetRandomDecoration(x, y - 1);
						tiles[x, y].texture = content.Load<Texture2D>("tiles/grasswall");
					}
					else if (!tiles[x, y - 1].texture.Name.Contains("wall"))
						tiles[x, y].texture = content.Load<Texture2D>("tiles/grasswall");
				}
			}
		}

		public void SetRandomDecoration(int x, int y)
		{
			if (tiles[x, y].tileType != TileType.None || tiles[x, y + 1].collisionType == CollisionType.FakeSolid)
				return;

			Random random = new Random();
			var rnd = random.Next(100);
			if (rnd <= 45)
			{
				rnd = random.Next(1, 5);
				tiles[x, y].texture = content.Load<Texture2D>($"tiles/flower{rnd}");
			}
			else if (rnd <= 60)
			{
				tiles[x, y].texture = content.Load<Texture2D>($"tiles/bush");
			}
		}

		public Tile InstantiateEndTile(int x, int y)
		{
			endPoint = GetTileBounds(x, y).Center;
			return new Tile(content.Load<Texture2D>("tiles/end"), CollisionType.Air, TileType.Static);
		}

		public Tile InstantiateMovingSpike(int x, int y, Direction direction)
		{
			MovingSpike spike = new MovingSpike(this, direction, Extentions.GetBottomCenter(GetTileBounds(x, y)), new Vector2(x, y));
			movingSpikes.Add(spike);
			return new Tile(null, CollisionType.Air, TileType.Entity);
		}

		public Tile InstantiateMovingPlatform(int x, int y)
		{
			MovingPlatform movingPlatform = new MovingPlatform(this, Extentions.GetBottomCenter(GetTileBounds(x, y)));
			movingPlatforms.Add(movingPlatform);
			return new Tile(null, CollisionType.Air, TileType.Entity);
		}

		public Tile InstantiateSpike(int x, int y, Direction direction)
		{
			Spike spike = new Spike(this, direction, Extentions.GetBottomCenter(GetTileBounds(x, y)));
			spikes.Add(spike);
			return new Tile(null, CollisionType.Air, TileType.Entity);
		}

		public Tile InstantiateStartTile(int x, int y)
		{
			startPosition = Extentions.GetBottomCenter(GetTileBounds(x, y));
			player = new Player(this, startPosition);
			camera.Position = startPosition;
			return new Tile(null, CollisionType.Air, TileType.Entity);
		}

		public Tile InstantiateCannonTile(int x, int y, Direction direction)
		{
			Cannon cannon = new Cannon(this, direction, Extentions.GetBottomCenter(GetTileBounds(x, y)));
			cannons.Add(cannon);
			return new Tile(null, CollisionType.Air, TileType.Entity);
		}

		public CollisionType GetCollisionType(int x, int y)
		{
			if (x < 0 || x >= Width) // игрок пытается выйти за горизонтальные пределы игрового уровня
				return CollisionType.Solid;

			if (y < 0 || y >= Height) // игрок пытается выйти за вертикальные пределы игрового уровня
				return CollisionType.Air; // возвращаем воздух, чтобы игрок смог прыгать/падать за пределы уровня

			return tiles[x, y].collisionType;
		}

		public Rectangle GetTileBounds(int x, int y)
		{
			return new Rectangle(x * Tile.width, y * Tile.height, Tile.width, Tile.height);
		}

		public bool CheckForSolidCollision(Rectangle rectangle)
		{
			Rectangle bounds = rectangle;
			int leftTile = (int)Math.Floor((float)bounds.Left / Tile.width);
			int rightTile = (int)Math.Ceiling((float)bounds.Right / Tile.width) - 1;
			int topTile = (int)Math.Floor((float)bounds.Top / Tile.height);
			int bottomTile = (int)Math.Ceiling((float)bounds.Bottom / Tile.height) - 1; ;

			for (int y = topTile; y <= bottomTile; ++y)
			{
				for (int x = leftTile; x <= rightTile; ++x)
				{
					CollisionType collision = GetCollisionType(x, y);
					if (collision == CollisionType.Solid)
					{
						Rectangle tileBounds = GetTileBounds(x, y);
						Vector2 depth = Extentions.GetIntersectionDepth(bounds, tileBounds);
						if (depth != Vector2.Zero)
							return true;
					}
				}
			}
			return false;
		}

		public void Update(GameTime gameTime)
		{
			if (levelEnded)
				return;

			player.Update(gameTime);
			if (player.BoundingRectangle.Top >= Height * Tile.height)
				player.OnKilled();

			foreach (Spike spike in spikes)
			{
				if (spike.BoundingRectangle.Intersects(player.BoundingRectangle))
				{
					player.OnKilled();
					break;
				}
			}
			foreach (MovingSpike movingSpike in movingSpikes)
			{
				if (movingSpike.BoundingRectangle.Intersects(player.BoundingRectangle) && movingSpike.isAlive)
				{
					player.OnKilled();
					break;
				}
				if (movingSpike.TriggerRect.Intersects(player.BoundingRectangle))
					movingSpike.TriggerMove();

				movingSpike.Update();
			}
			foreach (MovingPlatform movingPlatform in movingPlatforms)
			{
				movingPlatform.Update();
				if (movingPlatform.Bounds.Intersects(player.BoundingRectangle))
				{
					movingPlatform.TriggerMove();
					player.onPlatform = true;
					break;
				}
				else
				{
					player.onPlatform = false;
					movingPlatform.UnTriggerMove();
				}
			}

			foreach (Cannon cannon in cannons)
			{
				cannon.Update();
				if (cannon.ProjectileBoundingRectangle.Intersects(player.BoundingRectangle))
				{
					player.OnKilled();
					break;
				}
			}

			if (player.BoundingRectangle.Contains(endPoint))
				levelEnded = true;

			ProcessCamera(gameTime);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			foreach (var bg in bgLayers)
				bg.Draw(spriteBatch, camera.Position);

			foreach (Spike spike in spikes)
				spike.Draw(spriteBatch);

			foreach (MovingPlatform movingPlatform in movingPlatforms)
				movingPlatform.Draw(spriteBatch);

			foreach (Cannon cannon in cannons)
				cannon.Draw(spriteBatch);

			foreach (MovingSpike movingSpike in movingSpikes)
				movingSpike.Draw(spriteBatch);

			for (int y = 0; y < Height; ++y)
			{
				for (int x = 0; x < Width; ++x)
				{
					Texture2D texture = tiles[x, y].texture;
					if (texture != null)
					{
						Vector2 position = new Vector2(x, y) * Tile.size;
						spriteBatch.Draw(texture, position, Color.White);
					}
				}
			}
			player.Draw(spriteBatch);
		}
	}
}
