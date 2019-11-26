using System;
using OpenGLTests.src;

namespace OpenGLTests
{
    public interface IInteractable
    {
        Action OnInteraction { get; set; }
        bool Contains(GameCoordinate point);
        bool Visible { get; set; }
    }
}