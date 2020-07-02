using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSimulation
{
    class Pheromone : GameObject
    {
        public static void SpawnOn(World world, PointF pos, double intensity = 100)
        {
            if (intensity > 100) { intensity = 100; }

            List<Pheromone> neighbours = PheromonesNear(pos, world);
            //List<Pheromone> neighbours = world.PheromonesTestNear(pos)
            //    .Select(each => each as Pheromone)
            //    .Where(p => p != null).ToList();

            if (neighbours.Count == 0)//Solo si la cantidad de vecinos es 0 deberia ejecutarse.
            {
                // No objects there
                Pheromone clone = new Pheromone(intensity);//Clona 
                clone.Position = pos;
                pheromones[(int)pos.X,(int)pos.Y]=clone;
                //if (world.IsInside(clone.Position))
                //{
                world.Add(clone);
                //}
                //else
                //{
                //    clone.Position = Mod(clone.Position, world.size);
                //    world.Add(clone);
                //}
            }
            else
            {
                foreach (Pheromone p in neighbours)
                {
                    if (p.Intensity < intensity)
                    {
                        p.Intensity = intensity;
                    }
                }
            }
        }

        public static GameObject[,] pheromones = new GameObject[125,125];

        public static List<Pheromone> PheromonesNear(PointF pos,World world)
        {
            List<Pheromone> nearPheromones = new List<Pheromone>();
            //if (pos.X > 1 && pos.X < 124 && pos.Y > 1 && pos.Y < 124)
            //{
            pos = Mod(pos, world.size);

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if((int)pos.X+x>0&& (int)pos.X + x < 125)
                    {
                        if ((int)pos.Y + y > 0 && (int)pos.Y + y < 125)
                        {
                            Pheromone pheromoneTest = (Pheromone)pheromones[(int)pos.X + x, (int)pos.Y + y];
                            if (pheromoneTest != null)
                            {
                                if (world.Dist(pos, pheromoneTest.Position) < 1)
                                {
                                    nearPheromones.Add(pheromoneTest);
                                }
                            }
                        }
                    }
                }
            }
            //}
            return nearPheromones;
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

        //public static GameObject[,] pheromones=new GameObject[125,125];

        //public static Pheromone[] PheromonesNear(Point pos)
        //{
        //    Pheromone[] nearPheromones;
        //    for (int x = -1; x < 2; x++)
        //    {
        //        for (int y = -1; y < 2; y++)
        //        {
        //            if(pheromones[x,y])
        //        }
        //    }
        //    return nearPheromones;
        //}

        private double intensity;

        public Pheromone(double intensity = 100)
        {
            this.intensity = intensity;
            UpdateColor();
        }

        public double Intensity
        {
            get { return intensity; }
            set { intensity = value; }
        }

        public override void UpdateOn(World world)
        {
            if (intensity <= 1)
            {
                world.Remove(this);
            }
            else
            {
                SpreadOn(world);

                intensity *= 0.9f;
                UpdateColor();
            }
        }

        private void UpdateColor()
        {
            Color = Color.FromArgb((int)Math.Floor(intensity / 100 * 255), 255, 255, 0);
        }

        private void SpreadOn(World world)
        {
            float radius = 2;
            for (float x = Position.X - radius; x <= Position.X + radius; x++)
            {
                for (float y = Position.Y - radius; y <= Position.Y + radius; y++)
                {
                    if (x == Position.X && y == Position.Y) continue;
                    double squaredDist = Math.Pow(x - Position.X, 2) + Math.Pow(y - Position.Y, 2);
                    if (squaredDist <= radius)
                    {
                        double diffuse = 0.75;
                        PointF point = new PointF(x, y);
                        point = Mod(point, world.size);
                        SpawnOn(world, point, intensity * diffuse);
                    }
                }
            }
        }
    }
}
