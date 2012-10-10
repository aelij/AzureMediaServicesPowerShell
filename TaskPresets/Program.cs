using System;
using Two10.MediaServices;

namespace TaskPresets
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var taskPreset in Constants.taskPresets)
            {
                Console.WriteLine(taskPreset);
            }
        }
    }
}
