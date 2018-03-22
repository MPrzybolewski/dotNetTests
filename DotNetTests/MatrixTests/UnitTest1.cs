using System;
using System.Data;
using System.Diagnostics;
using MatrixLib;
using MatrixLib.Interfaces;
using MatrixLib.Interfaces.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MatrixTests
{
    [TestClass]
    public class MatrixTests
    {
        private IMyMatrixFileReader _matrixFileReader;
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void SetUp()
        {
            _matrixFileReader = new StubIMyMatrixFileReader
            {
                ReadIntMatrix = () => new[,]
                {
                    {1, 1, 1},
                    {2, 2, 2},
                    {3, 3, 3}
                },
                ReadDoubleMatrix = () => new[,]
                {
                    {4d, -2d, 4d, -2d},
                    {3d, 1d, 4d, 2d},
                    {2d, 4d, 2d, 1d},
                    {2d, -2d, 4d, 2d}
                }
            };
        }

        [TestMethod]
        public void AddIntMatrixTest()
        {
            //Arrange
            var a = new MyMatrix<int>(_matrixFileReader);
            var b = new MyMatrix<int>(_matrixFileReader);

            //Act
            var result = a + b;

            //Assert
            CollectionAssert.AreEqual(result.Matrix, new[,]
            {
                {2, 2, 2},
                {4, 4, 4},
                {6, 6, 6}
            });
        }

        [TestMethod]
        public void Subtract_IntValues_ShouldReturnProperMatrix()
        {
            //Arrange
            _matrixFileReader = new StubIMyMatrixFileReader
            {
                ReadIntMatrix = () => new[,]
                {
                    {1, 1, 1},
                    {2, 5, 2},
                    {3, 3, 4}
                }
            };
            var a = new MyMatrix<int>(_matrixFileReader);
            _matrixFileReader = new StubIMyMatrixFileReader
            {
                ReadIntMatrix = () => new[,]
                {
                    {1, 1, 1},
                    {3, 1, 0},
                    {5, 4, 3}
                }
            };
            var b = new MyMatrix<int>(_matrixFileReader);

            //Act
            var result = a - b;

            //Assert
            CollectionAssert.AreEqual(result.Matrix, new[,]
            {
                {0, 0, 0},
                {-1, 4, 2},
                {-2, -1, 1}
            });
        }

        [TestMethod]
        public void Multiply_IntValues_ShouldReturnProperMatrix()
        {
            //Arrange
            var a = new MyMatrix<int>(_matrixFileReader);
            var b = new MyMatrix<int>(_matrixFileReader);

            //Act
            var result = a * b;

            //Assert
            CollectionAssert.AreEqual(new[,]
            {
                {6, 6, 6},
                {12, 12, 12},
                {18, 18, 18}

            }, result.Matrix);
        }

        [TestMethod]
        public void GreatestIntNumberInRowIntTest()
        {
            //Arrange
            _matrixFileReader = new StubIMyMatrixFileReader
            {
                ReadIntMatrix = () => new[,]
                {
                    {1, 1, 1},
                    {2, 5, 2},
                    {3, 3, 4}
                }
            };
            const int expectedRowNumber = 2;

            //Act
            var result = new MyMatrix<int>(_matrixFileReader).FindRowWithMaxNumberInColumnUnderDiagonal(0);

            //Assert
            Assert.AreEqual(expectedRowNumber, result);
        }

        [TestMethod]
        public void GreatestDoubleNumberInRowTest()
        {
            //Arrange
            _matrixFileReader = new StubIMyMatrixFileReader
            {
                ReadDoubleMatrix = () => new[,]
                {
                    {1.0, 1.5, 1.2},
                    {2.0, 5.3, 2.2},
                    {3.1, 3.1, 41.23}
                }
            };
            const int expectedColumnNumber = 2;

            //Act
            var result = new MyMatrix<double>(_matrixFileReader).FindRowWithMaxNumberInColumnUnderDiagonal(2);

            //Assert
            Assert.AreEqual(expectedColumnNumber, result);
        }

        [TestMethod]
        public void GreatestIntNumberInMatrixRowTest()
        {
            //Arrange
            _matrixFileReader = new StubIMyMatrixFileReader
            {
                ReadIntMatrix = () => new[,]
                {
                    {1, 1, 1},
                    {2, 5, 2},
                    {3, 3, 4}
                }
            };
            const int expectedRowNumber = 1;

            //Act

            var result = new MyMatrix<int>(_matrixFileReader).FindRowAndColumnWithMaxElementInMatrix(0).row;

            //Assert
            Assert.AreEqual(expectedRowNumber, result);
        }

        [TestMethod]
        public void GreatestIntNumberInMatrixColumnTest()
        {
            //Arrange
            _matrixFileReader = new StubIMyMatrixFileReader
            {
                ReadIntMatrix = () => new[,]
                {
                    {1, 1, 1},
                    {2, 5, 2},
                    {3, 3, 4}
                }
            };
            const int expectedRowNumber = 1;

            //Act

            var result = new MyMatrix<int>(_matrixFileReader).FindRowAndColumnWithMaxElementInMatrix(0).column;

            //Assert
            Assert.AreEqual(expectedRowNumber, result);
        }

        [TestMethod]
        public void GreatestDoubleNumberInMatrixRowTest()
        {
            //Arrange
            _matrixFileReader = new StubIMyMatrixFileReader
            {
                ReadDoubleMatrix = () => new[,]
                {
                    {1.0, 1.5, 1.2},
                    {2.0, 5.3, 2.2},
                    {3.1, 3.1, 41.23}
                }
            };
            const int expectedRowNumber = 2;

            //Act

            var result = new MyMatrix<double>(_matrixFileReader).FindRowAndColumnWithMaxElementInMatrix(0).row;

            //Assert
            Assert.AreEqual(expectedRowNumber, result);
        }

        [TestMethod]
        public void GreatestDoubleNumberInMatrixColumnTest()
        {
            //Arrange
            _matrixFileReader = new StubIMyMatrixFileReader
            {
                ReadDoubleMatrix = () => new[,]
                {
                    {1.0, 1.5, 1.2},
                    {2.0, 5.3, 2.2},
                    {3.1, 3.1, 41.23}
                }
            };
            const int expectedRowNumber = 2;

            //Act
            var result = new MyMatrix<double>(_matrixFileReader).FindRowAndColumnWithMaxElementInMatrix(0).column;

            //Assert
            Assert.AreEqual(expectedRowNumber, result);
        }

        [TestMethod]
        public void FindIndexInVectorWithNumberTest()
        {
            //Arrange
            int[] a = { 4, 3, 2, 5, 6 };

            const int expectedIndexValue = 1;

            //Act
            var result = MyMatrix<int>.FindIndexWithNumberInVector(a, 3);

            //Assert
            Assert.AreEqual(expectedIndexValue, result);
        }

      

        [TestMethod]
        public void CountXVectorTest()
        {
            //Arrange
            _matrixFileReader = new StubIMyMatrixFileReader
            {
                ReadIntMatrix = () => new[,]
                {
                    {-1, 2, 1},
                    {0, -1, -1},
                    {0, 0, -3}
                }
            };
            int[] a = { -1, -2, -9 };

            int[] expectedXVector = { 2, -1, 3 };

            //Act
            var result = new MyMatrix<int>(_matrixFileReader).CountXVector(a);

            //Assert
            CollectionAssert.AreEqual(expectedXVector, result);
        }

        [TestMethod]
        public void SwapRowsTest()
        {
            //Arrange
            _matrixFileReader = new StubIMyMatrixFileReader
            {
                ReadDoubleMatrix = () => new[,]
                {
                    {0.3, 0.6, 0.6, 0.1},
                    {0.4, 0.4, 0.2, 0.6},
                    {0.3, 0.2, 0.3, 0.2}
                }
            };
            var matrixValues = _matrixFileReader.ReadDoubleMatrix();
            var vector = new[]
            {
                0.3, 0.8, 0.4
            };
            const int numberOfFirstRow = 0;
            const int numberOfSecondRow = 2;
            var matrix = new MyMatrix<double>(matrixValues);

            //Act
            matrix.SwapRows(numberOfFirstRow, numberOfSecondRow, vector);

            //Assert
            CollectionAssert.AreEqual(new[,]
            {
                {0.3, 0.2, 0.3, 0.2},
                {0.4, 0.4, 0.2, 0.6},
                {0.3, 0.6, 0.6, 0.1}
            }, matrix.Matrix);
        }

        [TestMethod]
        public void SwapColumnTest()
        {
            //Arrange
            _matrixFileReader = new StubIMyMatrixFileReader
            {
                ReadDoubleMatrix = () => new[,]
                {
                    {0.3, 0.6, 0.6, 0.1},
                    {0.4, 0.4, 0.2, 0.6},
                    {0.3, 0.2, 0.3, 0.2}
                }
            };
            int[] vector = new[]
            {
                3, 8, 3
            };
            const int numberOfFirstColumn = 0;
            const int numberOfSecondColumn = 2;
            var matrix = new MyMatrix<double>(_matrixFileReader);

            //Act
            matrix.SwapColumns(numberOfFirstColumn, numberOfSecondColumn, vector);

            //Assert
            CollectionAssert.AreEqual(new[,]
            {
                {0.6, 0.6, 0.3, 0.1},
                {0.2, 0.4, 0.4, 0.6},
                {0.3, 0.2, 0.3, 0.2}
            }, matrix.Matrix);
        }

        [TestMethod]
        public void MakeEchelonMatrixMatrixTest()
        {
            //Arrange
            _matrixFileReader = new StubIMyMatrixFileReader
            {
                ReadIntMatrix = () => new[,]
                {
                    {-1, 2, 1},
                    {1, -3, -2},
                    {3, -1, -1}
                }
            };

            int[] vector = new[]
            {
                -1, -1, 4
            };
            var matrix = new MyMatrix<int>(_matrixFileReader);

            //Act
            matrix.MakeRowEchelonMatrix(vector);

            //Assert
            CollectionAssert.AreEqual(new[,]
            {
                {-1, 2, 1},
                {0, -1, -1},
                {0, 0, -3}
            }, matrix.Matrix);
        }

        [TestMethod]
        public void MakeEchelonMatrixVectorTest()
        {
            //Arrange
            _matrixFileReader = new StubIMyMatrixFileReader
            {
                ReadIntMatrix = () => new[,]
                {
                    {-1, 2, 1},
                    {1, -3, -2},
                    {3, -1, -1}
                }
            };

            int[] vector = new[]
            {
                -1, -1, 4
            };
            var matrix = new MyMatrix<int>(_matrixFileReader);
            int[] expectedResult = { -1, -2, -9 };

            //Act
            var result = matrix.MakeRowEchelonMatrix(vector);

            //Assert
            CollectionAssert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GaussWithoutChoiceTest()
        {
            //Arrange
            _matrixFileReader = new StubIMyMatrixFileReader
            {
                ReadDoubleMatrix = () => new[,]
                {
                    {4d, -2d, 4d, -2d},
                    {3d, 1d, 4d, 2d},
                    {2d, 4d, 2d, 1d},
                    {2d, -2d, 4d, 2d}
                }
            };

            double[] vector = new[]
            {
                8d, 7d, 10d, 2d
            };
            var matrix = new MyMatrix<double>(_matrixFileReader);
            double[] expectedResult = { -1d, 2d, 3d, -2d };

            //Act
            var result = matrix.GaussWithoutChoice(vector);

            //Assert
            for (var j = 0; j < result.Length; j++)
            {
                Assert.AreEqual(result[j], expectedResult[j], 1e15);
            }
        }

        [TestMethod]
        public void GaussWithFullChoiceTest()
        {
            //Arrange
            _matrixFileReader = new StubIMyMatrixFileReader
            {
                ReadDoubleMatrix = () => new[,]
                {
                    {4d, -2d, 4d, -2d},
                    {3d, 1d, 4d, 2d},
                    {2d, 4d, 2d, 1d},
                    {2d, -2d, 4d, 2d}
                }
            };

            double[] vector = new[]
            {
                8d, 7d, 10d, 2d
            };
            var matrix = new MyMatrix<double>(_matrixFileReader);
            double[] expectedResult = { -1d, 2d, 3d, -2d };

            //Act
            var result = matrix.GaussWithFullChoice(vector);

            //Assert
            for (var j = 0; j < result.Length; j++)
            {
                Assert.AreEqual(result[j], expectedResult[j], 1e15);
            }
        }

        [TestMethod]
        public void MatrixToStringTest()
        {
            //Arrange
            var matrixArray = _matrixFileReader.ReadIntMatrix();
            var matrix = new MyMatrix<int>(_matrixFileReader);

            //Act
            var result = matrix.ToString();

            foreach (var value in matrixArray)
            {
                StringAssert.Contains(result, value.ToString());
            }
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", @"Data\DataGauss.csv", "DataGauss#csv", DataAccessMethod.Sequential)]
        public void GaussWithRowChoiceWithCSVFileTest()
        {
            //Arrange
            var parsing = ParseCsv(TestContext.DataRow);
            var expectedResult = parsing.expectedResult;
            var bVector = parsing.bVector;
            var a = new MyMatrix<double>(_matrixFileReader);


            //Act
            var result = a.GaussWithRowChoice(bVector);

            //Assert
            for (var j = 0; j < result.Length; j++)
            {
                Assert.AreEqual(result[j], expectedResult[j], 1e15);
            }
        }

        private static (double[] bVector, double[] expectedResult) ParseCsv(DataRow dataRow)
        {
            var result = new double[4];
            var expectedResult = new double[4];
            for (var i = 0; i < 4; i++)
            {
                result[i] = Convert.ToDouble(dataRow[i]);
            }

            for (var i = 0; i < 4; i++)
            {
                expectedResult[i] = Convert.ToDouble(dataRow[i + 4]);
            }

            return (result, expectedResult);
        }
    }

}
