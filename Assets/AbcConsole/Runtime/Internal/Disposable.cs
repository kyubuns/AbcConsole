using System;

namespace AbcConsole.Internal
{
    public class Disposable : IDisposable
    {
        public static IDisposable Create(Action callback)
        {
            return new Disposable(callback);
        }

        private Action _callback;

        public Disposable(Action callback)
        {
            _callback = callback;
        }

        public void Dispose()
        {
            _callback?.Invoke();
            _callback = null;
        }
    }
}