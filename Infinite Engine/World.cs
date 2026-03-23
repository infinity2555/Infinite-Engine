using System;

using NS_BaseAssets;
using NS_Loop;
using NS_Main;

namespace NS_World
{
    public class World
    {
        private struct TriangleToDraw
        {
            public VertexArray Vertices;
            public float AvgZ;
        }

        private static CircleShape DOT = new()
        {
            Radius = 4,
        };

        public static List<Obj> OBJECTS_IN_WORLD = new();

        private static List<TriangleToDraw> _allTriangles = new();

        static List<Vector3f> Rotated_Points = new();

        public static float dotProduct;
        public static Vector3f View_Vector;

        public static Vector3 LIGHT_DIRECTION = new(2, 3, 4);

        public static void DrawObject(Obj Obj)
        {
            Rotated_Points.Clear();
            _allTriangles.Clear();
            List<TriangleToDraw> allTriangles = new List<TriangleToDraw>();

            Vector3f[] points = Obj.Points;

            Vector2f[] Screen_Points = new Vector2f[points.Length];
            bool[] Is_Visible = new bool[points.Length];

            foreach (var obj in OBJECTS_IN_WORLD)
            {
                PrepareTriangles(obj);
            }

            for (int i = 0; i < points.Length; i++)
            {
                Vector3 cameraPoint = Vector3.Transform(new Vector3(points[i].X, points[i].Y, points[i].Z), Camera.CAMERA);

                Rotated_Points.Add(new Vector3f(cameraPoint.X, cameraPoint.Y, cameraPoint.Z));

                if (cameraPoint.Z < -0.01f)
                {
                    float screenX = (cameraPoint.X / -cameraPoint.Z) * 500 + (CL_Main.WINDOW_WIDTH / 2);
                    float screenY = (cameraPoint.Y / -cameraPoint.Z) * 500 + (CL_Main.WINDOW_HEIGHT / 2);

                    Screen_Points[i] = new Vector2f(screenX, screenY);
                    Is_Visible[i] = true;

                    if (Obj.FillType == Obj.FillTypes.Dots)
                    {
                        DOT.FillColor = Obj.ObjectColor;
                        DOT.Position = Screen_Points[i];
                        CL_Main.WINDOW.Draw(DOT);
                    }
                }
                else { Is_Visible[i] = false; }
            }

            if (Obj.FillType == Obj.FillTypes.Wireframe)
            {
                VertexArray line = new VertexArray(PrimitiveType.Lines, 2);
                int[] indices = Obj.Indices;

                for (int i = 0; i < indices.Length; i += 2)
                {
                    int start = indices[i];
                    int end= indices[i + 1];

                    if (Is_Visible[start] && Is_Visible[end])
                    {
                        line[0] = new Vertex(Screen_Points[start], Obj.ObjectColor);
                        line[1] = new Vertex(Screen_Points[end], Obj.ObjectColor);
                        CL_Main.WINDOW.Draw(line);
                    }
                }
            }

            if (Obj.FillType == Obj.FillTypes.Fill)
            {
                VertexArray triangle = new(PrimitiveType.Triangles, 3);
                int[] triangles = Obj.Triangles;

                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int startpoint = triangles[i];
                    int secondpoint = triangles[i + 1];
                    int thirdpoint = triangles[i + 2];

                    // Rotated_Points[startpoint]; etc . . .
                    Vector3f V1 = new Vector3f(
                        Rotated_Points[secondpoint].X - Rotated_Points[startpoint].X,
                        Rotated_Points[secondpoint].Y - Rotated_Points[startpoint].Y,
                        Rotated_Points[secondpoint].Z - Rotated_Points[startpoint].Z
                    );

                    Vector3f V2 = new Vector3f(
                        Rotated_Points[thirdpoint].X - Rotated_Points[startpoint].X,
                        Rotated_Points[thirdpoint].Y - Rotated_Points[startpoint].Y,
                        Rotated_Points[thirdpoint].Z - Rotated_Points[startpoint].Z
                    );

                    // ^^ 2 vectors because 2 triangles are making a single wall

                    Vector3f Normal = new(
                        V1.Y * V2.Z - V1.Z * V2.Y,
                        V1.Z * V2.X - V1.X * V2.Z,
                        V1.X * V2.Y - V1.Y * V2.X
                    );

                    View_Vector = Rotated_Points[startpoint];

                    dotProduct = (Normal.X * View_Vector.X) +
                                 (Normal.Y * View_Vector.Y) +
                                 (Normal.Z * View_Vector.Z);

                    triangle[0] = new Vertex(Screen_Points[startpoint], Color.Red);
                    triangle[1] = new Vertex(Screen_Points[secondpoint], Color.Green);
                    triangle[2] = new Vertex(Screen_Points[thirdpoint], Color.Blue);

                    float average_triangle_z = (Rotated_Points[startpoint].Z + 
                                                Rotated_Points[secondpoint].Z +
                                                Rotated_Points[thirdpoint].Z) / 3;

                    if (dotProduct <= 0)
                        continue;

                    _allTriangles.Sort((t1, t2) => t1.AvgZ.CompareTo(t2.AvgZ));

                    foreach (var tri in _allTriangles) // painters algorithm - draw by sorted way
                    {
                        CL_Main.WINDOW.Draw(tri.Vertices);
                    }
                }
            }
        }

