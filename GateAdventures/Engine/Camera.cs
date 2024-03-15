using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace GateAdventures.Engine;
public class Camera
{
	public float Zoom { get; set; }
	public Vector2 Position { get; set; }
	public Rectangle Bounds { get; protected set; }
	public Matrix Transform { get; protected set; }
	public Rectangle VisibleArea { get; protected set; }

	private float currentMouseWheelValue, previousMouseWheelValue, zoom, previousZoom;

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
	private Matrix CalculateTransformMatrix(Vector2 position, Rectangle bounds, float zoom)
	{
		Matrix transform = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) *
					Matrix.CreateScale(zoom) *
					Matrix.CreateTranslation(new Vector3(bounds.Width * 0.5f, bounds.Height * 0.5f, 0));

		return transform;
	}
	private Rectangle CalculateVisibleArea(Matrix transform, Rectangle bounds)
	{
		Matrix inverseViewMatrix = Matrix.Invert(Transform);

		Vector2 tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
		Vector2 tr = Vector2.Transform(new Vector2(Bounds.X, 0), inverseViewMatrix);
		Vector2 bl = Vector2.Transform(new Vector2(0, Bounds.Y), inverseViewMatrix);
		Vector2 br = Vector2.Transform(new Vector2(Bounds.Width, Bounds.Height), inverseViewMatrix);

		Vector2 min = new Vector2(
			MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
			MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
		Vector2 max = new Vector2(
			MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
			MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));

		return (new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y)));
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

	public void UpdateCamera(GameServiceContainer services)
	{
		GraphicsDeviceManager graphics = services.GetService<GraphicsDeviceManager>();
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
		previousMouseWheelValue = currentMouseWheelValue;
		currentMouseWheelValue = mouseState.ScrollWheelValue;

		if (currentMouseWheelValue > previousMouseWheelValue)
		{
			AdjustZoom(.05f);
		}

		if (currentMouseWheelValue < previousMouseWheelValue)
		{
			AdjustZoom(-.05f);
		}

		/*previousZoom = zoom;
		zoom = Zoom;*/

		// TODO: Store these somewhere not here
		int tileWidth = 32;
		int tileHeight = 32;
		int mapWidth = 30;
		int mapHeight = 30;
		Rectangle worldRectangle = new Rectangle(0, 0, mapWidth * tileWidth, mapHeight * tileHeight);

		// If the new location won't be off screen
		Vector2 newPosition = Position + cameraMovement;
		Matrix newTransform = CalculateTransformMatrix(newPosition, Bounds, Zoom);
		Rectangle newVisibleArea = CalculateVisibleArea(Transform, Bounds);
		if (worldRectangle.Contains(newVisibleArea))
		{
			UpdateMatrix(newTransform, newVisibleArea);
			SetCameraPosition(newPosition);
		} else
		{
			// Calculate the adjusted visible area
			int x = Math.Max(worldRectangle.Left, newVisibleArea.Left);
			int y = Math.Max(worldRectangle.Top, newVisibleArea.Top);
			int right = Math.Min(worldRectangle.Right, newVisibleArea.Right);
			int bottom = Math.Min(worldRectangle.Bottom, newVisibleArea.Bottom);

			// Adjust left edge if out of bounds
			if (x < worldRectangle.Left)
				x = worldRectangle.Left;

			// Adjust top edge if out of bounds
			if (y < worldRectangle.Top)
				y = worldRectangle.Top;

			// Adjust right edge if out of bounds
			if (right > worldRectangle.Right)
				right = worldRectangle.Right;

			// Adjust bottom edge if out of bounds
			if (bottom > worldRectangle.Bottom)
				bottom = worldRectangle.Bottom;

			int width = right - x;
			int height = bottom - y;

			newVisibleArea = new Rectangle(x, y, width, height);

			// Calculate the center of the adjusted visible area
			int centerX = newVisibleArea.X + newVisibleArea.Width / 2;
			int centerY = newVisibleArea.Y + newVisibleArea.Height / 2;

			// Update the camera matrix and position with the adjusted values
			newTransform = CalculateTransformMatrix(new Vector2(centerX, centerY), Bounds, Zoom);
			
			SetCameraPosition(new Vector2(centerX, centerY));
			UpdateMatrix(newTransform, newVisibleArea);
		}
		
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
