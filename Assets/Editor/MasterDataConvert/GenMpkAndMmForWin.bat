mpc -i %~dp0/../../Scripts/MMAutoCode/mpkClass -o %~dp0/../../Scripts/MMAutoCode/mpkGen/MmMsgPack.cs -n "DummyEgg.MasterDataWorker" -r MmGeneratedResolver

del /f /s /q %~dp0..\..\Scripts\MMAutoCode\mmgen

dotnet-mmgen -i %~dp0/../../Scripts/MMAutoCode/mpkClass -o  %~dp0/../../Scripts/MMAutoCode/mmgen -n "DummyEgg.MasterDataWorker" -c --roll-forward Major