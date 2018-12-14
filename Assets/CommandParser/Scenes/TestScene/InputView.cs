using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Act = Command.Actions;

public class FrameInput
{
    public string name;
    public string drawtext;
    public Act.INode cmd;

    public FrameInput(string name_, string drawtext_, Act.INode cmd_)
    {
        name = name_;
        drawtext = drawtext_;
        cmd = cmd_;
    }
}


public class InputView : MonoBehaviour
{
    private InputMgr imgr;
    private Dictionary<string, Act.INode> cmds = new Dictionary<string, Act.INode>();
    private List<FrameInput> loglist = new List<FrameInput>();

    public Text buttonlist;
    public Text cmdlist;
    public Text inputlog;
    public Text padtextlog;
    public bool padconnected {
        get;
        private set;
    }

    // Use this for initialization
    private void Start()
    {
        ButtonMapUpdate(IsExistPad());

        var shoryu = new Act.Sequence();
        shoryu.data.Add(new Act.Press_Dir(null, Command.Actions.Horizontal.FRONT, false));
        shoryu.data.Add(new Act.Press_Dir(Command.Actions.Vertical.DOWN, null, false));
        shoryu.data.Add(new Act.Press_Dir(Command.Actions.Vertical.DOWN, Command.Actions.Horizontal.FRONT, false));
        shoryu.data.Add(new Act.Press("z"));
        cmds.Add("昇竜拳", shoryu);

        var hado = new Act.Sequence();
        hado.data.Add(new Act.Press_Dir(Command.Actions.Vertical.DOWN, null));
        hado.data.Add(new Act.Press_Dir(null, Command.Actions.Horizontal.FRONT));
        hado.data.Add(new Act.Press("z"));
        cmds.Add("波動拳", hado);


        var shungoku = new Act.Sequence();
        shungoku.data.Add(new Act.Press("z"));
        shungoku.data.Add(new Act.Press("z"));
        shungoku.data.Add(new Act.Press_Dir(null, Command.Actions.Horizontal.FRONT, true));
        shungoku.data.Add(new Act.Press("x"));
        shungoku.data.Add(new Act.Press("c"));
        cmds.Add("瞬獄殺", shungoku);

        var teni = new Act.Sequence();
        teni.data.Add(new Act.Press_Dir(Command.Actions.Vertical.DOWN, null, true));
        teni.data.Add(new Act.Press_Dir(Command.Actions.Vertical.DOWN, null, true));
        teni.data.Add(new Act.SameTime(new List<Act.INode> { new Act.Press("z"), new Act.Press("x") }));
        cmds.Add("転移", teni);

        var sonic = new Act.Sequence();
        sonic.data.Add(new Act.Hold_Dir(Command.Actions.Vertical.DOWN, null));
        sonic.data.Add(new Act.Release_Dir(Command.Actions.Vertical.DOWN, null, 30));
        sonic.data.Add(new Act.Press_Dir(null, Command.Actions.Horizontal.FRONT, false));
        sonic.data.Add(new Act.Press("x"));
        cmds.Add("ソニックブーム", sonic);

        var tame = new Act.Sequence();
        tame.data.Add(new Act.Hold_Dir(Command.Actions.Vertical.DOWN, null, 30));
        tame.data.Add(new Act.Press("x"));
        cmds.Add("溜め", tame);

        var djump = new Act.Sequence();
        djump.data.Add(new Act.Press("z"));
        djump.data.Add(new Act.Press("z"));
        cmds.Add("二回", djump);




        loglist.Add(new FrameInput("F", "→", new Act.Press_Dir(null, Act.Horizontal.FRONT)));
        loglist.Add(new FrameInput("U", "↑", new Act.Press_Dir(Act.Vertical.UP, null)));
        loglist.Add(new FrameInput("B", "←", new Act.Press_Dir(null, Act.Horizontal.BACK)));
        loglist.Add(new FrameInput("D", "↓", new Act.Press_Dir(Act.Vertical.DOWN, null)));

        loglist.Add(new FrameInput("FU", "↗", new Act.Press_Dir(Act.Vertical.UP, Act.Horizontal.FRONT)));
        loglist.Add(new FrameInput("FD", "↘", new Act.Press_Dir(Act.Vertical.DOWN, Act.Horizontal.FRONT)));
        loglist.Add(new FrameInput("BU", "↖", new Act.Press_Dir(Act.Vertical.UP, Act.Horizontal.BACK)));
        loglist.Add(new FrameInput("BD", "↙", new Act.Press_Dir(Act.Vertical.DOWN, Act.Horizontal.BACK)));

        loglist.Add(new FrameInput("z", "z", new Act.Press("z")));
        loglist.Add(new FrameInput("x", "x", new Act.Press("x")));
        loglist.Add(new FrameInput("c", "c", new Act.Press("c")));


    }

