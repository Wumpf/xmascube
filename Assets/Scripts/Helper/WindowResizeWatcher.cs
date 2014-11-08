using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Helper
{
    class WindowResizeWatcher : IDisposable
    {
        public delegate void ResizeHandler(int width, int height);
        public ResizeHandler ResizeEvent;

        private int _lastWidth = -1;
        private int _lastHeight = -1;
        private bool _running = true;

        public IEnumerator CheckForResize()
        {
            while (_running)
            {
                if (_lastWidth != Screen.width || _lastHeight != Screen.height)
                {
                    if (ResizeEvent != null)
                        ResizeEvent(_lastWidth, _lastHeight);
                    _lastWidth = Screen.width;
                    _lastHeight = Screen.height;
                }
                yield return new WaitForSeconds(0.3f);
            }
        }

        public void Dispose()
        {
            _running = false;
        }
        ~WindowResizeWatcher()
        {
            _running = false;
        }
    }
}
