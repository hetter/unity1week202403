// <auto-generated />
#pragma warning disable CS0105
/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using MasterMemory.Validation;
using MasterMemory;
using MessagePack;
using System.Collections.Generic;
using System.IO;
using System;
using DummyEgg.MasterDataWorker.Tables;

namespace DummyEgg.MasterDataWorker
{
   public sealed class ImmutableBuilder : ImmutableBuilderBase
   {
        MemoryDatabase memory;

        public ImmutableBuilder(MemoryDatabase memory)
        {
            this.memory = memory;
        }

        public MemoryDatabase Build()
        {
            return memory;
        }

        public void ReplaceAll(System.Collections.Generic.IList<MstExp_MMItem> data)
        {
            var newData = CloneAndSortBy(data, x => x.lv, System.Collections.Generic.Comparer<int>.Default);
            var table = new MstExp_MMItemTable(newData);
            memory = new MemoryDatabase(
                table,
                memory.MstJobBaseData_MMItemTable,
                memory.MstJobDesc_MMItemTable,
                memory.MstJobProgress_MMItemTable,
                memory.mst_test_data_MMItemTable
            
            );
        }

        public void RemoveMstExp_MMItem(int[] keys)
        {
            var data = RemoveCore(memory.MstExp_MMItemTable.GetRawDataUnsafe(), keys, x => x.lv, System.Collections.Generic.Comparer<int>.Default);
            var newData = CloneAndSortBy(data, x => x.lv, System.Collections.Generic.Comparer<int>.Default);
            var table = new MstExp_MMItemTable(newData);
            memory = new MemoryDatabase(
                table,
                memory.MstJobBaseData_MMItemTable,
                memory.MstJobDesc_MMItemTable,
                memory.MstJobProgress_MMItemTable,
                memory.mst_test_data_MMItemTable
            
            );
        }

        public void Diff(MstExp_MMItem[] addOrReplaceData)
        {
            var data = DiffCore(memory.MstExp_MMItemTable.GetRawDataUnsafe(), addOrReplaceData, x => x.lv, System.Collections.Generic.Comparer<int>.Default);
            var newData = CloneAndSortBy(data, x => x.lv, System.Collections.Generic.Comparer<int>.Default);
            var table = new MstExp_MMItemTable(newData);
            memory = new MemoryDatabase(
                table,
                memory.MstJobBaseData_MMItemTable,
                memory.MstJobDesc_MMItemTable,
                memory.MstJobProgress_MMItemTable,
                memory.mst_test_data_MMItemTable
            
            );
        }

        public void ReplaceAll(System.Collections.Generic.IList<MstJobBaseData_MMItem> data)
        {
            var newData = CloneAndSortBy(data, x => x.job_id, System.Collections.Generic.Comparer<int>.Default);
            var table = new MstJobBaseData_MMItemTable(newData);
            memory = new MemoryDatabase(
                memory.MstExp_MMItemTable,
                table,
                memory.MstJobDesc_MMItemTable,
                memory.MstJobProgress_MMItemTable,
                memory.mst_test_data_MMItemTable
            
            );
        }

        public void RemoveMstJobBaseData_MMItem(int[] keys)
        {
            var data = RemoveCore(memory.MstJobBaseData_MMItemTable.GetRawDataUnsafe(), keys, x => x.job_id, System.Collections.Generic.Comparer<int>.Default);
            var newData = CloneAndSortBy(data, x => x.job_id, System.Collections.Generic.Comparer<int>.Default);
            var table = new MstJobBaseData_MMItemTable(newData);
            memory = new MemoryDatabase(
                memory.MstExp_MMItemTable,
                table,
                memory.MstJobDesc_MMItemTable,
                memory.MstJobProgress_MMItemTable,
                memory.mst_test_data_MMItemTable
            
            );
        }

        public void Diff(MstJobBaseData_MMItem[] addOrReplaceData)
        {
            var data = DiffCore(memory.MstJobBaseData_MMItemTable.GetRawDataUnsafe(), addOrReplaceData, x => x.job_id, System.Collections.Generic.Comparer<int>.Default);
            var newData = CloneAndSortBy(data, x => x.job_id, System.Collections.Generic.Comparer<int>.Default);
            var table = new MstJobBaseData_MMItemTable(newData);
            memory = new MemoryDatabase(
                memory.MstExp_MMItemTable,
                table,
                memory.MstJobDesc_MMItemTable,
                memory.MstJobProgress_MMItemTable,
                memory.mst_test_data_MMItemTable
            
            );
        }

        public void ReplaceAll(System.Collections.Generic.IList<MstJobDesc_MMItem> data)
        {
            var newData = CloneAndSortBy(data, x => x.job_id, System.Collections.Generic.Comparer<int>.Default);
            var table = new MstJobDesc_MMItemTable(newData);
            memory = new MemoryDatabase(
                memory.MstExp_MMItemTable,
                memory.MstJobBaseData_MMItemTable,
                table,
                memory.MstJobProgress_MMItemTable,
                memory.mst_test_data_MMItemTable
            
            );
        }

