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

    public int NodesAmount => nodesX.Count * nodesY.Count ; 

    internal List<int> nodesXRefs;

    internal ImmutableArray<double> NodesXWithoutFragmentation { get; set; }

    internal string? infoAboutX;

    internal List<int> nodesYRefs;

    public ImmutableArray<double> NodesYWithoutFragmentation { get; set; }

    internal string? infoAboutY;
    

    public Mesh2Dim()
    {
        borders = [];
        Elems = [];
        nodesX = [];
        nodesY = [];
        nodesXRefs = [];
        nodesYRefs = [];
        mu0 = [];
        sigma = [];
    }
}
