using System;
using System.Timers;

namespace ToolKit {
    public class DelayedCall {
        public static DelayedCall New(float delay, Action action) {
            return new DelayedCall(delay, action);
        }

        readonly Timer _timer;

        public DelayedCall(float delay, Action action) {
            _timer = new Timer(delay * 1000);
            _timer.Elapsed += (sender, e) => action();
            _timer.AutoReset = false;
            _timer.Start();
        }

        public void Cancel() {
            _timer.Dispose();
        }
    }
}
