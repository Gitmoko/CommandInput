using System;
using System.Collections.Generic;
public class InputMgr
{

    private Dictionary<string, Func<bool>> pressedFunc;
    private static int maxLogSize = 3000;
    private LinkedList<Dictionary<string, int>> pressedTimeLog = new LinkedList<Dictionary<string, int>>();

    public InputMgr(Dictionary<string, Func<bool>> pressedFunc_)
    {
        pressedFunc = pressedFunc_;
        pressedTimeLog.AddFirst(new Dictionary<string, int>());
        foreach (var e in pressedFunc)
        {
            pressedTimeLog.First.Value.Add(e.Key, 0);
        }
    }

    public void UpdateInput()
    {
        if (pressedTimeLog.Count == maxLogSize)
        {
            pressedTimeLog.RemoveLast();
        }
        var newlog = new Dictionary<string, int>();
        foreach (var e in pressedFunc)
        {
            var ispressed = e.Value();
            var prelog = pressedTimeLog.First.Value;
            if (ispressed)
            {
                newlog.Add(e.Key, prelog[e.Key] + 1);
            }
            else
            {
                if (prelog[e.Key] > 0)
                {
                    newlog.Add(e.Key, -1);
                }
                else
                {
                    newlog.Add(e.Key, 0);
                }
            }
        }
        pressedTimeLog.AddFirst(newlog);
    }

    public Dictionary<string, int> GetInputLog()
    {
        return pressedTimeLog.First.Value;

    }

    public LinkedList<Dictionary<string, int>> GetPressedTimeLog()
    {
        return pressedTimeLog;
    }

}
