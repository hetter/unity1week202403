/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

#if UNITY_EDITOR
public class MstExpMMAssetAssignment
{
[CLSCompliantAttribute(false)]
	public static MstExp_MMItem[] CreateItemTable(List<Dictionary<string, string>> allItemValueRowList)
	{
		if (allItemValueRowList == null || allItemValueRowList.Count == 0)
			return null;
		int rowCount = allItemValueRowList.Count;
		MstExp_MMItem[] items = new MstExp_MMItem[rowCount];
		for (int i = 0; i < items.Length; i++)
		{
			items[i] = new MstExp_MMItem();
			items[i].lv = Convert.ToInt32(allItemValueRowList[i]["lv"]);
			items[i].needExp = Convert.ToInt32(allItemValueRowList[i]["needExp"]);
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


