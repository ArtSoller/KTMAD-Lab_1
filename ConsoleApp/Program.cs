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

string BordersInfo = Path.GetFullPath("../../../../Data/Input/3D/Borders.dat");
string MeshInfo = Path.GetFullPath("../../../../Data/Input/3D/WholeMesh.dat");
string AnswerPath = Path.GetFullPath("../../../../Data/Output/");
string TimePath = Path.GetFullPath("../../../../Data/Input/Time.dat");

string Borders2DInfo = Path.GetFullPath("../../../../Data/Input/2D/Borders2D.dat");
string Mesh2DInfo = Path.GetFullPath("../../../../Data/Input/2D/WholeMesh2D.dat");


// Mesh2Dim myMesh = new();
// MeshReader.ReadMesh2D(Mesh2DInfo, Borders2DInfo, ref myMesh);
// MeshGenerator.GenerateMesh(ref myMesh);

// FEM2D myFEM = new(myMesh);
// myFEM.GenerateArrays();
// myFEM.ConstructMatrixAndVector();
// myFEM.SetSolver(new LOS());
// myFEM.Solve();
// myFEM.TestOutput2D(AnswerPath);
// myFEM.TestPoint(4.0 / 3.0, 4.0 / 3.0);
// myFEM.WriteData2D(AnswerPath);

Mesh3Dim myMesh = new();
MeshReader.ReadMesh(MeshInfo, BordersInfo, ref myMesh);
MeshGenerator.GenerateMesh(ref myMesh);

FEM3D fEM3D = new(myMesh);
fEM3D.GenerateArrays();
fEM3D.ConstructMatrixAndVector();
fEM3D.SetSolver(new LOS());
fEM3D.Solve();
fEM3D.TestOutput(AnswerPath);
fEM3D.TestPoint(4.0 / 3.0, 4.0 / 3.0, 4.0 / 3.0);
fEM3D.WriteData(AnswerPath);

//myFEM2D_test.ConstructMatrixAndVector();
//myFEM2D_test.SetSolver(new LOS());
//myFEM2D_test.Solve();
//myFEM2D_test.TestOutput(AnswerPath);
//myFEM2D_test.TestPoint(4.0 / 3.0, 4.0 / 3.0, 4.0 / 3.0);
//myFEM2D_test.WriteData(AnswerPath);

return 0;