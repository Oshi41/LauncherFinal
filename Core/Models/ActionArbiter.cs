using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ActionArbiter
    {
        public bool IsExecuting { get; protected set; }

        public void Do(Action action)
        {
            if (IsExecuting)
                return;

            try
            {
                IsExecuting = true;

                action?.Invoke();
            }
            catch (Exception e)
            {
                Trace.Write(e);
                throw;
            }
            finally
            {
                IsExecuting = false;
            }
        }
    }

    public class AsyncActionArbiter : ActionArbiter
    {
        private CancellationTokenSource _tokenSource;

        public async Task DoAsync(Action action)
        {
            _tokenSource = new CancellationTokenSource();

            var task = new Task(action, _tokenSource.Token);

            try
            {
                await task;
            }
            catch (Exception e)
            {
                Trace.Write(e);
                throw;
            }
            finally
            {
                IsExecuting = false;
            }
        }

        public void Cancel()
        {
            _tokenSource.Cancel();
        }
    }
}
