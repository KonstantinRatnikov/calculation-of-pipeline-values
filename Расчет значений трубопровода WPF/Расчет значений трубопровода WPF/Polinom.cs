using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Расчет_значений_трубопровода_WPF
{
    public class Polinom
    {
        
        public double[] coeffH { get; set; } //коэффициеты полинома по горизонтали
        public double[] coeffV { get; set; }//коэффициеты полинома по вертикали

        //координаты положения трубопровода
        double[] X;
        double[] Y;
        double[] Z;

        /// <summary>
        /// коэффициенты полинома введены файлом с координатами точек
        /// </summary>
        /// <param name="path">путь к файлу</param>
        public Polinom(string path)
        {
            ReadCoords(path);

            coeffV = PolynomialFit(X, Y, 4);
            coeffH = PolynomialFit(X, Z, 4);
        }

        /// <summary>
        /// Коэффициенты полинома введены пользователем
        /// </summary>
        /// <param name="v">массив коэффициентов полинома по вертикали</param>
        /// <param name="h">массив коэффициентов полинома по горизонтали</param>
        public Polinom(double[] v, double[] h)
        {
            this.coeffV = v;
            this.coeffH = h;            
        }

        //Метод, заполняющий значения координат трубопровода из файла
        void ReadCoords(string path)
        {
            int n =File.ReadLines(path).Count();
            X = new double[n];
            Y = new double[n];
            Z = new double[n];

            using (StreamReader fs = new StreamReader(path))
            {
                int i = 0;
                while (true)
                {
                    // Читаем строку из файла во временную переменную.
                    var temp1 = fs.ReadLine();
                    // Если достигнут конец файла, прерываем считывание.
                    if (temp1 == null) break;

                    if (temp1[0] == 'P') temp1 = temp1.Substring(temp1.IndexOf(",") + 1);
                    var temp = temp1.Replace("\t", " ").Trim().Replace(".", ",").Split(' ').ToArray();

                    X[i] = double.Parse(temp[0].Replace(".", ","));
                    Y[i] = double.Parse(temp[1].Replace(".", ","));
                    Z[i] = double.Parse(temp[2].Replace(".", ","));

                    i++;
                }

            }
        }

        /// <summary>
        /// Метод получения коэффициентов полинома
        /// </summary>
        /// <param name="x">массив координат x</param>
        /// <param name="y">массив координат y</param>
        /// <param name="degree">степень полинома</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        double[] PolynomialFit(double[] x, double[] y, int degree)
                        {
                            if (x.Length != y.Length)
                            {
                                throw new ArgumentException("The number of elements in x and y must be the same.");
                            }

                            int n = x.Length;
                            int m = degree + 1;
                            double[,] matrix = new double[m, m];
                            double[] right = new double[m];
                            double[] coef = new double[m];

                            // Вычисление элементов матрицы
                            for (int i = 0; i < m; i++)
                            {
                                for (int j = 0; j < m; j++)
                                {
                                    double sum = 0;
                                    for (int k = 0; k < n; k++)
                                    {
                                        sum += Math.Pow(x[k], i + j);
                                    }
                                    matrix[i, j] = sum;
                                }
                            }

                            // Вычисление элементов вектора правой части
                            for (int i = 0; i < m; i++)
                            {
                                double sum = 0;
                                for (int k = 0; k < n; k++)
                                {
                                    sum += y[k] * Math.Pow(x[k], i);
                                }
                                right[i] = sum;
                            }

                            // Решите систему уравнений, используя метод Гаусса.
                            for (int k = 0; k < m - 1; k++)
                            {
                                for (int i = k + 1; i < m; i++)
                                {
                                    double factor = matrix[i, k] / matrix[k, k];
                                    right[i] -= factor * right[k];
                                    for (int j = k; j < m; j++)
                                    {
                                        matrix[i, j] -= factor * matrix[k, j];
                                    }
                                }
                            }
                            for (int k = m - 1; k >= 0; k--)
                            {
                                double sum = 0;
                                for (int j = k + 1; j < m; j++)
                                {
                                    sum += matrix[k, j] * coef[j];
                                }
                                coef[k] = (right[k] - sum) / matrix[k, k];
                            }

                            return coef.Reverse().ToArray();
                        }
    }
}
