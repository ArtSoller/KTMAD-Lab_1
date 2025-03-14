namespace Project;

using System.Collections.Immutable;
using System.Numerics;
using MathObjects;
using Solver;
using Grid;
using DataStructs;
using System.Diagnostics;
using Functions;
using System.ComponentModel.DataAnnotations;
using System.Timers;
using System;

public class FEM2D : FEM
{

    public ArrayOfRibs ribsArr;

    public FEM2D(Mesh2Dim mesh)
    {
        mesh2D = mesh;
        
    }

    public void GenerateArrays()
    {
        timeMesh = [1.0D];
        if (mesh2D is null) throw new ArgumentNullException("mesh is null!");
        pointsArr = MeshGenerator.GenerateListOfPoints(mesh2D);
        ribsArr = MeshGenerator.GenerateListOfRibs(mesh2D, pointsArr);
        elemsArr = MeshGenerator.GenerateListOfElems2D(ref mesh2D);
        bordersArr = MeshGenerator.GenerateListOfBorders(mesh2D);
    }

    public void ConstructMatrixAndVector()
    {
        if (elemsArr is null) throw new ArgumentNullException("elemsArr is null");
        if (bordersArr is null) throw new ArgumentNullException("bordersArr is null");

        var sparceMatrix = new GlobalMatrix(ribsArr.Count);
        Generator.BuildPortait(ref sparceMatrix, ribsArr.Count, elemsArr);

        var G = new GlobalMatrix(sparceMatrix);
        Generator.FillMatrixG2D(ref G, ribsArr, elemsArr);

        var M = new GlobalMatrix(sparceMatrix);
        Generator.FillMatrixM2D(ref M, ribsArr, elemsArr);

        Matrix = G + M;

        var b = new GlobalVector(ribsArr.Count);
        Generator.FillVector2D(ref b, ribsArr, elemsArr, 0.0);
        Vector = b;

        Generator.ConsiderBoundaryConditions2D(ref Matrix, ref Vector, ribsArr, bordersArr, 0.0D);
    }

    public void Solve()
    {
        if (solver is null) throw new ArgumentNullException("Solver is null");
        if (Matrix is null) throw new ArgumentNullException("Matrix is null");
        if (Vector is null) throw new ArgumentNullException("Vector is null");
        Solutions = new GlobalVector[timeMesh.Length];
        Discrepancy = new GlobalVector[timeMesh.Length];
        (Solutions[0], Discrepancy[0]) = solver.Solve(Matrix, Vector);
        //
        //if (timeMesh.Length > 1)
        //{
        //    for (int i = 0; i < timeMesh.Length; i++)
        //    {
        //        if (i == 1 || i == 0)
        //        {
        //            Solutions[i] = new GlobalVector(ribsArr.Count);
        //            for (int j = 0; j < ribsArr.Count; j++)
        //            {
        //                var antinormal = ((ribsArr[j].b.X - ribsArr[j].a.X) / ribsArr[j].Length,
        //                                  (ribsArr[j].b.Y - ribsArr[j].a.Y) / ribsArr[j].Length,
        //                                  (ribsArr[j].b.Z - ribsArr[j].a.Z) / ribsArr[j].Length);
        //                var pointat = ((ribsArr[j].b.X + ribsArr[j].a.X) / 2.0D,
        //                               (ribsArr[j].b.Y + ribsArr[j].a.Y) / 2.0D,
        //                               (ribsArr[j].b.Z + ribsArr[j].a.Z) / 2.0D);

        //                var valueat = Function.A(pointat.Item1, pointat.Item2, pointat.Item3, timeMesh[i]);
        //                Solutions[i][j] = valueat.Item1 * antinormal.Item1 + valueat.Item2 * antinormal.Item2 + valueat.Item3 * antinormal.Item3;
        //            }
        //            continue;
        //        }
        //        double t0 = timeMesh[i];
        //        double t1 = timeMesh[i - 1];
        //        double t2 = timeMesh[i - 2];
        //        
        //        double deltT = t0 - t2;
        //        double deltT0 = t0 - t1;
        //        double deltT1 = t1 - t2;
        //    
        //        double tau0 = (deltT + deltT0) / (deltT * deltT0);
        //        double tau1 = deltT / (deltT1 * deltT0);
        //        double tau2 = deltT0 / (deltT * deltT1);
        //    
        //        var sparceMatrix = new GlobalMatrix(ribsArr.Count);
        //        Generator.BuildPortait(ref sparceMatrix, ribsArr.Count, elemsArr);

        //        var G = new GlobalMatrix(sparceMatrix);
        //        Generator.FillMatrixG(ref G, ribsArr, elemsArr);
        //
        //        var M = new GlobalMatrix(sparceMatrix);
        //        Generator.FillMatrixM(ref M, ribsArr, elemsArr);
        //
        //        Matrix = G + M + tau0 * M;

        //        var b = new GlobalVector(ribsArr.Count);
        //        Generator.FillVector3D(ref b, ribsArr, elemsArr, timeMesh[i]);
        //        Vector = b - tau2 * M * Solutions[i - 2] + tau1 * M * Solutions[i - 1];

        //        Generator.ConsiderBoundaryConditions(ref Matrix, ref Vector, ribsArr, bordersArr, timeMesh[i]);
        //        (Solutions[i], Discrepancy[i]) = solver.Solve(Matrix, Vector);
        //    }
        //}
    }

