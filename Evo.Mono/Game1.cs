using System;
using System.Linq;
using Evo.Mono.Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Evo.Mono;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _circleTexture;
    private Texture2D _crossTexture;
    private Texture2D _pixelTexture;
    private World _world;
    private FrameCounter _frameCounter;
    private GameSpeeds _gameSpeed;
    private GameSpeeds _previousGameSpeed;
    private bool _showTargets = false;
    private long _ticksSinceLastUpdate;
    private readonly int _updateInterval = (int)Math.Round(1000.0 / 60.0 * 10000.0);

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferHeight = 1000;
        _graphics.PreferredBackBufferWidth = 1000;
        _graphics.SynchronizeWithVerticalRetrace = true;
        IsFixedTimeStep = true;
        TargetElapsedTime = new TimeSpan((int)Math.Round(1000.0 / 144.0 * 10000.0));
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _gameSpeed = GameSpeeds.Normal;
        _ticksSinceLastUpdate = 0;
        _frameCounter = new FrameCounter();
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        _world = new World { Size = 960 };
        for (int i = 0; i < 10; i++)
        {
            _world.Entities.Add(new Creature(i, _world));
        }

        _world.Entities.First().Debug = false;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _circleTexture = Content.Load<Texture2D>("Textures/Circle");
        _crossTexture = Content.Load<Texture2D>("Textures/Cross");
        _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
        _pixelTexture.SetData(new[] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        KeyInput.GetState();
        if (KeyInput.HasBeenPressed(Keys.X)) _showTargets = !_showTargets;
        if (KeyInput.HasBeenPressed(Keys.OemPlus)) _gameSpeed = _gameSpeed.GetNextGameSpeed();
        if (KeyInput.HasBeenPressed(Keys.OemMinus)) _gameSpeed = _gameSpeed.GetPreviousGameSpeed();
        if (KeyInput.HasBeenPressed(Keys.Space))
        {
            if (_gameSpeed == GameSpeeds.Pause)
            {
                _gameSpeed = _previousGameSpeed;
            }
            else
            {
                _previousGameSpeed = _gameSpeed;
                _gameSpeed = GameSpeeds.Pause;
            }
        }

        if (_gameSpeed != GameSpeeds.Pause)
        {
            _ticksSinceLastUpdate += gameTime.ElapsedGameTime.Ticks;
            var gameUpdates = (int)(_ticksSinceLastUpdate / (_updateInterval * _gameSpeed.GetSpeedValue()));

            for (var i = 0; i < gameUpdates; i++)
            {
                foreach (var entity in _world.Entities)
                {
                    var creature = entity as Creature;
                    creature?.Update(gameTime);
                }
            }

            _ticksSinceLastUpdate -= (int)(_updateInterval * _gameSpeed.GetSpeedValue() * gameUpdates);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // Print fps to screen
        _frameCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        Window.Title = $"Speed: {_gameSpeed.ToAlternativeString()} FPS: {_frameCounter.AverageFramesPerSecond}";

        //_spriteBatch.DrawString(_spriteFont, fps, new Vector2(1, 1), Color.Black);

        GraphicsDevice.Clear(Color.DarkBlue);

        int screenWidth = GraphicsDevice.Viewport.Bounds.Width;
        int screenHeight = GraphicsDevice.Viewport.Bounds.Height;

        var mapPosition = new Vector2((screenWidth - _world.Size) / 2f, (screenHeight - _world.Size) / 2f);

        _spriteBatch.Begin();
        
        _spriteBatch.Draw(_pixelTexture, mapPosition, null,
            Color.CornflowerBlue, 0f, Vector2.Zero, new Vector2(_world.Size, _world.Size),
            SpriteEffects.None, 0f);

        foreach (Creature entity in _world.Entities)
        {
            _spriteBatch.Draw(_circleTexture, entity.TexturePosition + mapPosition, null, entity.Debug ? Color.Red : Color.Green,
                (entity.Direction + 90).ToRadians(), new Vector2(_circleTexture.Width / 2, _circleTexture.Height / 2),
                1f,
                SpriteEffects.None, 0f);
            if (_showTargets)
                _spriteBatch.Draw(_crossTexture,
                    entity._wanderTarget + mapPosition - (new Vector2(_crossTexture.Width / 2, _crossTexture.Height / 2)), null,
                    Color.Black,
                    0.785398f, new Vector2(_crossTexture.Width / 2, _crossTexture.Height / 2), 1f,
                    SpriteEffects.None, 1f);
        }

        // (x, y)

        /*
        var position = new Vector2(500, 500);
        var target = new Vector2(700, 300);

        Degrees direction = 90f;

        var positionToTarget = target - position;

        Degrees positionToTargetDirection = MathHelper.ToDegrees((float)Math.Atan2(positionToTarget.Y, positionToTarget.X));

        var positionInDirection = direction.ToVector2() * 100;
        positionInDirection += position;

        Console.WriteLine(positionToTargetDirection);

        _spriteBatch.Draw(_circleTexture, position, null, Color.Aqua,
            (direction+90).ToRadians(), new Vector2(_circleTexture.Width / 2, _circleTexture.Height / 2), 1f,
            SpriteEffects.None, 0f);
            */

        _spriteBatch.End();

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}