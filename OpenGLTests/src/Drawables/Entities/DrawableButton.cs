using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Screens;

namespace OpenGLTests.src.Drawables.Entities
{
    public class DrawableButton : Button, IInteractable
    {
        public DrawableButton(Entity d)
        {
            if (d.Animation == null) return;
            this.Animation = d.Animation;
            this.Animation.IsStatic = true;
            OnInteraction = () =>
            {
                Console.WriteLine(d + " clicked");
                EditorScreen.CurrentlySelected = d.Clone() as Entity;
            };
            d.Dispose();
        }
    }
}
