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
    public partial class Main : Form
    {
        private System.Timers.Timer timer = new System.Timers.Timer();
        public Form1()
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
    }
}
