using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTests.Interfaces
{
    public interface IMyMatrixFileReader
    {
        int[,] ReadIntMatrix();
        int[,] ReadDoubleMatrix();
    }
}
