using UnityEngine;

public class Health 
{ 
    public int Max { get; set; }
    public int Current { get; set; }

    public Health(int max)
    {
        Max = max;
        Current = Max;
    }
}
