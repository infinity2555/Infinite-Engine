using System;

namespace NS_BaseAssets
{
    public struct Obj
    {
        public enum FillTypes
        {
            Dots,
            Wireframe,
            Fill
        }
        public enum ObjectTypes
        {
            Cube,
        }

        public Vector3f Position { get; set; }
        public FillTypes FillType{ get; set; }
        public ObjectTypes Type { get; set; }
        public Color ObjectColor { get; set; }
        public Vector3f[] Points { get; private set; }
        public int[] Indices { get; private set; }
        public int[] Triangles { get; private set; }

        public Obj(Vector3f position, ObjectTypes objecttype, FillTypes filltype, Color color) // creating an object
        {
            Position = position;
            Type = objecttype;
            FillType = filltype;
            ObjectColor = color;

            Points = Functions.ReturnPoints(objecttype);   // return POINTS
            Indices = Functions.ReturnIndices(objecttype); // return INDICES
            Triangles = Functions.ReturnTriangles(objecttype); // return TRIANGLES


            Points = Functions.AllocatePointsWithPosition(Points, position);
        }
    }

    static class Functions
    {
        public static Vector3f[] AllocatePointsWithPosition(Vector3f[] points, Vector3f position)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X += position.X;
                points[i].Y += position.Y;
                points[i].Z += position.Z;
            }

            return points;
        }

        public static int[] ReturnTriangles(Obj.ObjectTypes objtype)
        {
            return objtype switch
            {
                Obj.ObjectTypes.Cube => Vertices_Presets.CUBE_TRIANGLES_PRESET,
                _ => Vertices_Presets.CUBE_TRIANGLES_PRESET
            };
        }

        public static Vector3f[] ReturnPoints(Obj.ObjectTypes objtype)
        {
            var preset = objtype switch
            {
                Obj.ObjectTypes.Cube => Vertices_Presets.CUBE_POINTS_PRESET,
                _ => Vertices_Presets.CUBE_POINTS_PRESET
            };

            Vector3f[] newPoints = new Vector3f[preset.Length];
            Array.Copy(preset, newPoints, preset.Length);
            return newPoints;
        }


        public static int[] ReturnIndices(Obj.ObjectTypes objtype)
        {
            return objtype switch
            {
                Obj.ObjectTypes.Cube => Vertices_Presets.CUBE_INDICES_PRESET,
                _                    => Vertices_Presets.CUBE_INDICES_PRESET
            };
        }
    }

    static class Vertices_Presets
    {
        /* === === === CUBE SECTION === === === */

        public static Vector3f[] CUBE_POINTS_PRESET =
        {
            // down
            new (-1, -1, -1),
            new (1, -1, -1),
            new (1, -1, 1),
            new (-1, -1, 1),

            // up
            new (-1, 1, -1),
            new (1, 1, -1),
            new (1, 1, 1),
            new (-1, 1, 1),
        };

        public static int[] CUBE_INDICES_PRESET =
        {
           0, 1,
           1, 2,
           2, 3,
           3, 0,
           
           4, 5,
           5, 6,
           6, 7,
           7, 4,

           0, 4,
           1, 5,
           2, 6,
           3, 7,
        };

        public static int[] CUBE_TRIANGLES_PRESET =
        {
           // front (Z = -1) - points 0,1,5,4
            0, 1, 5,
            5, 4, 0,

            // back (Z = 1) - points 2,3,7,6
            2, 3, 7,
            7, 6, 2,

            // left (X = -1) - points 3,0,4,7
            3, 0, 4,
            4, 7, 3,

            // right(X = 1) - points 1,2,6,5
            1, 2, 6,
            6, 5, 1,

            // up (Y = 1) - points 4,5,6,7
            4, 5, 6,
            6, 7, 4,

            // down (Y = -1) - points 3,2,1,0
            3, 2, 1,
            1, 0, 3
        };
    }
}