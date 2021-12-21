namespace Zigman.Engine;

public static class PayloadManager
{
    /// <summary>
    /// Thread/queue pair
    /// </summary>
    private class ThreadQueuePair
    {
        /// <summary>
        /// Thread
        /// </summary>
        public volatile Thread Thread = null!;
        
        /// <summary>
        /// Actions to be executed
        /// </summary>
        public volatile List<Action> Actions = new();

        /// <summary>
        /// Index in the list
        /// </summary>
        public volatile int Index;
    }

    /// <summary>
    /// All pairs
    /// </summary>
    private static volatile List<ThreadQueuePair> _pairs = new();

    /// <summary>
    /// Is initialized
    /// </summary>
    private static bool _isInitialized;

    /// <summary>
    /// Threads stopped
    /// </summary>
    private static volatile int _threadsStopped;

    /// <summary>
    /// Create a thread/queue pair
    /// </summary>
    private static void CreateThreadQueuePair()
    {
        var id = _pairs.Count;
        var pair = new ThreadQueuePair {
            Index = id,
            Thread = new Thread(() => {
                while (_isInitialized) {
                    var pair = _pairs[id];
                    if (pair.Actions.Count == 0) {
                        Thread.Sleep(200);
                    } else lock (_pairs[id].Actions) {
                        foreach (var t in pair.Actions) {
                            try { t(); }
                            catch (Exception e) { Console.WriteLine($"[Thread {id}] Exception occured: {e}"); }
                        }
                    }
                }

                Interlocked.Increment(ref _threadsStopped);
            })
        };
        _pairs.Add(pair);
        _pairs[id].Thread.Start();
        Console.WriteLine($"[CreateThreadQueuePair] Created a new thread at index {id}");
    }

    /// <summary>
    /// Initialize payload manager
    /// </summary>
    /// <param name="threads">Threads count</param>
    /// <exception cref="Exception">Payload manager already initialized</exception>
    public static void Initialize(int threads)
    {
        if (_isInitialized)
            throw new Exception("Payload manager already initialized!");
        _isInitialized = true;
        Console.WriteLine($"[Initialize] Creating new threads...");
        for (var i = 1; i <= threads; i++)
            CreateThreadQueuePair();
        Console.WriteLine($"[Initialize] Done!");
    }

    /// <summary>
    /// Throws an action into a thread with
    /// the smallest count of Actions in it
    /// </summary>
    /// <param name="action">Action</param>
    /// <exception cref="Exception">Payload manager not initialized yet</exception>
    public static void AddAction(Action action)
    {
        if (!_isInitialized)
            throw new Exception("Payload manager not initialized yet!");
        var ordered = _pairs.OrderBy(x => x.Actions.Count).ToList();
        lock (ordered[0].Actions) {
            ordered[0].Actions.Add(action);
            Console.WriteLine($"[AddAction] [Thread {ordered[0].Index}] {ordered[0].Actions.Count} enqueued in total");
        }
    }

    /// <summary>
    /// Uninitialize
    /// </summary>
    public static void Uninitialize()
    {
        Console.WriteLine($"[Uninitialize] Stopping all threads...");
        _isInitialized = false;
        while (_threadsStopped != _pairs.Count) { }
        _pairs.Clear();
        Console.WriteLine($"[Uninitialize] Done!");
    }
}