    public void WriteData2D(string path)
    {
        if (Solutions is null) throw new ArgumentNullException("No solutions");

        for (int t = 0; t < timeMesh.Length; t++)
        {
            using var sw = new StreamWriter(path + $"/Answer_{timeMesh[t]}.txt");
            for (int i = 0; i < Solutions[t].Size; i++)
                if (i == 16 || i == 24 || i == 26 || i == 27 || i == 29 || i == 37)
                    sw.WriteLine($"{i} {Solutions[t][i]:E8}");
            sw.Close();
        }
    }

    public void WriteData(string path)
    {
        if (Solutions is null) throw new ArgumentNullException("No solutions");

        for (int t = 0; t < timeMesh.Length; t++)
        {
            using var sw = new StreamWriter(path + $"/A_phi/Answer3D/Answer_{timeMesh[t]}.txt");
            for (int i = 0; i < Solutions[t].Size; i++)
                if (i == 16 || i == 24 || i == 26 || i == 27 || i == 29 || i == 37)
                    sw.WriteLine($"{i} {Solutions[t][i]:E8}");
            sw.Close();
        }
    }

    public void TestPoint(double x, double y)
    {
        Point testPoint = new(x, y);

        if (mesh2D.nodesX[0] <= x && x <= mesh2D.nodesX[^1] &&
            mesh2D.nodesY[0] <= y && y <= mesh2D.nodesY[^1])
        {
            foreach (var elem in elemsArr)
            {
                int[] elem_local = [elem[1], elem[2], 
                                    elem[0], elem[3]];

                var ribX = ribsArr[elem_local[2]];
                var ribY = ribsArr[elem_local[0]];

                // if inside local elem.
                if (ribX.a.X <= x && x <= ribX.b.X &&
                    ribY.a.Y <= y && y <= ribY.b.Y)
                {
                    var eps = (x - ribX.a.X) / (ribX.b.X - ribX.a.X);
                    var nu = (y - ribY.a.Y) / (ribY.b.Y - ribY.a.Y);


                    double[] q = [Solutions[0][elem_local[0]], Solutions[0][elem_local[1]], 
                                  Solutions[0][elem_local[2]], Solutions[0][elem_local[3]]];


                    var ans = BasisFunctions2DVec.GetValue(eps, nu, q);
                    var theorValue = Function.A(x, y, 0.0D);

                    Console.WriteLine($"FEM A  {ans.Item1:E15} {ans.Item2:E15}");
                    Console.WriteLine($"Theor  {theorValue.Item1:E15} {theorValue.Item2:E15}");

                    var currAbsDiscX = Math.Abs(ans.Item1 - theorValue.Item1);
                    var currAbsDiscY = Math.Abs(ans.Item2 - theorValue.Item2);

                    var currRelDiscX = currAbsDiscX / Math.Abs(theorValue.Item1);
                    var currRelDiscY = currAbsDiscY / Math.Abs(theorValue.Item2);

                    Console.WriteLine($"CurrAD {currAbsDiscX:E15} {currAbsDiscY:E15}");
                    Console.WriteLine($"CurrRD {currRelDiscX:E15} {currRelDiscY:E15}\n");
                    break;
                }
            }
        }
    }

