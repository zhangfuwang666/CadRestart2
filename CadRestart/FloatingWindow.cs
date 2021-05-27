using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CadRestart
{
    public partial class FloatingWindow : Form
    {
        private bool leftFlag;
        private Point mouseOff;
        private bool show=true;
        public FloatingWindow()
        {
            InitializeComponent();
        }
        public List<string> GetAllCadPath()
        {

            List<string> AllCadPath = new List<string>();
            List<string> Locations = new List<string>();
            var locaMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
            RegistryKey KeyAcad = locaMachine.OpenSubKey("Software\\Autodesk\\AutoCAD");

            string[] CadVersions = KeyAcad.GetSubKeyNames();
            foreach (var cadVersion in CadVersions)
            {
                //打开特定版本的CAD注册表
                RegistryKey keyCadVersion = KeyAcad.OpenSubKey(cadVersion);
                //获取表示个语言版本的AutoCAD注册表键值
                string[] cadNames = keyCadVersion.GetSubKeyNames();
                foreach (var cadName in cadNames)
                {
                    if (cadName.EndsWith("804"))//中文版本
                    {
                        //打开中文版本CAD所有的注册表键
                        RegistryKey KeyCadName = keyCadVersion.OpenSubKey(cadName);

                        try
                        {
                            if (KeyCadName.GetValue("AcadLocation") != null)
                            {
                                string value = KeyCadName.GetValue("AcadLocation").ToString();
                                AllCadPath.Add(value);

                            }

                        }
                        catch (Exception)
                        {

                            throw;
                        }

                    }
                }

            }
            return AllCadPath;
        }

        private void restart_Click(object sender, EventArgs e)
        {
            foreach (var pro in Process.GetProcesses())
            {

                if (pro.ProcessName.ToLower().StartsWith("acad"))//关闭Cad进程
                {

                    pro.Kill();

                }

            }
            var AllCadPath = GetAllCadPath();
            foreach (var CadPath in AllCadPath)
            {
                //启动软件
                string exePath = System.IO.Path.Combine(CadPath, "acad.exe");
                var info = new System.Diagnostics.ProcessStartInfo(exePath);
                info.UseShellExecute = true;
                info.WorkingDirectory = CadPath;// exePath.Substring(0, exePath.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
                System.Diagnostics.Process.Start(info);
            }
        }

        private void closed_Click(object sender, EventArgs e)
        {
            foreach (var pro in Process.GetProcesses())
            {

                if (pro.ProcessName.ToLower().StartsWith("acad"))//关闭Cad进程
                {

                    pro.Kill();

                }

            }
        }

        private void quit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOff = new Point(-e.X, -e.Y); //得到变量的值
                leftFlag = true;                  //点击左键按下时标注为true;
            }
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                leftFlag = false;//释放鼠标后标注为false;
            }
        }

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                Point mouseSet = Control.MousePosition;
                mouseSet.Offset(mouseOff.X, mouseOff.Y); //设置移动后的位置
                Location = mouseSet;
            }
        }

        private void manage_Click(object sender, EventArgs e)
        {
            FormAutoLoad formAutoLoad = new FormAutoLoad();
            formAutoLoad.Show();
        }

        private void hide_Click(object sender, EventArgs e)
        {
            if (show)
            {
                Hide();
                show = false;
            }
            else
            {
                Show();
                show = true;
            }
           
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
        }

        private void setting_Click(object sender, EventArgs e)
        {
            SheZhi sheZhi = new SheZhi();
            sheZhi.ShowDialog();
        }
    }
}
