using System;
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
            GL.Translate(-new GameCoordinate(0, 0).X, -new GameCoordinate(0, 0).Y , 0);

            //todo: i think theres a bug and with adding and removing entities while we are drawing. Fix this so we don't have to try catch and sometimes not render.
            try
            {
                //var size = GameState.Drawables.GetAllDrawables.Count;
                //List<Drawable> drawables = new List<Drawable>(size);
                //drawables = GameState.Drawables.GetAllDrawables.GetRange(0, size);
                foreach (var ent in GameState.Drawables.GetAllEntities)
                {
                    ent.DrawStep(drawer);
                }

                foreach (var ele in GameState.Drawables.GetAllElements)
                {
                    ele.DrawStep(drawer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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

            Bind(new Hotkey(
                input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.Tilde,
                _ => GameConsole.ToggleVisibility(),
                _ => { }
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
                    var xd = GameState.Drawables.GetAllDrawables.Where(d => d is IInteractable).ToList();
                    foreach (IInteractable i in xd)
                    {
                        if (i.Contains(clicked))
                        {
                            i.OnInteraction.Invoke();
                        }
                    }

                    //todo move all interactables from drawables to interactables
                    foreach (var i in GameState.Drawables.GetAllInteractables)
                    {
                        if (i.Contains(clicked))
                        {
                            i.OnInteraction.Invoke();
                        }
                    }

                    foreach(IClickable i in GameState.Drawables.GetAllDrawables.Where(d => d is IClickable))
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
                    GameCoordinate placed = new GameCoordinate(input.MouseButtonArgs.X, input.MouseButtonArgs.Y);
                    var xd = CoordinateFuckery.ClickToGLRelativeToCamera(placed, new GameCoordinate(0, 0));
                    Game.Hero.ActionHandler.OnMouseDown(xd);
                },
                input =>
                {
                    GameCoordinate placed = new GameCoordinate(input.MouseButtonArgs.X, input.MouseButtonArgs.Y);
                    var xd = CoordinateFuckery.ClickToGLRelativeToCamera(placed, new GameCoordinate(0, 0));
                    Game.Hero.ActionHandler.OnMouseUp(xd);
                }
            ));

            /*var x = 0;
            var y = 0;
            Bind(new Hotkey(
                input => input.IsMouseMove ,
                input =>
                {
                    x += input.MouseMoveArgs.XDelta;
                    y += input.MouseMoveArgs.YDelta;
                    var gc = new GameCoordinate(x, y);
                    var xd = CoordinateFuckery.ClickToGLRelativeToCamera(gc, new GameCoordinate(0, 0));
                    Console.WriteLine(xd);
                    //Console.WriteLine(gc);
                },
                input =>
                {

                }
            ));*/

        }

        public void Bind(Hotkey hotkey)
        {
            Keybindings.Add(hotkey.Activate);
            Keybindings.Add(hotkey.Deactivate);
        }

    }

}