    public void TestPoint(double x, double y, double z)
    {
        Point testPoint = new(x, y, z);

        if (mesh.nodesX[0] <= x && x <= mesh.nodesX[^1] &&
            mesh.nodesY[0] <= y && y <= mesh.nodesY[^1] &&
            mesh.nodesZ[0] <= z && z <= mesh.nodesZ[^1])
        {
            var absDiscX = 0.0D;
            var absDiscY = 0.0D;
            var absDiscZ = 0.0D;

            var absDivX = 0.0D;
            var absDivY = 0.0D;
            var absDivZ = 0.0D;

            var relDiscX = 0.0D;
            var relDiscY = 0.0D;
            var relDiscZ = 0.0D;

            var relDivX = 0.0D;
            var relDivY = 0.0D;
            var relDivZ = 0.0D;

            foreach (var elem in elemsArr)
            {
                int[] elem_local = [elem[0], elem[3], elem[8], elem[11],
                                    elem[1], elem[2], elem[9], elem[10],
                                    elem[4], elem[5], elem[6], elem[7]];

                var ribX = ribsArr[elem_local[0]];
                var ribY = ribsArr[elem_local[4]];
                var ribZ = ribsArr[elem_local[8]];

                // if inside local elem.
                if (ribX.a.X <= x && x <= ribX.b.X &&
                    ribY.a.Y <= y && y <= ribY.b.Y &&
                    ribZ.a.Z <= z && z <= ribZ.b.Z)
                {
                    var eps = (x - ribX.a.X) / (ribX.b.X - ribX.a.X);
                    var nu = (y - ribY.a.Y) / (ribY.b.Y - ribY.a.Y);
                    var khi = (z - ribZ.a.Z) / (ribZ.b.Z - ribZ.a.Z);


                    double[] q = [Solutions[0][elem_local[0]], Solutions[0][elem_local[1]], Solutions[0][elem_local[2]], Solutions[0][elem_local[3]],
                                  Solutions[0][elem_local[4]], Solutions[0][elem_local[5]], Solutions[0][elem_local[6]], Solutions[0][elem_local[7]],
                                  Solutions[0][elem_local[8]], Solutions[0][elem_local[9]], Solutions[0][elem_local[10]], Solutions[0][elem_local[11]]];


                    var ans = BasisFunctions3DVec.GetValue(eps, nu, khi, q);
                    var theorValue = Function.A(x, y, z, 0.0D);

                    Console.WriteLine($"FEM A  {ans.Item1:E15} {ans.Item2:E15} {ans.Item3:E15}");
                    Console.WriteLine($"Theor  {theorValue.Item1:E15} {theorValue.Item2:E15} {theorValue.Item3:E15}");

                    var currAbsDiscX = Math.Abs(ans.Item1 - theorValue.Item1);
                    var currAbsDiscY = Math.Abs(ans.Item2 - theorValue.Item2);
                    var currAbsDiscZ = Math.Abs(ans.Item3 - theorValue.Item3);

                    var currRelDiscX = currAbsDiscX / Math.Abs(theorValue.Item1);
                    var currRelDiscY = currAbsDiscY / Math.Abs(theorValue.Item2);
                    var currRelDiscZ = currAbsDiscZ / Math.Abs(theorValue.Item3);

                    Console.WriteLine($"CurrAD {currAbsDiscX:E15} {currAbsDiscY:E15} {currAbsDiscZ:E15}");
                    Console.WriteLine($"CurrRD {currRelDiscX:E15} {currRelDiscY:E15} {currRelDiscZ:E15}\n");

                    break;
                }
            }
        }
    }

public void TestOutput2D(string path)
    {
        using var sw = new StreamWriter(path + "/Answer_Test.txt");

        var absDiscX = 0.0D;
        var absDiscY = 0.0D;

        var absDivX = 0.0D;
        var absDivY = 0.0D;

        var relDiscX = 0.0D;
        var relDiscY = 0.0D;

        var relDivX = 0.0D;
        var relDivY = 0.0D;

        var squareDiffX = 0.0D;
        var squareDiffY = 0.0D;

        int iter = 0;

        foreach (var elem in elemsArr)
        {
            int[] elem_local = [elem[1], elem[2], 
                                elem[0], elem[3]];

            var x = 0.5D * (ribsArr[elem_local[2]].a.X + ribsArr[elem_local[2]].b.X);
            var y = 0.5D * (ribsArr[elem_local[0]].a.Y + ribsArr[elem_local[0]].b.Y);

            sw.WriteLine($"Points {x:E15} {y:E15}");

            var eps = (x - ribsArr[elem_local[2]].a.X) / (ribsArr[elem_local[2]].b.X - ribsArr[elem_local[2]].a.X);
            var nu = (y - ribsArr[elem_local[0]].a.Y) / (ribsArr[elem_local[0]].b.Y - ribsArr[elem_local[0]].a.Y);

            double[] q = [Solutions[0][elem_local[0]], Solutions[0][elem_local[1]], 
                          Solutions[0][elem_local[2]], Solutions[0][elem_local[3]]];

            var ans = BasisFunctions2DVec.GetValue(eps, nu, q);
            var theorValue = Function.A(x, y, 0.0D);

            sw.WriteLine($"FEM A  {ans.Item1:E15} {ans.Item2:E15}");
            sw.WriteLine($"Theor  {theorValue.Item1:E15} {theorValue.Item2:E15}");

            var currAbsDiscX = Math.Abs(ans.Item1 - theorValue.Item1);
            var currAbsDiscY = Math.Abs(ans.Item2 - theorValue.Item2);

            squareDiffX += currAbsDiscX * currAbsDiscX;
            squareDiffY += currAbsDiscY * currAbsDiscY;

            var currRelDiscX = currAbsDiscX / Math.Abs(theorValue.Item1);
            var currRelDiscY = currAbsDiscY / Math.Abs(theorValue.Item2);

            sw.WriteLine($"CurrAD {currAbsDiscX:E15} {currAbsDiscY:E15}");
            sw.WriteLine($"CurrRD {currRelDiscX:E15} {currRelDiscY:E15}\n");

            absDiscX += currAbsDiscX;
            absDiscY += currAbsDiscY;

            absDivX += theorValue.Item1;
            absDivY += theorValue.Item2;

            relDiscX += currRelDiscX * currRelDiscX;
            relDiscY += currRelDiscY * currRelDiscY;

            relDivX += theorValue.Item1 * theorValue.Item1;
            relDivY += theorValue.Item2 * theorValue.Item2;

            iter++;
        }
        sw.WriteLine($"Avg disc: {absDiscX / iter:E15} {absDiscY / iter:E15}");
        sw.WriteLine($"Rel disc: {Math.Sqrt(relDiscX / relDivX):E15} {Math.Sqrt(relDiscY / relDivY):E15}");
        sw.WriteLine($"SKO: {Math.Sqrt(squareDiffX / elemsArr.Length):E15} {Math.Sqrt(squareDiffY / elemsArr.Length):E15}");
        sw.Close();
    }