        private static void PrepareTriangles(Obj obj)
        {
            Camera.Normalize(LIGHT_DIRECTION); // normalize the light dir

            Vector3f[] points = obj.Points;
            Vector3f[] rotated = new Vector3f[points.Length];
            Vector2f[] screened = new Vector2f[points.Length];
            bool[] visible = new bool[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                Vector3 camP = Vector3.Transform(new Vector3(points[i].X, points[i].Y, points[i].Z), Camera.CAMERA);
                rotated[i] = new Vector3f(camP.X, camP.Y, camP.Z);

                if (camP.Z < -0.1f)
                {
                    float sX = (camP.X / -camP.Z) * 500 + (CL_Main.WINDOW_WIDTH / 2);
                    float sY = (camP.Y / -camP.Z) * 500 + (CL_Main.WINDOW_HEIGHT / 2);
                    screened[i] = new Vector2f(sX, sY);
                    visible[i] = true;
                }
            }

            // adding triangles to the list
            // * i didnt write that
            // 22:14, 07.03.2026
            // I have my brain fried first trying (and failing) to do the Z-Buffer raster,
            // and now trying to do the painter's algorithm.
            // Please forgive me for pasting this code from AI.
            if (obj.FillType == Obj.FillTypes.Fill)
            {
                for (int i = 0; i < obj.Triangles.Length; i += 3)
                {
                    VertexArray va = new VertexArray(PrimitiveType.Triangles, 3);

                    int i1 = obj.Triangles[i], i2 = obj.Triangles[i + 1], i3 = obj.Triangles[i + 2];
                    if (!visible[i1] || !visible[i2] || !visible[i3]) continue;

                    // Backface Culling
                    Vector3f v1 = rotated[i2] - rotated[i1];
                    Vector3f v2 = rotated[i3] - rotated[i1];
                    Vector3f normal = new(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X);
                    if (normal.X * rotated[i1].X + normal.Y * rotated[i1].Y + normal.Z * rotated[i1].Z <= 0) continue;

                    // lighting section
                    Vector3 normal3 = new Vector3(normal.X, normal.Y, normal.Z);
                    normal3 = Camera.Normalize(normal3);

                    Vector3 lightCam = Vector3.TransformNormal(LIGHT_DIRECTION, Camera.CAMERA);
                    lightCam = Camera.Normalize(lightCam);

                    float diffuse = Math.Max(0, Vector3.Dot(normal3, lightCam)) * 2.5f; // <-- strenght of light
                    float brightness = 0.1f + diffuse; // base brightness
                    brightness = Math.Min(brightness, 1f); // final brightness

                    byte ColorR = obj.ObjectColor.R;
                    byte ColorG = obj.ObjectColor.G;
                    byte ColorB = obj.ObjectColor.B;

                    byte finalColorR = (byte)(ColorR * brightness);
                    byte finalColorG = (byte)(ColorG * brightness);
                    byte finalColorB = (byte)(ColorB * brightness);

                    Color color = new Color(finalColorR, finalColorG, finalColorB);

                    va[0] = new Vertex(screened[i1], color);
                    va[1] = new Vertex(screened[i2], color);
                    va[2] = new Vertex(screened[i3], color);

                    _allTriangles.Add(new TriangleToDraw
                    {
                        Vertices = va,
                        AvgZ = (rotated[i1].Z + rotated[i2].Z + rotated[i3].Z) / 3f
                    });
                }
            }
        }

        public static void DrawAllObjects()
        {
            foreach (var element in OBJECTS_IN_WORLD)
            {
                DrawObject(element);
            }
        }

    }

    public class Camera
    {
        public static Vector3 CAMERA_POSITION = new(0, 0, 6);
        public static Vector3 CAMERA_VIEW = new(0, 0, 0);
        public static Vector3 CAMERA_UP = new(0, 1, 0);

        public static Vector3 CAMERA_FORWARD = CAMERA_VIEW - CAMERA_POSITION;

        public static Vector3 CAMERA_SIDES = Vector3.Cross(CAMERA_FORWARD, CAMERA_UP);

        public static float CAMERA_SPEED = 0.1f;

        public static Matrix4x4 CAMERA = Matrix4x4.CreateLookAt(
            CAMERA_POSITION,
            CAMERA_VIEW,
            CAMERA_UP
        );

        public static void Update() {
            CAMERA_FORWARD = Normalize(CAMERA_VIEW - CAMERA_POSITION);

            CAMERA_SIDES = Normalize(Vector3.Cross(CAMERA_FORWARD, CAMERA_UP));

            CAMERA = Matrix4x4.CreateLookAt(CAMERA_POSITION, CAMERA_VIEW, CAMERA_UP);
        }

        internal static Vector3 Normalize(Vector3 fwd) {
            float length = (float)Math.Sqrt(
                fwd.X * fwd.X +
                fwd.Y * fwd.Y +
                fwd.Z * fwd.Z
            );

            fwd.X /= length;
            fwd.Y /= length;
            fwd.Z /= length;

            return new Vector3(fwd.X, fwd.Y, fwd.Z);
        }
    }
}