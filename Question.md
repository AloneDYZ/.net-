
<div align="center">
<h2>常见问题❓</h2>
</div>

- **1️⃣连接不上，怎么办？**
	
>	1. `连接数据库`界面填写的`连接信息`真的正确无误?
>	2. `数据库服务器`有`防火墙/安全组`限制？
>	3. 用 [Navicat Premium](https://gitee.com/dotnetchina/DBCHM/attach_files) 连接数据库服务器试试！
	
- **2️⃣连接数据库时，点了 `连接/测试` ，半天没响应？**
	
>	可能是连接远程数据库网络不好的原因，可以把`连接超时`设置的小一些。
	
- **3️⃣SmartSQL可以连接上，但显示不了数据怎么办？**
>	- 导出文档前，数据库使用账号要给予`root级别`的权限，非root级别账号连接，可能会出现`表数据显示不全`或数据查询因权限不足，会`查不出来数据`！
>	- SmartSQL有Bug， [提Issue](https://gitee.com/dotnetchina/SmartSQL/issues/new) 反馈。
	
- **4️⃣表列的批注数据我想迁移，怎么办？**
>	1. 使用 SmartSQL 的 `XML导出`，对当前数据库的批注数据 就会导出一个xml文件。
>	2. 点`数据连接`， 切换至 目标数据库连
>	3. 再用`导入备注` 就可以选择刚刚的xml文件，如果数据库表结构相同，批注就会更新到目标数据库服上。
	
- **5️⃣数据库比较老，如  `Sql Server 2000 `，怎么使用SmartSQL？**
>	1. 下载安装 [Navicat Premium](https://gitee.com/dotnetchina/SmartSQL/attach_files)
>	2. 连接上老旧的数据库服务器，将数据库表结构脚本导出。
>	3. 找一台高版本的数据库服务器，新建一个临时数据库，将导出的脚本导入。
>	4. 然后用SmartSQL连接高版本的数据库服务器。
	
- **6️⃣chm文件可以正常导出，但是文件名中文乱码，打开显示 无法访问此页**
	
> 	这种情况，有一种可能是win系统的**区域设置**，勾选了；取消勾选后，可能不存在该问题。

- **7️⃣PostgreSql连接出现下面连接错误怎么办？**

![](https://gitee.com/izhaofu/SmartSQL/raw/master/Img/question/question_postgresql_connect.png)

原因：

> 这是在远程连接时pg_hba.conf文件没有配置正确。

解决方法：

>   1、找到`pg_hba.conf`文件（在Postgre安装文件目录下的data文件夹中）

>   2、打开`pg_hba.conf`文件，并在文件末尾添加

```
host    all             all              0.0.0.0/0              md5
```

> 3、重启postgresql的服务，应该就可以连了。

	
- **其他问题**
	
>	如遇其他问题，可以通过Issues反馈，记录问题，请写清楚遇到问题的原因、软件版本、系统环境、数据库版本、甚至数据库结构、复现步骤以及期望达到的效果；建议配上多张全屏大图，请勿使用局部截屏小图！方便我们这边可以迅速定位，解决问题。