using Command.Actions;
using System;
using System.Collections.Generic;

public class LinkedListNode_Indexed : ICloneable
{
    public LinkedListNode<Dictionary<string, int>> data {
        get;
        private set;
    }
    public int index = 0;


    public LinkedListNode_Indexed(LinkedListNode<Dictionary<string, int>> commandLog_, int index_)
    {
        data = commandLog_;
        index = index_;
    }

    public LinkedListNode_Indexed(LinkedListNode_Indexed commandLog_, int? index_)
    {
        data = commandLog_.data;
        if (index_ != null)
        {
            index = index_.Value;
        }
        else
        {
            index = commandLog_.index;
        }
    }

    public virtual object Clone()
    {
        var ret = new LinkedListNode_Indexed(data, index);
        return ret;
    }


    public LinkedListNode_Indexed Next {
        get {
            if (data.Next != null)
            {
                var ret = new LinkedListNode_Indexed(data.Next, index + 1);
                return ret;
            }
            else
            {
                return null;
            }

        }
    }
}

public class CommandVerify : CommandVisitor
{

    private LinkedListNode_Indexed nowCommandLogPosition;
    public bool commandAcceptFlag {
        get;
        private set;
    }


    public int margine = 30;
    public int sametime_margine = 3;

    public CommandVerify(LinkedListNode<Dictionary<string, int>> topCommandLog_, int margine_, int index = 0)
    {
        nowCommandLogPosition = new LinkedListNode_Indexed(topCommandLog_, index);
        margine = margine_;
    }

    private CommandVerify(LinkedListNode_Indexed topCommandLog_, int margine_, int index = 0)
    {
        nowCommandLogPosition = new LinkedListNode_Indexed(topCommandLog_, index);
        margine = margine_;
    }


    public bool isPress(LinkedListNode<Dictionary<string, int>> it, string button)
    {
        if (it == null)
        {
            return false;
        }
        var log = it.Value;
        return log[button] == 1;
    }
    public bool isHold(LinkedListNode<Dictionary<string, int>> it, string button, int time = 1)
    {
        if (it == null)
        {
            return false;
        }
        var log = it.Value;
        var ret = log[button] >= time;
        return ret;
    }

    public bool isRelease(LinkedListNode<Dictionary<string, int>> it, string button, int time = -1)
    {
        if (it == null)
        {
            return false;
        }
        var log = it.Value;
        var prelogNode = it.Next;

        if (prelogNode == null)
        {
            return false;
        }
        else
        {
            var ret = log[button] == -1 && prelogNode.Value[button] >= time;
            return ret;
        }
    }
    public void Visit(Sequence seq)
    {

        int cmd_max = seq.data.Count - 1;
        var nowcmdid = cmd_max;
        var nowlog = nowCommandLogPosition;
        while (nowlog != null && nowlog.index < margine)
        {
            var verifyvistor = new CommandVerify(nowlog, margine);
            seq.data[nowcmdid].Accept(verifyvistor);
            if (verifyvistor.commandAcceptFlag)
            {
                nowlog = verifyvistor.nowCommandLogPosition;
                if (nowcmdid == 0)
                {
                    commandAcceptFlag = true;
                    nowCommandLogPosition = nowlog;
                    return;
                }
                nowcmdid--;
            }
            else
            {
                if (nowcmdid == cmd_max)
                {
                    commandAcceptFlag = false;
                    return;
                }
                nowlog = nowlog.Next;
            }
        }
        commandAcceptFlag = false;

    }
    public void Visit(SameTime same)
    {
        var oldestacceptlog = (LinkedListNode_Indexed)nowCommandLogPosition.Clone();
        int? newestaccepttime = null;
        foreach (var e in same.data)
        {
            bool matched = false;
            var i = (LinkedListNode_Indexed)nowCommandLogPosition.Clone();
            i.index = 0;
            for (; i != null && i.index < sametime_margine; i = i.Next)
            {
                var verifyvisitor = new CommandVerify(i, 1, i.index);
                e.Accept(verifyvisitor);
                if (verifyvisitor.commandAcceptFlag)
                {
                    matched = true;
                    if (oldestacceptlog.index < verifyvisitor.nowCommandLogPosition.index)
                    {
                        oldestacceptlog.index = verifyvisitor.nowCommandLogPosition.index;
                    }
                    if (newestaccepttime == null || verifyvisitor.nowCommandLogPosition.index < newestaccepttime.Value)
                    {
                        newestaccepttime = i.index;
                    }
                    break;
                }
            }
            if (!matched)
            {
                commandAcceptFlag = false;
                return;
            }
        }

        if (newestaccepttime == 0)
        {
            commandAcceptFlag = true;
            nowCommandLogPosition = oldestacceptlog;
        }
        else
        {
            commandAcceptFlag = false;
            nowCommandLogPosition = oldestacceptlog;

        }
    }
    public void Visit(Hold hold)
    {
        commandAcceptFlag = isHold(nowCommandLogPosition.data, hold.button, hold.time);
        if (commandAcceptFlag)
        {
            nowCommandLogPosition = nowCommandLogPosition.Next;
        }
    }
    public void Visit(Press press)
    {
        commandAcceptFlag = isPress(nowCommandLogPosition.data, press.button);
        if (commandAcceptFlag)
        {
            nowCommandLogPosition = nowCommandLogPosition.Next;
        }
    }

