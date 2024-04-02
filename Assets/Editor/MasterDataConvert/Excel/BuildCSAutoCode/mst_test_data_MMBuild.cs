/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

#if UNITY_EDITOR
public class mst_test_dataMMAssetAssignment
{
[CLSCompliantAttribute(false)]
	public static mst_test_data_MMItem[] CreateItemTable(List<Dictionary<string, string>> allItemValueRowList)
	{
		if (allItemValueRowList == null || allItemValueRowList.Count == 0)
			return null;
		int rowCount = allItemValueRowList.Count;
		mst_test_data_MMItem[] items = new mst_test_data_MMItem[rowCount];
		for (int i = 0; i < items.Length; i++)
		{
			items[i] = new mst_test_data_MMItem();
			items[i].test_id = Convert.ToUInt32(allItemValueRowList[i]["test_id"]);
			items[i].test_str = allItemValueRowList[i]["test_str"];
			items[i].test_group = Convert.ToUInt32(allItemValueRowList[i]["test_group"]);
			items[i].test_float = Convert.ToSingle(allItemValueRowList[i]["test_float"]);
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


