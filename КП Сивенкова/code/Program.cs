﻿#define TESTING
// RELEASE
// TESTING
#define NODRAWING
#define SOLVE2DIM
using Project;
using System.Globalization;
using Solver;
using Manager;
using System.Numerics;
using Processor;
using Solution;
using MathObjects;
using Functions;
using Grid;
using DataStructs;


CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

string BordersInfo = Path.GetFullPath("../../../../Data/Input/Borders.dat");
string AnswerPath = Path.GetFullPath("../../../../Data/Output/");
string TimePath = Path.GetFullPath("../../../../Data/Input/Time.dat");

FEM3D myFEM3D_test = new();
myFEM3D_test.ConstructMesh(3, 3, 3);
myFEM3D_test.GenerateArrays();
myFEM3D_test.ConstructMatrixAndVector();
myFEM3D_test.SetSolver(new LOS());
myFEM3D_test.Solve();
myFEM3D_test.TestOutput(AnswerPath);
myFEM3D_test.WriteData(AnswerPath);

return 0;