    public void ButtonMapUpdate(bool isPad)
    {
        Dictionary<string, Func<bool>> pressedFunc = new Dictionary<string, Func<bool>>();
        if (isPad)
        {
            pressedFunc.Add("z", () => { return Input.GetButton("Fire1"); });
            pressedFunc.Add("x", () => { return Input.GetButton("Fire2"); });
            pressedFunc.Add("c", () => { return Input.GetButton("Fire3"); });
            pressedFunc.Add("U", () => { return Input.GetAxis("Vertical") > 0.0f; });
            pressedFunc.Add("D", () => { return Input.GetAxis("Vertical") < 0.0f; });
            pressedFunc.Add("F", () => { return Input.GetAxis("Horizontal") > 0.0f; });
            pressedFunc.Add("B", () => { return Input.GetAxis("Horizontal") < 0.0f; });
            padtextlog.text = "Pad connected";
            padconnected = true;
        }
        else
        {
            pressedFunc.Add("z", () => { return Input.GetKey(KeyCode.Z); });
            pressedFunc.Add("x", () => { return Input.GetKey(KeyCode.X); });
            pressedFunc.Add("c", () => { return Input.GetKey(KeyCode.C); });
            pressedFunc.Add("U", () => { return Input.GetKey(KeyCode.UpArrow); });
            pressedFunc.Add("D", () => { return Input.GetKey(KeyCode.DownArrow); });
            pressedFunc.Add("F", () => { return Input.GetKey(KeyCode.RightArrow); });
            pressedFunc.Add("B", () => { return Input.GetKey(KeyCode.LeftArrow); });
            padtextlog.text = "Pad disconnected";
            padconnected = false;
            imgr = new InputMgr(pressedFunc);
        }
        imgr = new InputMgr(pressedFunc);
    }

    public bool IsExistPad()
    {

        string[] temp = Input.GetJoystickNames();
        bool ret = false;

        //Check whether array contains anything
        if (temp.Length > 0)
        {
            //Iterate over every element
            for (int i = 0; i < temp.Length; ++i)
            {
                //Check if the string is empty or not
                if (!string.IsNullOrEmpty(temp[i]))
                {
                    //Not empty, controller temp[i] is connected
                    Debug.Log("Controller " + i + " is connected using: " + temp[i]);
                    ret = true;
                }
                else
                {
                    //If it is empty, controller i is disconnected
                    //where i indicates the controller number
                    Debug.Log("Controller: " + i + " is disconnected.");

                }
            }
        }
        return ret;
    }

    // Update is called once per frame
    private void Update()
    {
        var isPad = IsExistPad();
        if (isPad != padconnected)
        {
            ButtonMapUpdate(isPad);
        }
        imgr.UpdateInput();
        DrawLog();
    }

    private void DrawLog()
    {
        var log = imgr.GetInputLog();
        var text = buttonlist;
        text.text = "";
        foreach (var e in log)
        {
            text.text += e.Key.ToString() + ":" + e.Value + "\n";
        }

        var cmdtext = cmdlist;
        foreach (var cmd in cmds)
        {
            var verify = new CommandVerify(imgr.GetPressedTimeLog().First, 30);
            cmd.Value.Accept(verify);
            if (verify.commandAcceptFlag)
            {
                cmdtext.text = (cmd.Key + "\n") + cmdtext.text;
                break;
            }
        }

        var newinput = "";
        foreach (var button in loglist)
        {
            var verify = new CommandVerify(imgr.GetPressedTimeLog().First, 1);
            button.cmd.Accept(verify);
            if (verify.commandAcceptFlag)
            {
                newinput += button.drawtext;
            }
        }
        if (newinput != "")
        {
            inputlog.text = newinput + "\n" + inputlog.text;
        }
    }
}
