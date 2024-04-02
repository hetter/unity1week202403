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
   public sealed class DatabaseBuilder : DatabaseBuilderBase
   {
        public DatabaseBuilder() : this(null) { }
        public DatabaseBuilder(MessagePack.IFormatterResolver resolver) : base(resolver) { }

        public DatabaseBuilder Append(System.Collections.Generic.IEnumerable<MstExp_MMItem> dataSource)
        {
            AppendCore(dataSource, x => x.lv, System.Collections.Generic.Comparer<int>.Default);
            return this;
        }

        public DatabaseBuilder Append(System.Collections.Generic.IEnumerable<MstJobBaseData_MMItem> dataSource)
        {
            AppendCore(dataSource, x => x.job_id, System.Collections.Generic.Comparer<int>.Default);
            return this;
        }

        public DatabaseBuilder Append(System.Collections.Generic.IEnumerable<MstJobDesc_MMItem> dataSource)
        {
            AppendCore(dataSource, x => x.job_id, System.Collections.Generic.Comparer<int>.Default);
            return this;
        }

        public DatabaseBuilder Append(System.Collections.Generic.IEnumerable<MstJobProgress_MMItem> dataSource)
        {
            AppendCore(dataSource, x => x.job_id, System.Collections.Generic.Comparer<int>.Default);
            return this;
        }

        public DatabaseBuilder Append(System.Collections.Generic.IEnumerable<mst_test_data_MMItem> dataSource)
        {
            AppendCore(dataSource, x => x.test_id, System.Collections.Generic.Comparer<uint>.Default);
            return this;
        }

    }
}