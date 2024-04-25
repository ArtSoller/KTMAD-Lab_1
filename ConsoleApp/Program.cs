﻿#define TESTING
// RELEASE
// TESTING
#define NODRAWING
#define SOLVE2DIM
using Project;
using System.Globalization;
using Dumpify;
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
DumpConfig.Default.ColorConfig.ColumnNameColor = DumpColor.FromHexString("#FF0000");
DumpConfig.Default.ColorConfig.PropertyValueColor = DumpColor.FromHexString("#00FF00");
DumpConfig.Default.ColorConfig.NullValueColor = DumpColor.FromHexString("#0000FF");

string Calculation2dimArea = Path.GetFullPath("../../../../Data/Input/WholeMesh.dat");
string Layer1Area = Path.GetFullPath("../../../../Data/Input/Fields/Field1.dat");
string BordersInfo = Path.GetFullPath("../../../../Data/Input/Borders.dat");
string AnswerPath = Path.GetFullPath("../../../../Data/Output/");
string SubtotalsPath = Path.GetFullPath("../../../../Data/Subtotals/");
string TimePath = Path.GetFullPath("../../../../Data/Input/Time.dat");
string PicturesPath = Path.GetFullPath("../../../../Drawer/Pictures/");

// Pre-processor. Clearing output folders.
#if SOLVE2DIM
FolderManager.ClearFolder(AnswerPath + "/A_phi/Answer/");
FolderManager.ClearFolder(AnswerPath + "/A_phi/Discrepancy/");
FolderManager.ClearFolder(AnswerPath + "/E_phi/Answer/");
FolderManager.ClearFolder(AnswerPath + "/A_phi/Discrepancy/");
FolderManager.ClearFolder(AnswerPath + "/A_phi/Answer3D/");
#endif
FolderManager.ClearFolder(PicturesPath + "/A_phi/");
FolderManager.ClearFolder(PicturesPath + "/E_phi/");
FolderManager.ClearFolder(SubtotalsPath);


/*
int nx = 4;
int ny = 4;
int nz = 3;

List<double> nodesX = [0.0, 1.0, 2.0, 3.0];
List<double> nodesY = [0.0, 1.0, 2.0, 3.0];
List<double> nodesZ = [0.0, 1.0, 2.0];

double xMin = nodesX[0];
double xMax = nodesX[^1];
double yMin = nodesY[0];
double yMax = nodesY[^1];
double zMin = nodesZ[0];
double zMax = nodesZ[^1];

ArrayOfPoints arrpnt = new(nx * ny * nz);

foreach (var Z in nodesZ)
    foreach (var Y in nodesY)
        foreach (var X in nodesX)
            arrpnt.Append(new Point(X, Y, Z));

foreach (var pnt in arrpnt)
{
    if (pnt.Z == zMax)
        pnt.Type = Location.BoundaryII;
    else if (pnt.Z == zMin || pnt.X == xMin ||pnt.X == xMax || pnt.Y == yMin || pnt.Y == yMax)
        pnt.Type = Location.BoundaryI;
    else
        pnt.Type = Location.Inside;
}

ArrayOfRibs arr = new(3 * nx * ny * nz - nx * ny - nx * nz - ny * nz);

int nxny = nx * ny;

for (int k = 0; k < nz; k++)
{
    for (int j = 0; j < ny; j++)
    {
        for (int i = 0; i < nx - 1; i++) 
            arr.Add(new Rib(arrpnt[k * nxny + nx * j + i], arrpnt[k * nxny + nx * j + i + 1]));
        if (j != ny - 1)
            for (int i = 0; i < nx; i++)
                arr.Add(new Rib(arrpnt[k * nxny + nx * j + i], arrpnt[k * nxny + nx * (j + 1) + i]));
    }
    if (k != nz - 1)
        for (int j = 0; j < ny; j++)
            for (int i = 0; i < nx; i++)
                arr.Add(new Rib(arrpnt[k * nxny + nx * j + i], arrpnt[(k + 1) * nxny + nx * j + i]));
}

//return 0;


var arre = new ArrayOfElems(3 * 3 * 2);
        
int rx = 4 - 1;
int ry = 4 - 1;

int rxy = rx * ny + ry * nx;
int nxy = nx * ny;
int rz = 3 - 1;
int ramount = 3 * nx * ny * nz - nx * ny - nx * nz - ny * nz;

int jj = 0;
int kk = 0;
for (int i = 0; i < 4 * 4 * 3; i++)
{
    if (i + ry * jj + rxy * kk + rxy + nxy + rx + nx > ramount) 
        break;

    if (i % (nx * ry - 1 + kk * nx * ny) == 0 && i != 0)
    {
        i += nx;
        kk++;
        jj = 0;
    }
    else if (i % nx == nx - 1)
        jj++;
    else
    {
        int fst = i + ry * jj + rxy * kk; 
        arre.Add([i + ry * jj + rxy * kk, i + rx + ry * jj + rxy * kk, i + rx + 1 + ry * jj + rxy * kk, i + rx + nx + ry * jj + rxy * kk,
                  i + rxy * (kk + 1), i + rxy * (kk + 1) + 1, i + rxy * (kk + 1) + rx + 1, i + rxy * (kk + 1) + rx + 1 + 1,
                  fst + rxy + nxy, fst + rxy + nxy + rx, fst + rxy + nxy + rx + 1, fst + rxy + nxy + rx + nx]);          
    }
}

int ii = 0;
while (ii < arr.Count)
{
    if (arr[ii].typeOfRib == TypeOfRib.BoundaryI)
    {
        foreach (var elem in arre)
            foreach (var item in elem)
            {
                if (item > ii) break;
                if (item == ii)
                {
                    elem.Remove(item);
                    break;
                }
            }
        arr.Remove(ii);
        for (int i = 0; i < arre.Length; i++)
            for (int j = 0; j < arre[i].Count; j++) 
                if (arre[i][j] > ii)
                    arre[i][j] -= 1;
    }
    else
        ii++;
}

return 0;
*/

