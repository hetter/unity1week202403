/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using MasterMemory;
using MessagePack;

[MemoryTable("mst_test_data_MMItem"), MessagePackObject(true)]
public class mst_test_data_MMItem
{
    [PrimaryKey]
    public uint test_id { get; set; }
    public string test_str { get; set; }
    [SecondaryKey(0)]
    public uint test_group { get; set; }
    public float test_float { get; set; }
    public mst_test_data_MMItem() { }

    public mst_test_data_MMItem(uint test_id, string test_str, uint test_group, float test_float)
    {
        this.test_id = test_id;
        this.test_str = test_str;
        this.test_group = test_group;
        this.test_float = test_float;
    }
}


