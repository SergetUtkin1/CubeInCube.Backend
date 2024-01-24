using CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.Shapes;
using CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.AbstractProducts;
using CubeInCube.Backend.Domain.Entities.BasicElements;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using CubeInCube.Backend.Domain.Entities.AbstractFactory;
using CubeInCube.Backend.Domain.Entities.Models.AbstractFactory.Products.Shapes;

namespace ShapesInShape.ConsoleApplication.Models.AbstractFactory.ConcreteFactories
{
    public class CylinderInParallelepipedFactory : CaseFactory
    {
        public Cylinder[] InnerShapes { get; set; } = null!;
        public Parallelepiped BoundingShape { get; set; } = null!;
        private Matrix<double> MV0 = null!;

        public override void CreateBoundingShape(Dimension dimension) =>
            BoundingShape = new Parallelepiped(new Position(), dimension.Length, dimension.Width, dimension.Heigth);

        public override void CreateInnerShape(Position center, Dimension dimension)
        {
            MV0 = SurfCil0(dimension.Width, dimension.Heigth, 100);
            var сylinder = new Cylinder(center, dimension.Length, dimension.Width, dimension.Heigth, dimension.Theta, dimension.Fi);
            InnerShapes[_currentIndex] = сylinder;
        }
       
        public override void SetCountOfInnerShapes(int count)
        {
            InnerShapes = new Cylinder[count];
        }

        public override Position CreatePoint(Distribution distributionOfPosition)
        {
            double x, y, z;
            Position position;

            do
            {
                x = distributionOfPosition.GetValue(BoundingShape.Center.X - 0.5 * BoundingShape.Dimension.Length, BoundingShape.Center.X + 0.5 * BoundingShape.Dimension.Length);
                y = distributionOfPosition.GetValue(BoundingShape.Center.Y - 0.5 * BoundingShape.Dimension.Width, BoundingShape.Center.Y + 0.5 * BoundingShape.Dimension.Width);
                z = distributionOfPosition.GetValue(BoundingShape.Center.Z - 0.5 * BoundingShape.Dimension.Heigth, BoundingShape.Center.Z + 0.5 * BoundingShape.Dimension.Heigth);
                position = new Position(x, y, z);
            } while (!CheckPointInsideBounding(position));

            return position;
        }

        public override Shape[] GetArrayOfInnerShapes(Dimension[] dimensions, bool isSortingEnable)
        {
            var spheres = new Cylinder[dimensions.Length];

            for (int i = 0; i < dimensions.Length; i++)
            {
                spheres[i] = new Cylinder(new Position(),
                                        dimensions[i].Length,
                                        dimensions[i].Width,
                                        dimensions[i].Heigth,
                                        dimensions[i].Theta,
                                        dimensions[i].Fi);
            }

            if (isSortingEnable)
                Array.Sort(spheres, (a, b) => ((int)(b.Volume - a.Volume)));

            return spheres;
        }

        public override bool CheckIntersection()
        {
            var flag = false;
            var shape = InnerShapes[_currentIndex];

            if (HasIntersectionWithBound(shape))
            {
                flag = true;
            }
            else
            {
                for (int i = 0; i < _currentIndex; i++)
                {
                    if (HasIntersectionWithOtherShape(shape, InnerShapes[i]))
                    {
                        flag = true;
                        break;
                    }
                }
            }

            return flag;
        }

        protected override bool HasIntersectionWithBound(Shape shape)
        {
            double radius = 0.5 * shape.Dimension.Width;
            double halfHeight = 0.5 * shape.Dimension.Heigth;

            // Вычисляем половинные размеры параллелепипеда
            double halfWidth = 0.5 * BoundingShape.Dimension.Width;
            double halfHeightP = 0.5 * BoundingShape.Dimension.Heigth;
            double halfDepth = 0.5 * BoundingShape.Dimension.Length;

            // Проверяем пересечение по каждой оси
            bool intersectX = Math.Abs(MV0.Row(0).Enumerate().Max()) >= halfWidth + radius;
            bool intersectY = Math.Abs(MV0.Row(1).Enumerate().Max()) >= halfHeightP + radius;
            bool intersectZ = Math.Abs(MV0.Row(2).Enumerate().Max()) >= halfDepth + halfHeight;

            // Возвращаем true, если есть пересечение по любой из осей
            return intersectX || intersectY || intersectZ;
        }

