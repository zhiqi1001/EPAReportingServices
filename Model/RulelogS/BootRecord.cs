using System;
using System.Collections.Generic;
using PetaPoco;

namespace Model.RulelogS
{
    public class BootRecord
    {
        public int id { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
    }

    public class EditBootRecord
    {
        public int id { get; set; }

        /// <summary>
        /// 0 scr启停 1机组启停
        /// </summary>
        public int typeid { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
    }

    public class BootRecordSelect
    {
        public int id { get; set; }
        /// <summary>
        /// name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 点名
        /// </summary>
        public string pointname { get; set; }

        /// <summary>
        /// 0 scr启停 1机组启停
        /// </summary>
        public int typeid { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime starttime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime endtime { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
    }
}
