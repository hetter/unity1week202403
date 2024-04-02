#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
//using Excel;
using System.Reflection;
using System;
using ExcelDataReader;
using System.Data;
using System.Linq;
using UnityEditor;
using MessagePack.Resolvers;
using MessagePack;
using DummyEgg.MasterDataWorker;

//MasterData　Excelfile Reader
public static class ExcelDataReaderMM
{
    //ーーーーーーーーーーーコンフィグーーーーーーーーーーー
    //Excel 4行目：PrimaryKey
    const int excelPrimarryRow = 4;
    //Excel 3行目：メンバ変数名前
    const int excelNameRow = 3;
    //Excel 2行目：メンバ変数タイプ
    const int excelTypeRow = 2;
    //Excel 1行目：ClientFlag
    const int excelClientFlagRow = 1;
    //Excel ６行目以降はデータ
    const int excelDataRow = 6;
    //Excel データは2列から始めます
    const int excelDataColumn = 2;
    //C#ファイルPath
    static string excelCodePath = Application.dataPath + "/Scripts/MMAutoCode/mpkClass";
    //Asset（データ格納）ファイルPath
    public static string excelAssetPath = "Assets/Resources/MMAutoMaster.bytes";
    //C#クラス（データ読む用、Editorだけ使用もの）
    public static string readerClassPath = Application.dataPath + "/Editor/MasterDataConvert/ReaderAutoCode";
    //ーーーーーーーーーーーコンフィグーーーーーーーーーーー

    //MasterData DB Builder
    static DatabaseBuilder AssetBuilder;
    static long _timeRecord = 0;
    static byte[] _masterBytes = null;
    public static byte[] MasterBytes()
    {
        var fi = new System.IO.FileInfo(excelAssetPath);
        var fTime = fi.LastWriteTime.ToFileTimeUtc();

        if (fTime > _timeRecord)
        {
            _timeRecord = fTime;
            _masterBytes = File.ReadAllBytes(excelAssetPath);
        }

        return _masterBytes;
    }

    public static void MasterBytesRefresh()
    {
        var fi = new System.IO.FileInfo(excelAssetPath);
        _masterBytes = File.ReadAllBytes(excelAssetPath);
    }

    public struct PropertyData
    {
        public string PType; //type name
        public string Name; //value name
        public string Primary; //primary key
        public bool IsClient; //client flag
    }

    public static void DeleteAll()
    {
        new DirectoryInfo(excelCodePath).Delete(true);
        new DirectoryInfo(readerClassPath).Delete(true);
        new DirectoryInfo(BuildExcelWindow.ExcelFilePath + "/BuildCS").Delete(true);

    }


    #region --- Read Excel ---

    //C#クラスの生成
    public static void ReadAllInputFilesToCode()
    {
        //全てのExcelファイルを読む
        string[] excelFileFullPaths = Directory.GetFiles(BuildExcelWindow.ExcelFilePath, "*.xlsx");

        if (excelFileFullPaths == null || excelFileFullPaths.Length == 0)
        {
            Debug.Log("Excel file count == 0");
        }

        //C#クラス生成
        for (int i = 0; i < excelFileFullPaths.Length; i++)
        {
            var fullPath = excelFileFullPaths[i];
            ReadOneExcelToCode(fullPath);
        }

        //全てのCsvファイルを読む
        string[] csvFileFullPaths = Directory.GetFiles(BuildExcelWindow.ExcelFilePath, "*.csv");

        if (csvFileFullPaths == null || csvFileFullPaths.Length == 0)
        {
            Debug.Log("CSV file count == 0");
        }

        //C#クラス生成
        for (int i = 0; i < csvFileFullPaths.Length; i++)
        {
            var fullPath = csvFileFullPaths[i];
            ReadOneCSVToCode(fullPath);
        }

    }

