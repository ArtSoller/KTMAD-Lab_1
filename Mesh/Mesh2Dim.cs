namespace Grid;
using System.Collections.Immutable;
using DataStructs;

public class Mesh2Dim : Mesh
{
    public override int NodesAmountTotal 
    { 
        get => NodesAmountX * NodesAmountY;
    }

    public override int ElemsAmount
    {
        get => (NodesAmountX - 1) * (NodesAmountY - 1);
        set => ElemsAmount = value;
    }

    public int NodesAmountX
    { 
        get => nodesX.Count; 
    }

    internal List<int> nodesXRefs;

    internal ImmutableArray<double> NodesXWithoutFragmentation { get; set; }

    internal string? infoAboutX;

    public int NodesAmountY 
    { 
        get => nodesY.Count;
    }

    internal List<int> nodesYRefs;

    public ImmutableArray<double> NodesYWithoutFragmentation { get; set; }

    internal string? infoAboutY;
    

    public Mesh2Dim()
    {
        borders = new();
        Elems = new();
        nodesZ = new();
        nodesR = new();
        nodesXRefs = new();
        nodesYRefs = new();
        mu0 = new();
        sigma = new();
    }
}
