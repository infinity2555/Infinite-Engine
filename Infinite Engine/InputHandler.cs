using System;
using NS_Main;
using NS_World;

namespace NS_InputHandler
{
    public class Input
    {
        public static void Check()
        {
            Camera.CAMERA_FORWARD = UpdateMouse();

            Vector3 move = new();

            if (Keyboard.IsKeyPressed(Keyboard.Key.W)) 
                move += Camera.CAMERA_FORWARD * Camera.CAMERA_SPEED;
            
            if (Keyboard.IsKeyPressed(Keyboard.Key.S)) 
                move -= Camera.CAMERA_FORWARD * Camera.CAMERA_SPEED;
            
            if (Keyboard.IsKeyPressed(Keyboard.Key.D)) 
                move += Camera.CAMERA_SIDES * Camera.CAMERA_SPEED;
            
            if (Keyboard.IsKeyPressed(Keyboard.Key.A)) 
                move -= Camera.CAMERA_SIDES * Camera.CAMERA_SPEED;

            Camera.CAMERA_POSITION += move; // move 

            Camera.CAMERA = Matrix4x4.CreateLookAt(
                Camera.CAMERA_POSITION,
                Camera.CAMERA_POSITION + Camera.CAMERA_FORWARD,
                Vector3.UnitY); // update ... update the... camera...
        }                       // ...because you move your mouse ...


        private static Vector2i center;

        public static float MOUSE_SENSITIVITY = 0.1f;

        public static float YAW = -90f;
        public static float PITCH = 0f;

        public static (float, float) CreateMouseLookAt()
        {
            center = new Vector2i((int)CL_Main.WINDOW.Size.X / 2, (int)CL_Main.WINDOW.Size.Y / 2);

            Vector2i mousePos = Mouse.GetPosition(CL_Main.WINDOW);

            // MOUSE CURSOR DELTA
            float deltaX = mousePos.X - center.X;
            float deltaY = mousePos.Y - center.Y;

            Mouse.SetPosition(center, CL_Main.WINDOW);

            return (deltaX, deltaY);
        }

        public static Vector3 UpdateMouse()
        {
            var delta = CreateMouseLookAt();

            YAW += delta.Item1 * MOUSE_SENSITIVITY;
            PITCH += delta.Item2 * MOUSE_SENSITIVITY;

            Vector3 direction;
            direction.X = (float)(Math.Cos(Math.PI * YAW / 180.0) * Math.Cos(Math.PI * PITCH / 180.0));
            direction.Y = (float)Math.Sin(Math.PI * PITCH / 180.0);
            direction.Z = (float)(Math.Sin(Math.PI * YAW / 180.0) * Math.Cos(Math.PI * PITCH / 180.0));

            Vector3 newForward = Vector3.Normalize(direction);

            // what
            Camera.CAMERA_SIDES = Vector3.Normalize(Vector3.Cross(newForward, Vector3.UnitY));

            return Vector3.Normalize(direction);
        }
    }
}