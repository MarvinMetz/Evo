using System;
using System.Linq;
using Evo.Mono.Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Evo.Mono;

public class Evo : Game
{
    private SpriteBatch _spriteBatch;
    private Texture2D _circleTexture;
    private Texture2D _crossTexture;
    private Texture2D _pixelTexture;
    private SpriteFont _consolasFont;
    private World _world;
    private readonly FrameCounter _frameCounter;
    private GameSpeeds _gameSpeed;
    private GameSpeeds _previousGameSpeed;
    private bool _showTargets;
    private long _ticksSinceLastUpdate;
    private readonly int _updateInterval = (int)Math.Round(1000.0 / 60.0 * 10000.0);

    private const string ControlDescription = "Exit:          ESC\n" +
                                              "Show targets:  X\n" +
                                              "Pause:         SPACE\n" +
                                              "Speed up:      +\n" +
                                              "Speed down:    -\n" +
                                              "";

    public Evo()
    {
        var graphics = new GraphicsDeviceManager(this);
        graphics.IsFullScreen = false;
        graphics.PreferredBackBufferHeight = 1000;
        graphics.PreferredBackBufferWidth = 1000;
        graphics.SynchronizeWithVerticalRetrace = true;
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
        _world = new World(960, 16);
        for (var i = 0; i < 10; i++)
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
        _pixelTexture.SetData([Color.White]);

        _consolasFont = Content.Load<SpriteFont>("Fonts/Consolas");
    }

    protected override void Update(GameTime gameTime)
    {
        HandleInput();

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

    private void HandleInput()
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
    }

    protected override void Draw(GameTime gameTime)
    {
        // Print fps to screen
        _frameCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        Window.Title = $"Speed: {_gameSpeed.ToAlternativeString()} FPS: {_frameCounter.AverageFramesPerSecond}";

        GraphicsDevice.Clear(Color.DarkBlue);

        var screenWidth = GraphicsDevice.Viewport.Bounds.Width;
        var screenHeight = GraphicsDevice.Viewport.Bounds.Height;

        var mapPosition = new Vector2((screenWidth - _world.WorldSize) / 2f, (screenHeight - _world.WorldSize) / 2f);
        var textPosition = mapPosition + new Vector2(10, 10); // Margin

        _spriteBatch.Begin();

        _spriteBatch.Draw(_pixelTexture, mapPosition, null,
            Color.CornflowerBlue, 0f, Vector2.Zero, new Vector2(_world.WorldSize, _world.WorldSize),
            SpriteEffects.None, 0f);

        _spriteBatch.DrawString(_consolasFont, ControlDescription, textPosition, Color.Black, 0f, Vector2.Zero, 0.75f,
            SpriteEffects.None, 0f);

        foreach (var entity in _world.Entities)
        {
            if (entity is not Creature creature) continue;
            _spriteBatch.Draw(_circleTexture, creature.TexturePosition + mapPosition, null,
                creature.Debug ? Color.Red : Color.Green,
                (creature.Direction + 90).ToRadians(),
                new Vector2(_circleTexture.Width / 2f, _circleTexture.Height / 2f),
                1f,
                SpriteEffects.None, 0f);
            if (_showTargets)
                _spriteBatch.Draw(_crossTexture,
                    creature.WanderTarget + mapPosition -
                    new Vector2(_crossTexture.Width / 2f, _crossTexture.Height / 2f), null,
                    Color.Black,
                    0.785398f, new Vector2(_crossTexture.Width / 2f, _crossTexture.Height / 2f), 1f,
                    SpriteEffects.None, 1f);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}