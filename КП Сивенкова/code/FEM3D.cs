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

public class FEM3D : FEM
{
    public ArrayOfRibs ribsArr;

    // Maybe private?
    public List<Layer> Layers;

    public FEM3D()
    {
        Layers = [];
        mesh = new Mesh3Dim
        {
            nodesX = [],
            nodesY = [],
            nodesZ = []
        };
    }

    public void ConstructMesh(int nx, int ny, int nz)
    {
        if (mesh == null) throw new ArgumentNullException("Mesh is null");
        
        double hx = 2.0D;
        double hy = 2.0D;
        double hz = 2.0D;
        
        mesh.nodesX = [];
        mesh.nodesY = [];
        mesh.nodesZ = [];

        for (int i = 0; i < nx; i++)
        {
            double stepx = hx / (nx - 1);
            mesh.nodesX.Add(i * stepx);
        }

        for (int i = 0; i < ny; i++)
        {
            double stepy = hy / (ny - 1);
            mesh.nodesY.Add(i * stepy);
        }
        
        for (int i = 0; i < nz; i++)
        {
            double stepz = hz / (nz - 1);
            mesh.nodesZ.Add(i * stepz);
        }

        timeMesh = [1.0D];
    }

    

    public void GenerateArrays()
    {
        if (mesh is null) throw new ArgumentNullException("mesh is null!");
        pointsArr = MeshGenerator.GenerateListOfPoints(mesh);
        ribsArr = MeshGenerator.GenerateListOfRibs(mesh, pointsArr);
        elemsArr = MeshGenerator.GenerateListOfElems(mesh, ribsArr);
        bordersArr = MeshGenerator.GenerateListOfBorders(mesh);
    }

    public void AddField(Layer layer)
    {
        ArgumentNullException.ThrowIfNull(layer);
        Layers.Add(layer);
    }

    public void CommitFields()
    {
        if (elemsArr is null) throw new ArgumentNullException("elemsArr is null");

        foreach (var layer in Layers)
        {
            for (int i = 0; i < elemsArr.Length; i++)
            {
                double minz = Math.Min(ribsArr[elemsArr[i][^1]].a.Z, ribsArr[elemsArr[i][^1]].b.Z);
                double maxz = Math.Max(ribsArr[elemsArr[i][^1]].a.Z, ribsArr[elemsArr[i][^1]].b.Z);
                if (layer.z0 <= minz && maxz <= layer.z1)
                    elemsArr.sigmai[i] = layer.sigma;
            }
        }
    }

    public void ConstructMatrixAndVector()
    {
        if (elemsArr is null) throw new ArgumentNullException("elemsArr is null");
        if (bordersArr is null) throw new ArgumentNullException("bordersArr is null");
        
        var sparceMatrix = new GlobalMatrix(ribsArr.Count);
        Generator.BuildPortait(ref sparceMatrix, ribsArr.Count, elemsArr);

        var G = new GlobalMatrix(sparceMatrix);
        Generator.FillMatrixG(ref G, ribsArr, elemsArr);
        
        var M = new GlobalMatrix(sparceMatrix);
        Generator.FillMatrixM(ref M, ribsArr, elemsArr);
        
        Matrix = G + M;

        var b = new GlobalVector(ribsArr.Count);
        Generator.FillVector3D(ref b, ribsArr, elemsArr, 0.0);
        Vector = b;

        Generator.ConsiderBoundaryConditions(ref Matrix, ref Vector, ribsArr, bordersArr, 0.0D);
    }

    public void Solve()
    {
        if (solver is null) throw new ArgumentNullException("Solver is null");
        if (Matrix is null) throw new ArgumentNullException("Matrix is null");
        if (Vector is null) throw new ArgumentNullException("Vector is null");
        Solutions = new GlobalVector[timeMesh.Length];
        Discrepancy = new GlobalVector[timeMesh.Length];
        (Solutions[0], Discrepancy[0]) = solver.Solve(Matrix, Vector);
    }

    public void TestOutput(string path)
    {
        using var sw = new StreamWriter(path + "/Answer3D/Answer_Test.txt");
        
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
            var nu =  (y - ribsArr[elem_local[4]].a.Y) / (ribsArr[elem_local[4]].b.Y - ribsArr[elem_local[4]].a.Y);
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