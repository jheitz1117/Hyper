using System;
using System.Threading;

namespace Hyper.NodeServices
{
    internal sealed class HyperNodeTaskInfo : IDisposable
    {
        private readonly CancellationToken _masterToken;
        private CancellationTokenSource _taskTokenSource;

        #region Properties

        public CancellationToken Token
        {
            get
            {
                if (_taskTokenSource == null)
                    _taskTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_masterToken);

                return _taskTokenSource.Token;
            }
        }

        #endregion Properties

        #region Public Methods

        public HyperNodeTaskInfo(CancellationToken masterToken)
        {
            _masterToken = masterToken;
        }

        public void Cancel()
        {
            _taskTokenSource.Cancel();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion Public Methods

        #region Private Methods

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_taskTokenSource != null)
                    _taskTokenSource.Dispose();
            }
        }

        #endregion Private Methods
    }
}