    //データファイル（csv、xlsx）からC#クラス構造
    private static void _readOneFileToCode(string fileFullPath, Func<string, bool, ExcelMediumData> createMediumData)
    {
        //中間データの解析
        ExcelMediumData excelMediumData = createMediumData(fileFullPath, false);
        bool isBuildClsD = false;
        bool isBuildClsB = false;
        if (excelMediumData != null)
        {
            //MasterMemory/MessagePackage用の基本クラスを生成します
            string classCodeStr = ExcelCodeCreaterMM.CreateCodeStrByExcelData(excelMediumData);
            if (!string.IsNullOrEmpty(classCodeStr))
            {
                //CSharp.csを書き
                if (WriteCodeStrToSave(excelCodePath, excelMediumData.excelName + "_MMData", classCodeStr))
                {
                    Debug.Log("<color=green>Auto Create MMDATA Scripts Success : </color>" + excelMediumData.excelName);
                    isBuildClsD = true;
                }
            }

            //MasterMemoryのDBファイル（MMAutoMaster）へ書き込み用のクラスを生成します
            string buildClassCodeStr = ExcelCodeCreaterMM.CreateBuildAssetCode(excelMediumData);
            if (!string.IsNullOrEmpty(buildClassCodeStr))
            {
                //CSharp.csを書き
                if (WriteCodeStrToSave(BuildExcelWindow.ExcelFilePath + "/BuildCS", excelMediumData.excelName + "_MMBuild", buildClassCodeStr))
                {
                    Debug.Log("<color=green>Auto Create MMBuild Scripts Success : </color>" + excelMediumData.excelName);
                    isBuildClsB = true;
                }
            }
        }
        if (!(isBuildClsD && isBuildClsB))
            //生成失敗
            Debug.LogError("Auto Create Excel Scripts Fail : " + (excelMediumData == null ? "" : excelMediumData.excelName));
    }

    //CSVからC#クラス構造
    public static void ReadOneCSVToCode(string fileFullPath)
    {
        _readOneFileToCode(fileFullPath, CreateClassCodeByCSVPath);
    }
    //ExcelからC#クラス構造
    public static void ReadOneExcelToCode(string fileFullPath)
    {
        _readOneFileToCode(fileFullPath, CreateClassCodeByExcelPath);
    }

    //全てのテーブル名を取得します
    public static HashSet<string> GetTableSet()
    {
        HashSet<string> tableSet = new();
        var mmbytres = ExcelDataReaderMM.MasterBytes();
        foreach (var tinfo in MemoryDatabase.GetTableInfo(mmbytres))
        {
            string itemName = tinfo.TableName;
            tableSet.Add(itemName);
        }
        return tableSet;
    }

    #endregion

    #region --- Create Asset ---

