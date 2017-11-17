using System;
using System.Collections.Generic;
using System.Text;

namespace Exodus.Cli.SqlServer
{
    class View
    {
        public void Render(ViewModel viewModel)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success.");
            Console.ResetColor();
            var duration = viewModel.EndTime - viewModel.StartTime;
            Console.WriteLine($"Duration: {duration}");
            Console.WriteLine("Operations:");
            foreach (var log in viewModel.Logs)
            {
                Console.WriteLine($"  {log}");
            }
        }
    }
}
