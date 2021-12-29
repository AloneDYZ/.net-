﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartCode.Framework.Util;
using SQLite;

namespace SmartCode.Framework.SqliteModel
{
    public class ConnectConfigs
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        /// <summary>
        /// 连接名称
        /// </summary>
        public string ConnectName { get; set; }
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string ServerAddress { get; set; }
        /// <summary>
        /// 服务器端口号
        /// </summary>
        public int ServerPort { get; set; }
        /// <summary>
        /// 身份认证
        /// </summary>
        public int Authentication { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; } = DateTime.Now;
        /// <summary>
        /// 默认数据库
        /// </summary>
        public string DefaultDatabase { get; set; }
        /// <summary>
        /// Master数据库连接，查询系统库相关信息（不映射数据库)
        /// </summary>
        [Ignore]
        public string DbMasterConnectString => $"server={ServerAddress},{ServerPort};database=master;uid={UserName};pwd={EncryptHelper.Decode(Password)};";
        /// <summary>
        /// 默认数据库连接，查询默认数据库相关信息（不映射数据库)
        /// </summary>
        [Ignore]
        public string DbDefaultConnectString => $"server={ServerAddress},{ServerPort};database={DefaultDatabase};uid={UserName};pwd={EncryptHelper.Decode(Password)};";
    }
}