    public void TestOutput(string path)
    {
        using var sw = new StreamWriter(path + "/A_phi/Answer3D/Answer_Test.txt");

        var absDiscX = 0.0D;
        var absDiscY = 0.0D;
        var absDiscZ = 0.0D;

        var absDivX = 0.0D;
        var absDivY = 0.0D;
        var absDivZ = 0.0D;

        var relDiscX = 0.0D;
        var relDiscY = 0.0D;
        var relDiscZ = 0.0D;

        var relDivX = 0.0D;
        var relDivY = 0.0D;
        var relDivZ = 0.0D;

        var squareDiffX = 0.0D;
        var squareDiffY = 0.0D;
        var squareDiffZ = 0.0D;

        int iter = 0;

        foreach (var elem in elemsArr)
        {
            int[] elem_local = [elem[0], elem[3], elem[8], elem[11],
                                elem[1], elem[2], elem[9], elem[10],
                                elem[4], elem[5], elem[6], elem[7]];

            var x = 0.5D * (ribsArr[elem_local[0]].a.X + ribsArr[elem_local[0]].b.X);
            var y = 0.5D * (ribsArr[elem_local[4]].a.Y + ribsArr[elem_local[4]].b.Y);
            var z = 0.5D * (ribsArr[elem_local[8]].a.Z + ribsArr[elem_local[8]].b.Z);

            sw.WriteLine($"Points {x:E15} {y:E15} {z:E15}");

            var eps = (x - ribsArr[elem_local[0]].a.X) / (ribsArr[elem_local[0]].b.X - ribsArr[elem_local[0]].a.X);
            var nu = (y - ribsArr[elem_local[4]].a.Y) / (ribsArr[elem_local[4]].b.Y - ribsArr[elem_local[4]].a.Y);
            var khi = (z - ribsArr[elem_local[8]].a.Z) / (ribsArr[elem_local[8]].b.Z - ribsArr[elem_local[8]].a.Z);

            double[] q = [Solutions[0][elem_local[0]], Solutions[0][elem_local[1]], Solutions[0][elem_local[2]], Solutions[0][elem_local[3]],
                          Solutions[0][elem_local[4]], Solutions[0][elem_local[5]], Solutions[0][elem_local[6]], Solutions[0][elem_local[7]],
                          Solutions[0][elem_local[8]], Solutions[0][elem_local[9]], Solutions[0][elem_local[10]], Solutions[0][elem_local[11]]];

            var ans = BasisFunctions3DVec.GetValue(eps, nu, khi, q);
            var theorValue = Function.A(x, y, z, 0.0D);

            sw.WriteLine($"FEM A  {ans.Item1:E15} {ans.Item2:E15} {ans.Item3:E15}");
            sw.WriteLine($"Theor  {theorValue.Item1:E15} {theorValue.Item2:E15} {theorValue.Item3:E15}");

            var currAbsDiscX = Math.Abs(ans.Item1 - theorValue.Item1);
            var currAbsDiscY = Math.Abs(ans.Item2 - theorValue.Item2);
            var currAbsDiscZ = Math.Abs(ans.Item3 - theorValue.Item3);

            squareDiffX += currAbsDiscX * currAbsDiscX;
            squareDiffY += currAbsDiscY * currAbsDiscY;
            squareDiffZ += currAbsDiscZ * currAbsDiscZ;

            var currRelDiscX = currAbsDiscX / Math.Abs(theorValue.Item1);
            var currRelDiscY = currAbsDiscY / Math.Abs(theorValue.Item2);
            var currRelDiscZ = currAbsDiscZ / Math.Abs(theorValue.Item3);

            sw.WriteLine($"CurrAD {currAbsDiscX:E15} {currAbsDiscY:E15} {currAbsDiscZ:E15}");
            sw.WriteLine($"CurrRD {currRelDiscX:E15} {currRelDiscY:E15} {currRelDiscZ:E15}\n");

            absDiscX += currAbsDiscX;
            absDiscY += currAbsDiscY;
            absDiscZ += currAbsDiscZ;

            absDivX += theorValue.Item1;
            absDivY += theorValue.Item2;
            absDivZ += theorValue.Item3;

            relDiscX += currRelDiscX * currRelDiscX;
            relDiscY += currRelDiscY * currRelDiscY;
            relDiscZ += currRelDiscZ * currRelDiscZ;

            relDivX += theorValue.Item1 * theorValue.Item1;
            relDivY += theorValue.Item2 * theorValue.Item2;
            relDivZ += theorValue.Item3 * theorValue.Item3;

            iter++;
        }
        sw.WriteLine($"Avg disc: {absDiscX / iter:E15} {absDiscY / iter:E15} {absDiscZ / iter:E15}");
        sw.WriteLine($"Rel disc: {Math.Sqrt(relDiscX / relDivX):E15} {Math.Sqrt(relDiscY / relDivY):E15} {Math.Sqrt(relDiscZ / relDivZ):E15}");
        sw.WriteLine($"SKO: {Math.Sqrt(squareDiffX / elemsArr.Length):E15} {Math.Sqrt(squareDiffY / elemsArr.Length):E15} {Math.Sqrt(squareDiffZ / elemsArr.Length):E15}");
        sw.Close();
    }
}