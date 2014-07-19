﻿using System;
using RegionServer.Model.Interfaces;
using SubServerCommon.Math;

namespace RegionServer.Calculators.Lambdas
{
    public class LambdaRandom : ILambda
    {
        private readonly Random _rand;
        private readonly ILambda _max;
        private readonly bool _linear;

        public LambdaRandom(ILambda max, bool linear = true)
        {
            _rand = new Random();
            _max = max;
            _linear = linear;
        }

        #region Implementation of ILambda

        public float Calculate(Environment env)
        {
            if (_linear)
            {
                return _max.Calculate(env)*(float) _rand.NextDouble();
            }

            return _max.Calculate(env)*(float) _rand.NextGaussian();
        }

        #endregion
    }
}