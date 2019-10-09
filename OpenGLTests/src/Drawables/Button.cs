using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables
{
    class Button : Rectangle, IInteractable
    {
        public Button()
        {
            this.Color = Color.Coral; 
            this.Size = new GLCoordinate(0.2f, 0.2f);
            OnInteraction = () =>
            {
                Toggle();
            };
        }

        public void Toggle()
        {
            this.Color = Color.FromArgb(this.Color.A, this.Color.R, (this.Color.G + Byte.MaxValue / 2) % (Byte.MaxValue - 1), this.Color.B);
        }
    }
}