    public void Visit(Release release)
    {
        commandAcceptFlag = isRelease(nowCommandLogPosition.data, release.button, release.time);
        if (commandAcceptFlag)
        {
            nowCommandLogPosition = nowCommandLogPosition.Next;
        }
    }
    public void Visit(Press_Dir press_dir)
    {
        if (press_dir.v != null && press_dir.h != null)
        {
            var timev = nowCommandLogPosition.data.Value[press_dir.v == Vertical.UP ? "U" : "D"];
            var timeh = nowCommandLogPosition.data.Value[press_dir.h == Horizontal.FRONT ? "F" : "B"];

            commandAcceptFlag = timev >= 1 && timeh >= 1 && (timev == 1 ^ timeh == 1);
            if (commandAcceptFlag)
            {
                nowCommandLogPosition = nowCommandLogPosition.Next;
            }
            return;
        }
        else if (press_dir.v != null)
        {
            bool ret = false;
            var button = press_dir.v == Vertical.UP ? "U" : "D";
            if (press_dir.only)
            {
                ret = isPress(nowCommandLogPosition.data, button) &&
                    !isHold(nowCommandLogPosition.data, "F") &&
                    !isHold(nowCommandLogPosition.data, "B");
            }
            else
            {
                ret = (isPress(nowCommandLogPosition.data, button) && !isHold(nowCommandLogPosition.data, "B") && !isHold(nowCommandLogPosition.data, "F")) ||
                    (isHold(nowCommandLogPosition.data, button) && (isRelease(nowCommandLogPosition.data, "B") ^ isRelease(nowCommandLogPosition.data, "F")));
            }
            commandAcceptFlag = ret;
            if (commandAcceptFlag)
            {
                nowCommandLogPosition = nowCommandLogPosition.Next;
            }
            return;
        }
        else if (press_dir.h != null)
        {
            var button = press_dir.h == Horizontal.FRONT ? "F" : "B";
            var ret = false;
            if (press_dir.only)
            {
                ret = isPress(nowCommandLogPosition.data, button) && !isHold(nowCommandLogPosition.data, "U") && !isHold(nowCommandLogPosition.data, "D");
            }
            else
            {
                ret = (isPress(nowCommandLogPosition.data, button) && !isHold(nowCommandLogPosition.data, "U") && !isHold(nowCommandLogPosition.data, "D"))
                    || isHold(nowCommandLogPosition.data, button) && (isRelease(nowCommandLogPosition.data, "U") ^ isRelease(nowCommandLogPosition.data, "D"));
            }
            commandAcceptFlag = ret; if (commandAcceptFlag)
            {
                nowCommandLogPosition = nowCommandLogPosition.Next;
            }
        }
        //error
        return;
    }
    public void Visit(Hold_Dir hold_dir)
    {

        if (hold_dir.v != null && hold_dir.h != null)
        {
            var button_v = hold_dir.v == Vertical.UP ? "U" : "D";
            var button_h = hold_dir.h == Horizontal.FRONT ? "F" : "B";
            {
                var timev = nowCommandLogPosition.data.Value[button_v];
                var timhv = nowCommandLogPosition.data.Value[button_h];
                commandAcceptFlag = isHold(nowCommandLogPosition.data, button_v, hold_dir.time) && isHold(nowCommandLogPosition.data, button_h, hold_dir.time);
                if (commandAcceptFlag)
                {
                    nowCommandLogPosition = nowCommandLogPosition.Next;
                }
                return;
            }
        }
        else if (hold_dir.v != null)
        {
            var button_v = hold_dir.v == Vertical.UP ? "U" : "D";
            commandAcceptFlag = isHold(nowCommandLogPosition.data, button_v, hold_dir.time);
            if (commandAcceptFlag)
            {
                nowCommandLogPosition = nowCommandLogPosition.Next;
            }
            return;
        }
        else if (hold_dir.h != null)
        {
            var button_h = hold_dir.h == Horizontal.FRONT ? "F" : "B";
            commandAcceptFlag = isHold(nowCommandLogPosition.data, button_h, hold_dir.time);
            if (commandAcceptFlag)
            {
                nowCommandLogPosition = nowCommandLogPosition.Next;
            }
            return;
        }

        //error
        return;

    }
    public void Visit(Release_Dir release_dir)
    {
        if (nowCommandLogPosition.Next == null)
        {
            commandAcceptFlag = false;
            return;
        }

        var prelog = nowCommandLogPosition.Next;

        if (release_dir.v != null && release_dir.h != null)
        {
            var buttonv = release_dir.v == Vertical.UP ? "U" : "D";
            var buttonh = release_dir.h == Horizontal.FRONT ? "F" : "B";

            var timev = nowCommandLogPosition.data.Value[buttonv];
            var timeh = nowCommandLogPosition.data.Value[buttonh];
            var pretimev = prelog.data.Value[buttonv];
            var pretimeh = prelog.data.Value[buttonh];
            commandAcceptFlag = (timev == -1 || timeh == -1) && (pretimev > release_dir.time && pretimeh > release_dir.time);

            if (commandAcceptFlag)
            {
                nowCommandLogPosition = nowCommandLogPosition.Next;
            }
        }
        else if (release_dir.v != null)
        {
            var buttonv = release_dir.v == Vertical.UP ? "U" : "D";
            commandAcceptFlag = (isRelease(nowCommandLogPosition.data, buttonv, release_dir.time) && !isHold(nowCommandLogPosition.data, "F") && !isHold(nowCommandLogPosition.data, "B")) ||
                isHold(nowCommandLogPosition.data, buttonv) && (isPress(nowCommandLogPosition.data, "F") ^ isPress(nowCommandLogPosition.data, "B"));

            if (commandAcceptFlag)
            {
                nowCommandLogPosition = nowCommandLogPosition.Next;
            }
        }
        else if (release_dir.h != null)
        {
            var buttonh = release_dir.h == Horizontal.FRONT ? "F" : "B";
            commandAcceptFlag = (isRelease(nowCommandLogPosition.data, buttonh, release_dir.time) && !isHold(nowCommandLogPosition.data, "U") && !isHold(nowCommandLogPosition.data, "D")) ||
                isHold(nowCommandLogPosition.data, buttonh) && (isPress(nowCommandLogPosition.data, "U") ^ isPress(nowCommandLogPosition.data, "D"));
            if (commandAcceptFlag)
            {
                nowCommandLogPosition = nowCommandLogPosition.Next;
            }
        }

        //error
        return;

    }
}