using System.ComponentModel;

namespace CoreApiBase.Enums
{
    public enum TokenDataType
    {
        [Description("sub")]
        UserId,
        
        [Description("unique_name")]
        Username,
        
        [Description("email")]
        Email,
        
        [Description("SecurityStamp")]
        SecurityStamp,
        
        [Description("nameid")]
        NameId
    }
}
