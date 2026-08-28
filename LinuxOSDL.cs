using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Drawing;

namespace LinuxOSDL
{
    public partial class LinuxOSDL : Form
    {
        private WebClient? webClient;
        private string currentFilePath = string.Empty;
        private Form? downloadDialog;
        private Label? statusLabel;
        private Button? cancelButton;
        private bool isCancelled = false;

        public LinuxOSDL()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> debianVersions = new Dictionary<string, string>();
            debianVersions.Add("Debian 13.6", "https://cdimage.debian.org/debian-cd/current/amd64/iso-cd/debian-13.6.0-amd64-netinst.iso");
            debianVersions.Add("Debian 11.11 (Bullseye)", "https://cdimage.debian.org/cdimage/archive/11.11.0/amd64/iso-cd/debian-11.11.0-amd64-netinst.iso");
            debianVersions.Add("Debian Testing", "https://cdimage.debian.org/cdimage/weekly-builds/amd64/iso-cd/debian-testing-amd64-netinst.iso");
            

            ShowVersionDialog("Debian", debianVersions);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> gentooVersions = new Dictionary<string, string>();
            gentooVersions.Add("Gentoo AMD64 Stage3 (SystemD)", "https://bouncer.gentoo.org/fetch/root/all/releases/amd64/autobuilds/current-stage3-amd64-systemd/stage3-amd64-systemd-20250828T170108Z.tar.xz");
            gentooVersions.Add("Gentoo AMD64 Stage3 (OpenRC)", "https://bouncer.gentoo.org/fetch/root/all/releases/amd64/autobuilds/current-stage3-amd64-openrc/stage3-amd64-openrc-20250828T170108Z.tar.xz");
            gentooVersions.Add("Gentoo AMD64 Minimal ISO", "https://bouncer.gentoo.org/fetch/root/all/releases/amd64/autobuilds/current-install-amd64-minimal/install-amd64-minimal-20250828T170108Z.iso");

            ShowVersionDialog("Gentoo", gentooVersions);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> archVersions = new Dictionary<string, string>();
            archVersions.Add("Arch Linux (Latest ISO)", "https://mirror.rackspace.com/archlinux/iso/latest/archlinux-2026.08.01-x86_64.iso");
            archVersions.Add("Arch Linux (Previous ISO)", "https://mirror.rackspace.com/archlinux/iso/2026.07.01/archlinux-2026.07.01-x86_64.iso");
            archVersions.Add("Arch Linux (Bootstrap)", "https://mirror.rackspace.com/archlinux/iso/latest/archlinux-bootstrap-2026.08.01-x86_64.tar.gz");

            ShowVersionDialog("Arch Linux", archVersions);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> distroVersions = new Dictionary<string, string>();
            distroVersions.Add("Ubuntu 24.04.4 LTS Desktop", "https://releases.ubuntu.com/noble/ubuntu-24.04.4-desktop-amd64.iso");
            distroVersions.Add("Ubuntu 24.04.4 LTS Server", "https://releases.ubuntu.com/noble/ubuntu-24.04.4-live-server-amd64.iso");
            distroVersions.Add("Ubuntu 22.04.5 LTS Desktop", "https://releases.ubuntu.com/jammy/ubuntu-22.04.5-desktop-amd64.iso");
            distroVersions.Add("Linux Mint 22.3 Cinnamon", "https://mirrors.kernel.org/linuxmint/stable/22.3/linuxmint-22.3-cinnamon-64bit.iso");
            distroVersions.Add("Linux Mint 22.3 MATE", "https://mirrors.kernel.org/linuxmint/stable/22.3/linuxmint-22.3-mate-64bit.iso");
            distroVersions.Add("Linux Mint 22.3 Xfce", "https://mirrors.kernel.org/linuxmint/stable/22.3/linuxmint-22.3-xfce-64bit.iso");
            distroVersions.Add("Fedora 44 Workstation", "https://download.fedoraproject.org/pub/fedora/linux/releases/44/Workstation/x86_64/iso/Fedora-Workstation-Live-x86_64-44-1.14.iso");
            distroVersions.Add("Fedora 44 Server", "https://download.fedoraproject.org/pub/fedora/linux/releases/44/Server/x86_64/iso/Fedora-Server-dvd-x86_64-44-1.14.iso");
            distroVersions.Add("Pop!_OS 24.04 LTS (Intel/AMD)", "https://iso.pop-os.org/24.04/amd64/intel/10/pop-os_24.04_amd64_intel_10.iso");
            distroVersions.Add("Pop!_OS 24.04 LTS (NVIDIA)", "https://iso.pop-os.org/24.04/amd64/nvidia/10/pop-os_24.04_amd64_nvidia_10.iso");

