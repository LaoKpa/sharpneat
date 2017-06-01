﻿using System;
using System.Runtime.CompilerServices;

namespace ActivationFnBenchmarks
{
    public static class Functions
    {
        public static double LogisticFunctionSteep(double x)
        {
            return 1.0/(1.0 + Math.Exp(-4.9*x));
        }

        public static double LogisticApproximantSteep(double x)
        {
            return 1.0 / (1.0 + Exp(-4.9 * x));
        }

        public static double SoftSign(double x)
        {
            return 0.5 + (x / (2.0*(0.2+Math.Abs(x))));
        }

        // This might be based on the Pade approximant:
        //   https://en.wikipedia.org/wiki/Pad%C3%A9_approximant
        //   https://math.stackexchange.com/a/107666
        //
        // Or perhaps the maple minimax approximation:
        //   http://www.maplesoft.com/support/helpJP/Maple/view.aspx?path=numapprox/minimax
        public static double PolynomialApproximant(double x)
        {
            // Very close approximation to LogisticFunctionSteep that avoids exp.
            x = x * 4.9;
            double x2 = x*x;
            double e = 1.0 + Math.Abs(x) + x2*0.555 + x2*x2*0.143;

            double f = (x > 0) ? (1.0 / e) : e;
            return 1.0 / (1.0 + f);
        }

        public static double QuadraticSigmoid(double x)
        {
            const double t = 0.999;
            const double a = 0.00001;

            double sign = Math.Sign(x);
            x = Math.Abs(x);

            double y = 0;
            if(x >= 0 && x < t) {
                y = t - ((x - t) * (x - t));
            }
            else //if (x >= t) 
            {
                y = t + (x - t) * a;
            }

            return (y * sign * 0.5) + 0.5;
        }

        /// <summary>
        /// S-shaped rectified linear activation unit (SReLU).
        /// From:
        ///    https://en.wikipedia.org/wiki/Activation_function
        ///    https://arxiv.org/abs/1512.07030 [Deep Learning with S-shaped Rectified Linear Activation Units]
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double SReLU(double x)
        {
            const double tl = 0.001; // threshold (left).
            const double tr = 0.999; // threshold (right).
            const double a = 0.00001;

            double y;
            if(x > tl && x < tr) {
                y = x;
            }
            else if(x <= tl) {
                y = tl + (x - tl) * a;
            }
            else {
                y = tr + (x - tr) * a;
            }

            return y;
        }

        public static double SReLUShifted(double x)
        {
            const double tl = 0.001; // threshold (left).
            const double tr = 0.999; // threshold (right).
            const double a = 0.00001;

            x += 0.5;

            double y;
            if(x > tl && x < tr) {
                y = x;
            }
            else if(x <= tl) {
                y = tl + (x - tl) * a;
            }
            else {
                y = tr + (x - tr) * a;
            }

            return y;
        }

        public static double ArcTan(double x)
        {
            return (Math.Atan(x) + 1) * 0.5;

        }

        public static double TanH(double x)
        {
            const double halfpi = Math.PI / 2.0;
            const double piinv = 1.0 / Math.PI;
            return (Math.Tanh(x) + halfpi) * piinv;
        }


        // Fast exp approximation, from:
        // https://stackoverflow.com/a/412988/15703
        // https://pdfs.semanticscholar.org/35d3/2b272879a2018a2d33d982639d4be489f789.pdf (A Fast, Compact Approximation of the Exponential Function)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Exp(double val)
        {
            long tmp = (long)(1512775 * val + (1072693248 - 60801));
            return BitConverter.Int64BitsToDouble(tmp << 32);
        }
    }
}
