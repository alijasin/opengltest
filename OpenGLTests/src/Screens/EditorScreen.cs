using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Drawables.Entities;
using OpenGLTests.src.Drawables.Terrain;
using OpenGLTests.src.Util;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

namespace OpenGLTests.src.Screens
{
    class EditorScreen : Screen
    {
        public GameConsole GameConsole;
        public static DrawableRepository Drawables = new DrawableRepository();
        public static Drawable CurrentlySelected { get; set; }
        public static Button SnapToGridButton { get; set; }
        List<Line> gridLines = new List<Line>();
        private bool SnapToGrid = false;
        private List<Drawable> toWriteToJson = new List<Drawable>();

        public EditorScreen()
        {
            GameConsole = new GameConsole();
            Drawables.Add(GameConsole.container);
            GameConsole.AddDrawableToBar(new Crate(new GameCoordinate(0, 0)));
            GameConsole.AddDrawableToBar(new AngryDude(new GameCoordinate(0, 0)));
            GameConsole.AddDrawableToBar(new ChasingPerson(new GameCoordinate(0, 0)));
            GameConsole.AddDrawableToBar(new Swamper(new GameCoordinate(0, 0)));
            GameConsole.AddDrawableToBar(new PatrolGuy(new GameCoordinate(0, 0)));
            GameConsole.AddDrawableToBar(new BrickGate(new GameCoordinate(0, 0)));
            GameConsole.AddDrawableToBar(new BrickWall(new GameCoordinate(0, 0)));
            GameConsole.AddDrawableToBar(new FleeingPerson(new GameCoordinate(0, 0), null));

            #region buttons
            SnapToGridButton = new Button(new GLCoordinate(0.1f, 0.1f));
            GameState.Drawables.RegisterInteractable(SnapToGridButton);
            SnapToGridButton.Animation = new Animation(new SpriteSheet_Icons());
            SnapToGridButton.Animation.IsStatic = true;
            SnapToGridButton.Animation.SetSprite(SpriteID.icon_snap_to_grid);
            SnapToGridButton.Location = new GLCoordinate(0, 0.95f);
            SnapToGridButton.OnInteraction = () =>
            {
                SnapToGrid = !SnapToGrid;
                foreach (var gline in gridLines)
                {
                    gline.Visible = SnapToGrid;
                }
            };
            Drawables.Add(SnapToGridButton);


            var SaveButton = new Button(new GLCoordinate(0.1f, 0.1f));
            GameState.Drawables.RegisterInteractable(SaveButton);
            SaveButton.Animation = new Animation(new SpriteSheet_Icons());
            SaveButton.Animation.IsStatic = true;
            SaveButton.Animation.SetSprite(SpriteID.floor_1);
            SaveButton.Location = new GLCoordinate(0.1f, 0.95f);

            SaveButton.OnInteraction = () =>
            {
                Console.WriteLine("Saved to TestEditorOutPut.json");
                EntitySerializer.WriteToJsonFile("TestEditorOutPut.json", toWriteToJson);
            };
            Drawables.Add(SaveButton);



            var LoadButton = new Button(new GLCoordinate(0.1f, 0.1f));
            GameState.Drawables.RegisterInteractable(LoadButton);
            LoadButton.Animation = new Animation(new SpriteSheet_Icons());
            LoadButton.Animation.IsStatic = true;
            LoadButton.Animation.SetSprite(SpriteID.floor_1);
            LoadButton.Location = new GLCoordinate(0.2f, 0.95f);
            LoadButton.OnInteraction = () =>
            {
                JObject entitiesList = EntitySerializer.ReadFromJsonFile<JObject>("TestEditorOutPut.json");
                JArray entities = entitiesList["$values"] as JArray;
                foreach (var entity in entities)
                {
                    try
                    {
                        string sType = entity["$type"].ToString();
                        Type entityType = Type.GetType(sType);
                        Console.WriteLine("deserializing " + entityType);
                        dynamic xd = (JsonConvert.DeserializeObject(entity.ToString(), entityType) as Drawable);
                        Console.WriteLine(xd);
                        Drawables.Add(xd);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("fucked up deserializing " + entity.GetType() + e);
                    }

                }
            };
            Drawables.Add(LoadButton);
            #endregion
        }
        public override void Draw(DrawAdapter drawer)
        {
            GL.PushMatrix();
            GL.Translate(-new GameCoordinate(0, 0).X, -new GameCoordinate(0, 0).Y, 0);
            ActiveCamera.Step();

            try
            {
                foreach (var ent in Drawables.GetAllDrawables)
                {
                    ent.DrawStep(drawer);
                }
            }
            catch (Exception e)
            {
            }


            if (SnapToGrid)
            {
                for (int x = -10; x < 20; x++)
                {
                    drawer.DrawLine(new GLCoordinate(x * 0.1f, -1), new GLCoordinate(x * 0.1f, 1), Color.FromArgb(95, Color.Chocolate), LineType.Solid);
                }
                for (int y = -10; y < 20; y++)
                {
                    drawer.DrawLine(new GLCoordinate(-1f, y * 0.1f), new GLCoordinate(1f, y * 0.1f), Color.FromArgb(95, Color.Chocolate), LineType.Solid);
                }
            }

            GL.PopMatrix();
        }

