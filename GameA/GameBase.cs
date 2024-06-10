using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading;

namespace GameA;

public class GameBase : Game
{
	private GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;
	Thread _inputThread;

	Circle[] circles;
	Texture2D[] textures;

	Vector2 force1;

	Texture2D topBorderTexture;
	Texture2D bottomBorderTexture;
	Texture2D leftBorderTexture;
	Texture2D rightBorderTexture;

	public static Rectangle topBorderRect;
	public static Rectangle bottomBorderRect;
	public static Rectangle leftBorderRect;
	public static Rectangle rightBorderRect;
	public static Rectangle screenRect;
	
	Point mouseCoor;
	MouseState mouseState;

	bool spacePressed = false;

	public GameBase()
	{
		_graphics = new GraphicsDeviceManager(this);
		_inputThread = new Thread(GetInput);



		Content.RootDirectory = "Content";
		IsMouseVisible = true;
	}

	void GetInput()
	{
		int firstX, firstY;

		while (true)
		{
			mouseState = Mouse.GetState();

			firstX = mouseState.X;
			firstY = mouseState.Y;

			mouseCoor = new Point(firstX, firstY);

			if (mouseState.LeftButton == ButtonState.Pressed && screenRect.Contains(mouseCoor))
			{
				while (true)
				{	
					if(mouseState.LeftButton == ButtonState.Released)
					{
						break;
					}
					mouseState = Mouse.GetState();

					if(screenRect.Contains(new Point(mouseState.X, mouseState.Y))) { }

				}
			}
			Thread.Sleep(16);
		} 
	}

	protected override void Initialize()
	{
		// TODO: Add your initialization logic here
		base.Initialize();

		_spriteBatch = new SpriteBatch(GraphicsDevice);

		circles = new Circle[16];
		textures = new Texture2D[16];

		string s = "Assets/ball_";
		
		for (int i = 1; i <= 16; i++)
		{
			textures[i - 1] = Content.Load<Texture2D>(s + i);
		}

		Console.WriteLine(_graphics.PreferredBackBufferHeight);
		Console.WriteLine(_graphics.PreferredBackBufferWidth);

		topBorderRect = new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, 40);
		bottomBorderRect = new Rectangle(0, _graphics.PreferredBackBufferHeight - 40, _graphics.PreferredBackBufferWidth, 40);
		leftBorderRect = new Rectangle(0, 0, 40, _graphics.PreferredBackBufferHeight);
		rightBorderRect = new Rectangle(_graphics.PreferredBackBufferWidth - 40, 0, 40, _graphics.PreferredBackBufferHeight);
		screenRect = new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

		circles[0] = new Circle(new Vector2(232, 400), textures[0], "ball_1");
		circles[1] = new Circle(new Vector2(100, 200), textures[1], "ball_2");
		circles[2] = new Circle(new Vector2(140, 200), textures[2], "ball_3");
		circles[3] = new Circle(new Vector2(180, 200), textures[3], "ball_4");
		circles[4] = new Circle(new Vector2(220, 200), textures[4], "ball_5");
		circles[5] = new Circle(new Vector2(260, 200), textures[5], "ball_6");
		circles[6] = new Circle(new Vector2(300, 200), textures[6], "ball_7");
		circles[7] = new Circle(new Vector2(340, 200), textures[7], "ball_8");
		circles[8] = new Circle(new Vector2(380, 200), textures[8], "balle_9");
		circles[9] = new Circle(new Vector2(420, 200), textures[9], "balle_10");
		circles[10] = new Circle(new Vector2(460, 200), textures[10], "ball_11");
		circles[11] = new Circle(new Vector2(500, 200), textures[11], "ball_12");
		circles[12] = new Circle(new Vector2(540, 200), textures[12], "ball_13");
		circles[13] = new Circle(new Vector2(580, 200), textures[13], "ball_14");
		circles[14] = new Circle(new Vector2(620, 200), textures[14], "ball_15");
		circles[15] = new Circle(new Vector2(660, 200), textures[15], "ball_16");

		force1 = new Vector2(0,0);

		_inputThread.Start();
    }

	protected override void LoadContent()
	{
		topBorderTexture = new Texture2D(_graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
		topBorderTexture.SetData<Color>(new Color[] {Color.White});

		bottomBorderTexture = new Texture2D(_graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
		bottomBorderTexture.SetData<Color>(new Color[] {Color.White});

		leftBorderTexture = new Texture2D(_graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
		leftBorderTexture.SetData<Color>(new Color[] { Color.White });

		rightBorderTexture = new Texture2D(_graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
		rightBorderTexture.SetData<Color>(new Color[] { Color.White });
	}

	protected override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
		ApplyPhysics.gameTime = gameTime;


		if(Keyboard.GetState().IsKeyDown(Keys.Up)) 
		{
			force1.Y -= 100;
		}

		if (Keyboard.GetState().IsKeyDown(Keys.Down))
		{
			force1.Y += 100;
		}

		if (Keyboard.GetState().IsKeyDown(Keys.Left))
		{
			force1.X -= 100;
		}

		if (Keyboard.GetState().IsKeyDown(Keys.Right))
		{
			force1.X += 100;
		}

		if(Keyboard.GetState().IsKeyDown(Keys.Space))
		{
			spacePressed = true;
		}

		if(Keyboard.GetState().IsKeyUp(Keys.Space) && spacePressed)
		{
			spacePressed = false;

            Console.WriteLine("a");

			circles[15].physics.Force = force1;

			force1 = Vector2.Zero;
		}

		ApplyPhysics.ApplySpeeds();

	}

	protected override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
		GraphicsDevice.Clear(Color.DarkGray);

		_spriteBatch.Begin();

		for (int i = 0; i < circles.Length; i++)
		{
			_spriteBatch.Draw(
			textures[i],
			circles[i].texturePosition,
			null,
			Color.White,
			0f,
			Vector2.Zero,
			Vector2.One,
			SpriteEffects.None,
			0f
			);
		}

		_spriteBatch.Draw(topBorderTexture, new Vector2(0, 0), topBorderRect, Color.Gold);

		_spriteBatch.Draw(bottomBorderTexture, new Vector2(0, _graphics.PreferredBackBufferHeight - 40), bottomBorderRect, Color.Red);

		_spriteBatch.Draw(leftBorderTexture, new Vector2(0, 0), leftBorderRect, Color.White);

		_spriteBatch.Draw(rightBorderTexture, new Vector2(_graphics.PreferredBackBufferWidth - 40, 0), rightBorderRect, Color.Violet);

		_spriteBatch.End();

	}
}