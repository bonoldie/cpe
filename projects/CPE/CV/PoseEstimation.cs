using System;
using System.Drawing;
using System.Linq;
using CPE.Utils;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Storage;

namespace CPE.CV
{

    public enum PoseEstimationMethods
    {
        Fiore
    }

    public struct Pose
    {
        public Matrix<double> Rotation;
        public MathNet.Numerics.LinearAlgebra.Vector<double> Translation;
        public double Scale;
    }

    public class PoseEstimation
    {
        public PoseEstimationMethods Method = PoseEstimationMethods.Fiore;

        public PoseEstimation()
        {

        }

        public Pose EstimatePose(Matrix<double> Points2D, Matrix<double> Points3D, Matrix<double> K)
        {
            // Svd(Points2D);

            var colOnes2D = CreateVector.Dense(new double[Points2D.RowCount]);
            colOnes2D.Clear();
            colOnes2D = colOnes2D.Add(1);

            var colOnes3D = CreateVector.Dense(new double[Points3D.RowCount]);
            colOnes3D.Clear();
            colOnes3D = colOnes3D.Add(1);

            Points2D = Points2D.Append(colOnes2D.ToColumnMatrix()).Transpose();
            Points3D = Points3D.Append(colOnes3D.ToColumnMatrix()).Transpose();

            Logger.NLogger.Info(Points3D.ToString(Points3D.RowCount, Points3D.ColumnCount));
            Logger.NLogger.Info(Points2D.ToString(Points2D.RowCount, Points2D.ColumnCount));

            // TODO: check matrices sizes
            // TODO: check matrices normalization

            int N = Points2D.ColumnCount;
            int R = Points3D.Rank();

            if (((3 * R - 1) / 2) > N)
            {
                throw new Exception("Not enought correspondences");
            }

            var V = Points3D.Svd().VT.Transpose();


            var VR = V.SubMatrix(0, V.RowCount, R, V.ColumnCount - R);


            Matrix<double> D = CreateMatrix.Dense<double>(3 * N, N);

            for (int i = 0; i < N; i++)
            {
                D.SetSubMatrix(i * 3, i, Points2D.Column(i).ToColumnMatrix());
            }

            var VRKron = VR.Transpose().KroneckerProduct(K.Inverse());


            var VRKronD = VRKron * D;


            var VFinal = VRKronD.Svd().VT.Transpose();

            var VFinalCol = VFinal.Column(VFinal.ColumnCount - 1);

            var Mol = K.Inverse() * Points2D;

            var VFinal3Col = VFinalCol.ToRowMatrix().Transpose().Append(VFinalCol.ToRowMatrix().Transpose()).Append(VFinalCol.ToRowMatrix().Transpose()).Transpose();

            var X = VFinal3Col.PointwiseMultiply(Mol);

            // Absolute orientation estimation

            Points3D = Points3D.SubMatrix(0, 3, 0, Points3D.ColumnCount);

            var XMean = X.ReduceColumns((v1, v2) => v1 + v2) / X.ColumnCount;

            var Points3DMean = Points3D.ReduceColumns((v1, v2) => v1 + v2) / Points3D.ColumnCount;

            // Logger.NLogger.Info(Points3DMean.ToString());

            // Points3D = CreateMatrix.Dense<double>();

            for (int i = 0; i < 3; ++i)
            {
                Points3D.SetRow(i, Points3D.Row(i).Subtract(Points3DMean[i]));
                X.SetRow(i, X.Row(i).Subtract(XMean[i]));
            }

            double s = X.Column(0).Norm(2) / Points3D.Column(0).Norm(2);

            var  __ = (Points3D * X.Transpose()).Svd();
            var VTPos = __.VT;
            var UPos = __.U;

            var SigmaVal = CreateVector.Dense(new double[] { 1, 1, (UPos * VTPos).Determinant() });

            var Sigma = CreateMatrix.DenseDiagonal(3,.0d);

            Sigma.SetDiagonal(SigmaVal);

            var RPos = (UPos * Sigma * VTPos).Transpose();

            var  TPos = ((1/s) * XMean) - (RPos * Points3DMean); 

            return new Pose(){Rotation=RPos, Translation=TPos, Scale=s};        
        }
    }

}