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

/// <summary>
/// Represents a camera that handles movement, zoom, and view transformations in a 2D game.
/// </summary>
public class Camera
{
	/// <summary>
	/// Gets or sets the zoom level of the camera. 
	/// A value greater than 1 zooms in, while a value less than 1 zooms out.
	/// </summary>
	public float Zoom { get; set; }

	/// <summary>
	/// Gets or sets the current position of the camera in world coordinates.
	/// </summary>
	public Vector2 Position { get; set; }

	/// <summary>
	/// Gets the bounds of the camera's viewport.
	/// </summary>
	public Rectangle Bounds { get; protected set; }

	/// <summary>
	/// Gets the transformation matrix used to render the camera's view.
	/// </summary>
	public Matrix Transform { get; protected set; }

	/// <summary>
	/// Gets the visible area of the game world based on the current camera position and zoom level.
	/// </summary>
	public Rectangle VisibleArea { get; protected set; }

	private float _currentMouseWheelValue, _previousMouseWheelValue, _oldZoom;
	private Vector2 _oldPosition;
	private Matrix _oldTransform;

	/// <summary>
	/// Initializes a new instance of the <see cref="Camera"/> class.
	/// </summary>
	/// <param name="services">The service container to retrieve graphics-related services.</param>
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

	/// <summary>
	/// Moves the camera by the specified amount.
	/// </summary>
	/// <param name="movePosition">The amount to move the camera in world coordinates.</param>
	public void MoveCamera(Vector2 movePosition)
	{
		Vector2 newPosition = Position + movePosition;
		Position = newPosition;
	}

	/// <summary>
	/// Sets the camera position to the specified coordinates.
	/// </summary>
	/// <param name="cameraPosition">The new position of the camera in world coordinates.</param>
	public void SetCameraPosition(Vector2 cameraPosition)
	{
		Position = cameraPosition;
	}

	/// <summary>
	/// Adjusts the camera zoom by the specified amount.
	/// </summary>
	/// <param name="zoomAmount">The amount to change the zoom level.</param>
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

	/// <summary>
	/// Updates the camera's position and zoom based on input and ensures it stays within the map boundaries.
	/// </summary>
	/// <param name="graphics">The graphics device manager for screen dimensions.</param>
	public void UpdateCamera(GraphicsDeviceManager graphics)
	{
		_oldPosition = Position;
		_oldTransform = Transform;
		_oldZoom = Zoom;

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

		// TODO: Store these somewhere not here
		int tileWidth = 32;
		int tileHeight = 32;
		int mapWidth = 30;
		int mapHeight = 30;

		float mapWidthInPixels = tileWidth * mapWidth;
		float mapHeightInPixels = tileHeight * mapHeight;

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

		// Ensure zoom level doesn't allow the visible area to exceed map size
		float maxZoomOutX = Bounds.Width / mapWidthInPixels;
		float maxZoomOutY = Bounds.Height / mapHeightInPixels;
		float maxZoomOut = Math.Max(maxZoomOutX, maxZoomOutY);

		if (Zoom < maxZoomOut)
		{
			Zoom = maxZoomOut;
		}

		Vector2 newPosition = Position + cameraMovement;
		Matrix newTransform = CalculateTransformMatrix(newPosition, Bounds, Zoom);
		Rectangle newVisibleArea = CalculateVisibleArea(newTransform, Bounds);

		// Adjust position if the visible area goes out of bounds
		if (newVisibleArea.Left < 0)
		{
			newPosition.X += -newVisibleArea.Left;
		}
		if (newVisibleArea.Top < 0)
		{
			newPosition.Y += -newVisibleArea.Top;
		}
		if (newVisibleArea.Right > mapWidthInPixels)
		{
			newPosition.X -= newVisibleArea.Right - mapWidthInPixels;
		}
		if (newVisibleArea.Bottom > mapHeightInPixels)
		{
			newPosition.Y -= newVisibleArea.Bottom - mapHeightInPixels;
		}

		// Clamp position to ensure the camera stays within bounds
		float halfWidth = newVisibleArea.Width / 2f;
		float halfHeight = newVisibleArea.Height / 2f;

		newPosition.X = MathHelper.Clamp(newPosition.X, halfWidth, mapWidthInPixels - halfWidth);
		newPosition.Y = MathHelper.Clamp(newPosition.Y, halfHeight, mapHeightInPixels - halfHeight);

		// Update camera if within bounds
		Transform = CalculateTransformMatrix(newPosition, Bounds, Zoom);
		VisibleArea = CalculateVisibleArea(Transform, Bounds);
		Position = newPosition;
	}

	/// <summary>
	/// Calculates the transformation matrix for the camera.
	/// </summary>
	/// <param name="position">The camera position.</param>
	/// <param name="bounds">The viewport bounds.</param>
	/// <param name="zoom">The current zoom level.</param>
	/// <returns>The calculated transformation matrix.</returns>
	private Matrix CalculateTransformMatrix(Vector2 position, Rectangle bounds, float zoom)
	{
		Matrix transform = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) *
					Matrix.CreateScale(zoom) *
					Matrix.CreateTranslation(new Vector3(bounds.Width * 0.5f, bounds.Height * 0.5f, 0));

		return transform;
	}

	/// <summary>
	/// Calculates the visible area of the game world based on the current transformation matrix.
	/// </summary>
	/// <param name="transform">The transformation matrix.</param>
	/// <param name="bounds">The viewport bounds.</param>
	/// <returns>The calculated visible area in world coordinates.</returns>
	private Rectangle CalculateVisibleArea(Matrix transform, Rectangle bounds)
	{
		Matrix inverseViewMatrix = Matrix.Invert(transform);

		// Corners top-left, top-right, bottom-left, bottom-right
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

	/// <summary>
	/// Gets the mouse position relative to the screen.
	/// </summary>
	/// <returns>The mouse position in screen coordinates.</returns>
	public Vector2 getMouseScreenPosition()
	{
		MouseState mouseState = Mouse.GetState();
		Vector2 mouse = mouseState.Position.ToVector2();

		return new Vector2(mouse.X, mouse.Y);
	}

	/// <summary>
	/// Gets the mouse position relative to the game world.
	/// </summary>
	/// <returns>The mouse position in world coordinates.</returns>
	public Vector2 getMouseScenePosition()
	{
		MouseState mouseState = Mouse.GetState();
		Vector2 mouse = mouseState.Position.ToVector2();

		mouse = Vector2.Transform(mouse, Matrix.Invert(Transform));
		return mouse;
	}
}