            ShowVersionDialog("Distribution Based", distroVersions);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string downloadsFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            downloadsFolder = Path.Combine(downloadsFolder, "Downloads");

            MessageBox.Show(
                "Every download is saved in the Downloads folder.\n\n" +
                "The download will be saved to:\n" + downloadsFolder + "\n\n" +
                "Note: Some download links may change over time.\n" +
                "If a link fails, visit the distribution's official website.\n\n" +
                "If you want me to add a distro, feel free to make an issue on GitHub.",
                "Download Information",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void ShowVersionDialog(string distroName, Dictionary<string, string> versions)
        {
            Form versionDialog = new Form();
            versionDialog.Text = "Select " + distroName + " Version";
            versionDialog.Width = 400;
            versionDialog.Height = 300;
            versionDialog.StartPosition = FormStartPosition.CenterParent;
            versionDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            versionDialog.MaximizeBox = false;
            versionDialog.MinimizeBox = false;

            ListBox versionList = new ListBox();
            versionList.Dock = DockStyle.Fill;
            versionList.Items.Add("Select a version...");

            foreach (string versionName in versions.Keys)
            {
                versionList.Items.Add(versionName);
            }
            versionList.SelectedIndex = 0;

            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 50;

            Button downloadButton = new Button();
            downloadButton.Text = "Download";
            downloadButton.Dock = DockStyle.Right;
            downloadButton.Width = 100;
            downloadButton.Height = 40;
            downloadButton.Margin = new Padding(5);

            Button cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.Dock = DockStyle.Right;
            cancelButton.Width = 100;
            cancelButton.Height = 40;
            cancelButton.Margin = new Padding(5);

            downloadButton.Click += delegate (object? obj, EventArgs args)
            {
                if (versionList.SelectedIndex > 0)
                {
                    string selectedVersion = versionList.SelectedItem!.ToString()!;
                    if (versions.TryGetValue(selectedVersion, out string? downloadUrl) && !string.IsNullOrEmpty(downloadUrl))
                    {
                        versionDialog.DialogResult = DialogResult.OK;
                        versionDialog.Close();
                        StartDownload(selectedVersion, downloadUrl);
                    }
                    else
                    {
                        MessageBox.Show("Download URL not found for the selected version.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a version to download.", "Selection Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            cancelButton.Click += delegate (object? obj, EventArgs args)
            {
                versionDialog.DialogResult = DialogResult.Cancel;
                versionDialog.Close();
            };

            buttonPanel.Controls.Add(downloadButton);
            buttonPanel.Controls.Add(cancelButton);
            versionDialog.Controls.Add(versionList);
            versionDialog.Controls.Add(buttonPanel);

            versionDialog.ShowDialog();
        }

        private void StartDownload(string versionName, string downloadUrl)
        {
            isCancelled = false;

            // Create a simple download dialog
            downloadDialog = new Form();
            downloadDialog.Text = "Downloading: " + versionName;
            downloadDialog.Width = 400;
            downloadDialog.Height = 150;
            downloadDialog.StartPosition = FormStartPosition.CenterParent;
            downloadDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            downloadDialog.MaximizeBox = false;
            downloadDialog.MinimizeBox = false;

            TableLayoutPanel mainPanel = new TableLayoutPanel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Padding = new Padding(20);
            mainPanel.RowCount = 2;
            mainPanel.ColumnCount = 1;
            mainPanel.RowStyles.Clear();
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40));

            statusLabel = new Label();
            statusLabel.Text = "Downloading " + versionName + "...";
            statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            statusLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            statusLabel.Dock = DockStyle.Fill;

            cancelButton = new Button();
            cancelButton.Text = "Cancel Download";
            cancelButton.Dock = DockStyle.Fill;
            cancelButton.Font = new Font("Arial", 10);
            cancelButton.BackColor = Color.LightCoral;
            cancelButton.FlatStyle = FlatStyle.Flat;

            mainPanel.Controls.Add(statusLabel, 0, 0);
            mainPanel.Controls.Add(cancelButton, 0, 1);

            downloadDialog.Controls.Add(mainPanel);

            // Set up the download
#pragma warning disable SYSLIB0014 // WebClient is obsolete
            webClient = new WebClient();
#pragma warning restore SYSLIB0014

            string downloadsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads"
            );

            if (!Directory.Exists(downloadsPath))
            {
                Directory.CreateDirectory(downloadsPath);
            }

            string fileName = Path.GetFileName(new Uri(downloadUrl).LocalPath);
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = versionName.Replace(" ", "_") + ".iso";
            }

            currentFilePath = Path.Combine(downloadsPath, fileName);

            // Setup event handlers
            if (webClient != null)
            {
                webClient.DownloadFileCompleted += OnDownloadFileCompleted;
            }

            if (cancelButton != null)
            {
                cancelButton.Click += OnCancelButtonClick;
            }

            if (downloadDialog != null)
            {
                downloadDialog.FormClosing += OnDownloadDialogClosing;
            }

            // Show the dialog
            if (downloadDialog != null)
            {
                downloadDialog.Show(this);
            }

            // Start the download
            try
            {
                if (webClient != null)
                {
                    webClient.DownloadFileAsync(new Uri(downloadUrl), currentFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting download: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (downloadDialog != null)
                {
                    downloadDialog.Close();
                }
            }
        }

        private void OnDownloadFileCompleted(object? sender, AsyncCompletedEventArgs e)
        {
            if (downloadDialog != null && downloadDialog.InvokeRequired)
            {
                downloadDialog.Invoke(new Action(() => HandleDownloadCompleted(e)));
            }
            else
            {
                HandleDownloadCompleted(e);
            }
        }

        private void HandleDownloadCompleted(AsyncCompletedEventArgs e)
        {
            if (e.Cancelled || isCancelled)
            {
                if (statusLabel != null)
                {
                    statusLabel.Text = "Download Cancelled!";
                    statusLabel.ForeColor = Color.Red;
                }

                if (cancelButton != null)
                {
                    cancelButton.Text = "Close";
                    cancelButton.BackColor = Color.LightGray;
                    cancelButton.Click -= OnCancelButtonClick;
                    cancelButton.Click += delegate (object? obj, EventArgs args)
                    {
                        if (downloadDialog != null)
                        {
                            downloadDialog.Close();
                        }
                    };
                }

                MessageBox.Show("Download cancelled.", "Cancelled",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Delete partial file if it exists
                if (File.Exists(currentFilePath))
                {
                    try
                    {
                        File.Delete(currentFilePath);
                    }
                    catch { }
                }
            }
            else if (e.Error != null)
            {
                if (statusLabel != null)
                {
                    statusLabel.Text = "Download Failed!";
                    statusLabel.ForeColor = Color.Red;
                }

                if (cancelButton != null)
                {
                    cancelButton.Text = "Close";
                    cancelButton.BackColor = Color.LightGray;
                    cancelButton.Click -= OnCancelButtonClick;
                    cancelButton.Click += delegate (object? obj, EventArgs args)
                    {
                        if (downloadDialog != null)
                        {
                            downloadDialog.Close();
                        }
                    };
                }

                MessageBox.Show("Download failed: " + e.Error.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (statusLabel != null)
                {
                    statusLabel.Text = "Download Complete!";
                    statusLabel.ForeColor = Color.Green;
                }

                if (cancelButton != null)
                {
                    cancelButton.Text = "Close";
                    cancelButton.BackColor = Color.LightGreen;
                    cancelButton.Click -= OnCancelButtonClick;
                    cancelButton.Click += delegate (object? obj, EventArgs args)
                    {
                        if (downloadDialog != null)
                        {
                            downloadDialog.Close();
                        }
                    };
                }

                DialogResult result = MessageBox.Show(
                    "Download completed!\nSaved to: " + currentFilePath +
                    "\n\nWould you like to open the folder?",
                    "Success",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                if (result == DialogResult.Yes)
                {
                    if (!string.IsNullOrEmpty(currentFilePath))
                    {
                        string? directory = Path.GetDirectoryName(currentFilePath);
                        if (!string.IsNullOrEmpty(directory))
                        {
                            Process.Start("explorer.exe", directory);
                        }
                    }
                }
            }
        }

        private void OnCancelButtonClick(object? sender, EventArgs e)
        {
            if (webClient != null && webClient.IsBusy)
            {
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to cancel the download?",
                    "Confirm Cancel",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    isCancelled = true;
                    webClient.CancelAsync();

                    if (statusLabel != null)
                    {
                        statusLabel.Text = "Cancelling...";
                        statusLabel.ForeColor = Color.Orange;
                    }
                }
            }
            else
            {
                if (downloadDialog != null)
                {
                    downloadDialog.Close();
                }
            }
        }

        private void OnDownloadDialogClosing(object? sender, FormClosingEventArgs e)
        {
            if (webClient != null && webClient.IsBusy)
            {
                DialogResult result = MessageBox.Show(
                    "Download in progress. Are you sure you want to close?",
                    "Confirm Close",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    isCancelled = true;
                    webClient.CancelAsync();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
    }
}