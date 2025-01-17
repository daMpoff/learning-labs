using System;
using System.Collections.Generic;

namespace SimplexMethod;

public class Program
{
    public static void Main()
    {
        double[] objectiveFunction = new double[3] { 4, 1, 1 };
        double[,] systemLimits = new double[2, 4]
        {
            { -1, 4, 3, 6 },
            { 1, 1, 2, 4 }
        };
        SimplexMethod simplexMethod = new SimplexMethod(objectiveFunction, systemLimits);
        List<double> results = simplexMethod.Calculate();
        foreach(var result in results)
        {
            Console.WriteLine(result);
        }
    }
}