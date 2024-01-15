using System.Diagnostics;

namespace GCCollectionMonitor.Maui;

public class GCCollectionMonitor
{
    private readonly List<Tuple<string, WeakReference<object>>> _references = [];

    public void MonitorForCollection(object target)
    {
        string targetType = target.GetType().Name;
        _references.Add(new Tuple<string, WeakReference<object>>(targetType, new WeakReference<object>(target)));
    }

    public async void CollectAndCheck()
    {
        const int maxCollections = 5;
        var currentCollection = 0;
        while (++currentCollection <= maxCollections && _references.Count != 0)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            foreach (Tuple<string, WeakReference<object>> reference in _references.ToArray())
                if (reference.Item2.TryGetTarget(out object? target))
                {
                    if (currentCollection == maxCollections)
                    {
                        Debug.WriteLine($"🧟 {target.GetType().Name} is a zombie!");
                        _references.Remove(reference);
                    }
                }
                else
                {
                    Debug.WriteLine($"✅{reference.Item1} released after {currentCollection} collections");
                    _references.Remove(reference);
                }

            await Task.Delay(500);
        }
    }
}