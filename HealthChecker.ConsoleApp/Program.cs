using System.Diagnostics;
using System.Linq;

namespace HealthChecker.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new HealthCheckerService();
            service.Run();
        }
    }
}
