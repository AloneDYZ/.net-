﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HandyControl.Controls;
using HandyControl.Data;
using SmartCode.Framework;
using SmartCode.Framework.Exporter;
using SmartCode.Framework.PhysicalDataModel;
using SmartCode.Framework.SqliteModel;
using SmartCode.Tool.Annotations;

namespace SmartCode.Tool.Views
{
    //定义委托
    public delegate void ConnectChangeRefreshHandler(ConnectConfigs connectConfig);
    /// <summary>
    /// GroupManage.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectManage : INotifyPropertyChanged
    {
        public event ConnectChangeRefreshHandler ChangeRefreshEvent;
        public ConnectManage()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region DependencyProperty
        public static readonly DependencyProperty ConnectionProperty = DependencyProperty.Register(
            "Connection", typeof(DataBasesConfig), typeof(ConnectManage), new PropertyMetadata(default(DataBasesConfig)));
        public DataBasesConfig Connection
        {
            get => (DataBasesConfig)GetValue(ConnectionProperty);
            set => SetValue(ConnectionProperty, value);
        }

        public static readonly DependencyProperty SelectedDataBaseProperty = DependencyProperty.Register(
            "SelectedDataBase", typeof(string), typeof(ConnectManage), new PropertyMetadata(default(string)));
        public string SelectedDataBase
        {
            get => (string)GetValue(SelectedDataBaseProperty);
            set => SetValue(SelectedDataBaseProperty, value);
        }

        public new static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(ConnectManage), new PropertyMetadata(default(string)));
        public new string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty DataListProperty = DependencyProperty.Register(
            "DataList", typeof(List<ConnectConfigs>), typeof(ConnectManage), new PropertyMetadata(default(List<ConnectConfigs>)));
        public List<ConnectConfigs> DataList
        {
            get => (List<ConnectConfigs>)GetValue(DataListProperty);
            set
            {
                SetValue(DataListProperty, value);
                OnPropertyChanged(nameof(DataList));
            }
        }
        #endregion

