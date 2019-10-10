﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Util;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenGLTests.src
{
    abstract class Screen
    {
        public abstract void Draw(DrawAdapter drawer);
        protected List<HotkeyMapping> Keybindings { get; } = new List<HotkeyMapping>();
       
        public Screen()
        {
            SetupInputBindings();
        }

        public virtual void HandleInput(InputUnion input)
        {
            foreach (var keybinding in Keybindings)
            {
                keybinding.Tickle(input);
            }
        }

        protected abstract void SetupInputBindings();
    }

    class GameScreen : Screen
    {
        public GameState Game { get; set; }= new GameState();

        public override void Draw(DrawAdapter drawer)
        {
            GL.PushMatrix();
            //GL.Translate(-GameState.ActiveCamera.Location.X*2/GibbWindow.WIDTH -1, -GameState.ActiveCamera.Location.Y*2/GibbWindow.HEIGHT - 1, 0);
            GL.Translate(-new GameCoordinate(0, 0).X, -new GameCoordinate(0, 0).Y , 0);
            //GL.Translate(GameState.ActiveCamera.Location.X, -GameState.ActiveCamera.Location.Y, 0);
            foreach (var drawable in GameState.Drawables.Get)
            {
                drawable.Draw(drawer);
            }

            GL.PopMatrix();
        }

        protected override void SetupInputBindings()
        {
            // Keyboard
            Bind(new Hotkey(
                input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.D,
                _ => GameState.ActiveCamera.Speed.X = 0.01f,
                _ => GameState.ActiveCamera.Speed.X = 0
            ));

            Bind(new Hotkey(
                input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.A,
                _ => GameState.ActiveCamera.Speed.X = -0.01f,
                _ => GameState.ActiveCamera.Speed.X = 0
            ));

            Bind(new Hotkey(
                input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.W,
                _ => GameState.ActiveCamera.Speed.Y = -0.01f,
                _ => GameState.ActiveCamera.Speed.Y = 0
            ));

            Bind(new Hotkey(
                input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.S,
                _ => GameState.ActiveCamera.Speed.Y = 0.01f,
                _ => GameState.ActiveCamera.Speed.Y = 0
            ));
            /*            Bind(new Hotkey(
                            input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.D,
                            _ => Game.Hero.Speed.X = 0.01f,
                            _ => Game.Hero.Speed.X = 0
                        ));

                        Bind(new Hotkey(
                            input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.A,
                            _ => Game.Hero.Speed.X = -0.01f,
                            _ => Game.Hero.Speed.X = 0
                        ));

                        Bind(new Hotkey(
                            input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.W,
                            _ => Game.Hero.Speed.Y = -0.01f,
                            _ => Game.Hero.Speed.Y = 0
                        ));

                        Bind(new Hotkey(
                            input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.S,
                            _ => Game.Hero.Speed.Y = 0.01f,
                            _ => Game.Hero.Speed.Y = 0
                        ));*/

            // Mouse
            Bind(new Hotkey(
                input => input.IsMouseInput && input.MouseButtonArgs.Button == MouseButton.Left,
                input =>
                {
                    GameCoordinate clicked = new GameCoordinate(input.MouseButtonArgs.X, input.MouseButtonArgs.Y);
                    var xd = GameState.Drawables.Get.Where(d => d is IInteractable).ToList();
                    foreach (IInteractable i in xd)
                    {
                        if (i.Contains(clicked))
                        {
                            i.OnInteraction.Invoke();
                        }
                    }

                    foreach(IClickable i in GameState.Drawables.Get.Where(d => d is IClickable))
                    {
                        if (i.Contains(clicked))
                        {
                            i.OnClick(clicked);
                        }
                    }
                },
                input =>
                {

                }
            ));

            Bind(new Hotkey(
                input => input.IsMouseInput && input.MouseButtonArgs.Button == MouseButton.Right,
                input =>
                {
                    if (GameState.CombatMode)
                    {
                        Game.Hero.ActionHandler.RemoveAllPlacedAfterActive();
                        var rs = Game.Hero.ActionHandler.GetActiveRangeShape();

                        if (rs != null)
                        {
                            rs.Visible = true;
                        }
                    }
                    else
                    {
                        
                    }
                },
                input =>
                {
                    GameCoordinate clicked = new GameCoordinate(input.MouseButtonArgs.X, input.MouseButtonArgs.Y);
                    var xd = CoordinateFuckery.ClickToGLRelativeToCamera(clicked, new GameCoordinate(0, 0));
                    var rs = Game.Hero.ActionHandler.GetActiveRangeShape();

                    if (GameState.CombatMode)
                    {
                        if (rs != null)
                        {
                            rs.Visible = false;
                            if (rs.Contains(xd))
                            {
                                Game.Hero.ActionHandler.EnqueueAction(xd);
                            }
                        }
                    }
                    else
                    {
                        Game.Hero.ActionHandler.DoActiveAction(xd);
                    }
                }
            ));
        }

        public void Bind(Hotkey hotkey)
        {
            Keybindings.Add(hotkey.Activate);
            Keybindings.Add(hotkey.Deactivate);
        }

    }

}
