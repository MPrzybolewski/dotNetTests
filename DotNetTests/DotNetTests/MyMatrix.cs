﻿using DotNetTests.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTests
{
    public class MyMatrix<T> where T : new()
    {
        public int rows => Matrix.GetLength(0);
        public int columns => Matrix.GetLength(1);
        public T[,] Matrix;
        public T[,] DefaultMatrix;

        public MyMatrix(IMyMatrixFileReader matrixReader)
        {
            if (typeof(T) == typeof(int))
            {
                Matrix = (dynamic)matrixReader.ReadIntMatrix().Clone();
                DefaultMatrix = (dynamic)matrixReader.ReadIntMatrix().Clone();
            }
            else if (typeof(T) == typeof(double))
            {
                Matrix = (dynamic)matrixReader.ReadDoubleMatrix().Clone();
                DefaultMatrix = (dynamic)matrixReader.ReadDoubleMatrix().Clone();
            }
        }

        public MyMatrix(T[,] matrix)
        {
            Matrix = matrix;
        }

        public MyMatrix(int rows, int columns)
        {
            Matrix = new T[rows, columns];
        }

        public static MyMatrix<T> operator +(MyMatrix<T> firstMatrix, MyMatrix<T> secondMatrix)
        {
            if ((firstMatrix.rows == secondMatrix.rows) && (firstMatrix.columns == secondMatrix.columns))
            {
                MyMatrix<T> temp = new MyMatrix<T>(firstMatrix.rows, firstMatrix.columns);
                for (int i = 0; i < firstMatrix.rows; i++)
                {
                    for (int j = 0; j < firstMatrix.columns; j++)
                    {
                        temp.Matrix[i, j] = (dynamic)firstMatrix.Matrix[i, j] + (dynamic)secondMatrix.Matrix[i, j];
                    }
                }
                return temp;
            }
            return new MyMatrix<T>(1, 1);
        }

        public static MyMatrix<T> operator *(MyMatrix<T> firstMatrix, MyMatrix<T> secondMatrix)
        {
            MyMatrix<T> result = new MyMatrix<T>(firstMatrix.rows, secondMatrix.columns);
            for (int i = 0; i < firstMatrix.rows; i++)
            {
                for (int j = 0; j < secondMatrix.columns; j++)
                {
                    T[] instance = new T[firstMatrix.rows];
                    instance[0] = new T();
                    for (int k = 0; k < secondMatrix.rows; k++)
                    {
                        instance[0] = (dynamic)instance[0] + (dynamic)firstMatrix.Matrix[i, k] * (dynamic)secondMatrix.Matrix[k, j];
                    }
                    result.Matrix[i, j] = (dynamic)instance[0];
                }
            }
            return result;
        }

        public static T[] operator *(MyMatrix<T> firstMatrix, T[] vector)
        {
            T[] instance = new T[firstMatrix.rows];
            for (int i = 0; i < firstMatrix.rows; i++)
            {
                instance[i] = new T();
            }
            for (int i = 0; i < firstMatrix.rows; i++)
            {
                for (int j = 0; j < firstMatrix.columns; j++)
                {
                    instance[i] += (dynamic)firstMatrix.Matrix[i, j] * (dynamic)vector[j];
                }
            }
            return instance;
        }

        public T[] gaussWithoutChoice(T[] bVector)
        {
            bVector = makeRowEchelonMatrix(bVector);
            T[] xVector = countXVector(bVector);
            setDefaultMatrix();
            return xVector;
        }


        public T[] gaussWithRowChoice(T[] bVector)
        {
            bVector = makeRowEchelonMatrixWithRowChoice(bVector);
            T[] xVector = countXVector(bVector);
            setDefaultMatrix();
            return xVector;
        }

        public T[] gaussWithFullChoice(T[] bVector)
        {
            int[] xVectorNumberChangeTable = new int[bVector.Length];
            for (int i = 0; i < bVector.Length; i++)
            {
                xVectorNumberChangeTable[i] = i + 1;
            }
            bVector = makeRowEchelonMatrixWithFullChoice(bVector, xVectorNumberChangeTable);
            T[] xVector = countModifiedXVector(bVector, xVectorNumberChangeTable);
            setDefaultMatrix();
            return xVector;
        }

        private T[] makeRowEchelonMatrix(T[] bVector)
        {
            for (int k = 0; k < columns; k++)
            {
                for (int i = k; i < rows - 1; i++)
                {
                    T numberForMultiply = (dynamic)Matrix[i + 1, k] / Matrix[k, k];

                    for (int j = k; j < columns; j++)
                    {
                        Matrix[i + 1, j] -= ((dynamic)Matrix[k, j] * numberForMultiply);
                    }

                    bVector[i + 1] -= ((dynamic)bVector[k] * numberForMultiply);
                }
            }

            return bVector;
        }

        private T[] makeRowEchelonMatrixWithRowChoice(T[] bVector)
        {
            for (int k = 0; k < columns; k++)
            {
                int rowWithDiagonalNumber = k;
                int rowNumberWithMaxNumberInColumn = findRowWithMaxNumberInColumnUnderDiagonal(k);

                if (rowNumberWithMaxNumberInColumn != rowWithDiagonalNumber)
                {
                    bVector = swapRows(rowWithDiagonalNumber, rowNumberWithMaxNumberInColumn, bVector);
                }

                for (int i = k; i < rows - 1; i++)
                {
                    T numberForMultiply = (dynamic)Matrix[i + 1, k] / Matrix[k, k];

                    for (int j = k; j < columns; j++)
                    {
                        Matrix[i + 1, j] -= ((dynamic)Matrix[k, j] * numberForMultiply);
                    }

                    bVector[i + 1] -= ((dynamic)bVector[k] * numberForMultiply);
                }

            }
            return bVector;
        }


        private T[] makeRowEchelonMatrixWithFullChoice(T[] bVector, int[] xVectorNumberChangeTable)
        {
            for (int k = 0; k < columns; k++)
            {

                int rowNumberWithDiagonalPoint = k;
                int rowNumberWithMaxNumberInMatrix = rowNumberWithDiagonalPoint;
                int columnNumberWithMaxNumberInMatrix = rowNumberWithDiagonalPoint;

                findRowAndColumnWithMaxElementInMatrix(rowNumberWithDiagonalPoint, ref rowNumberWithMaxNumberInMatrix, ref columnNumberWithMaxNumberInMatrix);
                if (rowNumberWithMaxNumberInMatrix != rowNumberWithDiagonalPoint)
                {
                    bVector = swapRows(rowNumberWithDiagonalPoint, rowNumberWithMaxNumberInMatrix, bVector);
                }

                if (columnNumberWithMaxNumberInMatrix != rowNumberWithDiagonalPoint)
                {
                    xVectorNumberChangeTable = swapColumns(rowNumberWithDiagonalPoint, columnNumberWithMaxNumberInMatrix, xVectorNumberChangeTable);
                }

                for (int i = k; i < rows - 1; i++)
                {
                    T numberForMultiply = (dynamic)Matrix[i + 1, k] / Matrix[k, k];

                    for (int j = k; j < columns; j++)
                    {
                        Matrix[i + 1, j] -= ((dynamic)Matrix[k, j] * numberForMultiply);
                    }

                    bVector[i + 1] -= ((dynamic)bVector[k] * numberForMultiply);
                }
            }

            return bVector;
        }

        private int findRowWithMaxNumberInColumnUnderDiagonal(int columnNumber)
        {
            int rowNumberWithMaxNumberInColumn = columnNumber;
            int firstRowUnderDiagonal = columnNumber + 1;
            for (int i = firstRowUnderDiagonal; i < rows; i++)
            {
                if ((dynamic)Matrix[rowNumberWithMaxNumberInColumn, columnNumber] < Matrix[i, columnNumber])
                {
                    rowNumberWithMaxNumberInColumn = i;
                }
            }
            return rowNumberWithMaxNumberInColumn;
        }

        private void findRowAndColumnWithMaxElementInMatrix(int rowNumberWithDiagonalPoint, ref int rowNumberWithMaxNumberInMatrix, ref int columnNumberWithMaxNumberInMatrix)
        {
            int columnNumberWithDiagonalPoint = rowNumberWithDiagonalPoint;

            for (int i = rowNumberWithDiagonalPoint; i < rows; i++)
            {
                for (int j = columnNumberWithDiagonalPoint; j < columns; j++)
                {
                    if ((dynamic)Matrix[rowNumberWithMaxNumberInMatrix, columnNumberWithMaxNumberInMatrix] < Matrix[i, j])
                    {
                        rowNumberWithMaxNumberInMatrix = i;
                        columnNumberWithMaxNumberInMatrix = j;
                    }
                }
            }
        }

        private T[] swapRows(int rowWithDiagonalNumber, int rowNumberWithMaxNumber, T[] bVector)
        {
            T[] tempRow = new T[columns];
            T tempValue;
            for (int i = 0; i < columns; i++)
            {
                tempRow[i] = Matrix[rowWithDiagonalNumber, i];
                Matrix[rowWithDiagonalNumber, i] = Matrix[rowNumberWithMaxNumber, i];
                Matrix[rowNumberWithMaxNumber, i] = tempRow[i];
            }

            tempValue = bVector[rowWithDiagonalNumber];
            bVector[rowWithDiagonalNumber] = bVector[rowNumberWithMaxNumber];
            bVector[rowNumberWithMaxNumber] = tempValue;

            return bVector;
        }

        private int[] swapColumns(int columnNumberWithDiagonalPoint, int columnNumberWithMaxNumber, int[] xVector)
        {
            T[] tempColumn = new T[rows];
            int tempValue;
            for (int i = 0; i < rows; i++)
            {
                tempColumn[i] = Matrix[i, columnNumberWithDiagonalPoint];
                Matrix[i, columnNumberWithDiagonalPoint] = Matrix[i, columnNumberWithMaxNumber];
                Matrix[i, columnNumberWithMaxNumber] = tempColumn[i];
            }
            tempValue = xVector[columnNumberWithDiagonalPoint];
            xVector[columnNumberWithDiagonalPoint] = xVector[columnNumberWithMaxNumber];
            xVector[columnNumberWithMaxNumber] = tempValue;

            return xVector;
        }

        private T[] countXVector(T[] bVector)
        {
            T[] xVector = new T[bVector.Length];
            for (int i = bVector.Length - 1; i >= 0; i--)
            {
                int j = i;
                T numerator = bVector[i];
                while (j < (columns - 1))
                {
                    numerator -= ((dynamic)Matrix[i, j + 1] * xVector[j + 1]);
                    j++;
                }
                xVector[i] = (dynamic)numerator / Matrix[i, i];

            }

            return xVector;
        }

        private T[] countModifiedXVector(T[] bVector, int[] xVectorNumberChangeTable)
        {
            T[] xVector = new T[bVector.Length];
            xVector = countXVector(bVector);

            int indexTemp;
            T valueTemp;
            Console.WriteLine("Wektor x przed zmiana");
            for (int i = 0; i < xVector.Length; i++)
            {
                Console.WriteLine("{0} - {1}", i + 1, xVectorNumberChangeTable[i]);
            }

            for (int i = 0; i < xVector.Length; i++)
            {
                if (xVectorNumberChangeTable[i] != i + 1)
                {
                    int indexWithNumber = findIndexWithNumber(xVectorNumberChangeTable, i + 1);

                    indexTemp = xVectorNumberChangeTable[i];
                    xVectorNumberChangeTable[i] = xVectorNumberChangeTable[indexWithNumber];
                    xVectorNumberChangeTable[indexWithNumber] = indexTemp;

                    valueTemp = xVector[i];
                    xVector[i] = xVector[indexWithNumber];
                    xVector[indexWithNumber] = valueTemp;
                }
            }

            Console.WriteLine("Wektor x po zmianie");
            for (int i = 0; i < xVector.Length; i++)
            {
                Console.WriteLine("{0} - {1}", i + 1, xVectorNumberChangeTable[i]);
            }

            return xVector;
        }

        private int findIndexWithNumber(int[] xVector, int number)
        {
            for (int i = 0; i < xVector.Length; i++)
            {
                if (xVector[i] == number)
                {
                    return i;
                }
            }
            return 0;
        }

        public void setDefaultMatrix()
        {
            Matrix = (T[,])DefaultMatrix.Clone();
        }


    }
}
