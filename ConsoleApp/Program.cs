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
string MeshInfo = Path.GetFullPath("../../../../Data/Input/WholeMesh.dat");
string AnswerPath = Path.GetFullPath("../../../../Data/Output/");
string TimePath = Path.GetFullPath("../../../../Data/Input/Time.dat");

string Borders2DInfo = Path.GetFullPath("../../../../Data/Input/Borders2D.dat");
string Mesh2DInfo = Path.GetFullPath("../../../../Data/Input/WholeMesh2D.dat");

//Mesh3Dim myMesh = new();
//MeshReader.ReadMesh(MeshInfo, BordersInfo, ref myMesh);
//MeshGenerator.GenerateMesh(ref myMesh);


// Для теста узлы:
// 48 69 70 88 51 52 91 92 55 73 74 95
FEM3D myFEM3D_test = new();
FEM2D myFEM2D_test = new();

//myFEM3D_test.ConstructMesh(6, 6, 6);
myFEM3D_test.GenerateArrays(MeshInfo, BordersInfo);
myFEM3D_test.ConstructMatrixAndVector();
myFEM3D_test.SetSolver(new LOS());
myFEM3D_test.Solve();
myFEM3D_test.TestOutput(AnswerPath);
myFEM3D_test.TestPoint(4.0 / 3.0, 4.0 / 3.0, 4.0 / 3.0);
myFEM3D_test.WriteData(AnswerPath);

myFEM2D_test.GenerateArrays2D(Mesh2DInfo, Borders2DInfo);
myFEM2D_test.ConstructMatrixAndVector();
myFEM2D_test.SetSolver(new LOS());
myFEM2D_test.Solve();
myFEM2D_test.TestOutput(AnswerPath);
myFEM2D_test.TestPoint(4.0 / 3.0, 4.0 / 3.0, 4.0 / 3.0);
myFEM2D_test.WriteData(AnswerPath);

return 0;