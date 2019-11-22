using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Drawables.Terrain;
using OpenGLTests.src.Util;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

namespace OpenGLTests.src.Screens
{
    class GameScreen : Screen
    {
        public GameState Game { get; set; } = new GameState();

        public override void Draw(DrawAdapter drawer)
        {
            GL.PushMatrix();
            GL.Translate(-new GameCoordinate(0, 0).X, -new GameCoordinate(0, 0).Y, 0);

            object l = true;

            lock (l)
            {
                foreach (var ent in GameState.Drawables.GetAllEntities)
                {
                    ent.DrawStep(drawer);
                }

                foreach (var ele in GameState.Drawables.GetAllElements)
                {
                    ele.DrawStep(drawer);
                }

                if(Player.Cursor != null) Player.Cursor.DrawStep(drawer);
                GameState.RainGenerator.Draw(drawer);
            }

            GL.PopMatrix();
        }

        protected override void SetupInputBindings()
        {
            #region KeyBoardBinds
            // Keyboard
            Bind(new Hotkey(
                input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.D,
                _ => GameState.Players.ForEach(p => p.ActiveCamera.Speed.X = p.ActiveCamera.BaseSpeed.X),
                _ => GameState.Players.ForEach(p => p.ActiveCamera.Speed.X = 0)
            ));

            Bind(new Hotkey(
                input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.A,
                _ => GameState.Players.ForEach(p => p.ActiveCamera.Speed.X = -p.ActiveCamera.BaseSpeed.X),
                _ => GameState.Players.ForEach(p => p.ActiveCamera.Speed.X = 0)
            ));

            Bind(new Hotkey(
                input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.W,
                _ => GameState.Players.ForEach(p => p.ActiveCamera.Speed.Y = -p.ActiveCamera.BaseSpeed.Y),
                _ => GameState.Players.ForEach(p => p.ActiveCamera.Speed.Y = 0)
            ));

            Bind(new Hotkey(
                input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.S,
                _ => GameState.Players.ForEach(p => p.ActiveCamera.Speed.Y = p.ActiveCamera.BaseSpeed.Y),
                _ => GameState.Players.ForEach(p => p.ActiveCamera.Speed.Y = 0)
            ));

            Bind(new Hotkey(
                input => input.IsKeyboardInput && (input.KeyboardArgs.Key == OpenTK.Input.Key.E || input.KeyboardArgs.Key == Key.Tab),
                _ => Game.Hero.InventoryButton.ShowInventory(),
                _ => { }
            ));

            Bind(new Hotkey(
                input => input.IsKeyboardInput && (input.KeyboardArgs.Key == OpenTK.Input.Key.C || input.KeyboardArgs.Key == Key.Tilde),
                _ => Game.Hero.EquipmentHandler.ToggleVisibility(),
                _ => { }
            ));
#endregion
            #region ActionBar Hotkeys
            Bind(new Hotkey(
                input => input.IsKeyboardInput && (input.KeyboardArgs.Key == OpenTK.Input.Key.Number1),
                _ =>
                {
                    Game.Hero.ActionBar.GetActionButton(0).OnInteraction.Invoke();
                },
                _ => { }
            ));
            Bind(new Hotkey(
                input => input.IsKeyboardInput && (input.KeyboardArgs.Key == OpenTK.Input.Key.Number2),
                _ =>
                {
                    Game.Hero.ActionBar.GetActionButton(1).OnInteraction.Invoke();
                },
                _ => { }
            ));
            Bind(new Hotkey(
                input => input.IsKeyboardInput && (input.KeyboardArgs.Key == OpenTK.Input.Key.Number3),
                _ =>
                {
                    Game.Hero.ActionBar.GetActionButton(2).OnInteraction.Invoke();
                },
                _ => { }
            ));
            Bind(new Hotkey(
                input => input.IsKeyboardInput && (input.KeyboardArgs.Key == OpenTK.Input.Key.Number4),
                _ =>
                {
                    Game.Hero.ActionBar.GetActionButton(3).OnInteraction.Invoke();
                },
                _ => { }
            ));
            Bind(new Hotkey(
                input => input.IsKeyboardInput && (input.KeyboardArgs.Key == OpenTK.Input.Key.Number5),
                _ =>
                {
                    Game.Hero.ActionBar.GetActionButton(4).OnInteraction.Invoke();
                },
                _ => { }
            ));
            Bind(new Hotkey(
                input => input.IsKeyboardInput && (input.KeyboardArgs.Key == OpenTK.Input.Key.Number6),
                _ =>
                {
                    Game.Hero.ActionBar.GetActionButton(5).OnInteraction.Invoke();
                },
                _ => { }
            ));
            Bind(new Hotkey(
                input => input.IsKeyboardInput && (input.KeyboardArgs.Key == OpenTK.Input.Key.Number7),
                _ =>
                {
                    Game.Hero.ActionBar.GetActionButton(6).OnInteraction.Invoke();
                },
                _ => { }
            ));
            Bind(new Hotkey(
                input => input.IsKeyboardInput && (input.KeyboardArgs.Key == OpenTK.Input.Key.Number8),
                _ =>
                {
                    Game.Hero.ActionBar.GetActionButton(7).OnInteraction.Invoke();
                },
                _ => { }
            ));
            #endregion
            #region MouseBinds
            // Mouse
            Bind(new Hotkey(
                input => input.IsMouseInput && input.MouseButtonArgs.Button == MouseButton.Left,
                input =>
                {
                    GameCoordinate clicked = new GameCoordinate(input.MouseButtonArgs.X, input.MouseButtonArgs.Y);
                    var xd = CoordinateFuckery.ClickToGLRelativeToCamera(clicked, new GameCoordinate(0, 0));

                    foreach (var i in GameState.Drawables.GetAllInteractables)
                    {
                        if (i.Contains(clicked) && i.Visible)
                        {
                            if (i is Button b)
                            {
                                if(b.Enabled) i.OnInteraction.Invoke();
                            }
                            else
                            {
                                i.OnInteraction.Invoke();
                            }
                        }
                    }

                    //get all iinteractables instead? todo test.
                    foreach (ILeftClickable i in GameState.Drawables.GetAllDrawables.Where(d => d is ILeftClickable && d.Visible))
                    {
                        if (i.LeftClickFilter(Game.Hero, xd))
                        {
                            i.OnLeftClick(Game.Hero, clicked);
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
                    GameCoordinate placed = new GameCoordinate(input.MouseButtonArgs.X, input.MouseButtonArgs.Y);
                    var xd = CoordinateFuckery.ClickToGLRelativeToCamera(placed, new GameCoordinate(0, 0));
                    if (Game.Hero.ActionStatus == ActionReturns.Ongoing && Game.Hero.InCombat) return; 
                    Game.Hero.ActionHandler.OnMouseDown(xd);


                },
                input =>
                {
                    GameCoordinate placed = new GameCoordinate(input.MouseButtonArgs.X, input.MouseButtonArgs.Y);
                    var xd = CoordinateFuckery.ClickToGLRelativeToCamera(placed, new GameCoordinate(0, 0));


                    bool somethingWasClicked = false;
                    foreach (IRightClickable i in GameState.Drawables.GetAllInteractables.Where(d => d is IRightClickable && d.Visible))
                    {
                        if (i.RightClickFilter(Game.Hero, placed))
                        {
                            i.OnRightClick(Game.Hero, placed);
                            somethingWasClicked = true;
                        }
                    }

                    if (somethingWasClicked) return;
                    if (Game.Hero.ActionStatus == ActionReturns.Ongoing && Game.Hero.InCombat) return; //dont let place while hero is doing actions in combat
                    Game.Hero.ActionHandler.OnMouseUp(xd);
                }
            ));
            
            Bind(new Hotkey(
                input => input.IsMouseMove && (input.MouseMoveArgs.XDelta != 0 && input.MouseMoveArgs.XDelta != 0),
                input =>
                {
                    var xx = input.MouseMoveArgs.X;
                    var yy = input.MouseMoveArgs.Y;
                    GameCoordinate xxdd = new GameCoordinate(xx, yy);
                    var xd = CoordinateFuckery.ClickToGLRelativeToCamera(xxdd, new GameCoordinate(0, 0));

                    //Player.Cursor.Location = xd;
                },
                input =>
                {

                }
            ));
            #endregion
        }
    }
}