        private void GroupManage_OnLoaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                var sqLiteHelper = new SQLiteHelper();
                var datalist = sqLiteHelper.db.Table<ConnectConfigs>().ToList();
                Dispatcher.Invoke(() =>
                {
                    DataList = datalist;
                });
                IExporter exporter = new SqlServer2008Exporter();
            });
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = (ListBox)sender;
            if (listBox.SelectedItems.Count > 0)
            {
                var connect = (ConnectConfigs)listBox.SelectedItems[0];
                HidId.Text = connect.ID.ToString();
                TextConnectName.Text = connect.ConnectName;
                TextServerAddress.Text = connect.ServerAddress;
                TextServerPort.Value = connect.ServerPort;
                TextServerName.Text = connect.UserName;
                TextServerPassword.Password = connect.Password;
                ComboAuthentication.SelectedItem = connect.Authentication == 0 ? SQLServer : Windows;
                TextDefaultDataBase.Text = connect.DefaultDatabase;
                BtnDelete.Visibility = Visibility.Visible;
                BtnConnect.IsEnabled = true;
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (!CheckConnectForm())
            {
                return;
            }
            var connectId = Convert.ToInt32(HidId.Text);
            var connectName = TextConnectName.Text.Trim();
            var serverAddress = TextServerAddress.Text.Trim();
            var serverPort = TextServerPort.Value;
            var authentication = ComboAuthentication.SelectedValue == SQLServer ? 1 : 0;
            var userName = TextServerName.Text.Trim();
            var password = TextServerPassword.Password.Trim();
            var defaultDataBase = TextDefaultDataBase.Text.Trim();
            var sqLiteHelper = new SQLiteHelper();
            var connectConfig = new ConnectConfigs();
            if (connectId > 0)
            {
                connectConfig = sqLiteHelper.db.Table<ConnectConfigs>().FirstOrDefault(x => x.ID == connectId);
                if (connectConfig == null)
                {
                    Growl.Warning(new GrowlInfo { Message = $"当前连接不存在或已被删除", WaitTime = 1, ShowDateTime = false });
                    return;
                }
                var connectAny = sqLiteHelper.db.Table<ConnectConfigs>().FirstOrDefault(x => x.ConnectName == connectName && x.ID != connectId);
                if (connectAny != null)
                {
                    Growl.Warning(new GrowlInfo { Message = $"已存在相同名称的连接名", WaitTime = 1, ShowDateTime = false });
                    return;
                }
                connectConfig.ConnectName = connectName;
                connectConfig.ServerAddress = serverAddress;
                connectConfig.ServerPort = Convert.ToInt32(serverPort);
                connectConfig.UserName = userName;
                connectConfig.Password = password;
                connectConfig.DefaultDatabase = defaultDataBase;
                connectConfig.Authentication = authentication;
                sqLiteHelper.db.Update(connectConfig);
            }
            else
            {
                var connect = sqLiteHelper.db.Table<ConnectConfigs>().FirstOrDefault(x => x.ConnectName.ToLower() == connectName.ToLower());
                if (connect != null)
                {
                    Growl.Warning(new GrowlInfo { Message = $"已存在相同名称的连接名", WaitTime = 1, ShowDateTime = false });
                    return;
                }
                connectConfig = new ConnectConfigs()
                {
                    ConnectName = connectName,
                    ServerAddress = serverAddress,
                    ServerPort = Convert.ToInt32(serverPort),
                    Authentication = authentication,
                    UserName = userName,
                    Password = password,
                    CreateDate = 1212,
                    DefaultDatabase = defaultDataBase
                };
                sqLiteHelper.db.Insert(connectConfig);
            }
            Task.Run(() =>
            {
                var datalist = sqLiteHelper.db.Table<ConnectConfigs>().
                    ToList();
                Dispatcher.Invoke(() =>
                {
                    DataList = datalist;
                    if (ChangeRefreshEvent != null)
                    {
                        ChangeRefreshEvent(connectConfig);
                        this.Close();
                    }
                });
            });
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDelete_OnClick(object sender, RoutedEventArgs e)
        {
            var sqLiteHelper = new SQLiteHelper();
            var connectId = Convert.ToInt32(HidId.Text);
            if (connectId < 1)
            {
                return;
            }
            Task.Run(() =>
            {
                sqLiteHelper.db.Delete<ConnectConfigs>(connectId);
                var datalist = sqLiteHelper.db.Table<ConnectConfigs>().
                   ToList();
                Dispatcher.Invoke(() =>
                {
                    ResetData();
                    DataList = datalist;
                    if (ChangeRefreshEvent != null)
                    {
                        //ChangeRefreshEvent();
                    }
                });
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 添加重置表单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReset_OnClick(object sender, RoutedEventArgs e)
        {
            if (HidId.Text == "0")
            {

            }
            ResetData();
        }

        private void ResetData()
        {
            BtnDelete.Visibility = Visibility.Hidden;
            HidId.Text = "0";
            TextConnectName.Text = "";
            TextServerAddress.Text = "";
            TextServerPort.Value = 1433;
            TextServerName.Text = "";
            TextServerPassword.Password = "";
            ComboAuthentication.SelectedItem = SQLServer;
            TextDefaultDataBase.Text = "master";
            ListConnects.SelectedItem = null;
            BtnConnect.IsEnabled = false;
            BtnTestConnect.IsEnabled = false;
        }

        /// <summary>
        /// 文本框实时监测
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Text_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TextConnectName.Text.Trim()) && !string.IsNullOrEmpty(TextServerAddress.Text.Trim()) && !string.IsNullOrEmpty(TextServerName.Text.Trim()) && !string.IsNullOrEmpty(TextServerPassword.Password.Trim()) && !string.IsNullOrEmpty(TextDefaultDataBase.Text.Trim()))
            {
                BtnConnect.IsEnabled = true;
                BtnTestConnect.IsEnabled = true;
            }
        }

        /// <summary>
        /// 密码框实时监测
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextServerPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TextConnectName.Text.Trim()) && !string.IsNullOrEmpty(TextServerAddress.Text.Trim()) && !string.IsNullOrEmpty(TextServerName.Text.Trim()) && !string.IsNullOrEmpty(TextServerPassword.Password.Trim()) && !string.IsNullOrEmpty(TextDefaultDataBase.Text.Trim()))
            {
                BtnConnect.IsEnabled = true;
                BtnTestConnect.IsEnabled = true;
            }
        }

        /// <summary>
        /// 校验表单数据
        /// </summary>
        private bool CheckConnectForm()
        {
            #region MyRegion
            var connectName = TextConnectName.Text.Trim();
            var serverAddress = TextServerAddress.Text.Trim();
            var serverPort = TextServerPort.Value;
            var authentication = ComboAuthentication.SelectedValue == SQLServer ? 1 : 0;
            var userName = TextServerName.Text.Trim();
            var password = TextServerPassword.Password.Trim();
            var defaultDataBase = TextDefaultDataBase.Text.Trim();
            if (string.IsNullOrEmpty(connectName))
            {
                Growl.Warning(new GrowlInfo { Message = $"请填写连接名称", WaitTime = 1, ShowDateTime = false });
                return false;
            }
            if (string.IsNullOrEmpty(serverAddress))
            {
                Growl.Warning(new GrowlInfo { Message = $"请填写服务器地址", WaitTime = 1, ShowDateTime = false });
                return false;
            }
            if (serverPort < 1)
            {
                Growl.Warning(new GrowlInfo { Message = $"请填写端口号", WaitTime = 1, ShowDateTime = false });
                return false;
            }
            if (string.IsNullOrEmpty(userName))
            {
                Growl.Warning(new GrowlInfo { Message = $"请填写用户名", WaitTime = 1, ShowDateTime = false });
                return false;
            }
            if (string.IsNullOrEmpty(password))
            {
                Growl.Warning(new GrowlInfo { Message = $"请填写密码", WaitTime = 1, ShowDateTime = false });
                return false;
            }
            if (string.IsNullOrEmpty(defaultDataBase))
            {
                Growl.Warning(new GrowlInfo {Message = $"请填写默认数据库", WaitTime = 1, ShowDateTime = false});
                return false;
            }
            return true;
            #endregion
        }

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnTestConnect_OnClick(object sender, RoutedEventArgs e)
        {
            if (!CheckConnectForm())
            {
                return;
            }
            var connectionString = $"server={TextServerAddress.Text.Trim()},{TextServerPort.Value};database=master;uid={TextServerName.Text.Trim()};pwd={TextServerPassword.Password.Trim()};";
            PageLoading.Visibility = Visibility.Visible;
            BtnTestConnect.IsEnabled = false;
            Task.Run(() =>
            {
                try
                {
                    IExporter exporter = new SqlServer2008Exporter();
                    exporter.GetDatabases(connectionString);
                    Dispatcher.Invoke(() =>
                    {
                        PageLoading.Visibility = Visibility.Collapsed;
                        BtnTestConnect.IsEnabled = true;
                        Growl.Success(new GrowlInfo { Message = $"连接成功", WaitTime = 1, ShowDateTime = false });
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        PageLoading.Visibility = Visibility.Collapsed;
                        BtnTestConnect.IsEnabled = true;
                        Growl.Warning(new GrowlInfo { Message = $"连接失败\r" + ex.Message, WaitTime = 1, ShowDateTime = false });
                    });
                }
            });
        }
    }
}