/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using MasterMemory;
using MessagePack;

[MemoryTable("MstJobDesc_MMItem"), MessagePackObject(true)]
public class MstJobDesc_MMItem
{
    [PrimaryKey]
    public int job_id { get; set; }
    public string name { get; set; }
    public string desc { get; set; }
    public MstJobDesc_MMItem() { }

    public MstJobDesc_MMItem(int job_id, string name, string desc)
    {
        this.job_id = job_id;
        this.name = name;
        this.desc = desc;
    }
}


