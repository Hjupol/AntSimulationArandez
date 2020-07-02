using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSimulation
{
    class World
    {
        private Random rnd = new Random();

        private const int width = 125;
        private const int height = 125;
        public Size size = new Size(width, height);
        private HashSet<GameObject> objects = new HashSet<GameObject>();

        private List<GameObject>[,] spatialObjects = new List<GameObject>[width,height];
        private int cellSize = 1;
        private Pen pen = new Pen(Color.Wheat);
        

        public IEnumerable<GameObject> GameObjects { get { return objects.ToArray(); } }

        public int Width { get { return width; } }
        public int Height { get { return height; } }

        public PointF Center { get { return new PointF(width / 2, height / 2); } }

        public bool IsInside(PointF p)
        {
            return p.X >= 0 && p.X < width
                && p.Y >= 0 && p.Y < height;
        }
        
        public PointF RandomPoint()
        {
            return new PointF(rnd.Next(width), rnd.Next(height));
        }

        public float Random()
        {
            return (float)rnd.NextDouble();
        }

        public float Random(float min, float max)
        {
            return (float)rnd.NextDouble() * (max - min) + min;
        }

        public void Add(GameObject obj)
        {
            objects.Add(obj);
            if (!IsInside(obj.Position))
            {
                obj.Position = Mod(obj.Position, size);
            }
            GetBucketAt(obj.Position).Add(obj);
        }

        public void Remove(GameObject obj)
        {
            objects.Remove(obj);

            GetBucketAt(obj.Position).Remove(obj);
        }

        public void Update()
        {
            foreach (GameObject obj in GameObjects)
            {
                var oldPosition = obj.Position;

                obj.InternalUpdateOn(this);
                obj.Position = Mod(obj.Position, size);

                if (oldPosition != obj.Position)
                {
                    GetBucketAt(oldPosition).Remove(obj);
                    GetBucketAt(obj.Position).Add(obj);
                }
            }
        }

        public void DrawOn(Graphics graphics)
        {
            graphics.FillRectangle(Brushes.White, 0, 0, width, height);
            foreach (GameObject obj in GameObjects)
            {
                pen.Color = obj.Color;
                graphics.FillRectangle(pen.Brush, obj.Bounds);
            }
        }

        public double Dist(PointF a, PointF b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        public double Dist(float x1, float y1, float x2, float y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        public List<GameObject> GetBucketAt(PointF pos)
        {
            if (IsInside(pos))
            {
                int convertedPosX = (int)(pos.X / cellSize);
                int convertedPosY = (int)(pos.Y / cellSize);
                var bucket = spatialObjects[convertedPosX, convertedPosY];
                if (bucket == null)
                {
                    bucket = new List<GameObject>();
                    spatialObjects[convertedPosX, convertedPosY] = bucket;
                }
                return bucket;
            }
            else
            {
                return null;
            }
        }

        public List<GameObject> GetBucketAt(float x, float y)
        {
            if (IsInside(new PointF(x,y)))
            {
                int convertedPosX = (int)(x / cellSize);
                int convertedPosY = (int)(y / cellSize);
                var bucket = spatialObjects[convertedPosX, convertedPosY];
                if (bucket == null)
                {
                    bucket = new List<GameObject>();
                    spatialObjects[convertedPosX, convertedPosY] = bucket;
                }
                return bucket;
            }
            else
            {
                return null;
            }
        }

        // http://stackoverflow.com/a/10065670/4357302
        private static float Mod(float a, float n)
        {
            float result = a % n;
            if ((result < 0 && n > 0) || (result > 0 && n < 0))
                result += n;
            return result;
        }

        private static PointF Mod(PointF p, SizeF s)
        {
            return new PointF(Mod(p.X, s.Width), Mod(p.Y, s.Height));
        }
        
        public IEnumerable<GameObject> GameObjectsNear(PointF pos, float dist = 1)
        {
            return ObjectsCloseTo(pos,dist);
        }

        public IEnumerable<GameObject> PheromonesTestNear(PointF pos, float dist = 1)
        {
            return GameObjects.Where(t => Dist(t.Position, pos) < dist);
        }

        public List<GameObject> ObjectsCloseTo(PointF pos,float dist)
        {
            List<GameObject> objectsClose = new List<GameObject>();
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    objectsClose.AddRange(CloseObj(pos, GetBucketAt(pos.X + x, pos.Y + y),dist));
                }
            }
            return objectsClose;

        }

        public List<GameObject> GetPheromonesCloseTo(PointF pos)
        {
            List<GameObject> objectsClose = new List<GameObject>();
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    GetBucketAt(pos);
                }
            }
            return objectsClose;
        }

        public List<GameObject> CloseObj(PointF position, List<GameObject> bucketObjects,float dist)
        {
            List<GameObject> objectsCloseList = new List<GameObject>();
            if (bucketObjects != null)
            {
                foreach (GameObject g in bucketObjects)
                {
                    if (Dist(position, g.Position) < dist
                        && Dist(position, g.Position) < dist)
                    {
                        objectsCloseList.Add(g);
                    }
                }
            }
            return objectsCloseList;
        }
    }
}
