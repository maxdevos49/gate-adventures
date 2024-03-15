using System;
using GateAdventures.Engine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GateAdventures;

// Example scene
public class HelloScene : Scene
{
	private Vector2 _ballVelocity;
	private Vector2 _ballPosition;
	private Texture2D _ballTexture;
	private MouseState _previousMouseState;

	public override void Initialize(GameServiceContainer services)
	{
		base.Initialize(services);

		_ballVelocity = Vector2.Zero;
		_ballPosition = GraphicsDevice.Viewport.Bounds.Center.ToVector2();
		_previousMouseState = Mouse.GetState();
	}

	public override void LoadContent()
	{
		_ballTexture = Content.Load<Texture2D>("ball");
	}

	public override void HandleInput()
	{
		MouseState currentMouseState = Mouse.GetState();
		Vector2 mousePosition = new(currentMouseState.X, currentMouseState.Y);

		// Calculate steering force with maximum magnitude of 4 in 1 frame
		Vector2 desiredDirection = Vector2.Subtract(mousePosition, _ballPosition);
		float desiredMagnitude = Math.Min(Vector2.Distance(mousePosition, _ballPosition), 4);
		desiredDirection.Normalize();
		desiredDirection = Vector2.Multiply(desiredDirection, desiredMagnitude);

		// Apply steering force
		_ballVelocity += desiredDirection;

		// Limit velocity to a maximum magnitude of 50
		float velocityMagnitude = Math.Min(Vector2.Distance(Vector2.Zero, _ballVelocity), 25);
		_ballVelocity.Normalize();
		_ballVelocity = Vector2.Multiply(_ballVelocity, velocityMagnitude);

		// Apply friction
		_ballVelocity = Vector2.Multiply(_ballVelocity, 0.9f);

		if (_previousMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released)
		{
			SceneManager.Start(new WorldScene());
			_previousMouseState = currentMouseState;

			return;
		}

		_previousMouseState = currentMouseState;
	}

	public override void Update(GameTime gameTime)
	{
		_ballPosition = Vector2.Add(_ballPosition, _ballVelocity);

		// Horizontal bounds.
		Rectangle bounds = GraphicsDevice.Viewport.Bounds;
		if (_ballPosition.X < 0)
		{
			_ballPosition.X = 0;
		}
		else if (_ballPosition.X + 16 > bounds.Width)
		{
			_ballPosition.X = bounds.Width;
		}

		// Vertical bounds.
		if (_ballPosition.Y < 0)
		{
			_ballPosition.Y = 0;
		}
		else if (_ballPosition.Y + 16 > bounds.Height)
		{
			_ballPosition.Y = bounds.Height;
		}
	}

	public override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.CornflowerBlue);
		SpriteBatch.Begin();

		SpriteBatch.Draw(_ballTexture, Vector2.Subtract(_ballPosition, new Vector2(16, 16)), new Rectangle(0, 0, 32, 32), Color.White);

		SpriteBatch.End();
	}
}
