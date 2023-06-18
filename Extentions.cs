using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgGame
{
	public static class Extentions
	{
        public static Vector2 GetIntersectionDepth(this Rectangle rectA, Rectangle rectB)
        {
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
            {
                return Vector2.Zero;
            }

            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;

            return new Vector2(depthX, depthY);
        }

        public static Vector2 GetBottomCenter(this Rectangle rect)
        {
            return new Vector2(rect.X + rect.Width / 2.0f, rect.Bottom);
        }

        public static Vector2 GetOriginByRotation(Direction direction, Texture2D texture)
		{
            switch (direction)
            {
                case Direction.Left:
                    return new Vector2(0, texture.Height / 2);
                case Direction.Right:
                    return new Vector2(texture.Width, texture.Height / 2);
                case Direction.Up:
                    return new Vector2(texture.Width / 2, texture.Height);
                case Direction.Down:
                    return new Vector2(texture.Width / 2, 0);
                default:
                    return Vector2.Zero;
            }
        }
        public static void DrawRectangle(Rectangle rectangle, SpriteBatch spriteBatch, Color color)
		{
            Texture2D _texture;
            _texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            _texture.SetData(new Color[] { Color.White });

            spriteBatch.Draw(_texture, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, 1), color);
            spriteBatch.Draw(_texture, new Rectangle(rectangle.Right, rectangle.Top, 1, rectangle.Height), color);
            spriteBatch.Draw(_texture, new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, 1), color);
            spriteBatch.Draw(_texture, new Rectangle(rectangle.Left, rectangle.Top, 1, rectangle.Height), color);
        }
    }
}
