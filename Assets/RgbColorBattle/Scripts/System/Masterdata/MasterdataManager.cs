using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DummyEgg.MasterDataWorker;

namespace DummyEgg.ProjectGK.MasterData
{
    class MasterdataManager: SingletonClass<MasterdataManager>
    {
        public MemoryDatabase Data;
        public async UniTask Load()
        {
            // 事前にビルドしたマスタを読み込み
            var byteAssets = Resources.LoadAsync<TextAsset>("MMAutoMaster");
            await byteAssets;
            var textAsset = byteAssets.asset as TextAsset;
            // データベースにアクセスするためのインスタンスの作成
            Data = new MemoryDatabase(textAsset.bytes);
        }
    }
}
