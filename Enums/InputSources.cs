using System;

namespace AsciiPhoto.Enums
{
    /// <summary>
    /// The types of sources where input images can be read from.
    /// </summary>
    [Flags]
    public enum InputSources
    {
        NotSet = 0,

        Folder = 1,

        File = 2,

        Screen = 4,
    }
}