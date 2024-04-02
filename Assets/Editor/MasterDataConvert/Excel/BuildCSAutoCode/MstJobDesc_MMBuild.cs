/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

#if UNITY_EDITOR
public class MstJobDescMMAssetAssignment
{
[CLSCompliantAttribute(false)]
	public static MstJobDesc_MMItem[] CreateItemTable(List<Dictionary<string, string>> allItemValueRowList)
	{
		if (allItemValueRowList == null || allItemValueRowList.Count == 0)
			return null;
		int rowCount = allItemValueRowList.Count;
		MstJobDesc_MMItem[] items = new MstJobDesc_MMItem[rowCount];
		for (int i = 0; i < items.Length; i++)
		{
			items[i] = new MstJobDesc_MMItem();
			items[i].job_id = Convert.ToInt32(allItemValueRowList[i]["job_id"]);
			items[i].name = allItemValueRowList[i]["name"];
			items[i].desc = allItemValueRowList[i]["desc"];
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


