namespace SextantSample.WinUI.ViewModels;

public class CounterRoute
{
    public CounterRoute(string label, int counter)
    {
        Label = label;
        Counter = counter;
    }

    public int Counter { get; private set; }

    public string Label { get; private set; }

    public ActivationType ActivationType { get; set; }
}
