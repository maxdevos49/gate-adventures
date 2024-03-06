using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GateAdventures;

public class Game1 : Game
{
	private GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;
	private Texture2D _ballTexture;
	private Texture2D _brickTexture;
	private Texture2D _paddleTexture;

	private BrickBreaker _brickBreaker;

	public Game1()
	{
		_graphics = new GraphicsDeviceManager(this)
		{
			GraphicsProfile = GraphicsProfile.HiDef
		};
		Content.RootDirectory = "Content";

		IsMouseVisible = true;
		Window.AllowUserResizing = true;
		IsFixedTimeStep = true;
		TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d); //30fps
	}

	protected override void Initialize()
	{
		_brickBreaker = new BrickBreaker(GraphicsDevice.Viewport.Bounds);

		base.Initialize();
	}

	protected override void LoadContent()
	{
		_spriteBatch = new SpriteBatch(GraphicsDevice);

		_ballTexture = Content.Load<Texture2D>("ball");
		_brickTexture = Content.Load<Texture2D>("brick");
		_paddleTexture = Content.Load<Texture2D>("paddle");
	}

	protected override void Update(GameTime gameTime)
	{
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			Exit();

		_brickBreaker.Simulate();

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.CornflowerBlue);

		_spriteBatch.Begin();

		foreach (var brick in _brickBreaker.Bricks)
		{
			_spriteBatch.Draw(_brickTexture, brick, Color.White);
		}

		_spriteBatch.Draw(_ballTexture, _brickBreaker.Ball, Color.White);
		_spriteBatch.Draw(_paddleTexture, _brickBreaker.Paddle, Color.White);

		_spriteBatch.End();

		base.Draw(gameTime);
	}
}

class BrickBreaker
{
	private readonly Rectangle _viewport;
	private readonly List<Rectangle> _bricks;
	private Rectangle _ball;
	private Vector2 _ballVelocity;
	private Rectangle _paddle;

	public readonly int BrickWidth = 64;
	public readonly int BrickHeight = 16;
	public readonly int BrickPadding = 5;
	public readonly int BallDiameter = 16;
	public readonly int PaddleWidth = 128;
	public readonly int PaddleHeight = 16;

	public IList<Rectangle> Bricks { get => _bricks.AsReadOnly(); }
	public Rectangle Ball { get => _ball; }
	public Rectangle Paddle { get => _paddle; }

