
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Text.Json;
using Server.Config.Core;


namespace cfg.test
{
public sealed partial class Equipment : test.ItemBase
{
    public Equipment(JsonElement _buf)  : base(_buf) 
    {
        Attr = (test.DemoEnum)_buf.GetProperty("attr").GetInt32();
        Value = _buf.GetProperty("value").GetInt32();
    }

    public static Equipment DeserializeEquipment(JsonElement _buf)
    {
        return new test.Equipment(_buf);
    }

    public readonly test.DemoEnum Attr;
    public readonly int Value;
   
    public const int __ID__ = -76837102;
    public override int GetTypeId() => __ID__;

    public override void ResolveRef(Tables tables)
    {
        base.ResolveRef(tables);
        
        
    }

    public override string ToString()
    {
        return "{ "
        + "id:" + Id + ","
        + "name:" + Name + ","
        + "desc:" + Desc + ","
        + "attr:" + Attr + ","
        + "value:" + Value + ","
        + "}";
    }
}

}