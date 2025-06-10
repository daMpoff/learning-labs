using System;
using System.Collections.Generic;

namespace NumericalDifferentiation;

public class NewtonDifferentiator : IDiscreteDifferentiator
{
    public string Name => "Производная Ньютона (разности)";
    // Минимум 2 точки, но формула работает тем точнее, чем больше точек
    public int RequiredPoints => 2;

    public double Differentiate(IReadOnlyList<(double x, double y)> points, double x, out List<int> usedIndices)
    {
        if (points.Count < 2)
            throw new ArgumentException("Требуется минимум 2 точки");

        // Проверяем равномерность шага
        double h = points[1].x - points[0].x;
        for (int i = 1; i < points.Count - 1; i++)
            if (Math.Abs(points[i + 1].x - points[i].x - h) > 1e-8)
                throw new ArgumentException("Шаг по x должен быть постоянным для формулы Ньютона!");

        double x0 = points[0].x;
        double q = (x - x0) / h;

        // Собираем конечные разности до максимально возможного порядка
        int n = points.Count;
        var deltas = new List<double[]> { new double[n] };
        for (int i = 0; i < n; i++)
            deltas[0][i] = points[i].y;

        for (int order = 1; order < n; order++)
        {
            double[] prev = deltas[order - 1];
            double[] curr = new double[n - order];
            for (int i = 0; i < curr.Length; i++)
                curr[i] = prev[i + 1] - prev[i];
            deltas.Add(curr);
        }

        // Суммируем слагаемые по формуле
        double res = 0;
        double[] coeffs = new double[n];
        if (n > 1) coeffs[1] = (2 * q - 1) / 2;
        if (n > 2) coeffs[2] = (3 * q * q - 6 * q + 2) / 6;
        if (n > 3) coeffs[3] = (4 * Math.Pow(q, 3) - 18 * q * q + 22 * q - 6) / 24;

        // базовое слагаемое (Δy0)
        res += deltas[1][0];

        // остальные слагаемые
        for (int k = 2; k < n; k++)
        {
            // формулы выше 4-го порядка можно легко расширить, если нужно
            double c = 0;
            if (k == 2) c = coeffs[1];
            else if (k == 3) c = coeffs[2];
            else if (k == 4) c = coeffs[3];
            else
            {
                // дальше можно выразить через обобщённую формулу, если нужно
                break; // можно реализовать больше порядков по необходимости
            }
            res += c * deltas[k][0];
        }

        res /= h;
        usedIndices = Enumerable.Range(0, Math.Min(5, n)).ToList(); // выделить первые точки (сколько реально использовано)
        return res;
    }
}
