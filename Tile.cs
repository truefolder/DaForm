using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProgGame
{
	public enum CollisionType
	{
		Air,
		Solid,
		FakeSolid
	}

	public enum TileType
	{
		None,
		Static,
		Entity
	}
	public struct Tile
	{
		public Texture2D texture;
		public CollisionType collisionType;
		public TileType tileType;

		public const int width = 32;
		public const int height = 32;
		public static readonly Vector2 size = new Vector2(width, height);

		public Tile(Texture2D texture, CollisionType collisionType, TileType tileType)
		{
			this.texture = texture;
			this.collisionType = collisionType;
			this.tileType = tileType;
		}
	}
}
