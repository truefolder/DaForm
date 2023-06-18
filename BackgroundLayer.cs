using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;

namespace ProgGame
{
	public class BackgroundLayer
	{
        public Texture2D texture;
        public float scrollRate;

        public BackgroundLayer(ContentManager content, string path, float scrollRate)
        {
            texture = content.Load<Texture2D>(path);
            this.scrollRate = scrollRate;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            int segmentWidth = texture.Width;

            float x = cameraPosition.X * scrollRate;
            int leftSegment = (int)Math.Floor(x / segmentWidth);
            x = (x / segmentWidth - leftSegment) * -segmentWidth;
            var totalX = x;
            for (int i = 0; i < (spriteBatch.GraphicsDevice.Viewport.Width % segmentWidth) + 1; ++i)
			{
                totalX += segmentWidth;
                spriteBatch.Draw(texture, new Vector2(totalX, cameraPosition.Y - 208), Color.White);
            }
        }
    }
}
