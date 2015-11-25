using System;
using PetaPoco;

namespace Model
{
    /// <summary>
    /// 公式配置
    /// </summary>
    [TableName("tn_FormulaSetting")]
    public class FormulaSetting
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PointName { get; set; }
        public string TableName { get; set; }
        public string Formula { get; set; }
        public string Remarks { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime DataUpdateTime { get; set; }

    }
}
