using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlantUMLEditor;

internal class Debouncer
{
    private List<CancellationTokenSource> _stepperCancelTokens = new List<CancellationTokenSource>();
    private readonly int _millisecondsToWait;
    private readonly object _lockThis = new object(); // Use a locking object to prevent the debouncer to trigger again while the func is still running

    public Debouncer(int millisecondsToWait = 300)
    {
        _millisecondsToWait = millisecondsToWait;
    }

    public void Debounce(Action func)
    {
        CancelAllStepperTokens();
        var newTokenSrc = new CancellationTokenSource();

        lock (_lockThis)
        {
            _stepperCancelTokens.Add(newTokenSrc);
        }

        _ = Task.Delay(_millisecondsToWait, newTokenSrc.Token).ContinueWith(task => // Create new request
        {
            if (!newTokenSrc.IsCancellationRequested) // if it has not been cancelled
            {
                CancelAllStepperTokens(); // Cancel any that remain (there shouldn't be any)
                _stepperCancelTokens = new List<CancellationTokenSource>();

                lock (_lockThis)
                {
                    func(); // run
                }
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void CancelAllStepperTokens()
    {
        foreach (var token in _stepperCancelTokens)
        {
            if (!token.IsCancellationRequested)
            {
                token.Cancel();
            }
        }
    }
}
