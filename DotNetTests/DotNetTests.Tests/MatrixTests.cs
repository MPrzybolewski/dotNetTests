using System;
using DotNetTests.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetTests.Interfaces.F

namespace DotNetTests.Tests
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
        public void Add_IntValues_ShouldReturnProperMatrix()
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
    }
}