        protected override bool HasIntersectionWithOtherShape(Shape shape, Shape otherShape)
        {
            var flag = false;
            var сil1cord = CreateVector.DenseOfArray(new double[3] { shape.Center.X, shape.Center.Y, shape.Center.Z});
            var cil2cord = CreateVector.DenseOfArray(new double[3] { otherShape.Center.X, otherShape.Center.Y, otherShape.Center.Z });

            double R = 0.5 * shape.Dimension.Width;
            double hH = 0.5 * shape.Dimension.Heigth;
            double InterCenterDistance = (cil2cord - сil1cord).L2Norm();

            if (InterCenterDistance > Math.Sqrt(Math.Pow(shape.Dimension.Width, 2) + Math.Pow(shape.Dimension.Heigth, 2)))
                return flag;

            Matrix<double> Msurf2 = SurfCilRotD(MV0, shape.Dimension.Theta, shape.Dimension.Fi);
            Msurf2.SetSubMatrix(2, 0, Msurf2.SubMatrix(2, 1, 0, Msurf2.ColumnCount) + InterCenterDistance);

            Matrix<double> Msurf2rot = SurfCilRotDRev(Msurf2, otherShape.Dimension.Theta, otherShape.Dimension.Fi);

            for (int i = 0; i < Msurf2rot.ColumnCount; i++)
            {
                double x = Msurf2rot[0, i];
                double y = Msurf2rot[1, i];
                double z = Msurf2rot[2, i];

                if (Math.Pow(x, 2) + Math.Pow(y, 2) >= Math.Pow(R, 2))
                    continue;

                if (Math.Pow(z, 2) <= Math.Pow(hH, 2))
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }

        private Matrix<double> SurfCilRotDRev(Matrix<double> MV, double ZenitAngle, double Azimuth)
        {
            int noCs = MV.ColumnCount;

            Matrix<double> Mrot = Matrix<double>.Build.Dense(3, noCs);
            Mrot.SetSubMatrix(0, 0, Mv(1.0, 0.0, 0.0, -ZenitAngle) * (Mv(0.0, 0.0, 1.0, -Azimuth) * MV.SubMatrix(0, MV.RowCount, 0, noCs)));

            return Mrot;
        }

        protected override bool CheckPointInsideBounding(Position position)
        {
            var flag = false;
            var xPlanes = (BoundingShape.Center.X - 0.5 * BoundingShape.Dimension.Length, BoundingShape.Center.X + 0.5 * BoundingShape.Dimension.Length);
            var yPlanes = (BoundingShape.Center.Y - 0.5 * BoundingShape.Dimension.Width, BoundingShape.Center.Y + 0.5 * BoundingShape.Dimension.Width);
            var zPlanes = (BoundingShape.Center.Z - 0.5 * BoundingShape.Dimension.Heigth, BoundingShape.Center.Z + 0.5 * BoundingShape.Dimension.Heigth);

            var xCondition = (xPlanes.Item1 < position.X && position.X < xPlanes.Item2);
            var yCondition = (yPlanes.Item1 < position.Y && position.Y < yPlanes.Item2);
            var zCondition = (zPlanes.Item1 < position.Z && position.Z < zPlanes.Item2);
            if (xCondition && yCondition && zCondition)
            {
                flag = true;
            }

            return flag;
        }

        protected Matrix<double> SurfCil0(double D, double H, int N)
        {
            double R = 0.5 * D;
            double S = Math.PI * (2 * R * R + 2 * R * H); // площадь поверхности цилиндра
            double dS = S / N; // приблизительный размер элементарной площадки
            double No = Math.Round(Math.PI * R * R / dS); // приблизительное количество точек на основании
            int Nr = (int)Math.Round(Math.Sqrt(No)); // количество точек по радиусу (на основании)
            double Nb = Math.Round(2 * Math.PI * R * H / dS); // количество точек на боковой поверхности
            double[] r = new double[Nr];
            for (int i = 0; i < Nr; i++)
            {
                r[i] = R * (i + 1) / Nr; // радиусы точек на основании
            }

            List<double> Vox1 = new List<double>();
            List<double> Voy1 = new List<double>();
            List<double> Voz1 = new List<double>();
            foreach (double rr in r)
            {
                int Nf = (int)Math.Round(2 * Nr * rr / R); // количество точек по углу на данном радиусе основании
                double[] f = new double[Nf]; // углы точек на данном радиусе основании
                for (int i = 0; i < Nf; i++)
                {
                    f[i] = 2 * Math.PI * i / Nf;
                    Vox1.Add(rr * Math.Cos(f[i])); // координаты точек на верхнем основании
                    Voy1.Add(rr * Math.Sin(f[i]));
                    Voz1.Add(0.5 * H);
                }
            }

            double[] F = new double[2 * Nr]; // углы точек на боковой поверхности
            for (int i = 0; i < 2 * Nr; i++)
            {
                F[i] = 2 * Math.PI * i / (2 * Nr);
            }

            int Nh = (int)Math.Round(0.5 * Nb / (2 * Nr)); // количество точек по высоте на верхней половине боковой поверхности
            double[] h = new double[Nh]; // z-координаты точек на верхней половине боковой поверхности
            for (int i = 0; i < Nh; i++)
            {
                h[i] = 0.25 * H + (0.5 * H - 0.5 * H / Nh) * i / (Nh - 1);
            }

            List<double> Vox2 = new List<double>(new double[2 * Nr * Nh]);
            List<double> Voy2 = new List<double>(new double[2 * Nr * Nh]);
            List<double> Voz2 = new List<double>(new double[2 * Nr * Nh]);
            for (int i = 0; i < 2 * Nr * Nh; i++)
            {
                int FIndex = i - 2 * Nr * (i / (2 * Nr));
                Vox2[i] = R * Math.Cos(F[FIndex]); // координаты точек на верхней половине боковой поверхности
                Voy2[i] = R * Math.Sin(F[FIndex]);
                Voz2[i] = h[i / (2 * Nr)];
            }   

            double[] Vox12 = Vox1.ToArray().Concat(Vox2.ToArray()).ToArray(); // соединение координат точек на верхнем основании и верхней половине боковой поверхности
            double[] Voy12 = Voy1.ToArray().Concat(Voy2.ToArray()).ToArray();
            double[] Voz12 = Voz1.ToArray().Concat(Voz2.ToArray()).ToArray();

            double[] Vox = Vox12.Concat(Vox12.ToArray()).ToArray(); // соединение координат точек на верхней и нижней половинах поверхности
            double[] Voy = Voy12.Concat(Voy12.ToArray()).ToArray();
            double[] Voz = Voz12.Concat(Voz12.Select(v => -v).ToArray()).ToArray();

            Matrix<double> surfacePoints = Matrix<double>.Build.Dense(3, Vox.Length);
            for (int i = 0; i < Vox.Length; i++)
            {
                surfacePoints[0, i] = Vox[i];
                surfacePoints[1, i] = Voy[i];
                surfacePoints[2, i] = Voz[i];
            }

            return surfacePoints;
        }

        private Matrix<double> Mv(double x, double y, double z, double ZenitAngle)
        {
            double cosZenitAngle = Math.Cos(ZenitAngle);
            double sinZenitAngle = Math.Sin(ZenitAngle);

            return Matrix<double>.Build.DenseOfArray(new double[,]
            {
            { cosZenitAngle + (1 - cosZenitAngle) * Math.Pow(x, 2), (1 - cosZenitAngle) * x * y - sinZenitAngle * z, (1 - cosZenitAngle) * x * z + sinZenitAngle * y },
            { (1 - cosZenitAngle) * x * y + sinZenitAngle * z, cosZenitAngle + (1 - cosZenitAngle) * Math.Pow(y, 2), (1 - cosZenitAngle) * y * z - sinZenitAngle * x },
            { (1 - cosZenitAngle) * x * z - sinZenitAngle * y, (1 - cosZenitAngle) * y * z + sinZenitAngle * x, cosZenitAngle + (1 - cosZenitAngle) * Math.Pow(z, 2) }
            });
        }

        private Matrix<double> SurfCilRotD(Matrix<double> MV, double θ, double φ)
        {
            int noCs = MV.ColumnCount;

            Matrix<double> Mrot = Matrix<double>.Build.Dense(3, noCs);
            Mrot.SetSubMatrix(0, 0, Mv(0.0, 0.0, 1.0, φ) * (Mv(1.0, 0.0, 0.0, θ) * MV.SubMatrix(0, MV.RowCount, 0, noCs)));

            return Mrot;
        }
    } 
}
