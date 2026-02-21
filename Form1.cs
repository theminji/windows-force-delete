using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ForceDeleteApp
{
    public partial class Form1 : Form
    {
        string targetPath;
        List<Process> lockingProcesses;
        ListBox lstProcesses;
        Button btnKillTasks;
        Button btnForceDelete;
        Label lblInfo;

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(string path)
        {
            targetPath = path;
            InitializeComponentCustom();
            LoadLockingProcesses();
        }

        private void InitializeComponentCustom()
        {
            this.Text = "Force Delete";
            this.Size = new Size(500, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            try { this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }

            lblInfo = new Label();
            lblInfo.Text = "Target: " + targetPath + "\n\nChecking processes...";
            lblInfo.Location = new Point(10, 10);
            lblInfo.Size = new Size(460, 65);

            lstProcesses = new ListBox();
            lstProcesses.Location = new Point(10, 85);
            lstProcesses.Size = new Size(460, 155);

            btnKillTasks = new Button();
            btnKillTasks.Text = "Kill Locking Tasks";
            btnKillTasks.Location = new Point(10, 260);
            btnKillTasks.Size = new Size(150, 30);
            btnKillTasks.Click += BtnKillTasks_Click;

            btnForceDelete = new Button();
            btnForceDelete.Text = "Force Delete";
            btnForceDelete.Location = new Point(170, 260);
            btnForceDelete.Size = new Size(150, 30);
            btnForceDelete.Click += BtnForceDelete_Click;

            Button btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(330, 260);
            btnCancel.Size = new Size(140, 30);
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.Add(lblInfo);
            this.Controls.Add(lstProcesses);
            this.Controls.Add(btnKillTasks);
            this.Controls.Add(btnForceDelete);
            this.Controls.Add(btnCancel);
        }

        private void LoadLockingProcesses()
        {
            lstProcesses.Items.Clear();
            try
            {
                lockingProcesses = FileUtil.WhoIsLocking(targetPath);
                if (lockingProcesses == null || lockingProcesses.Count == 0)
                {
                    lblInfo.Text = "Target: " + targetPath + "\n\nNo processes found locking this file/folder.";
                    lstProcesses.Items.Add("None");
                }
                else
                {
                    lblInfo.Text = "Target: " + targetPath + "\n\nThe following processes are locking it:";
                    foreach (var p in lockingProcesses)
                    {
                        try
                        {
                            lstProcesses.Items.Add($"{p.ProcessName} (PID: {p.Id}) - {p.MainModule?.FileName}");
                        }
                        catch
                        {
                            lstProcesses.Items.Add($"{p.ProcessName} (PID: {p.Id})");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblInfo.Text = "Error finding locking processes.";
                // MessageBox.Show(ex.Message, "Error");
            }
        }

        private void BtnKillTasks_Click(object sender, EventArgs e)
        {
            if (lockingProcesses == null || lockingProcesses.Count == 0)
            {
                return;
            }

            foreach (var p in lockingProcesses)
            {
                try
                {
                    if (!p.HasExited)
                    {
                        p.Kill();
                        p.WaitForExit(1000);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not kill {p.ProcessName}: {ex.Message}");
                }
            }
            LoadLockingProcesses();
        }

        private void BtnForceDelete_Click(object sender, EventArgs e)
        {
            // First kill
            BtnKillTasks_Click(null, null);

            // Now delete
            try
            {
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                else if (Directory.Exists(targetPath))
                {
                    Directory.Delete(targetPath, true);
                }
                else
                {
                    MessageBox.Show("File/folder already deleted or not found.", "Info");
                    this.Close();
                    return;
                }
                MessageBox.Show("Successfully deleted!", "Success");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not delete. Make sure it's not locked by another process or you have permissions.\n\n" + ex.Message, "Error");
                LoadLockingProcesses();
            }
        }
    }
}
