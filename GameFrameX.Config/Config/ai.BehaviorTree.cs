
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Text.Json;
using GameFrameX.Config.Core;


namespace cfg.ai
{
public sealed partial class BehaviorTree : GameFrameX.Config.Core.BeanBase
{
    public BehaviorTree(JsonElement _buf) 
    {
        Id = _buf.GetProperty("id").GetInt32();
        Name = _buf.GetProperty("name").GetString();
        Desc = _buf.GetProperty("desc").GetString();
        BlackboardId = _buf.GetProperty("blackboard_id").GetString();
        BlackboardId_Ref = null;
        Root = ai.ComposeNode.DeserializeComposeNode(_buf.GetProperty("root"));
    }

    public static BehaviorTree DeserializeBehaviorTree(JsonElement _buf)
    {
        return new ai.BehaviorTree(_buf);
    }

    public readonly int Id;
    public readonly string Name;
    public readonly string Desc;
    public readonly string BlackboardId;
    public ai.Blackboard BlackboardId_Ref;
    public readonly ai.ComposeNode Root;
   
    public const int __ID__ = 159552822;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
        
        
        
        BlackboardId_Ref = tables.TbBlackboard.GetOrDefault(BlackboardId);
        Root?.ResolveRef(tables);
    }

    public override string ToString()
    {
        return "{ "
        + "id:" + Id + ","
        + "name:" + Name + ","
        + "desc:" + Desc + ","
        + "blackboardId:" + BlackboardId + ","
        + "root:" + Root + ","
        + "}";
    }
}

}