/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

#if UNITY_EDITOR
public class MstJobBaseDataMMAssetAssignment
{
[CLSCompliantAttribute(false)]
	public static MstJobBaseData_MMItem[] CreateItemTable(List<Dictionary<string, string>> allItemValueRowList)
	{
		if (allItemValueRowList == null || allItemValueRowList.Count == 0)
			return null;
		int rowCount = allItemValueRowList.Count;
		MstJobBaseData_MMItem[] items = new MstJobBaseData_MMItem[rowCount];
		for (int i = 0; i < items.Length; i++)
		{
			items[i] = new MstJobBaseData_MMItem();
			items[i].job_id = Convert.ToInt32(allItemValueRowList[i]["job_id"]);
			items[i].hp = Convert.ToInt32(allItemValueRowList[i]["hp"]);
			items[i].mp = Convert.ToInt32(allItemValueRowList[i]["mp"]);
			items[i].phy_atk = Convert.ToInt32(allItemValueRowList[i]["phy_atk"]);
			items[i].phy_def = Convert.ToInt32(allItemValueRowList[i]["phy_def"]);
			items[i].mag_atk = Convert.ToInt32(allItemValueRowList[i]["mag_atk"]);
			items[i].mag_def = Convert.ToInt32(allItemValueRowList[i]["mag_def"]);
			items[i].dex = Convert.ToInt32(allItemValueRowList[i]["dex"]);
		}
		return items;
	}
	[CLSCompliantAttribute(false)]
	public static void CreateAsset(List<Dictionary<string, string>> allItemValueRowList, DummyEgg.MasterDataWorker.DatabaseBuilder builder)
	{
		var ct = CreateItemTable(allItemValueRowList);
		builder.Append(ct);
	}
	[CLSCompliantAttribute(false)]
	public static void ReplaceAsset(List<Dictionary<string, string>> allItemValueRowList, DummyEgg.MasterDataWorker.ImmutableBuilder builder)
	{
		var ct = CreateItemTable(allItemValueRowList);
		builder.ReplaceAll(ct);
	}
}
#endif


