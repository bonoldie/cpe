using System;
using CPE.Utils;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace CPE.CV
{

    public enum PoseEstimationMethods
    {
        Fiore
    }

    public struct Pose
    {
        Matrix<double> Rotation;
        MathNet.Numerics.LinearAlgebra.Vector<double> Translation;
    }

    public class PoseEstimation
    {

        public PoseEstimationMethods Method = PoseEstimationMethods.Fiore;

        public PoseEstimation()
        {

        }

        public Pose estimatePose(Matrix<double> Points2D, Matrix<double> Points3D, Matrix<double> K)
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

            // TODO: check matrices sizes
            // TODO: check matrices normalization

            int N = Points2D.ColumnCount;
            int R = Points3D.Rank();

            if (((3 * R - 1) / 2) > N)
            {
                throw new Exception("Not enought correspondences");
            }

            var V = Points3D.Svd().VT.Transpose();

            //Logger.NLogger.Info(V.ToString(V.RowCount, V.ColumnCount));
            //Logger.NLogger.Info(Points3D.ToString(Points3D.RowCount, Points3D.ColumnCount));

            var VR = V.SubMatrix(R, V.ColumnCount - R, 0, V.RowCount);

            Matrix<double> D = CreateMatrix.Dense<double>(3 * N, N);

            for (int i = 0; i < N; i++)
            {
                D.SetSubMatrix(i * 3, i, Points2D.Column(i).ToColumnMatrix());
            }

            var VRKron = VR.KroneckerProduct(K.Inverse());

            var VFinal = (VRKron * D).Svd().VT;

            var VFinalCol = VFinal.Column(VFinal.ColumnCount - 1);
            Logger.NLogger.Info(VFinalCol.ToString());


            return new Pose();
        }
    }

}