	public BrickBreaker(Rectangle viewport)
	{
		_viewport = viewport;

		_bricks = new List<Rectangle>();
		int paddingTop = 32;
		int cols = viewport.Width / (BrickWidth + BrickPadding);
		int paddingLeft = viewport.Width - (BrickWidth + BrickPadding) * cols - BrickPadding;
		int rows = viewport.Height / (BrickHeight + BrickPadding + paddingTop);
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				_bricks.Add(new Rectangle(
					paddingLeft + ((BrickWidth + BrickPadding / 2) * j), //x position
					paddingTop + ((BrickHeight + BrickPadding / 2) * i), // y position
					BrickWidth,// width
					BrickHeight// height
				));
			}
		}

		_ball = new Rectangle(viewport.Width / 2 - BallDiameter / 2, viewport.Height - BallDiameter * 2 - 1, BallDiameter, BallDiameter);
		int scale = 5;
		Random rand = new();
		Vector2 randomDirection = new(rand.NextSingle(), -1);
		randomDirection.Normalize();
		_ballVelocity = randomDirection * scale;
		_paddle = new Rectangle(viewport.Width / 2 - PaddleWidth / 2, viewport.Height - PaddleHeight - 1, PaddleWidth, PaddleHeight);
	}

	public void Simulate()
	{
		if (Keyboard.GetState().IsKeyDown(Keys.Right))
		{
			_paddle.X += 3;

			if (_paddle.X + _paddle.Width > _viewport.Width)
			{
				_paddle.X = _viewport.Width - _paddle.Width - 1;
			}
		}
		else if (Keyboard.GetState().IsKeyDown(Keys.Left))
		{
			_paddle.X -= 3;

			if (_paddle.X < 0)
			{
				_paddle.X = 1;
			}
		}

		_ball.X += (int)_ballVelocity.X;
		_ball.Y += (int)_ballVelocity.Y;

		if (_ball.X < 0)
		{
			_ball.X = 0;
			_ballVelocity.X = -_ballVelocity.X;
		}
		else if (_ball.X + BallDiameter > _viewport.Width)
		{
			_ball.X = _viewport.Width - BallDiameter;
			_ballVelocity.X = -_ballVelocity.X;
		}

		if (_ball.Y < 0)
		{
			_ball.Y = 0;
			_ballVelocity.Y = -_ballVelocity.Y;
		}
		else if (_ball.Y + BallDiameter > _viewport.Height)
		{
			_ball = new Rectangle(_viewport.Width / 2 - BallDiameter / 2, _viewport.Height - BallDiameter * 2 - 1, BallDiameter, BallDiameter);
			int scale = 5;
			Random rand = new();
			Vector2 randomDirection = new(rand.NextSingle(), -1);
			randomDirection.Normalize();
			_ballVelocity = randomDirection * scale;
			_paddle = new Rectangle(_viewport.Width / 2 - PaddleWidth / 2, _viewport.Height - PaddleHeight - 1, PaddleWidth, PaddleHeight);
		}

		for (int i = _bricks.Count - 1; i >= 0; i--)
		{
			Rectangle brick = _bricks[i];

			if (_ball.Intersects(brick))
			{
				_bricks.Remove(brick);
				if (brick.IntersectionPlane(_ball) == RectangleExtension.Plane.Vertical)
				{
					_ballVelocity.X = -_ballVelocity.X;
				}
				else
				{
					_ballVelocity.Y = -_ballVelocity.Y;
				}

				break;
			}
		}

		if (_paddle.Intersects(_ball))
		{
			_ball.Y = _paddle.Y - BallDiameter;

			int degrees = -(_ball.X + _paddle.Width / 2 - (_paddle.X + _paddle.Width / 2) - _paddle.Width / 2);
			Matrix rot = Matrix.CreateRotationZ((float)(degrees * (Math.PI / 180)));
			_ballVelocity = Vector2.Transform(Vector2.UnitY, rot);
			_ballVelocity.X *= 5;
			_ballVelocity.Y *= -5;
		}

	}

}

public static class RectangleExtension
{
	public enum Side
	{
		None,
		Top,
		Right,
		Bottom,
		Left
	}

	public enum Plane
	{
		None,
		Vertical,
		Horizontal
	}

	/// <summary>
	/// Indicates the intersection side of rect1 by rect2.
	/// </summary>
	/// <param name="rect1"></param>
	/// <param name="rect2"></param>
	/// <returns></returns>
	public static Side IntersectionSide(this Rectangle r1, Rectangle r2)
	{
		var dx = r1.X + r1.Width / 2 - (r2.X + r2.Width / 2);
		var dy = r1.Y + r1.Height / 2 - (r2.Y + r2.Height / 2);
		var width = (r1.Width + r2.Width) / 2;
		var height = (r1.Height + r2.Height) / 2;
		var crossWidth = width * dy;
		var crossHeight = height * dx;
		var collision = Side.None;
		if (Math.Abs(dx) <= width && Math.Abs(dy) <= height)
		{
			if (crossWidth > crossHeight)
			{
				collision = (crossWidth > (-crossHeight)) ? Side.Bottom : Side.Left;
			}
			else
			{
				collision = (crossWidth > -crossHeight) ? Side.Right : Side.Top;
			}
		}
		return collision;
	}

	public static Plane IntersectionPlane(this Rectangle rect1, Rectangle rect2)
	{
		var side = rect1.IntersectionSide(rect2);

		return side switch
		{
			Side.None => Plane.None,
			Side.Top => Plane.Horizontal,
			Side.Right => Plane.Vertical,
			Side.Bottom => Plane.Horizontal,
			Side.Left => Plane.Vertical,
			_ => throw new ArgumentOutOfRangeException(nameof(side), $"Not expected direction value: {side}"),
		};
	}

}