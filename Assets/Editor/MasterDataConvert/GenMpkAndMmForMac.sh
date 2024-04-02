cur_dir=$(dirname $0)

dotnet mpc -i $cur_dir/../../Scripts/MMAutoCode -o $cur_dir/../../Scripts/MMAutoCode/mpkGen/MmMsgPack.cs -n "ExtraMile.Metaverse.MasterData" -r MmGeneratedResolver

rm -r  $cur_dir/../../Scripts/MMAutoCode/mmgen

dotnet dotnet-mmgen -i $cur_dir/../../Scripts/MMAutoCode/ -o  $cur_dir/../../Scripts/MMAutoCode/mmgen -n "ExtraMile.Metaverse.MasterData" -c --roll-forward Major

