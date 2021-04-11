using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace CenterHexes
{
	public class Tiles
	{
		public const float HEXHEIGHT = 52;
		public const float HEXWIDTH = HEXHEIGHT * 0.8695f;
		public const float VERTICALOFFSET = HEXHEIGHT * 3.0f / 4.0f;
		public const float C = HEXHEIGHT - VERTICALOFFSET;
		public const float M = C / (HEXWIDTH / 2);

		List<Tile> _tiles;
		Texture2D _circle;
		Texture2D _rect;
		Random _r;
		Rectangle _area;

		public Tiles(ContentManager content, GraphicsDevice graphicsDevice)
		{
			_circle = content.Load<Texture2D>("circ");
			_rect = new Texture2D(graphicsDevice, 1, 1);
			Color[] colors = new Color[] { Color.Red };
			_rect.SetData(colors);

			_r = new Random();

			_tiles = new List<Tile>();

			_tiles.Add(new Tile(1, 1));
			_tiles.Add(new Tile(4, 4));

			_area = GetBoundingRectangle(_tiles);
		}

		/// <summary>
		/// Takes a list of positions on a hexagonal grid and returns a bounding rectangle
		/// </summary>
		public Rectangle GetBoundingRectangle(List<Tile> tiles)
		{
			int westMost = int.MaxValue;
			int eastMost = int.MinValue;
			int northMost = int.MinValue;
			int southMost = int.MaxValue;
			
			float height = 0;
			float width = 0;

			float offsetWest = HEXWIDTH / 2;
			float offsetEast = 0;

			foreach (Tile tile in tiles)
			{
				if (tile.Position.X >= eastMost)
				{
					if (tile.Position.X > eastMost)
						offsetEast = 0;

					eastMost = tile.Position.X;

					if (tile.Position.Y % 2 != 0 && offsetEast == 0)
						offsetEast = HEXWIDTH/2;
				}

				if (tile.Position.X <= westMost)
				{
					if (tile.Position.X < westMost)
						offsetWest = HEXWIDTH / 2;

					westMost = tile.Position.X;

					if (Math.Abs(tile.Position.Y) % 2 != 1 && offsetWest == HEXWIDTH / 2)
						offsetWest = 0;
				}

				if (tile.Position.Y >= northMost)
					northMost = tile.Position.Y;

				if (tile.Position.Y <= southMost)
					southMost = tile.Position.Y;
			}

			height += (((southMost*-1) + northMost) * VERTICALOFFSET + VERTICALOFFSET + C);
			width += (((westMost*-1) + eastMost) * HEXWIDTH + HEXWIDTH) + offsetEast - offsetWest;

			Vector2 pos = new Vector2(-(HEXWIDTH / 2), -(HEXHEIGHT / 2));
			pos.Y += southMost * VERTICALOFFSET;
			pos.X += westMost * HEXWIDTH + offsetWest;

			return new Rectangle(pos.ToPoint().X, pos.ToPoint().Y, (int)width, (int)height);
		}

		//Generate a random set of tiles
		public void Next()
		{
			int size = _r.Next(1, 6);
			_tiles.Clear();
			for (int i = 0; i < 15; i++)
				_tiles.Add(new Tile(_r.Next(-size, size), _r.Next(-size, size)));
			_area = GetBoundingRectangle(_tiles);

		}

		public void Draw (SpriteBatch spriteBatch)
		{

			for (int y = - 4; y <= 4; y++)
				for (int x = -4; x <= 4; x++)
					spriteBatch.Draw(_circle, GridToPixel(new Position(x, y)) - new Vector2(HEXWIDTH / 2, HEXHEIGHT / 2), Color.Gray);

			spriteBatch.Draw(_rect, _area, null, new Color(1,1,1,0.1f));


			foreach (Tile tile in _tiles)
				spriteBatch.Draw(_circle, GridToPixel(tile.Position)-new Vector2(HEXWIDTH/2, HEXHEIGHT/2), Color.White);

			spriteBatch.Draw(_circle, GridToPixel(new Position(0, 0)) - new Vector2(HEXWIDTH / 2, HEXHEIGHT / 2), new Color(0,1,0,0.1f));

		}

		internal static Position PointToGridPosition(float x, float y)
		{
			if (float.IsNaN(x) || float.IsNaN(y))
				return null;
			// Find the row and column of the box that the point falls in.
			float Y = (y / VERTICALOFFSET);
			Y = (float)Math.Round((decimal)(Y - C / 2), 0);
			float X = (x / HEXWIDTH);

			//If odd row
			if (Y % 2 != 0)
				X -= 0.5f;

			X = (float)Math.Round((decimal)(X), 0);

			float relY = y - (Y * VERTICALOFFSET);
			float relX;

			if (Y % 2 != 0)
				relX = (x - (X * HEXWIDTH)) - HEXWIDTH / 2;
			else
				relX = x - (X * HEXWIDTH);

			// Work out if the point is above either of the hexagon's top edges
			if (relY > (M * relX) + (C * 2)) // Left edge
			{
				Y++;
				if ((Y % 2 != 0))
					X--;
			}

			if (relY > (-M * relX) + (C * 2)) // Right edge
			{
				Y++;
				X++;
				if ((Y % 2 != 0))
					X--;
			}

			return new Position((short)X, (short)Y);
		}

		static public Vector2 GridToPixel(Position position)
		{
			if (position == null)
				return new Vector2(0, 0);

			float startX;
			float startY;

			if (position.Y % 2 != 0)
				startX = HEXWIDTH / 2.0f;
			else
				startX = 0;

			startY = position.Y * VERTICALOFFSET;
			return new Vector2(startX + position.X * HEXWIDTH, startY);
		}
	}

	public class Tile
	{
		public Tile(int x, int y)
		{
			Position = new Position(x, y);
		}

		public Position Position;
	}

	public class Position
	{
		public Position(int x, int y)
		{
			X = x;
			Y = y;
		}
		public int X;
		public int Y;
	}
}
