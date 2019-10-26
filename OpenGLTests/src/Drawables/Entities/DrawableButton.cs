using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables.Entities
{
    public class DrawableButton : Button, IInteractable
    {
        public DrawableButton(Drawable d)
        {
            if (d.Animation == null) return;
            this.Animation = d.Animation;
            this.Animation.IsStatic = true;
            OnInteraction = () =>
            {
                Console.WriteLine(d + " clicked");
                EditorScreen.CurrentlySelected = d.Clone() as Drawable;
            };
            d.Dispose();
        }
    }
}
