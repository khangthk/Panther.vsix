using System;
using System.Threading.Tasks;

namespace Panther.Commands
{
    internal abstract class CommandBase
    {
        public bool IsExecuting { get; private set; } = false;
        protected bool Cancel = false;

        public async Task OnCommandAsync(object sender, EventArgs e)
        {
            if (!IsExecuting)
            {
                try
                {
                    OnCommandBegin();
                    if (!Cancel)
                    {
                        IsExecuting = true;
                        await ExecuteAsync();
                    }
                }
                finally
                {
                    OnCommandDone();
                    IsExecuting = false;
                }
            }
        }

        protected virtual void OnCommandBegin() => Cancel = false;

        protected virtual void OnCommandDone() => Cancel = false;

        protected abstract Task ExecuteAsync();

        public abstract string GetText();

        public virtual bool IsEnabled() => !IsExecuting;

        public virtual bool IsChecked() => false;
    }
}
