using MatrixLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib
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

        public static MyMatrix<T> operator -(MyMatrix<T> firstMatrix, MyMatrix<T> secondMatrix)
        {
            if ((firstMatrix.rows == secondMatrix.rows) && (firstMatrix.columns == secondMatrix.columns))
            {
                MyMatrix<T> temp = new MyMatrix<T>(firstMatrix.rows, firstMatrix.columns);
                for (int i = 0; i < firstMatrix.rows; i++)
                {
                    for (int j = 0; j < firstMatrix.columns; j++)
                    {
                        temp.Matrix[i, j] = (dynamic)firstMatrix.Matrix[i, j] - (dynamic)secondMatrix.Matrix[i, j];
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

        public T[] GaussWithoutChoice(T[] bVector)
        {
            bVector = MakeRowEchelonMatrix(bVector);
            T[] xVector = CountXVector(bVector);
            SetDefaultMatrix();
            return xVector;
        }


        public T[] GaussWithRowChoice(T[] bVector)
        {
            bVector = MakeRowEchelonMatrixWithRowChoice(bVector);
            T[] xVector = CountXVector(bVector);
            SetDefaultMatrix();
            return xVector;
        }

        public T[] GaussWithFullChoice(T[] bVector)
        {
            int[] xVectorNumberChangeTable = new int[bVector.Length];
            for (int i = 0; i < bVector.Length; i++)
            {
                xVectorNumberChangeTable[i] = i + 1;
            }
            bVector = MakeRowEchelonMatrixWithFullChoice(bVector, xVectorNumberChangeTable);
            T[] xVector = CountModifiedXVector(bVector, xVectorNumberChangeTable);
            SetDefaultMatrix();
            return xVector;
        }

        public T[] MakeRowEchelonMatrix(T[] bVector)
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

        public T[] MakeRowEchelonMatrixWithRowChoice(T[] bVector)
        {
            for (int k = 0; k < columns; k++)
            {
                int rowWithDiagonalNumber = k;
                int rowNumberWithMaxNumberInColumn = FindRowWithMaxNumberInColumnUnderDiagonal(k);

                if (rowNumberWithMaxNumberInColumn != rowWithDiagonalNumber)
                {
                    bVector = SwapRows(rowWithDiagonalNumber, rowNumberWithMaxNumberInColumn, bVector);
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


        public T[] MakeRowEchelonMatrixWithFullChoice(T[] bVector, int[] xVectorNumberChangeTable)
        {
            for (int k = 0; k < columns; k++)
            {

                int rowNumberWithDiagonalPoint = k;
                int rowNumberWithMaxNumberInMatrix = rowNumberWithDiagonalPoint;
                int columnNumberWithMaxNumberInMatrix = rowNumberWithDiagonalPoint;

                var greatestPosition = FindRowAndColumnWithMaxElementInMatrix(rowNumberWithDiagonalPoint);
                if (greatestPosition.row != rowNumberWithDiagonalPoint)
                {
                    bVector = SwapRows(rowNumberWithDiagonalPoint, greatestPosition.row, bVector);
                }

                if (greatestPosition.column != rowNumberWithDiagonalPoint)
                {
                    xVectorNumberChangeTable = SwapColumns(rowNumberWithDiagonalPoint, greatestPosition.column, xVectorNumberChangeTable);
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

        public int FindRowWithMaxNumberInColumnUnderDiagonal(int columnNumber)
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

        public (int row, int column) FindRowAndColumnWithMaxElementInMatrix(int rowNumberWithDiagonalPoint = 0)
        {
            int columnNumberWithDiagonalPoint = rowNumberWithDiagonalPoint;
            var result = (row: rowNumberWithDiagonalPoint, column: rowNumberWithDiagonalPoint);
            for (int i = rowNumberWithDiagonalPoint; i < rows; i++)
            {
                for (int j = columnNumberWithDiagonalPoint; j < columns; j++)
                {
                    if ((dynamic)Matrix[i, j] > Matrix[result.row, result.column])
                    {
                        result = (i, j);
                    }
                }
            }
            return result;
        }

        public T[] SwapRows(int rowWithDiagonalNumber, int rowNumberWithMaxNumber, T[] bVector)
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

        public int[] SwapColumns(int columnNumberWithDiagonalPoint, int columnNumberWithMaxNumber, int[] xVector)
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

        public T[] CountXVector(T[] bVector)
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

        public  T[] CountModifiedXVector(T[] bVector, int[] xVectorNumberChangeTable)
        {
            T[] xVector = new T[bVector.Length];
            xVector = CountXVector(bVector);

            int indexTemp;
            T valueTemp;

            for (int i = 0; i < xVector.Length; i++)
            {
                if (xVectorNumberChangeTable[i] != i + 1)
                {
                    int indexWithNumber = FindIndexWithNumberInVector(xVectorNumberChangeTable, i + 1);

                    indexTemp = xVectorNumberChangeTable[i];
                    xVectorNumberChangeTable[i] = xVectorNumberChangeTable[indexWithNumber];
                    xVectorNumberChangeTable[indexWithNumber] = indexTemp;

                    valueTemp = xVector[i];
                    xVector[i] = xVector[indexWithNumber];
                    xVector[indexWithNumber] = valueTemp;
                }
            }
            
            return xVector;
        }
        
        public static int FindIndexWithNumberInVector(int[] xVector, int number)
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

        public void SetDefaultMatrix()
        {
            Matrix = (T[,])DefaultMatrix.Clone();
        }

        public override string ToString()
        {
            var result = String.Empty;
            for (int i = 0; i < rows; i++)
            {
                result += "| ";
                for (int j = 0; j < columns; j++)
                {
                    if (j != 0)
                    {
                        result += "| ";
                    }
                    result += String.Format("{0:N3}", Matrix[i, j]);

                }
                result += "|\n";
            }

            return result;
        }
    }
}
