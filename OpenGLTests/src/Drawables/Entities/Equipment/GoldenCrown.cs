using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables.Elements;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables.Entities.Equipment
{
    public class GoldenCrown : HeadItem
    {
        public GoldenCrown(Unit owner) : base(owner)
        {
            this.Icon = SpriteID.equipment_icon_golden_crown;
            this.Rarity = Rarity.Epic;
        }
    }
}