    //テーブルデータをDBファイルに書く
    private static void CreateOneAsset(ExcelMediumData excelMediumData)
    {
        if (excelMediumData != null)
        {
            //タイプ処理：Assembly-CSharp-EditorからAssembly-CSharpへ
            Type type = null;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type tempType = asm.GetType(excelMediumData.excelName + "MMAssetAssignment");
                if (tempType != null)
                {
                    type = tempType;
                    break;
                }
            }
            if (type != null)
            {
                if (File.Exists(excelAssetPath))
                {
                    //既存のdbファイルをメモリdbに読む
                    var extBytes = ExcelDataReaderMM.MasterBytes();
                    var extDatabase = new MemoryDatabase(extBytes);
                    var immBuilder = extDatabase.ToImmutableBuilder();

                    //AssetBuilderに指定テーブルのデータを書く,databaseBinaryに格納
                    MethodInfo diffMethodInfo = type.GetMethod("ReplaceAsset");
                    if (diffMethodInfo != null)
                    {
                        diffMethodInfo.Invoke(null, new object[] { excelMediumData.allItemValueRowList, immBuilder });
                    }
                    var databaseBinary = immBuilder.Build().ToDatabaseBuilder().Build();

                    //更新された内容をdbファイルに上書き
                    var dir = Path.GetDirectoryName(excelAssetPath);
                    Directory.CreateDirectory(dir);
                    using (var stream = new FileStream(excelAssetPath, FileMode.Create))
                    {
                        stream.Write(databaseBinary, 0, databaseBinary.Length);
                    }
                }
                else
                {

                    MethodInfo methodInfo = type.GetMethod("CreateAsset");
                    if (methodInfo != null)
                    {
                        //メモリdb（AssetBuilder）をビルドする
                        AssetBuilder = new DatabaseBuilder();

                        //AssetBuilderに指定テーブルのデータを書く,databaseBinaryに格納
                        methodInfo.Invoke(null, new object[] { excelMediumData.allItemValueRowList, AssetBuilder });
                        var databaseBinary = AssetBuilder.Build();

                        //AssetBuilderの内容をdbファイルに保存
                        var dir = Path.GetDirectoryName(excelAssetPath);
                        Directory.CreateDirectory(dir);
                        using (var stream = new FileStream(excelAssetPath, FileMode.Create))
                        {
                            stream.Write(databaseBinary, 0, databaseBinary.Length);
                        }
                    }
                }

                //Assetファイル生成成功
                Debug.Log("<color=green>Auto Create Asset Success : </color>" + excelMediumData.excelName);
                return;
            }
        }
        //Assetファイル生成失敗
        Debug.LogWarning("Auto Create Asset Fail : " + (excelMediumData == null ? "" : excelMediumData.excelName));
    }

    //指定のExcelファイルをMasterMemoryのDBファイル（MMAutoMaster）へ書き込み
    public static void CreateOneExcelAsset(string excelFileFullPath)
    {
        //中間データの解析
        ExcelMediumData excelMediumData = CreateClassCodeByExcelPath(excelFileFullPath, true);
        CreateOneAsset(excelMediumData);
    }

    public static void CreateAllAsset()
    {
        //xlsxからAssetデータファイル
        _CreateAllAsset(CreateOneExcelAsset, "xlsx");
        //CSVからAssetデータファイル
        _CreateAllAsset(CreateOneCsvAsset, "csv");
    }

    private static void _CreateAllAsset(Action<string> fCreateOneAssetFunc, string fileExtension)
    {
        //全てのInputファイルを読む
        string[] inputFileFullPaths = Directory.GetFiles(BuildExcelWindow.ExcelFilePath, $"*.{fileExtension}");
        if (inputFileFullPaths == null || inputFileFullPaths.Length == 0)
        {
            Debug.Log($"{fileExtension} file count == 0");
            return;
        }

        //Asset書き込み
        for (int i = 0; i < inputFileFullPaths.Length; i++)
        {
            fCreateOneAssetFunc(inputFileFullPaths[i]);
        }
    }

    //指定のCSVファイルをMasterMemoryのDBファイル（MMAutoMaster）へ書き込み
    public static void CreateOneCsvAsset(string csvFileFullPath)
    {
        //中間データの解析
        ExcelMediumData excelMediumData = CreateClassCodeByCSVPath(csvFileFullPath, true);
        CreateOneAsset(excelMediumData);
    }

    //DBファイル読むクラス生成する（デバッグ用ソースコード）
    public static void CreateReadAssetClass()
    {
        string classCodeStr = ExcelCodeCreaterMM.CreateReadAssetClass().ToString();
        if (!string.IsNullOrEmpty(classCodeStr))
        {
            //CSharp.csを書き
            if (WriteCodeStrToSave(readerClassPath, "MMAssetReader", classCodeStr))
            {
                Debug.Log("<color=green>Auto Create MMAssetReader Scripts Success : </color>" + "MMAssetReader");
            }
        }
    }
    #endregion


    #region  --- check asset ---
    //CreateReadAssetClass()を生成したクラスを使って、Consoleに指定テーブルのデータを出力
    public static void ReadTable(string tableName)
    {
        MasterBytesRefresh();

        //タイプ処理：Assembly-CSharp-EditorからAssembly-CSharpへ
        Type type = null;
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type tempType = asm.GetType("MmdbAssetReader");
            if (tempType != null)
            {
                type = tempType;
                break;
            }
        }

        MethodInfo readMethodInfo = type.GetMethod($"Read_{tableName}_Table");
        if (readMethodInfo != null)
        {
            readMethodInfo.Invoke(null, null);
        }
    }
    #endregion

    #region --- private ---

    //Excelファイルから中間デーブルデータへ
    private static ExcelMediumData CreateClassCodeByExcelPath(string excelFileFullPath, bool isReadData)
    {
        if (string.IsNullOrEmpty(excelFileFullPath))
            return null;

        excelFileFullPath = excelFileFullPath.Replace("\\", "/");

        FileStream stream = File.Open(excelFileFullPath, FileMode.Open, FileAccess.Read);
        if (stream == null)
            return null;
        //Excel読む
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        //無効Excel
        if (excelReader == null)// || !excelReader.IsValid)
        {
            Debug.Log("Invalid excel ： " + excelFileFullPath);
            return null;
        }

        List<List<string>> excelDatas = new();
        var dataSetTable = excelReader.AsDataSet().Tables[0];
        excelReader.Close();
        DataRowCollection rowCollection = dataSetTable.Rows;
        for (int readInx = 0; readInx < dataSetTable.Rows.Count; readInx++)
        {
            if (dataSetTable.Columns.Count == 0)
                continue;

            //read every line
            List<string> datas = new List<string>();
            for (int j = 0; j < dataSetTable.Columns.Count; ++j)
            {
                try
                {
                    var addData = rowCollection[readInx][j].ToString(); //excelReader.GetString(j);
                    datas.Add(addData);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("CreateClassCodeByExcelPath Error: " + e.Message + " Stack: " + e.StackTrace);
                }

            }
            excelDatas.Add(datas);
        }

        if (excelDatas.Count != 0)
        {
            string genClassName = excelFileFullPath.Split('/').LastOrDefault();
            genClassName = genClassName.Split('\\').LastOrDefault();
            genClassName = genClassName.Replace(".xlsx", "");

            return CreateMediumData(excelDatas, genClassName, isReadData);
        }
        else
            return null;
    }

    //CSVファイルから中間デーブルデータへ
    private static ExcelMediumData CreateClassCodeByCSVPath(string csvFileFullPath, bool isReadData)
    {
        // 読み込みたいCSVファイルのパスを指定して開く
        using StreamReader sr = new StreamReader(csvFileFullPath);
        {
            List<List<string>> csvDatas = new();
            // 末尾まで繰り返す
            while (!sr.EndOfStream)
            {
                // CSVファイルの一行を読み込む
                string line = sr.ReadLine();
                // 読み込んだ一行をカンマ毎に分けて配列に格納する
                string[] values = line.Split(',');

                // 配列からリストに格納する
                List<string> lists = new List<string>();
                lists.AddRange(values);

                csvDatas.Add(lists);
            }

            if (csvDatas.Count != 0)
            {

                string genClassName = csvFileFullPath.Split('/').LastOrDefault();
                genClassName = genClassName.Split('\\').LastOrDefault();
                genClassName = genClassName.Replace(".csv", "");

                return CreateMediumData(csvDatas, genClassName, isReadData);
            }
        }
        return null;
    }


    //中間デーブルデータを生成します
    private static ExcelMediumData CreateMediumData(List<List<string>> inputData, string genClassName, bool isReadData)
    {
        //<item name,item type>
        PropertyData[] propertyDatas = { };
        string[] propertyDatasPType = null;
        string[] propertyDatasPrimary = null;
        bool[] propertyDatasIsClient = null;

        //List<KeyValuePair<item name, item value>[]>，all data，record by row
        List<Dictionary<string, string>> allItemValueRowList = new List<Dictionary<string, string>>();

        //item nums
        int propertyCount = 0;
        //now row index
        int curRowIndex = 1;

        int dealCol = excelDataColumn - 1;

        for (int readInx = 0; readInx < inputData.Count; readInx++)
        {
            var dataSetTable = inputData[readInx];
            if (dataSetTable.Count == 0)
                continue;

            //read every line
            List<string> datas = new List<string>();
            for (int j = dealCol; j < dataSetTable.Count; ++j)
            {
                try
                {
                    var addData = inputData[readInx][j].ToString();
                    if (!string.IsNullOrEmpty(addData))
                        datas.Add(addData);
                    else if (curRowIndex != excelNameRow && curRowIndex < excelDataRow)
                        datas.Add(string.Empty);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("CreateClassCodeByExcelPath Error: " + e.Message + " Stack: " + e.StackTrace);
                }

            }
            //ingnore when first empty
            if (datas.Count == 0) // || string.IsNullOrEmpty(datas[0]))
            {
                curRowIndex++;
                continue;
            }
            //excelDataRow行目以降：データ処理
            if (curRowIndex >= excelDataRow)
            {
                if (!isReadData)
                    break;
                //invaild data
                if (propertyCount <= 0)
                    return null;

                Dictionary<string, string> itemDic = new Dictionary<string, string>(propertyCount);
                //read every item
                for (int j = 0; j < propertyCount; j++)
                {
                    if (j < datas.Count)
                        itemDic[propertyDatas[j].Name] = datas[j];
                    else
                        itemDic[propertyDatas[j].Name] = null;
                }
                allItemValueRowList.Add(itemDic);
            }
            //excelNameRow行目：メンバ変数名前
            else if (curRowIndex == excelNameRow)
            {
                //メンバ数
                propertyCount = datas.Count;
                if (propertyCount <= 0)
                    return null;
                //メンバ名記録時にpropertyDatasの初期化を行う
                propertyDatas = new PropertyData[datas.Count];
                for (int i = 0; i < propertyDatas.Length; i++)
                {
                    propertyDatas[i].Name = datas[i];
                    if (propertyDatasIsClient != null)
                        propertyDatas[i].IsClient = propertyDatasIsClient[i];
                    if (propertyDatasPType != null)
                        propertyDatas[i].PType = propertyDatasPType[i];
                    if (propertyDatasPrimary != null)
                        propertyDatas[i].Primary = propertyDatasPrimary[i];
                }
            }
            //excelClientFlagRow行目：ClientFlagの処理
            else if (curRowIndex == excelClientFlagRow)
            {
                //ClientFlagの処理
                if (propertyDatasIsClient == null)
                    propertyDatasIsClient = new bool[datas.Count];
                for (int i = 0; i < propertyDatasIsClient.Length; i++)
                {
                    var flagStr = System.Text.RegularExpressions.Regex.Replace(datas[i], @"\s", "");
                    var flagVal = (flagStr != "s" && flagStr != "S");
                    propertyDatasIsClient[i] = flagVal;

                    if (i < propertyDatas.Length)
                        propertyDatas[i].IsClient = flagVal;
                }
            }
            //excelNameRow行目：PrimaryKey設置
            else if (curRowIndex == excelPrimarryRow)
            {

                //メンバPrimaryKey設置記録
                if (propertyDatasPrimary == null)
                    propertyDatasPrimary = new string[datas.Count];
                for (int i = 0; i < propertyDatasPrimary.Length; i++)
                {
                    propertyDatasPrimary[i] = datas[i];
                    if (i < propertyDatas.Length)
                        propertyDatas[i].Primary = datas[i];
                }
            }

            //excelTypeRow行目：メンバ変数タイプ処理
            else if (curRowIndex == excelTypeRow)
            {
                //メンバ変数タイプ記録
                if (propertyDatasPType == null)
                    propertyDatasPType = new string[datas.Count];
                for (int i = 0; i < propertyDatasPType.Length; i++)
                {
                    propertyDatasPType[i] = datas[i];
                    if (i < propertyDatas.Length)
                        propertyDatas[i].PType = datas[i];
                }
            }
            curRowIndex++;
        }

        if (propertyDatas.Length == 0)
            return null;

        ExcelMediumData excelMediumData = new ExcelMediumData();
        //クラス名
        excelMediumData.excelName = genClassName;//excelReader.Name;
        //conver data
        for (int i = 0; i < propertyCount; i++)
        {
            //メンバ名は被った場
            if (excelMediumData.propertyNameTypeDic.ContainsKey(propertyDatas[i].Name))
                return null;
            excelMediumData.propertyNameTypeDic.Add(propertyDatas[i].Name, propertyDatas[i].PType);
            excelMediumData.propertyPrimary.Add(propertyDatas[i].Name, propertyDatas[i].Primary);
            excelMediumData.propertyIsClient.Add(propertyDatas[i].Name, propertyDatas[i].IsClient);
        }
        excelMediumData.allItemValueRowList = allItemValueRowList;
        return excelMediumData;
    }

    //C#ファイルの書き
    private static bool WriteCodeStrToSave(string writeFilePath, string codeFileName, string classCodeStr)
    {
        if (string.IsNullOrEmpty(codeFileName) || string.IsNullOrEmpty(classCodeStr))
            return false;
        //path check
        if (!Directory.Exists(writeFilePath))
            Directory.CreateDirectory(writeFilePath);
        //generate
        StreamWriter sw = new StreamWriter(writeFilePath + "/" + codeFileName + ".cs");
        sw.WriteLine(classCodeStr);
        sw.Close();
        //
        UnityEditor.AssetDatabase.Refresh();
        return true;
    }
    #endregion

}
#endif