        public void RemoveMstJobDesc_MMItem(int[] keys)
        {
            var data = RemoveCore(memory.MstJobDesc_MMItemTable.GetRawDataUnsafe(), keys, x => x.job_id, System.Collections.Generic.Comparer<int>.Default);
            var newData = CloneAndSortBy(data, x => x.job_id, System.Collections.Generic.Comparer<int>.Default);
            var table = new MstJobDesc_MMItemTable(newData);
            memory = new MemoryDatabase(
                memory.MstExp_MMItemTable,
                memory.MstJobBaseData_MMItemTable,
                table,
                memory.MstJobProgress_MMItemTable,
                memory.mst_test_data_MMItemTable
            
            );
        }

        public void Diff(MstJobDesc_MMItem[] addOrReplaceData)
        {
            var data = DiffCore(memory.MstJobDesc_MMItemTable.GetRawDataUnsafe(), addOrReplaceData, x => x.job_id, System.Collections.Generic.Comparer<int>.Default);
            var newData = CloneAndSortBy(data, x => x.job_id, System.Collections.Generic.Comparer<int>.Default);
            var table = new MstJobDesc_MMItemTable(newData);
            memory = new MemoryDatabase(
                memory.MstExp_MMItemTable,
                memory.MstJobBaseData_MMItemTable,
                table,
                memory.MstJobProgress_MMItemTable,
                memory.mst_test_data_MMItemTable
            
            );
        }

        public void ReplaceAll(System.Collections.Generic.IList<MstJobProgress_MMItem> data)
        {
            var newData = CloneAndSortBy(data, x => x.job_id, System.Collections.Generic.Comparer<int>.Default);
            var table = new MstJobProgress_MMItemTable(newData);
            memory = new MemoryDatabase(
                memory.MstExp_MMItemTable,
                memory.MstJobBaseData_MMItemTable,
                memory.MstJobDesc_MMItemTable,
                table,
                memory.mst_test_data_MMItemTable
            
            );
        }

        public void RemoveMstJobProgress_MMItem(int[] keys)
        {
            var data = RemoveCore(memory.MstJobProgress_MMItemTable.GetRawDataUnsafe(), keys, x => x.job_id, System.Collections.Generic.Comparer<int>.Default);
            var newData = CloneAndSortBy(data, x => x.job_id, System.Collections.Generic.Comparer<int>.Default);
            var table = new MstJobProgress_MMItemTable(newData);
            memory = new MemoryDatabase(
                memory.MstExp_MMItemTable,
                memory.MstJobBaseData_MMItemTable,
                memory.MstJobDesc_MMItemTable,
                table,
                memory.mst_test_data_MMItemTable
            
            );
        }

        public void Diff(MstJobProgress_MMItem[] addOrReplaceData)
        {
            var data = DiffCore(memory.MstJobProgress_MMItemTable.GetRawDataUnsafe(), addOrReplaceData, x => x.job_id, System.Collections.Generic.Comparer<int>.Default);
            var newData = CloneAndSortBy(data, x => x.job_id, System.Collections.Generic.Comparer<int>.Default);
            var table = new MstJobProgress_MMItemTable(newData);
            memory = new MemoryDatabase(
                memory.MstExp_MMItemTable,
                memory.MstJobBaseData_MMItemTable,
                memory.MstJobDesc_MMItemTable,
                table,
                memory.mst_test_data_MMItemTable
            
            );
        }

        public void ReplaceAll(System.Collections.Generic.IList<mst_test_data_MMItem> data)
        {
            var newData = CloneAndSortBy(data, x => x.test_id, System.Collections.Generic.Comparer<uint>.Default);
            var table = new mst_test_data_MMItemTable(newData);
            memory = new MemoryDatabase(
                memory.MstExp_MMItemTable,
                memory.MstJobBaseData_MMItemTable,
                memory.MstJobDesc_MMItemTable,
                memory.MstJobProgress_MMItemTable,
                table
            
            );
        }

        public void Removemst_test_data_MMItem(uint[] keys)
        {
            var data = RemoveCore(memory.mst_test_data_MMItemTable.GetRawDataUnsafe(), keys, x => x.test_id, System.Collections.Generic.Comparer<uint>.Default);
            var newData = CloneAndSortBy(data, x => x.test_id, System.Collections.Generic.Comparer<uint>.Default);
            var table = new mst_test_data_MMItemTable(newData);
            memory = new MemoryDatabase(
                memory.MstExp_MMItemTable,
                memory.MstJobBaseData_MMItemTable,
                memory.MstJobDesc_MMItemTable,
                memory.MstJobProgress_MMItemTable,
                table
            
            );
        }

        public void Diff(mst_test_data_MMItem[] addOrReplaceData)
        {
            var data = DiffCore(memory.mst_test_data_MMItemTable.GetRawDataUnsafe(), addOrReplaceData, x => x.test_id, System.Collections.Generic.Comparer<uint>.Default);
            var newData = CloneAndSortBy(data, x => x.test_id, System.Collections.Generic.Comparer<uint>.Default);
            var table = new mst_test_data_MMItemTable(newData);
            memory = new MemoryDatabase(
                memory.MstExp_MMItemTable,
                memory.MstJobBaseData_MMItemTable,
                memory.MstJobDesc_MMItemTable,
                memory.MstJobProgress_MMItemTable,
                table
            
            );
        }

    }
}