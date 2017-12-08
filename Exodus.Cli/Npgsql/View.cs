using System;
using System.Collections.Generic;
using System.Text;

namespace Exodus.Cli.Npgsql
{
    class View
    {
        public void Render(ViewModel viewModel)
        {
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
