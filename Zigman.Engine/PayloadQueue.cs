using Timer = System.Timers.Timer;

namespace Zigman.Engine;

/// <summary>
/// Payload queue
/// </summary>
public class PayloadQueue
{
    private int _index;
    private readonly Timer _timer = new();

    /// <summary>
    /// New payload queue
    /// </summary>
    /// <param name="addPayload">Add payload action</param>
    /// <param name="payloads">Payload list</param>
    /// <param name="delay">Timer delay</param>
    public PayloadQueue(Action<Action> addPayload, IReadOnlyList<Action> payloads, int delay)
    {
        Console.WriteLine($"[PayloadQueue] Delay: {delay} ms");
        Console.WriteLine($"[PayloadQueue] Payloads count: {payloads.Count}");
        _timer.Interval = delay;
        _timer.Elapsed += (_, _) => {
            Console.WriteLine($"[PayloadQueue] Running payload with index {_index}");
            try { addPayload(payloads[_index]); }
            catch (Exception e) { _timer.Stop(); Console.WriteLine(e); }

            if (_index + 1 > payloads.Count - 1) {
                Console.WriteLine($"[PayloadQueue] No payloads left, stopping the timer");
                _timer.Stop();
            }
            else _index++;
        };
    }
    
    /// <summary>
    /// Start the queue timer
    /// </summary>
    public void Start()
        => _timer.Start();
}