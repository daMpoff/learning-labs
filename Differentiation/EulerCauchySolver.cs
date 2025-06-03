using System;
using System.Collections.Generic;
using MathNet.Symbolics;
using Expr = MathNet.Symbolics.SymbolicExpression;

namespace Differentiation
{
    public static class EulerCauchySolver
    {
        /// <summary>
        /// Решает задачу Коши для ОДУ n-го порядка методом Эйлера.
        /// </summary>
        /// <param name="order">Порядок ОДУ</param>
        /// <param name="rhs">Правая часть (SymbolicExpression)</param>
        /// <param name="initialConditions">Начальные условия (y0, y1, ...)</param>
        /// <param name="x0">Начальное значение x</param>
        /// <param name="xEnd">Конечное значение x</param>
        /// <param name="step">Шаг</param>
        /// <returns>Словарь: имя переменной -> список значений</returns>
        public static Dictionary<string, List<double>> Solve(
            int order,
            Expr rhs,
            List<double> initialConditions,
            double x0,
            double xEnd,
            double step)
        {
            int steps = (int)Math.Ceiling((xEnd - x0) / step) + 1;
            var result = new Dictionary<string, List<double>>();
            for (int i = 0; i < order; i++)
                result[$"y{i}"] = new List<double>(steps) { initialConditions[i] };
            var xList = new List<double>(steps) { x0 };

            // Текущее состояние
            double[] y = new double[order];
            initialConditions.CopyTo(y);
            double x = x0;

            // Подготовим переменные для подстановки
            var variables = new Dictionary<string, FloatingPoint>();

            for (int k = 1; k < steps; k++)
            {
                // y0' = y1, y1' = y2, ..., y_{n-2}' = y_{n-1}, y_{n-1}' = f(x, y0, ..., y_{n-1})
                double[] yNext = new double[order];
                for (int i = 0; i < order - 1; i++)
                    yNext[i] = y[i] + step * y[i + 1];

                // y_{n-1}' = f(x, y0, ..., y_{n-1})
                variables["x"] = x;
                for (int i = 0; i < order; i++)
                    variables[$"y{i}"] = y[i];
                double f = rhs.Evaluate(variables).RealValue;
                yNext[order - 1] = y[order - 1] + step * f;

                // Сохраняем значения
                for (int i = 0; i < order; i++)
                    result[$"y{i}"].Add(yNext[i]);
                x += step;
                xList.Add(x);
                Array.Copy(yNext, y, order);
            }
            result["x"] = xList;
            return result;
        }
    }
} 