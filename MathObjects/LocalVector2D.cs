using static Functions.Function;

namespace MathObjects;


public class LocalVector2D : Vector
{
    private readonly double x0;
    private readonly double x1;
    private readonly double y0;
    private readonly double y1;
    private readonly double xm;
    private readonly double ym;
    private readonly double t;

    private LocalMatrixM2D M;

    public override double this[int i] 
    { 
        get 
        {
            static double ScalarMult((double, double) a, (double, double) b) 
            => a.Item1 * b.Item1 + a.Item2 * b.Item2;

            (double, double) Divide ((double, double) v, double sc)
            => (v.Item1 / sc, v.Item2 / sc); 

            double Length((double, double) v) 
            => Math.Sqrt(Math.Pow(v.Item1, 2) + Math.Pow(v.Item2, 2));

            // Очень спорно.
            List<double> vect = [
                ScalarMult(F(x0, ym, t), (0.0D, 1.0D)),
                ScalarMult(F(x1, ym, t), (0.0D, 1.0D)),
                ScalarMult(F(xm, y0, t), (1.0D, 0.0D)),
                ScalarMult(F(xm, y1, t), (1.0D, 0.0D))
            ];
            
            double ans = 0.0D;
            for (int j = 0; j < vect.Count; j++)
                ans += M[i, j] * vect[j];
            return ans;
        }
    }

    public LocalVector2D(double x0, double x1, double y0, double y1, double t)
    {
        this.x0 = x0;
        this.x1 = x1;
        this.y0 = y0;
        this.y1 = y1;
        this.t = t;
        xm = 0.5D * (x1 + x0);
        ym = 0.5D * (y1 + y0);
        M = new(1.0, x1 - x0, y1 - y0);
        Generate();
    }

    private void Generate()
    {
        static double ScalarMult((double, double) a, (double, double) b) 
        => a.Item1 * b.Item1 + a.Item2 * b.Item2;

        List<double> vect = [
            ScalarMult(F(x0, ym, t), (0.0D, 1.0D)),
            ScalarMult(F(x1, ym, t), (0.0D, 1.0D)),
            ScalarMult(F(xm, y0, t), (1.0D, 0.0D)),
            ScalarMult(F(xm, y1, t), (1.0D, 0.0D)),
        ];

        double ans = 0.0D;
        for (int i = 0; i < vect.Count; i++)
        {
            for (int j = 0; j < vect.Count; j++)
                ans += M[i, j] * vect[j];
            ans = 0.0D;
        }
    }
}