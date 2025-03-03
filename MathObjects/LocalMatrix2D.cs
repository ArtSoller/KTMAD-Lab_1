namespace MathObjects;

public class LocalMatrixG2D (double mu, double hx, double hy) : Matrix
{
    private readonly double _mu = mu;
    private readonly double _hx = hx;
    private readonly double _hy = hy;

    public override double this[int i, int j]
    {
        get
        {
            return (i % 4, j % 4)
            switch
            {
                (0, 0) or (1, 1) => _hy / (_hx * _mu),
                (0, 1) or (1, 0) => -1.0D * _hy / (_mu * _hx),
                (0, 2) or (1, 3) or (2, 0) or (3, 1) => -1.0D / _mu,
                (0, 3) or (1, 2) or (2, 1) or (3, 0) =>  1.0D / _mu,
                (2, 2) or (3, 3) => _hx / (_hy * _mu),
                (2, 3) or (3, 2) => -1.0D * _hx / (_mu * _hy),
                _ => throw new ArgumentOutOfRangeException("Out of local matrix 2d range"),
            };
        }
        set{}
    }
}

public class LocalMatrixM2D(double gamma, double hx, double hy) : Matrix
{
    private readonly double _gamma = gamma;
    private readonly double _hx = hx;
    private readonly double _hy = hy;

    private readonly double[,] D = {{2.0D, 1.0D, 0.0D, 0.0D},
                                    {1.0D, 2.0D, 0.0D, 0.0D},
                                    {0.0D, 0.0D, 2.0D, 1.0D},
                                    {0.0D, 0.0D, 1.0D, 2.0D}};

    public override double this[int i, int j]
    {
        get => _gamma * _hx * _hy * D[i, j] / 6.0D;
        set{}
    }
}