using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityEngine.Profiling
{

    internal class ProfilerSample : IDisposable
    {
        private bool disposed;

        public ProfilerSample(string name, Object targetObject = null)
        {
            if (targetObject)
                Profiler.BeginSample(name, targetObject);
            else
                Profiler.BeginSample(name);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                Profiler.EndSample();
            }
        }
    }

}