        protected override void SetupInputBindings()
        {
            // Keyboard
            Bind(new Hotkey(
                input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.D,
                _ => ActiveCamera.Speed.X = 0.05f,
                _ => ActiveCamera.Speed.X = 0
            ));

            Bind(new Hotkey(
                input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.A,
                _ => ActiveCamera.Speed.X = -0.05f,
                _ => ActiveCamera.Speed.X = 0
            ));

            Bind(new Hotkey(
                input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.W,
                _ => ActiveCamera.Speed.Y = -0.05f,
                _ => ActiveCamera.Speed.Y = 0
            ));

            Bind(new Hotkey(
                input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.S,
                _ => ActiveCamera.Speed.Y = 0.05f,
                _ => ActiveCamera.Speed.Y = 0
            ));
            Bind(new Hotkey(
                input => input.IsKeyboardInput && input.KeyboardArgs.Key == OpenTK.Input.Key.Tilde,
                _ => GameConsole.ToggleVisibility(),
                _ => { }
            ));
            // Mouse
            Bind(new Hotkey(
                input => input.IsMouseInput && input.MouseButtonArgs.Button == MouseButton.Left,
                input =>
                {
                    GameCoordinate clicked = new GameCoordinate(input.MouseButtonArgs.X, input.MouseButtonArgs.Y);
                    var xd = CoordinateFuckery.ClickToGLRelativeToCamera(clicked, new GameCoordinate(0, 0));

                    //should be xd?
                    if (!GameConsole.container.Contains(clicked) && !SnapToGridButton.Contains(clicked) && CurrentlySelected != null)
                    {
                        if (SnapToGrid) xd = RNG.SnapCoordinate(xd, new GameCoordinate(0.1f, 0.1f));
                        CurrentlySelected.Location = xd;
                        CurrentlySelected.Visible = true;
                        Drawables.Add(CurrentlySelected);
                        CurrentlySelected = CurrentlySelected.Clone() as Drawable;
                        toWriteToJson.Add(CurrentlySelected);
                    }


                    foreach (var inter in Drawables.GetAllInteractables)
                    {
                        if (inter.Contains(clicked))
                        {
                            inter.OnInteraction.Invoke();
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

                },
                input =>
                {
                    GameCoordinate clicked = new GameCoordinate(input.MouseButtonArgs.X, input.MouseButtonArgs.Y);
                    var xd = CoordinateFuckery.ClickToGLRelativeToCamera(clicked, new GameCoordinate(0, 0));

                    List<Drawable> hackToRemove = new List<Drawable>();
                    foreach (var d in Drawables.GetAllEntities)
                    {
                        if (xd.Distance(d.Location) < 0.1f)
                        {
                            hackToRemove.Add(d);
                        }
                    }
                    foreach (var hack in hackToRemove)
                    {
                        hack.Visible = false;
                        Drawables.Remove(hack);
                        toWriteToJson.Add(hack);
                    }

                }
            ));
        }

        private T Create<T>() where T : class, new()
        {
            return new T();
        }
    }

}
