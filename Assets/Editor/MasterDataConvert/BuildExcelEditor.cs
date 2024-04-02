#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using DummyEgg.MasterDataWorker;
using MessagePack;
using MessagePack.Resolvers;

[CLSCompliant(false)]
public class BuildExcelWindow : EditorWindow
{
    //コンフィグ
    //マスタデータExcle Pathの設置
    public static string ExcelFilePath = Application.dataPath + "/Editor/MasterDataConvert/Excel";
    //

    [MenuItem("Tools/Excel To MasterData Window")]
    public static void ShowExcelWindow()
    {
        //show excel tool window
        EditorWindow.GetWindow(typeof(BuildExcelWindow));
    }

    private string showNotify;
    private Vector2 scrollPosition = Vector2.zero;

    private List<string> fileNameList = new List<string>();
    private List<string> filePathList = new List<string>();

    private void Awake()
    {
        titleContent.text = "Excel Data Reader";
    }

    private void OnEnable()
    {
        showNotify = "";
        GetExcelFile();
    }

    private void OnDisable()
    {
        showNotify = "";

        fileNameList.Clear();
        filePathList.Clear();
    }

    private readonly string[] _tabToggles = { "Excel/CSV To Class", "Write To MasterMemoryDb", "Read MasterMemoryDb" };

    private int _tabIndex;


