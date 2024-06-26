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

namespace DummyEgg.MasterDataWorker.Tables
{
   public sealed partial class mst_test_data_MMItemTable : TableBase<mst_test_data_MMItem>, ITableUniqueValidate
   {
        public Func<mst_test_data_MMItem, uint> PrimaryKeySelector => primaryIndexSelector;
        readonly Func<mst_test_data_MMItem, uint> primaryIndexSelector;

        readonly mst_test_data_MMItem[] secondaryIndex0;
        readonly Func<mst_test_data_MMItem, uint> secondaryIndex0Selector;

        public mst_test_data_MMItemTable(mst_test_data_MMItem[] sortedData)
            : base(sortedData)
        {
            this.primaryIndexSelector = x => x.test_id;
            this.secondaryIndex0Selector = x => x.test_group;
            this.secondaryIndex0 = CloneAndSortBy(this.secondaryIndex0Selector, System.Collections.Generic.Comparer<uint>.Default);
            OnAfterConstruct();
        }

        partial void OnAfterConstruct();

        public RangeView<mst_test_data_MMItem> SortBytest_group => new RangeView<mst_test_data_MMItem>(secondaryIndex0, 0, secondaryIndex0.Length - 1, true);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public mst_test_data_MMItem FindBytest_id(uint key)
        {
            var lo = 0;
            var hi = data.Length - 1;
            while (lo <= hi)
            {
                var mid = (int)(((uint)hi + (uint)lo) >> 1);
                var selected = data[mid].test_id;
                var found = (selected < key) ? -1 : (selected > key) ? 1 : 0;
                if (found == 0) { return data[mid]; }
                if (found < 0) { lo = mid + 1; }
                else { hi = mid - 1; }
            }
            return ThrowKeyNotFound(key);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool TryFindBytest_id(uint key, out mst_test_data_MMItem result)
        {
            var lo = 0;
            var hi = data.Length - 1;
            while (lo <= hi)
            {
                var mid = (int)(((uint)hi + (uint)lo) >> 1);
                var selected = data[mid].test_id;
                var found = (selected < key) ? -1 : (selected > key) ? 1 : 0;
                if (found == 0) { result = data[mid]; return true; }
                if (found < 0) { lo = mid + 1; }
                else { hi = mid - 1; }
            }
            result = default;
            return false;
        }

        public mst_test_data_MMItem FindClosestBytest_id(uint key, bool selectLower = true)
        {
            return FindUniqueClosestCore(data, primaryIndexSelector, System.Collections.Generic.Comparer<uint>.Default, key, selectLower);
        }

        public RangeView<mst_test_data_MMItem> FindRangeBytest_id(uint min, uint max, bool ascendant = true)
        {
            return FindUniqueRangeCore(data, primaryIndexSelector, System.Collections.Generic.Comparer<uint>.Default, min, max, ascendant);
        }

        public mst_test_data_MMItem FindBytest_group(uint key)
        {
            return FindUniqueCore(secondaryIndex0, secondaryIndex0Selector, System.Collections.Generic.Comparer<uint>.Default, key, true);
        }
        
        public bool TryFindBytest_group(uint key, out mst_test_data_MMItem result)
        {
            return TryFindUniqueCore(secondaryIndex0, secondaryIndex0Selector, System.Collections.Generic.Comparer<uint>.Default, key, out result);
        }

        public mst_test_data_MMItem FindClosestBytest_group(uint key, bool selectLower = true)
        {
            return FindUniqueClosestCore(secondaryIndex0, secondaryIndex0Selector, System.Collections.Generic.Comparer<uint>.Default, key, selectLower);
        }

        public RangeView<mst_test_data_MMItem> FindRangeBytest_group(uint min, uint max, bool ascendant = true)
        {
            return FindUniqueRangeCore(secondaryIndex0, secondaryIndex0Selector, System.Collections.Generic.Comparer<uint>.Default, min, max, ascendant);
        }


        void ITableUniqueValidate.ValidateUnique(ValidateResult resultSet)
        {
#if !DISABLE_MASTERMEMORY_VALIDATOR

            ValidateUniqueCore(data, primaryIndexSelector, "test_id", resultSet);       
            ValidateUniqueCore(secondaryIndex0, secondaryIndex0Selector, "test_group", resultSet);       

#endif
        }

#if !DISABLE_MASTERMEMORY_METADATABASE

        public static MasterMemory.Meta.MetaTable CreateMetaTable()
        {
            return new MasterMemory.Meta.MetaTable(typeof(mst_test_data_MMItem), typeof(mst_test_data_MMItemTable), "mst_test_data_MMItem",
                new MasterMemory.Meta.MetaProperty[]
                {
                    new MasterMemory.Meta.MetaProperty(typeof(mst_test_data_MMItem).GetProperty("test_id")),
                    new MasterMemory.Meta.MetaProperty(typeof(mst_test_data_MMItem).GetProperty("test_str")),
                    new MasterMemory.Meta.MetaProperty(typeof(mst_test_data_MMItem).GetProperty("test_group")),
                    new MasterMemory.Meta.MetaProperty(typeof(mst_test_data_MMItem).GetProperty("test_float")),
                },
                new MasterMemory.Meta.MetaIndex[]{
                    new MasterMemory.Meta.MetaIndex(new System.Reflection.PropertyInfo[] {
                        typeof(mst_test_data_MMItem).GetProperty("test_id"),
                    }, true, true, System.Collections.Generic.Comparer<uint>.Default),
                    new MasterMemory.Meta.MetaIndex(new System.Reflection.PropertyInfo[] {
                        typeof(mst_test_data_MMItem).GetProperty("test_group"),
                    }, false, true, System.Collections.Generic.Comparer<uint>.Default),
                });
        }

#endif
    }
}