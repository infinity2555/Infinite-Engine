using System;

using NS_Main;
using NS_BaseAssets;
using NS_World;
using NS_InputHandler;

namespace NS_Loop
{
    public class CL_Loop
    {
        internal static void Loop()
        {
            /* === === === ! IMPORTANT ! === === === */
            // Cubes have a length of around 3.2f. Example : X 1 Cube and X < 3.2f Cube are sticking and bugging.

            Obj object1 = new(
                new Vector3f(4, 0, 9),
                Obj.ObjectTypes.Cube,
                Obj.FillTypes.Fill,
                Color.Red
            );
            World.OBJECTS_IN_WORLD.Add(object1);

            Obj object2 = new(
                new Vector3f(1f,4, 9),
                Obj.ObjectTypes.Cube,
                Obj.FillTypes.Fill,
                Color.Blue
            );
            World.OBJECTS_IN_WORLD.Add(object2);

            Obj object3 = new(
                new Vector3f(-4, 0, 6),
                Obj.ObjectTypes.Cube,
                Obj.FillTypes.Fill,
                Color.Green
            );
            World.OBJECTS_IN_WORLD.Add(object3);

            Obj object4 = new(
                new Vector3f(-2, 2, 11),
                Obj.ObjectTypes.Cube,
                Obj.FillTypes.Fill,
                Color.Cyan
            );
            World.OBJECTS_IN_WORLD.Add(object4);

            Obj object5 = new(
                new Vector3f(-7, -2, 9),
                Obj.ObjectTypes.Cube,
                Obj.FillTypes.Fill,
                Color.White
            );
            World.OBJECTS_IN_WORLD.Add(object5);

            // MAIN LOOP
            while (CL_Main.WINDOW.IsOpen)
            {
                CL_Main.WINDOW.DispatchEvents(); // dispatch events (closing window)

                CL_Main.WINDOW.Clear(); // Clear the scene

                World.DrawAllObjects();

                Input.Check();

                CL_Main.WINDOW.Display(); // Finish loop
            }
        }
    }
}
