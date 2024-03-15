using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GateAdventures.Engine;

public class Camera
{
	public float Zoom { get; set; }
	public Vector2 Position { get; set; }
	public Rectangle Bounds { get; protected set; }
	public Matrix Transform { get; protected set; }
	public Rectangle VisibleArea { get; protected set; }

	private float _currentMouseWheelValue, _previousMouseWheelValue;
	private Vector2 _oldPosition;
	private Matrix _oldTransform;

	public Camera(GameServiceContainer services)
	{
		GraphicsDeviceManager graphics = services.GetService<GraphicsDeviceManager>();
		Viewport viewport = graphics.GraphicsDevice.Viewport;

		Bounds = viewport.Bounds;
		Zoom = 1f;
		Position = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);

		Transform = CalculateTransformMatrix(Position, Bounds, Zoom);
		VisibleArea = CalculateVisibleArea(Transform, Bounds);
	}

	private void UpdateMatrix(Matrix transform, Rectangle visibleArea)
	{
		Transform = transform;
		VisibleArea = visibleArea;
	}

	public void MoveCamera(Vector2 movePosition)
	{
		Vector2 newPosition = Position + movePosition;
		Position = newPosition;
	}

	public void SetCameraPosition(Vector2 cameraPosition)
	{
		Position = cameraPosition;
	}

	public void AdjustZoom(float zoomAmount)
	{
		Zoom += zoomAmount;
		if (Zoom < .35f)
		{
			Zoom = .35f;
		}
		if (Zoom > 2f)
		{
			Zoom = 2f;
		}
	}

	public void UpdateCamera(GraphicsDeviceManager graphics)
	{
		_oldPosition = Position;
		_oldTransform = Transform;

		KeyboardState state = Keyboard.GetState();
		MouseState mouseState = Mouse.GetState();

		Vector2 cameraMovement = Vector2.Zero;
		float moveSpeed = 15 * (1 / Zoom);

		// Handle manual camera movement
		if (state.IsKeyDown(Keys.Up) || getMouseScreenPosition().Y <= 3)
		{
			cameraMovement.Y = -moveSpeed;
		}

		if (state.IsKeyDown(Keys.Down) || getMouseScreenPosition().Y >= graphics.PreferredBackBufferHeight)
		{
			cameraMovement.Y = moveSpeed;
		}

		if (state.IsKeyDown(Keys.Left) || getMouseScreenPosition().X <= 0)
		{
			cameraMovement.X = -moveSpeed;
		}

		if (state.IsKeyDown(Keys.Right) || getMouseScreenPosition().X >= graphics.PreferredBackBufferWidth)
		{
			cameraMovement.X = moveSpeed;
		}

		// Handle zoom
		_previousMouseWheelValue = _currentMouseWheelValue;
		_currentMouseWheelValue = mouseState.ScrollWheelValue;

		if (_currentMouseWheelValue > _previousMouseWheelValue)
		{
			AdjustZoom(.05f);
		}

		if (_currentMouseWheelValue < _previousMouseWheelValue)
		{
			AdjustZoom(-.05f);
		}

		// TODO: Store these somewhere not here
		int tileWidth = 32;
		int tileHeight = 32;
		int mapWidth = 32;
		int mapHeight = 32;

		Vector2 newPosition = Position + cameraMovement;
		Matrix newTransform = CalculateTransformMatrix(newPosition, Bounds, Zoom);
		Rectangle newVisibleArea = CalculateVisibleArea(Transform, Bounds);

		// Trying to figure out how to make it so the view area stays in the map
		Debug.WriteLine(newVisibleArea.ToString(), newPosition.ToString());
		Debug.WriteLine(newVisibleArea.Bottom);

		if (newVisibleArea.Left < 0
			|| newVisibleArea.Top < 0
			|| newVisibleArea.Right > tileWidth * mapWidth
			|| newVisibleArea.Bottom > tileHeight * mapHeight)
		{
			Position = _oldPosition;
			Transform = _oldTransform;
			VisibleArea = CalculateVisibleArea(Transform, Bounds);
		}
		else
		{
			Position = newPosition;
			Transform = newTransform;
			VisibleArea = newVisibleArea;
		}
	}

	private Matrix CalculateTransformMatrix(Vector2 position, Rectangle bounds, float zoom)
	{
		Matrix transform = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) *
					Matrix.CreateScale(zoom) *
					Matrix.CreateTranslation(new Vector3(bounds.Width * 0.5f, bounds.Height * 0.5f, 0));

		return transform;
	}

	private Rectangle CalculateVisibleArea(Matrix transform, Rectangle bounds)
	{
		Matrix inverseViewMatrix = Matrix.Invert(transform);

		Vector2 tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
		Vector2 tr = Vector2.Transform(new Vector2(bounds.X, 0), inverseViewMatrix);
		Vector2 bl = Vector2.Transform(new Vector2(0, bounds.Y), inverseViewMatrix);
		Vector2 br = Vector2.Transform(new Vector2(bounds.Width, bounds.Height), inverseViewMatrix);

		Vector2 min = new Vector2(
			MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
			MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
		Vector2 max = new Vector2(
			MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
			MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));

		return (new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y)));
	}

	public Vector2 getMouseScreenPosition()
	{
		MouseState mouseState = Mouse.GetState();
		Vector2 mouse = mouseState.Position.ToVector2();

		return new Vector2(mouse.X, mouse.Y);
	}

	public Vector2 getMouseScenePosition()
	{
		MouseState mouseState = Mouse.GetState();
		Vector2 mouse = mouseState.Position.ToVector2();

		mouse = Vector2.Transform(mouse, Matrix.Invert(Transform));
		return mouse;
	}

}
