using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src
{
    /// <summary>
    /// Interface for playing sounds. (Hopefully cross platform.)
    /// EnumSound is the name for each soundfile.
    /// Sounds are located in sound folder.
    ///
    /// Really shitty itnerface. will be improved as felix grows older.
    /// </summary>
    interface IBoomBox
    {
        //loads audio files into memory. This is allowed to take a rather long time since it will be done before game starts.(less than 10 seconds though)
        void LoadAudio();

        void PlaySound(EnumSound s);
        void SetBackgroundMusic(EnumSound s, bool loop = false);
    }
}
