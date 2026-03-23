global using SFML.Window;
global using SFML.System;
global using SFML.Graphics;

global using System.Numerics;
using System;

using NS_Loop;
using NS_World;

namespace NS_Main
{
    public class CL_Main
    {
        public static uint WINDOW_WIDTH = 800;
        public static uint WINDOW_HEIGHT = 600;

        const string WINDOW_NAME = "Infinite Engine";

        private static Clock DELTATIME_CLOCK = new();

        public static RenderWindow WINDOW;

        public static void Main()
        {
            // THE WINDOW
            WINDOW = new(new((WINDOW_WIDTH, WINDOW_HEIGHT)), WINDOW_NAME);

            // CLOSING WINDOW
            WINDOW.Closed += (__, _) => WINDOW.Close();

            WINDOW.SetFramerateLimit(60);

            // normalize camera
            Camera.Normalize(Camera.CAMERA_FORWARD);

            // start the main loop
            CL_Loop.Loop();
        }
    }
}