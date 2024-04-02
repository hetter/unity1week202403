#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;
using System.IO;
using DummyEgg.MasterDataWorker;

//MasterMemory C# Code Creator
public class ExcelCodeCreaterMM
{

    #region --- Create Code ---

    //MasterMemory/MessagePackage用の基本クラスを生成します
    public static string CreateCodeStrByExcelData(ExcelMediumData excelMediumData)
    {
        if (excelMediumData == null)
            return null;
        //Excel name
        string excelName = excelMediumData.excelName;
        if (string.IsNullOrEmpty(excelName))
            return null;
        //Dictionary<item name, item type>
        Dictionary<string, string> propertyNameTypeDic = excelMediumData.propertyNameTypeDic;
        if (propertyNameTypeDic == null || propertyNameTypeDic.Count == 0)
            return null;

        Dictionary<string, string> propertyPrimary = excelMediumData.propertyPrimary;

        Dictionary<string, bool> propertyIsClient = excelMediumData.propertyIsClient;

        string itemClassName = excelName + "_MMItem";
        
        //generate class
        StringBuilder classSource = new StringBuilder();
        classSource.Append("/*Auto Create, Don't Edit !!!*/\n");
        classSource.Append("\n");
        //add using
        classSource.Append("using UnityEngine;\n");
        classSource.Append("using System.Collections.Generic;\n");
        classSource.Append("using System;\n");
        classSource.Append("using System.IO;\n");

        classSource.Append("using MasterMemory;\n");
        classSource.Append("using MessagePack;\n");

        classSource.Append("\n");
        //
        classSource.Append(CreateExcelRowItemClass(itemClassName, propertyNameTypeDic, propertyPrimary, propertyIsClient));
        classSource.Append("\n");

        return classSource.ToString();
    }

    //MasterMemoryのDBファイル（MMAutoMaster）へ書き込み用のクラスを生成します
    public static string CreateBuildAssetCode(ExcelMediumData excelMediumData)
    {
        if (excelMediumData == null)
            return null;
        //generate class
        StringBuilder classSource = new StringBuilder();
        classSource.Append("/*Auto Create, Don't Edit !!!*/\n");
        classSource.Append("\n");
        //add using
        classSource.Append("using UnityEngine;\n");
        classSource.Append("using System.Collections.Generic;\n");
        classSource.Append("using System;\n");
        classSource.Append("using System.IO;\n");
        classSource.Append("\n");
        classSource.Append(CreateExcelAssetClass(excelMediumData));
        classSource.Append("\n");
        return classSource.ToString();
    }

    //----------

    //generate row data
    private static StringBuilder CreateExcelRowItemClass(string itemClassName,
                                                            Dictionary<string, string> propertyNameTypeDic,
                                                            Dictionary<string, string> propertyPrimary,
                                                            Dictionary<string, bool> propertyIsClient)
    {
        StringBuilder classSource = new StringBuilder();
        classSource.Append($"[MemoryTable(\"{itemClassName}\"), MessagePackObject(true)]\n");
        classSource.Append("public class " + itemClassName + "\n");
        classSource.Append("{\n");
        //define data name
        foreach (var item in propertyNameTypeDic)
        {
            int primaryInx = 0;
            bool isClient = true;

            if(propertyPrimary.TryGetValue(item.Key, out string primaryStr))
            {
                int.TryParse(primaryStr, out primaryInx);
            }

            if (propertyIsClient.TryGetValue(item.Key, out bool isCl))
                isClient = isCl;

            if (isClient)
                classSource.Append(CreateCodeProperty(item.Key, item.Value, primaryInx));
        }
        classSource.Append($"\tpublic {itemClassName}() " + "{ }\n");
        classSource.Append("}\n");
        return classSource;
    }