var borders = new List<List<int>>([[1, 1, 0, 1],
                                   [1, 1, 1, 2],
                                   [1, 1, 2, 3],
                                   [1, 1, 3, 7],
                                   [1, 1, 7, 11],
                                   [1, 1, 11, 15],
                                   [1, 1, 12, 13],
                                   [1, 1, 13, 14],
                                   [1, 1, 14, 15],
                                   [1, 1, 0, 4],
                                   [1, 1, 4, 8],
                                   [1, 1, 8, 12]]);

Sym(TestClass.A, TestClass.b, borders);


// Для теста узлы:
// 48 69 70 88 51 52 91 92 55 73 74 95
FEM3D myFEM3D_test = new();
myFEM3D_test.ConstructMesh();
myFEM3D_test.GenerateArrays();
myFEM3D_test.ConstructMatrixAndVector();
myFEM3D_test.SetSolver(new LOS());
myFEM3D_test.Solve();
myFEM3D_test.TestOutput(AnswerPath);
myFEM3D_test.WriteData(AnswerPath);

static void Sym(GlobalMatrix m, GlobalVector v, List<List<int>> borders)
{
    foreach (var border in borders)
    {
        var al = m._au;
        var au = m._al;
        for (int i = 2; i < 6; i++)
        {
            for (int j = m._ig[border[i]]; j < m._ig[border[i] + 1]; j++)
                if (al[j] != 0)
                {
                    var k = al[m._jg[j]];
                    var f = -1.0D * k * v[border[i]];
                    v[m._jg[j]] += f;
                    al[j] = 0.0D;
                }
            for (int j = 0; j < m._jg.Count; j++)
                if (m._jg[j] == border[i])
                    if (au[j] != 0)
                    {
                        var k = au[j];
                        var f = -1.0D * k * v[border[i]];
                        v[m._jg[j]] += f;
                        au[m._jg[j]] = 0.0D;
                    }
        }
        m._au = al;
        m._al = au;
    }
}
return 0;


// Main process of 2-dim task.
FEM2D myFEM2D = new();
myFEM2D.ReadData(Calculation2dimArea, BordersInfo, TimePath);
myFEM2D.ConstructMesh();
myFEM2D.SubmitGeneratedData();
#if SOLVE2DIM
myFEM2D.SetSolver(new LOS());
myFEM2D.Solve();
//myFEM2D.GenerateVectorEphi();
myFEM2D.WriteData(AnswerPath);
myFEM2D.WriteDiscrepancy(AnswerPath);
#endif

// Post-processor of zero-layer. Drawing A_phi and E_phi.
#if DRAWING
int a = Postprocessor.DrawA_phi();
int b = Postprocessor.DrawE_phi();
Console.WriteLine($"Drawing A_phi finished with code: {a}\n" +
                  $"Drawing E_phi finished with code: {b}");
#endif


return 0;

// Converting 2-dim results into 3-dim form

FEM3D myFEM3D = new(myFEM2D);
myFEM3D.ConstructMesh(myFEM2D);
myFEM3D.ConvertResultTo3Dim(myFEM2D);
myFEM3D.GenerateArrays();
myFEM3D.AddField(MeshReader.ReadField(Layer1Area));
myFEM3D.CommitFields();
myFEM3D.ConstructMatrixAndVector();

myFEM3D.SetSolver(new LOS());