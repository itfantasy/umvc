﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using itfantasy.umvc;

/// <summary>
/// 描述：
/// 作者： 
/// </summary>
public class RadarCommand : Command {

    public const int Index = Worker_Nan.Index + 200;

    public override void Execute(INotice notice)
    {
        switch(notice.GetType())
        {
            case Command_Show:
                GameObject root = GameObject.Find("UIRoot");
                RegisterMediator<RadarMediator>(root.transform.Find("RadarWindow").gameObject);
                break;
        }
        base.Execute(notice);
    }
}