    //define item name
    private static string CreateCodeProperty(string name, string type, int primaryKey)
    {
        Debug.Log($"processing type:{type}");
        if (string.IsNullOrEmpty(name))
            return null;
        //if (name == "id")
        //    return null;

        switch (type.ToLower())
        {
            //type
            case "int":
                type = "int";
                break;
            case "long":
                type = "long";
                break;
            case "uint":
            case "unsigned_int":
                type = "uint";
                break;
            case "float":
                type = "float";
                break;
            case "bool":
                type = "bool";
                break;
            default:
                {
                    if (type.StartsWith("enum") || type.StartsWith("Enum") || type.StartsWith("ENUM"))
                        type = type.Split('|').LastOrDefault();
                    else
                        type = "string";
                    break;
                }
        }

        //MasterMemoryにおげるプライマリキー設定
        string propertyStr = string.Empty;
        if (primaryKey == 1)
            propertyStr += "\t[PrimaryKey]\n";
        else if(primaryKey == 2)
            propertyStr += "\t[SecondaryKey(0)]\n";
        propertyStr += "\tpublic " + type + " " + name + " { get; set; }\n";
        return propertyStr;
    }

    //書き込み用のクラスの詳細項目
    private static StringBuilder CreateExcelAssetClass(ExcelMediumData excelMediumData)
    {
        if (excelMediumData == null)
            return null;

        string excelName = excelMediumData.excelName;
        if (string.IsNullOrEmpty(excelName))
            return null;

        Dictionary<string, string> propertyNameTypeDic = excelMediumData.propertyNameTypeDic;
        if (propertyNameTypeDic == null || propertyNameTypeDic.Count == 0)
            return null;

        string itemClassName = excelName + "_MMItem";

        StringBuilder classSource = new StringBuilder();
        classSource.Append("#if UNITY_EDITOR\n");
        //class name
        classSource.Append("public class " + excelName + "MMAssetAssignment\n");
        classSource.Append("{\n");
        //function name
        classSource.Append("[CLSCompliantAttribute(false)]\n");
        classSource.Append($"\tpublic static {itemClassName}[] CreateItemTable(List<Dictionary<string, string>> allItemValueRowList)\n");
        //may to do:try/catch
        classSource.Append("\t{\n");
        classSource.Append("\t\tif (allItemValueRowList == null || allItemValueRowList.Count == 0)\n");
        classSource.Append("\t\t\treturn null;\n");
        classSource.Append("\t\tint rowCount = allItemValueRowList.Count;\n");
        classSource.Append("\t\t" + itemClassName + "[] items = new " + itemClassName + "[rowCount];\n");
        classSource.Append("\t\tfor (int i = 0; i < items.Length; i++)\n");
        classSource.Append("\t\t{\n");
        classSource.Append("\t\t\titems[i] = new " + itemClassName + "();\n");
        foreach (var item in propertyNameTypeDic)
        {
            if (excelMediumData.propertyIsClient.ContainsKey(item.Key) && excelMediumData.propertyIsClient[item.Key])
            {
                classSource.Append("\t\t\titems[i]." + item.Key + " = ");

                classSource.Append(AssignmentCodeProperty("allItemValueRowList[i][\"" + item.Key + "\"]", propertyNameTypeDic[item.Key]));
                classSource.Append(";\n");
            }
        }
        classSource.Append("\t\t}\n");
        classSource.Append("\t\treturn items;\n");
        classSource.Append("\t}\n");
        //

        //function name（新しいデータ書き込み）
        classSource.Append($"\t[CLSCompliantAttribute(false)]\n");
        classSource.Append($"\tpublic static void CreateAsset(List<Dictionary<string, string>> allItemValueRowList, DummyEgg.MasterDataWorker.DatabaseBuilder builder)\n");
        //may to do:try/catch
        classSource.Append("\t{\n");
        classSource.Append("\t\tvar ct = CreateItemTable(allItemValueRowList);\n");
        classSource.Append("\t\tbuilder.Append(ct);\n");
        classSource.Append("\t}\n");
        //

        //function name（データ書き換え関数）
        classSource.Append($"\t[CLSCompliantAttribute(false)]\n");
        classSource.Append($"\tpublic static void ReplaceAsset(List<Dictionary<string, string>> allItemValueRowList, DummyEgg.MasterDataWorker.ImmutableBuilder builder)\n");
        //may to do:try/catch
        classSource.Append("\t{\n");
        classSource.Append("\t\tvar ct = CreateItemTable(allItemValueRowList);\n");
        classSource.Append("\t\tbuilder.ReplaceAll(ct);\n");
        classSource.Append("\t}\n");
        //

        classSource.Append("}\n");
        classSource.Append("#endif\n");
        return classSource;
    }

