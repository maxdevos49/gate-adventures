using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GateAdventures;

public class Game1 : Game
{
	private Texture2D colorSprite;
	private GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;
	private Effect _effect;
	private Rectangle _rectangle;
	//sampler s0;

	public Game1()
	{
		_graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
	}

	protected override void Initialize()
	{
		// TODO: Add your initialization logic here

		base.Initialize();
	}

	protected override void LoadContent()
	{
		_spriteBatch = new SpriteBatch(GraphicsDevice);
		// TODO: use this.Content to load your game content here
		colorSprite = Content.Load<Texture2D>("ColorfulTestSprite");
		_rectangle = new Rectangle(0, 0, colorSprite.Width, colorSprite.Height);
		//_effect = Content.Load<Effect>("effect1"); //primitive grayscale
		//_effect = Content.Load<Effect>("effect2"); //negative
		_effect = Content.Load<Effect>("effect3"); //pixel
	}

	protected override void Update(GameTime gameTime)
	{
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			Exit();

		// TODO: Add your update logic here

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.CornflowerBlue);

		_spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
		_effect.CurrentTechnique.Passes[0].Apply(); //uncomment this and one of the effects above
		//_spriteBatch.Draw(colorSprite, new Vector2(20, 20), Color.White);
		_spriteBatch.Draw(colorSprite, new Vector2(20, 20), _rectangle, Color.White, 0, new Vector2(0,0), 4f, SpriteEffects.None, 1);
		//void SpriteBatch.Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
		_spriteBatch.End();
		// TODO: Add your drawing code here

		base.Draw(gameTime);
	}
}
