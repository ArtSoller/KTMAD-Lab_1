using System.Collections.Immutable;
using System.Diagnostics;
using DataStructs;
using System.Reflection.Metadata;

namespace Grid;

public static class MeshReader
{
    private static readonly double mu0 = 4.0D * Math.PI * Math.Pow(10.0D, -7);

    public static void ReadMesh(string meshPath, string bordersPath, ref Mesh3Dim mesh)
    {
        string _currPath = meshPath;
        try
        {
            // Считывание параметров для расчетной области.
            using (var sr = new StreamReader(_currPath))
            {
                mesh.nodesX = sr.ReadLine().Split().Select(double.Parse).ToList();
                for (int i = 0; i < mesh.nodesX.Count; i++)
                    mesh.nodesXRefs.Add(i);
                mesh.NodesXWithoutFragmentation = mesh.nodesX.ToImmutableArray();
                mesh.infoAboutX = sr.ReadLine() ?? "";


                mesh.nodesY = sr.ReadLine().Split().Select(double.Parse).ToList();
                for (int i = 0; i < mesh.nodesY.Count; i++)
                    mesh.nodesYRefs.Add(i);
                mesh.NodesYWithoutFragmentation = mesh.nodesY.ToImmutableArray();
                mesh.infoAboutY = sr.ReadLine() ?? "";

                mesh.nodesZ = sr.ReadLine().Split().Select(double.Parse).ToList();
                for (int i = 0; i < mesh.nodesZ.Count; i++)
                    mesh.nodesZRefs.Add(i);
                mesh.NodesZWithoutFragmentation = mesh.nodesZ.ToImmutableArray();
                mesh.infoAboutZ = sr.ReadLine() ?? "";

                int elemsAmount = int.Parse(sr.ReadLine() ?? "0");
                for (int i = 0; i < elemsAmount; i++)
                {
                    string[] str = sr.ReadLine().Split();
                    mesh.mu0.Add(double.Parse(str[7]));
                    mesh.sigma.Add(double.Parse(str[8]));
                    mesh.Elems.Add([int.Parse(str[0]), int.Parse(str[1]),
                                int.Parse(str[2]), int.Parse(str[3]),
                                int.Parse(str[4]), int.Parse(str[5]),
                                int.Parse(str[6])]);

                }
            }

            // Считывание данных для границ.
            _currPath = bordersPath;
            using (var sr = new StreamReader(_currPath))
            {
                mesh.bordersAmount = int.Parse(sr.ReadLine() ?? "0");
                for (int i = 0; i < mesh.bordersAmount; i++)
                    mesh.borders.Add(sr.ReadLine().Split().Select(int.Parse).ToList());
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error during reading {_currPath} file. Wrong data format: {ex}");
            throw new IOException();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during reading {_currPath} file: {ex}");
            throw new IOException();
        }
    }

    public static void ReadMesh2D(string meshPath, string bordersPath, ref Mesh2Dim mesh)
    {
        string _currPath = meshPath;
        try
        {
            // Считывание параметров для расчетной области.
            using (var sr = new StreamReader(_currPath))
            {
                mesh.nodesX = [.. sr.ReadLine().Split().Select(double.Parse)];
                for (int i = 0; i < mesh.nodesX.Count; i++)
                    mesh.nodesXRefs.Add(i);
                mesh.NodesXWithoutFragmentation = [.. mesh.nodesX];
                mesh.infoAboutX = sr.ReadLine() ?? "";

                mesh.nodesY = [.. sr.ReadLine().Split().Select(double.Parse)];
                for (int i = 0; i < mesh.nodesY.Count; i++)
                    mesh.nodesYRefs.Add(i);
                mesh.NodesYWithoutFragmentation = [.. mesh.nodesY];
                mesh.infoAboutY = sr.ReadLine() ?? "";

                int elemsAmount = int.Parse(sr.ReadLine() ?? "0");
                for (int i = 0; i < elemsAmount; i++)
                {
                    string[] str = sr.ReadLine().Split();
                    mesh.mu0.Add(double.Parse(str[5]));
                    mesh.sigma.Add(double.Parse(str[6]));
                    mesh.Elems.Add([int.Parse(str[0]), int.Parse(str[1]),
                                int.Parse(str[2]), int.Parse(str[3]),
                                int.Parse(str[4])]);

                }
            }

            // Считывание данных для границ.
            _currPath = bordersPath;
            using (var sr = new StreamReader(_currPath))
            {
                mesh.bordersAmount = int.Parse(sr.ReadLine() ?? "0");
                for (int i = 0; i < mesh.bordersAmount; i++)
                    mesh.borders.Add([.. sr.ReadLine().Split().Select(int.Parse)]);
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error during reading {_currPath} file. Wrong data format: {ex}");
            throw new IOException();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during reading {_currPath} file: {ex}");
            throw new Exception();
        }
    }      

    public static Mesh3Dim ReadMesh(string path)
    {
        var mesh = new Mesh3Dim();
        try
        {
            using var sr = new StreamReader(path);

            mesh.nodesX = sr.ReadLine().Split().Select(double.Parse).ToList();
            for (int i = 0; i < mesh.nodesX.Count; i++)
                mesh.nodesXRefs.Add(i);            
            mesh.NodesXWithoutFragmentation = mesh.nodesX.ToImmutableArray();
            mesh.infoAboutX = sr.ReadLine() ?? "";

            mesh.nodesY = sr.ReadLine().Split().Select(double.Parse).ToList();
            for (int i = 0; i < mesh.nodesY.Count; i++)
                mesh.nodesYRefs.Add(i);
            mesh.NodesYWithoutFragmentation = mesh.nodesY.ToImmutableArray();
            mesh.infoAboutY = sr.ReadLine() ?? "";

            mesh.nodesZ = sr.ReadLine().Split().Select(double.Parse).ToList();
            for (int i = 0; i < mesh.nodesZ.Count; i++)
                mesh.nodesZRefs.Add(i);
            mesh.NodesZWithoutFragmentation = mesh.nodesZ.ToImmutableArray();
            mesh.infoAboutZ = sr.ReadLine() ?? "";

            string[] str = sr.ReadLine().Split();
            mesh.mu0.Add(double.Parse(str[0]));
            mesh.sigma.Add(double.Parse(str[1]));
        }
        catch(Exception ex)
        {
            Debug.WriteLine($"Exception during reading file {path}: {ex.Message}.\nReturn empty mesh.");
        }
        return mesh;    
    }
}