    //generate asset memeber
    private static string AssignmentCodeProperty(string stringValue, string type)
    {
        switch (type.ToLower())
        {
            case "int":
                return "Convert.ToInt32(" + stringValue + ")";
            case "long":
                return "Convert.ToInt64(" + stringValue + ")";
            case "uint":
            case "unsigned_int":
                return "Convert.ToUInt32(" + stringValue + ")";
            case "float":
                return "Convert.ToSingle(" + stringValue + ")";
            case "bool":
                return "Convert.ToBoolean(" + stringValue + ")";
            default:
                {
                    if (type.StartsWith("enum") || type.StartsWith("Enum") || type.StartsWith("ENUM"))
                    {
                        return "(" + type.Split('|').LastOrDefault() + ")(Convert.ToInt32(" + stringValue + "))";
                    }
                    else
                        return stringValue;
                }
        }
    }

    //DBファイル読むクラス生成する（デバッグ用ソースコード）
    public static StringBuilder CreateReadAssetClass()
    {
        StringBuilder classSource = new StringBuilder();

       
        classSource.Append("#if UNITY_EDITOR\n");

        classSource.Append("/*Auto Create, Don't Edit !!!*/\n");
        classSource.Append("\n");
        //add using
        classSource.Append("using UnityEngine;\n");
        classSource.Append("using System.Collections.Generic;\n");
        classSource.Append("using System;\n");
        classSource.Append("using System.IO;\n");
        classSource.Append("using DummyEgg.MasterDataWorker;\n");
        classSource.Append("\n");

        //class name
        classSource.Append("public class MmdbAssetReader\n");
        classSource.Append("{\n");
        //function name

        classSource.Append("\tstatic MemoryDatabase DataBase = null;\n");

        var mmbytres = ExcelDataReaderMM.MasterBytes();
        foreach(var tinfo in MemoryDatabase.GetTableInfo(mmbytres))
        {
            string itemName = tinfo.TableName;
            classSource.Append("\n");
            classSource.Append($"\tpublic static void Read_{itemName}_Table()\n");
            classSource.Append("\t{\n");

            classSource.Append($"\t\t//if (DataBase == null)\n");
            classSource.Append("\t\t{\n");

            classSource.Append($"\t\t  var mmbytres = ExcelDataReaderMM.MasterBytes();\n");
            classSource.Append($"\t\t  DataBase = new MemoryDatabase(mmbytres);\n");
            classSource.Append("\t\t}\n");
        

            classSource.Append($"\t\tvar type = typeof({itemName});\n");
            classSource.Append($"\t\tvar propInfo = type.GetProperties();\n");
            classSource.Append($"\t\tforeach (var ii in DataBase.{itemName}Table.All)\n");
            classSource.Append("\t\t{\n");
            classSource.Append($"\t\tvar itmInfo = \"{itemName}:\";\n");
            classSource.Append($"\t\tforeach (var pp in propInfo)\n");
            classSource.Append($"\t\t\titmInfo += pp.Name + \":\" + pp.GetValue(ii) + \", \";\n");
            classSource.Append("\t\tDebug.Log($\" <color=green>{itmInfo}</color> \");\n");
            classSource.Append("\t\t}\n");
            classSource.Append("\t}\n");
        }
        //classSource.Append($"\t\t\n");

        classSource.Append("}\n");
        classSource.Append("#endif\n");
        return classSource;
    }
    #endregion

}
#endif