    private void OnGUI()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            _tabIndex = GUILayout.Toolbar(_tabIndex, _tabToggles, new GUIStyle(EditorStyles.toolbarButton), GUI.ToolbarButtonSize.FitToContents);
        }
        EditorGUILayout.LabelField(_tabToggles[_tabIndex]);

        if (_tabIndex == 0)
        {
            InitMPK();
            _excelToClassOnGui();
        }
        else if (_tabIndex == 1)
        {
            InitMPK();
            _excelToAssetOnGui();
        }
        else if(_tabIndex == 2)
        {
            InitMPK();
            _readAssetFileOnGui();
        }
    }

    private void _readAssetFileOnGui()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition,
        GUILayout.Width(position.width), GUILayout.Height(position.height));
        GUILayout.Space(10);
        foreach (var tname in ExcelDataReaderMM.GetTableSet())
        {
            //throw new NotImplementedException();
            if (GUILayout.Button($"{tname}", GUILayout.Width(200), GUILayout.Height(30)))
            {
                ExcelDataReaderMM.ReadTable(tname);
            }
        }


        GUILayout.EndScrollView();
    }

    private void _excelToAssetOnGui()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition,
        GUILayout.Width(position.width), GUILayout.Height(position.height));
        //C# script generate
        GUILayout.Space(10);
        //GUILayout.Label("Write To MasterMemoryDb");
        for (int i = 0; i < fileNameList.Count; i++)
        {
            if (GUILayout.Button(fileNameList[i], GUILayout.Width(200), GUILayout.Height(30)))
            {
                SelectCodeToAssetByIndex(i);
            }
        }
        if (GUILayout.Button("All Files", GUILayout.Width(200), GUILayout.Height(30)))
        {
            SelectCodeToAssetByIndex(-1);
        }
        //
        GUILayout.Space(20);
        GUILayout.Label(showNotify);
        //
        GUILayout.EndScrollView();
        //this.Repaint();
    }

    private void _excelToClassOnGui()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition,
        GUILayout.Width(position.width), GUILayout.Height(position.height));
        //C# script generate
        GUILayout.Space(10);
        //GUILayout.Label("Excel/CSV To Script");
        for (int i = 0; i < fileNameList.Count; i++)
        {
            if (GUILayout.Button(fileNameList[i], GUILayout.Width(200), GUILayout.Height(30)))
            {
                SelectInputFileToCodeByIndex(i);
            }
        }
        if (GUILayout.Button("All Files", GUILayout.Width(200), GUILayout.Height(30)))
        {
            SelectInputFileToCodeByIndex(-1);
        }
        //
        GUILayout.Space(20);


        if (GUILayout.Button("!!!!Delete All Table And Build All Files", GUILayout.Width(300), GUILayout.Height(30)))
        {
            ExcelDataReaderMM.DeleteAll();
            SelectInputFileToCodeByIndex(-1);
        }

        GUILayout.Space(20);
        GUILayout.Label(showNotify);
        //
        GUILayout.EndScrollView();
        //this.Repaint();
    }

    //find Excel files
    private void GetExcelFile()
    {
        fileNameList.Clear();
        filePathList.Clear();

        if (!Directory.Exists(ExcelFilePath))
        {
            showNotify = "invaild path：" + ExcelFilePath;
            return;
        }

        string[] excelFileFullPaths = Directory.GetFiles(ExcelFilePath, "*.xlsx");

        string[] csvFileFullPaths = Directory.GetFiles(ExcelFilePath, "*.csv");

        if ((excelFileFullPaths == null || excelFileFullPaths.Length == 0) &&
            (csvFileFullPaths == null || csvFileFullPaths.Length == 0))
        {
            showNotify = ExcelFilePath + "could not found Excel file";
            return;
        }

        if(excelFileFullPaths != null)
            filePathList.AddRange(excelFileFullPaths);

        if(csvFileFullPaths != null)
            filePathList.AddRange(csvFileFullPaths);

        for (int i = 0; i < filePathList.Count; i++)
        {
            string fileName = filePathList[i].Split('/').LastOrDefault();
            fileName = fileName.Split('\\').LastOrDefault();
            fileNameList.Add(fileName);
        }
        showNotify = "Found Excel File：" + fileNameList.Count;
    }

    //C# script generate
    private void SelectInputFileToCodeByIndex(int index)
    {
        if (index >= 0 && index < filePathList.Count)
        {
            string fullPath = filePathList[index];
            if(fullPath.LastIndexOf(".xlsx") != -1)
                ExcelDataReaderMM.ReadOneExcelToCode(fullPath);
            else if (fullPath.LastIndexOf(".csv") != -1)
                ExcelDataReaderMM.ReadOneCSVToCode(fullPath);
        }
        else
        {
            ExcelDataReaderMM.ReadAllInputFilesToCode();
        }
    }

    //asset file generate
    private void SelectCodeToAssetByIndex(int index)
    {
        if (index >= 0 && index < filePathList.Count)
        {
            string fullPath = filePathList[index];
            if (fullPath.LastIndexOf(".xlsx") != -1)
                ExcelDataReaderMM.CreateOneExcelAsset(fullPath);
            else if (fullPath.LastIndexOf(".csv") != -1)
                ExcelDataReaderMM.CreateOneCsvAsset(fullPath);
        }
        else
        {
            ExcelDataReaderMM.CreateAllAsset();
        }

        ExcelDataReaderMM.CreateReadAssetClass();
    }

    public static void InitMPK()
    {
        // MessagePack の初期化
        try
        {
            StaticCompositeResolver.Instance.Register(new[]{
            MasterMemoryResolver.Instance, // set MasterMemory generated resolver
            DummyEgg.MasterDataWorker.Resolvers.MmGeneratedResolver.Instance,    // set MessagePack generated resolver
            StandardResolver.Instance      // set default MessagePack resolver
            });
            var options = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);
            MessagePackSerializer.DefaultOptions = options;
        }
        catch (Exception)
        {

            // 初期化を複数回実行すると下記の例外が発生するが
            // データベースのビルドには影響がないため例外は無視する
            // InvalidOperationException: Register must call on startup(before use GetFormatter<T>).
            //Debug.LogWarning(ex.Message);
        }

    }
}

//中間データ処理
public class ExcelMediumData
{
    //data class name
    public string excelName;
    //Dictionary<itemName, itemType>
    public Dictionary<string, string> propertyNameTypeDic = new();
    //Dictionary<itemName, itemPrimary>
    public Dictionary<string, string> propertyPrimary = new();
    //Dictionary<itemName, isClient>
    public Dictionary<string, bool> propertyIsClient = new();
    //record all data by row
    public List<Dictionary<string, string>> allItemValueRowList;
}
#endif
