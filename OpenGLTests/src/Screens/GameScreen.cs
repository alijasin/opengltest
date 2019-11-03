using System;
using System.Collections.Generic;
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

            //Cursor.Draw();

            //todo: i think theres a bug and with adding and removing entities while we are drawing. Fix this so we don't have to try catch and sometimes not render.

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

                GameState.RainGenerator.Draw(drawer);
            }


            GL.PopMatrix();
        }

        protected override void SetupInputBindings()
        {
            // Keyboard
            Bind(new Hotkey(
                input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.D,
                _ => ActiveCamera.Speed.X = ActiveCamera.BaseSpeed.X,
                _ => ActiveCamera.Speed.X = 0
            ));

            Bind(new Hotkey(
                input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.A,
                _ => ActiveCamera.Speed.X = -ActiveCamera.BaseSpeed.X,
                _ => ActiveCamera.Speed.X = 0
            ));

            Bind(new Hotkey(
                input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.W,
                _ => ActiveCamera.Speed.Y = -ActiveCamera.BaseSpeed.Y,
                _ => ActiveCamera.Speed.Y = 0
            ));

            Bind(new Hotkey(
                input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.S,
                _ => ActiveCamera.Speed.Y = ActiveCamera.BaseSpeed.Y,
                _ => ActiveCamera.Speed.Y = 0
            ));

            Bind(new Hotkey(
                input => input.IsKeyboardInput && (input.KeyboardArgs.Key == OpenTK.Input.Key.E || input.KeyboardArgs.Key == Key.Tab),
                _ => Game.Hero.Inventory.Visible = true,
                _ => Game.Hero.Inventory.Visible = false
            ));
            // Mouse
            Bind(new Hotkey(
                input => input.IsMouseInput && input.MouseButtonArgs.Button == MouseButton.Left,
                input =>
                {
                    GameCoordinate clicked = new GameCoordinate(input.MouseButtonArgs.X, input.MouseButtonArgs.Y);
                    var xd = CoordinateFuckery.ClickToGLRelativeToCamera(clicked, new GameCoordinate(0, 0));
                    Game.Hero.TestOnClick(xd);
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

                    foreach (IClickable i in GameState.Drawables.GetAllDrawables.Where(d => d is IClickable && d.Visible))
                    {
                        Console.WriteLine(xd);
                        if (i.Contains(xd))
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
                    GameCoordinate placed = new GameCoordinate(input.MouseButtonArgs.X, input.MouseButtonArgs.Y);
                    var xd = CoordinateFuckery.ClickToGLRelativeToCamera(placed, new GameCoordinate(0, 0));
                    //xd = xd.SnapCoordinate();
                    Game.Hero.ActionHandler.OnMouseDown(xd);


                },
                input =>
                {
                    GameCoordinate placed = new GameCoordinate(input.MouseButtonArgs.X, input.MouseButtonArgs.Y);
                    var xd = CoordinateFuckery.ClickToGLRelativeToCamera(placed, new GameCoordinate(0, 0));
                    //xd = xd.SnapCoordinate();
                    Game.Hero.ActionHandler.OnMouseUp(xd);
                }
            ));
            /*
            Bind(new Hotkey(
                input => input.IsMouseMove && (input.MouseMoveArgs.XDelta != 0 && input.MouseMoveArgs.XDelta != 0),
                input =>
                {
                    var xx = input.MouseMoveArgs.X;
                    var yy = input.MouseMoveArgs.Y;
                    GameCoordinate xxdd = new GameCoordinate(xx, yy);
                    var xd = CoordinateFuckery.ClickToGLRelativeToCamera(xxdd, new GameCoordinate(0, 0));
                    Console.WriteLine(xd);
                    Cursor.Draw(xd);
                    //Console.WriteLine(gc);
                },
                input =>
                {

                }
            ));*/

        }



    }

}
