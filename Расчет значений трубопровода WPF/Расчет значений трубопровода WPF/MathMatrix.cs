using System;

namespace Расчет_значений_трубопровода_WPF
{
    class MathMatrix
    {
        /// <summary>
        /// транспонирование матрицы
        /// </summary>
        /// <param name="matix">матрица</param>
        /// <returns></returns>
        public double[,] Transpose(double[,] matix)
        {
            double[,] result = new double[matix.Length / (matix.GetUpperBound(0) + 1), matix.GetUpperBound(0) + 1];
            for (int i = 0; i < result.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < result.Length / (result.GetUpperBound(0) + 1); j++)
                {
                    result[i, j] = matix[j, i];
                }
            }
            return result;
        }

        /// <summary>
        /// умножение матрицы на вектор
        /// </summary>
        /// <param name="m1">матрица</param>
        /// <param name="m2">вектор</param>
        /// <returns></returns>
        public double[] MatrixMultiplication(double[,] m1, double[] m2)
        {
            double[] result = new double[m2.Length];

            for (int i = 0; i < m1.GetUpperBound(0) + 1; i++)
            {
                result[i] = 0;
                for (int k = 0; k < m1.Length / (m1.GetUpperBound(0) + 1); k++)
                    result[i] += m1[i, k] * m2[k];
            }
            return result;
        }
        /// <summary>
        /// умножение матрицы на число
        /// </summary>
        /// <param name="matix">матрица</param>
        /// <param name="number">число</param>
        /// <returns></returns>
        public double[,] MultiplicationByNumber(double[,] matix, double number)
        {
            double[,] result = new double[matix.GetUpperBound(0) + 1, matix.Length / (matix.GetUpperBound(0) + 1)];
            for (int i = 0; i < matix.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < matix.Length / (matix.GetUpperBound(0) + 1); j++)
                {
                    result[i, j] = matix[i, j] * number;
                }
            }
            return result;
        }

        /// <summary>
        /// получение определителя через минор
        /// </summary>
        /// <param name="matrix">матрица</param>
        /// <returns></returns>
        public double Det(double[,] matrix)
        {
            double det = 0;
            if (matrix.Length == 1)
                return matrix[0, 0];
            else
                for (int i = 0; i < matrix.GetUpperBound(0) + 1; i++)
                {
                    det += matrix[0, i] * Math.Pow(-1, i) * Det(SmallerMatrix(matrix, 0, i));
                }
            return det;
        }
        /// <summary>
        /// получение матрицы минора
        /// </summary>
        /// <param name="matrix">матрица</param>
        /// <param name="row">число строк</param>
        /// <param name="col">число столбцов</param>
        /// <returns></returns>
        public double[,] SmallerMatrix(double[,] matrix, int row, int col)
        {
            double[,] smallMatrix = new double[matrix.GetUpperBound(0), matrix.Length / (matrix.GetUpperBound(0) + 1) - 1];
            int countRow = 0;
            for (int i = 0; i < smallMatrix.GetUpperBound(0) + 1; i++)
            {
                if (i == row)
                    countRow = 1;
                int countCol = 0;
                for (int j = 0; j < smallMatrix.Length / (smallMatrix.GetUpperBound(0) + 1); j++)
                {
                    if (j == col)
                        countCol = 1;
                    smallMatrix[i, j] = matrix[i + countRow, j + countCol];
                }
            }
            return smallMatrix;
        }
        /// <summary>
        /// обратнfz матрицу
        /// </summary>
        /// <param name="matrix">матрица</param>
        /// <returns></returns>
        public double[,] InverseMatrix(double[,] matrix)
        {
            if (Det(matrix) != 0)
                return MultiplicationByNumber(AttachedMatrix(matrix), 1 / Det(matrix));
            else
                return matrix;
        }
        /// <summary>
        /// алгебаическое дополнение
        /// </summary>
        /// <param name="matrix">матрица</param>
        /// <returns></returns>
        public double[,] AttachedMatrix(double[,] matrix)
        {
            double[,] attachedMatrix = new double[matrix.GetUpperBound(0) + 1, matrix.Length / (matrix.GetUpperBound(0) + 1)];
            for (int i = 0; i < attachedMatrix.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < attachedMatrix.Length / (attachedMatrix.GetUpperBound(0) + 1); j++)
                {
                    attachedMatrix[i, j] = Det(SmallerMatrix(matrix, i, j)) * Math.Pow(-1, i + j);
                }
            }
            return Transpose(attachedMatrix);
        }
    }
}
