/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using MasterMemory;
using MessagePack;

[MemoryTable("MstJobBaseData_MMItem"), MessagePackObject(true)]
public class MstJobBaseData_MMItem
{
    [PrimaryKey]
    public int job_id { get; set; }
    public int hp { get; set; }
    public int mp { get; set; }
    public int phy_atk { get; set; }
    public int phy_def { get; set; }
    public int mag_atk { get; set; }
    public int mag_def { get; set; }
    public int dex { get; set; }
    public MstJobBaseData_MMItem() { }

    public MstJobBaseData_MMItem(int job_id, int hp, int mp, int phy_atk, int phy_def, int mag_atk, int mag_def, int dex)
    {
        this.job_id = job_id;
        this.hp = hp;
        this.mp = mp;
        this.phy_atk = phy_atk;
        this.phy_def = phy_def;
        this.mag_atk = mag_atk;
        this.mag_def = mag_def;
        this.dex = dex;
    }
}


