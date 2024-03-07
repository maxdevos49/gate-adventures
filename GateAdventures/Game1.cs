using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GateAdventures;

public class Game1 : Game
{
	private GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;
	private Texture2D _ballTexture;
	private Texture2D _brickTexture;
	private Texture2D _paddleTexture;
	private Texture2D _heartTexture;
	private Texture2D _gameOverTexture;

	private BrickBreaker _brickBreaker;

	public Game1()
	{
		_graphics = new GraphicsDeviceManager(this)
		{
			GraphicsProfile = GraphicsProfile.HiDef
		};
		Content.RootDirectory = "Content";

		IsMouseVisible = true;
		IsFixedTimeStep = true;
		TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);
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
		_heartTexture = Content.Load<Texture2D>("heart");
		_gameOverTexture = Content.Load<Texture2D>("gameover");
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
		Color background = new(Color.White, 0);

		GraphicsDevice.Clear(background);

		_spriteBatch.Begin();

		foreach (var brick in _brickBreaker.Bricks)
		{
			if (brick.Hardness == 1)
			{
				_spriteBatch.Draw(_paddleTexture, brick.Bounds, Color.White);
			}
			else
			{
				_spriteBatch.Draw(_brickTexture, brick.Bounds, Color.White);
			}
		}

		_spriteBatch.Draw(_ballTexture, _brickBreaker.Ball, Color.White);
		_spriteBatch.Draw(_paddleTexture, _brickBreaker.Paddle, Color.White);

		for (int i = 0; i < _brickBreaker.Lives; i++)
		{
			int x = i * 16;
			_spriteBatch.Draw(_heartTexture, new Rectangle(x, 0, 16, 16), Color.White);
		}

		if (_brickBreaker.Lives <= 0)
		{
			int x = GraphicsDevice.Viewport.Bounds.Width / 2 - 64;
			int y = GraphicsDevice.Viewport.Bounds.Height / 2 - 32;
			_spriteBatch.Draw(_gameOverTexture, new Rectangle(x, y, 128, 64), Color.White);
		}

		_spriteBatch.End();


		base.Draw(gameTime);
	}
}

class BrickBreaker
{
	private readonly Random _rand;
	private readonly Rectangle _viewport;
	private readonly List<Brick> _bricks;
	private Rectangle _ball;
	private Vector2 _ballVelocity;
	private Rectangle _paddle;

	public readonly int BrickPadding = 10;
	public readonly int BallSpeed = 5;
	public readonly int BallDiameter = 16;
	public readonly int PaddleWidth = 128;
	public readonly int PaddleHeight = 16;

	public IList<Brick> Bricks { get => _bricks.AsReadOnly(); }
	public Rectangle Ball { get => _ball; }
	public Rectangle Paddle { get => _paddle; }
	public int Lives { get; private set; }

	public BrickBreaker(Rectangle viewport)
	{
		_rand = new Random();
		_viewport = viewport;
		_bricks = new List<Brick>();
		Lives = 5;

		int paddingTop = 64;

		int cols = viewport.Width / (Brick.Width + BrickPadding);
		int rows = viewport.Height / (Brick.Height + BrickPadding + paddingTop);
		int paddingLeft = viewport.Width - (Brick.Width + BrickPadding) * cols - BrickPadding;

		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				int x = paddingLeft + ((Brick.Width + BrickPadding / 2) * j);
				int y = paddingTop + ((Brick.Height + BrickPadding / 2) * i);

				_bricks.Add(new Brick(_rand.Next(1, 3), x, y));
			}
		}

		ResetBall();
		ResetPaddle();
	}

	public void Simulate()
	{
		if (Lives <= 0)
		{
			return;
		}

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

		// Left and right bounds.
		if (_ball.X < 0 || _ball.X + BallDiameter > _viewport.Width)
		{
			ReflectBall(RectangleExtension.Plane.Vertical);
			if (_ball.X < 0)
			{
				_ball.X = 0;
			}
			else
			{
				_ball.X = _viewport.Width - BallDiameter;
			}
		}

		// Top Bounds.
		if (_ball.Y < 0)
		{
			ReflectBall(RectangleExtension.Plane.Horizontal);
			_ball.Y = 0;
		}

		// Bottom bounds.
		if (_ball.Y + BallDiameter > _viewport.Height)
		{
			ResetBall();
			ResetPaddle();
			Lives--;
		}

		// Brick collisions
		for (int i = _bricks.Count - 1; i >= 0; i--)
		{
			Brick brick = _bricks[i];

			if (_ball.Intersects(brick.Bounds))
			{
				brick.Collide();

				if (brick.IsBroken)
				{
					_bricks.Remove(_bricks[i]);
				}

				ReflectBall(brick.Bounds.IntersectionPlane(_ball));
				break;
			}
		}

		// Paddle collisions
		if (_paddle.Intersects(_ball))
		{
			_ball.Y = _paddle.Y - BallDiameter;
			ReflectBallOnPaddle();
		}

	}

	private void ResetBall()
	{
		_ball = new Rectangle(_viewport.Width / 2 - BallDiameter / 2, _viewport.Height - BallDiameter * 2 - 1, BallDiameter, BallDiameter);

		int degrees = _rand.Next(-64, 64);
		Matrix rot = Matrix.CreateRotationZ((float)(degrees * (Math.PI / 180)));
		_ballVelocity = Vector2.Transform(Vector2.UnitY, rot);
		_ballVelocity.X *= BallSpeed;
		_ballVelocity.Y *= -BallSpeed;
	}

	private void ReflectBall(RectangleExtension.Plane plane)
	{
		if (plane == RectangleExtension.Plane.Vertical)
		{
			_ballVelocity.X = -_ballVelocity.X;
		}
		else if (plane == RectangleExtension.Plane.Horizontal)
		{
			_ballVelocity.Y = -_ballVelocity.Y;
		}
	}

	private void ReflectBallOnPaddle()
	{
		int degrees = -(_ball.X + _paddle.Width / 2 - (_paddle.X + _paddle.Width / 2) - _paddle.Width / 2);
		degrees = Math.Min(Math.Max(degrees, -64), 64);
		Matrix rot = Matrix.CreateRotationZ((float)(degrees * (Math.PI / 180)));
		Vector2 normalizedVelocity = Vector2.Transform(Vector2.UnitY, rot);
		_ballVelocity = Vector2.Multiply(normalizedVelocity, BallSpeed);
		_ballVelocity.Y = -_ballVelocity.Y;
	}

	private void ResetPaddle()
	{
		_paddle = new Rectangle(_viewport.Width / 2 - PaddleWidth / 2, _viewport.Height - PaddleHeight - 1, PaddleWidth, PaddleHeight);
	}
}

public class Brick
{
	public static readonly int Width = 64;
	public static readonly int Height = 16;
	public Rectangle Bounds { get; private set; }
	public int Hardness { get; private set; }
	public bool IsBroken { get => Hardness <= 0; }

	public Brick(int hardness, int x, int y)
	{
		Hardness = hardness;
		Bounds = new Rectangle(x, y, Width, Height);
	}

	public void Collide()
	{
		Hardness--;
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
				collision = (crossWidth > -crossHeight) ? Side.Bottom : Side.Left;
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