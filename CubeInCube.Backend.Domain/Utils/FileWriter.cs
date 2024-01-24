using CubeInCube.Backend.Domain.Entities.BasicElements;

namespace CubeInCube.Backend.Domain.Utils
{
    public class FileWriter
    {
        private string FilePath { get; set; } = $"C://logsShapeInShape/log.txt";
        private string Folder { get; set; } = $"C://logsShapeInShape";
        private string Separator { get; set; } = ";".ToString();
        public int Count { get; set; } = 0;

        public void Write(Position center, Dimension dimension)
        {
            if (!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }

            FilePath = $"C://logsShapeInShape/log{Count}.txt";
            using (StreamWriter fileStream = File.Exists(FilePath) ? File.AppendText(FilePath) : File.CreateText(FilePath))
            {
                fileStream.WriteLine($"{center.X}{Separator} {center.Y}{Separator} {center.Z}{Separator} {dimension.Length}{Separator} ");
            }

        }
    }
}
