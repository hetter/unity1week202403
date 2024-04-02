#if UNITY_EDITOR
/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using DummyEgg.MasterDataWorker;

public class MmdbAssetReader
{
	static MemoryDatabase DataBase = null;

	public static void Read_MstExp_MMItem_Table()
	{
		//if (DataBase == null)
		{
		  var mmbytres = ExcelDataReaderMM.MasterBytes();
		  DataBase = new MemoryDatabase(mmbytres);
		}
		var type = typeof(MstExp_MMItem);
		var propInfo = type.GetProperties();
		foreach (var ii in DataBase.MstExp_MMItemTable.All)
		{
		var itmInfo = "MstExp_MMItem:";
		foreach (var pp in propInfo)
			itmInfo += pp.Name + ":" + pp.GetValue(ii) + ", ";
		Debug.Log($" <color=green>{itmInfo}</color> ");
		}
	}

	public static void Read_MstJobBaseData_MMItem_Table()
	{
		//if (DataBase == null)
		{
		  var mmbytres = ExcelDataReaderMM.MasterBytes();
		  DataBase = new MemoryDatabase(mmbytres);
		}
		var type = typeof(MstJobBaseData_MMItem);
		var propInfo = type.GetProperties();
		foreach (var ii in DataBase.MstJobBaseData_MMItemTable.All)
		{
		var itmInfo = "MstJobBaseData_MMItem:";
		foreach (var pp in propInfo)
			itmInfo += pp.Name + ":" + pp.GetValue(ii) + ", ";
		Debug.Log($" <color=green>{itmInfo}</color> ");
		}
	}

	public static void Read_MstJobDesc_MMItem_Table()
	{
		//if (DataBase == null)
		{
		  var mmbytres = ExcelDataReaderMM.MasterBytes();
		  DataBase = new MemoryDatabase(mmbytres);
		}
		var type = typeof(MstJobDesc_MMItem);
		var propInfo = type.GetProperties();
		foreach (var ii in DataBase.MstJobDesc_MMItemTable.All)
		{
		var itmInfo = "MstJobDesc_MMItem:";
		foreach (var pp in propInfo)
			itmInfo += pp.Name + ":" + pp.GetValue(ii) + ", ";
		Debug.Log($" <color=green>{itmInfo}</color> ");
		}
	}

	public static void Read_MstJobProgress_MMItem_Table()
	{
		//if (DataBase == null)
		{
		  var mmbytres = ExcelDataReaderMM.MasterBytes();
		  DataBase = new MemoryDatabase(mmbytres);
		}
		var type = typeof(MstJobProgress_MMItem);
		var propInfo = type.GetProperties();
		foreach (var ii in DataBase.MstJobProgress_MMItemTable.All)
		{
		var itmInfo = "MstJobProgress_MMItem:";
		foreach (var pp in propInfo)
			itmInfo += pp.Name + ":" + pp.GetValue(ii) + ", ";
		Debug.Log($" <color=green>{itmInfo}</color> ");
		}
	}

	public static void Read_mst_test_data_MMItem_Table()
	{
		//if (DataBase == null)
		{
		  var mmbytres = ExcelDataReaderMM.MasterBytes();
		  DataBase = new MemoryDatabase(mmbytres);
		}
		var type = typeof(mst_test_data_MMItem);
		var propInfo = type.GetProperties();
		foreach (var ii in DataBase.mst_test_data_MMItemTable.All)
		{
		var itmInfo = "mst_test_data_MMItem:";
		foreach (var pp in propInfo)
			itmInfo += pp.Name + ":" + pp.GetValue(ii) + ", ";
		Debug.Log($" <color=green>{itmInfo}</color> ");
		}
	}
}
#endif

