/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using MasterMemory;
using MessagePack;

[MemoryTable("MstExp_MMItem"), MessagePackObject(true)]
public class MstExp_MMItem
{
    [PrimaryKey]
    public int lv { get; set; }
    public int needExp { get; set; }
    public MstExp_MMItem() { }

    public MstExp_MMItem(int lv, int needExp)
    {
        this.lv = lv;
        this.needExp = needExp;
    }
}


