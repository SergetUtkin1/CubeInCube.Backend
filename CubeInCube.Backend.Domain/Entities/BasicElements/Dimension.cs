namespace CubeInCube.Backend.Domain.Entities.BasicElements
{
    public class Dimension
    {
        public double Length { get; set; }
        public double Width { get; set; }
        public double Heigth { get; set; }
        public double Theta { get; set; } = 0.0;
        public double Fi { get; set; } = 0.0;

        public Dimension()
        {

        }

        public Dimension(double length, double width, double heigth, double theta, double fi)
        {
            Length = length;
            Width = width;
            Heigth = heigth;
            Theta = theta;
            Fi = fi;
        }

        public Dimension(double width, double heigth, double theta, double fi)
        {
            Length = width;
            Width = width;
            Heigth = heigth;
            Theta = theta;
            Fi = fi;
        }

        public Dimension(double length, double width, double heigth)
        {
            Length = length;
            Width = width;
            Heigth = heigth;
        }

        public Dimension(double length)
        {
            Length = length;
            Width = length;
            Heigth = length;
        }
    }
}
