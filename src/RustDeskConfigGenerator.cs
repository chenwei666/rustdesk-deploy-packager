using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

public class RustDeskConfigGenerator : Form
{
    private readonly TextBox idServer = new TextBox();
    private readonly TextBox relayServer = new TextBox();
    private readonly TextBox apiServer = new TextBox();
    private readonly TextBox key = new TextBox();
    private readonly TextBox password = new TextBox();
    private readonly TextBox installer = new TextBox();
    private readonly TextBox output = new TextBox();
    private readonly TextBox log = new TextBox();
    private readonly Label title = new Label();
    private readonly Label subtitle = new Label();
    private readonly Label[] labels = new Label[7];
    private readonly ComboBox language = new ComboBox();
    private readonly Button browseInstaller = new Button();
    private readonly Button latest = new Button();
    private readonly Button browseOutput = new Button();
    private readonly Button build = new Button();
    private readonly Label status = new Label();
    private bool zh = false;

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new RustDeskConfigGenerator());
    }

    public RustDeskConfigGenerator()
    {
        Text = "RustDesk Config EXE Generator";
        Width = 980;
        Height = 680;
        MinimumSize = new Size(980, 680);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9F);
        BackColor = Color.FromArgb(245, 247, 250);

        try
        {
            string icon = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RustDeskConfigGenerator.ico");
            if (File.Exists(icon)) Icon = new Icon(icon);
        }
        catch { }

        BuildLayout();
        ApplyLanguage();
    }

    private void BuildLayout()
    {
        Panel sidebar = new Panel { Left = 0, Top = 0, Width = 275, Height = ClientSize.Height, Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom, BackColor = Color.FromArgb(20, 88, 143) };
        Controls.Add(sidebar);

        Label mark = new Label { Text = "RD", Left = 32, Top = 34, Width = 64, Height = 64, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 22F, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.FromArgb(39, 174, 229) };
        sidebar.Controls.Add(mark);

        title.Left = 32; title.Top = 120; title.Width = 220; title.Height = 72;
        title.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        title.ForeColor = Color.White;
        sidebar.Controls.Add(title);

        subtitle.Left = 32; subtitle.Top = 205; subtitle.Width = 220; subtitle.Height = 110;
        subtitle.Font = new Font("Segoe UI", 9.5F);
        subtitle.ForeColor = Color.FromArgb(210, 232, 246);
        sidebar.Controls.Add(subtitle);

        Label footer = new Label { Text = "Open-source friendly\nNo embedded defaults", Left = 32, Top = 565, Width = 220, Height = 60, ForeColor = Color.FromArgb(190, 220, 238) };
        sidebar.Controls.Add(footer);

        Panel content = new Panel { Left = 305, Top = 28, Width = 630, Height = 590, Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom, BackColor = Color.White };
        content.Paint += delegate(object s, PaintEventArgs e) { ControlPaint.DrawBorder(e.Graphics, content.ClientRectangle, Color.FromArgb(225, 230, 236), ButtonBorderStyle.Solid); };
        Controls.Add(content);

        language.DropDownStyle = ComboBoxStyle.DropDownList;
        language.Items.AddRange(new object[] { "English", "中文" });
        language.SelectedIndex = 0;
        language.Left = 492; language.Top = 22; language.Width = 105;
        language.SelectedIndexChanged += delegate { zh = language.SelectedIndex == 1; ApplyLanguage(); };
        content.Controls.Add(language);

        int y = 70;
        AddField(content, 0, idServer, y); y += 48;
        AddField(content, 1, relayServer, y); y += 48;
        AddField(content, 2, apiServer, y); y += 48;
        AddField(content, 3, key, y); y += 48;
        AddField(content, 4, password, y); y += 48;
        AddFileField(content, 5, installer, browseInstaller, latest, y); y += 48;
        AddFileField(content, 6, output, browseOutput, null, y); y += 64;

        StyleButton(build, Color.FromArgb(20, 126, 214), Color.White);
        build.Left = 402; build.Top = y; build.Width = 195; build.Height = 38;
        build.Click += delegate { BuildPackage(); };
        content.Controls.Add(build);

        status.Left = 30; status.Top = y + 7; status.Width = 350; status.Height = 24;
        status.ForeColor = Color.FromArgb(80, 92, 108);
        content.Controls.Add(status);

        log.Left = 30; log.Top = y + 55; log.Width = 567; log.Height = 155;
        log.Multiline = true; log.ScrollBars = ScrollBars.Vertical; log.ReadOnly = true;
        log.BorderStyle = BorderStyle.FixedSingle;
        log.BackColor = Color.FromArgb(248, 250, 252);
        content.Controls.Add(log);

        output.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "RustDesk-Custom-Setup.exe");
    }

    private void AddField(Control parent, int index, TextBox box, int y)
    {
        Label label = new Label { Left = 30, Top = y + 7, Width = 145, Height = 24, ForeColor = Color.FromArgb(38, 48, 63) };
        labels[index] = label;
        parent.Controls.Add(label);
        StyleTextBox(box);
        box.Left = 185; box.Top = y; box.Width = 412;
        parent.Controls.Add(box);
    }

    private void AddFileField(Control parent, int index, TextBox box, Button browse, Button extra, int y)
    {
        Label label = new Label { Left = 30, Top = y + 7, Width = 145, Height = 24, ForeColor = Color.FromArgb(38, 48, 63) };
        labels[index] = label;
        parent.Controls.Add(label);
        StyleTextBox(box);
        box.Left = 185; box.Top = y; box.Width = extra == null ? 300 : 210;
        parent.Controls.Add(box);

        StyleButton(browse, Color.FromArgb(235, 240, 246), Color.FromArgb(38, 48, 63));
        browse.Left = extra == null ? 497 : 405; browse.Top = y - 1; browse.Width = 100; browse.Height = 28;
        browse.Click += index == 5 ? new EventHandler(BrowseInstaller) : new EventHandler(BrowseOutput);
        parent.Controls.Add(browse);

        if (extra != null)
        {
            StyleButton(extra, Color.FromArgb(232, 247, 255), Color.FromArgb(20, 88, 143));
            extra.Left = 497; extra.Top = y - 1; extra.Width = 100; extra.Height = 28;
            extra.Click += DownloadLatest;
            parent.Controls.Add(extra);
        }
    }

    private static void StyleTextBox(TextBox box)
    {
        box.BorderStyle = BorderStyle.FixedSingle;
        box.Font = new Font("Segoe UI", 9.5F);
    }

    private static void StyleButton(Button button, Color back, Color fore)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.BackColor = back;
        button.ForeColor = fore;
        button.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
    }

    private void ApplyLanguage()
    {
        Text = zh ? "RustDesk 配置封装生成器" : "RustDesk Config EXE Generator";
        title.Text = zh ? "RustDesk\n配置封装" : "RustDesk\nPackager";
        subtitle.Text = zh
            ? "填写服务器信息，选择 RustDesk 官方安装包，生成一个可直接分发的单文件安装 EXE。"
            : "Create a single distributable setup EXE with your RustDesk server settings embedded.";

        string[] zhLabels = { "ID 服务器", "中继服务器", "API 服务器", "Key", "固定密码", "RustDesk 安装包", "输出 EXE" };
        string[] enLabels = { "ID server", "Relay server", "API server", "Key", "Fixed password", "RustDesk installer", "Output EXE" };
        string[] active = zh ? zhLabels : enLabels;
        for (int i = 0; i < labels.Length; i++) labels[i].Text = active[i];

        browseInstaller.Text = zh ? "浏览" : "Browse";
        browseOutput.Text = zh ? "浏览" : "Browse";
        latest.Text = zh ? "最新版" : "Latest";
        build.Text = zh ? "生成安装 EXE" : "Build setup EXE";
        status.Text = zh ? "准备就绪" : "Ready";
    }

    private void BrowseInstaller(object sender, EventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Filter = "RustDesk installer|rustdesk*.exe|EXE files|*.exe";
        if (dialog.ShowDialog() == DialogResult.OK) installer.Text = dialog.FileName;
    }

    private void BrowseOutput(object sender, EventArgs e)
    {
        SaveFileDialog dialog = new SaveFileDialog();
        dialog.Filter = "EXE files|*.exe";
        dialog.FileName = "RustDesk-Custom-Setup.exe";
        if (dialog.ShowDialog() == DialogResult.OK) output.Text = dialog.FileName;
    }

    private void DownloadLatest(object sender, EventArgs e)
    {
        try
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string cache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RustDeskConfigGenerator");
            Directory.CreateDirectory(cache);

            Log(zh ? "正在读取 RustDesk 最新版本..." : "Reading latest RustDesk release...");
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent", "RustDeskConfigGenerator");
                string json = client.DownloadString("https://api.github.com/repos/rustdesk/rustdesk/releases/latest");
                Match match = Regex.Match(json, @"""browser_download_url""\s*:\s*""(?<url>https://github\.com/rustdesk/rustdesk/releases/download/[^""]+/rustdesk-[^""]*x86_64\.exe)""");
                if (!match.Success) throw new Exception("Could not find latest Windows x86_64 RustDesk installer.");

                string url = match.Groups["url"].Value.Replace("\\/", "/");
                string file = Path.Combine(cache, Path.GetFileName(new Uri(url).LocalPath));
                Log((zh ? "正在下载 " : "Downloading ") + Path.GetFileName(file) + "...");
                client.DownloadFile(url, file);
                installer.Text = file;
                Log((zh ? "下载完成: " : "Downloaded: ") + file);
            }
        }
        catch (Exception ex)
        {
            Log((zh ? "下载失败: " : "Download failed: ") + ex.Message);
            MessageBox.Show((zh ? "下载失败。你仍可用“浏览”选择本地 RustDesk 安装包。\r\n\r\n" : "Download failed. You can still use Browse to select a local RustDesk installer.\r\n\r\n") + ex.Message, zh ? "下载失败" : "Download failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BuildPackage()
    {
        try
        {
            ValidateInputs();
            string temp = Path.Combine(Path.GetTempPath(), "RustDeskPackager-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(temp);
            string scriptPath = Path.Combine(temp, "install-rustdesk-custom.ps1");
            string bootstrapPath = Path.Combine(temp, "RustDeskCustomSetup.cs");
            string installerCopy = Path.Combine(temp, Path.GetFileName(installer.Text));

            File.WriteAllText(scriptPath, CreateInstallScript(), new UTF8Encoding(false));
            File.WriteAllText(bootstrapPath, BootstrapperSource, new UTF8Encoding(false));
            File.Copy(installer.Text, installerCopy, true);

            string csc = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework64\v4.0.30319\csc.exe");
            if (!File.Exists(csc)) csc = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework\v4.0.30319\csc.exe");
            if (!File.Exists(csc)) throw new Exception("Could not find .NET Framework csc.exe.");

            string args = "/nologo /codepage:65001 /target:winexe /platform:anycpu /out:\"" + output.Text + "\" " +
                "/resource:\"" + scriptPath + "\",install-rustdesk-custom.ps1 " +
                "/resource:\"" + installerCopy + "\"," + Path.GetFileName(installerCopy) + " " +
                "\"" + bootstrapPath + "\"";

            Log(zh ? "正在编译安装 EXE..." : "Compiling setup EXE...");
            ProcessStartInfo psi = new ProcessStartInfo(csc, args);
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            Process p = Process.Start(psi);
            string stdout = p.StandardOutput.ReadToEnd();
            string stderr = p.StandardError.ReadToEnd();
            p.WaitForExit();
            if (p.ExitCode != 0) throw new Exception(stdout + Environment.NewLine + stderr);

            Log((zh ? "完成: " : "Done: ") + output.Text);
            MessageBox.Show((zh ? "生成完成:\r\n" : "Done:\r\n") + output.Text, zh ? "完成" : "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            Log((zh ? "错误: " : "Error: ") + ex.Message);
            MessageBox.Show(ex.Message, zh ? "生成失败" : "Build failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ValidateInputs()
    {
        if (String.IsNullOrWhiteSpace(idServer.Text)) throw new Exception(zh ? "请填写 ID 服务器。" : "ID server is required.");
        if (String.IsNullOrWhiteSpace(relayServer.Text)) throw new Exception(zh ? "请填写中继服务器。" : "Relay server is required.");
        if (String.IsNullOrWhiteSpace(apiServer.Text)) throw new Exception(zh ? "请填写 API 服务器。" : "API server is required.");
        if (String.IsNullOrWhiteSpace(key.Text)) throw new Exception(zh ? "请填写 Key。" : "Key is required.");
        if (String.IsNullOrWhiteSpace(password.Text)) throw new Exception(zh ? "请填写固定密码。" : "Fixed password is required.");
        if (!File.Exists(installer.Text)) throw new Exception(zh ? "请先选择或下载 RustDesk 安装包。" : "Select or download a RustDesk installer first.");
        if (String.IsNullOrWhiteSpace(output.Text)) throw new Exception(zh ? "请选择输出 EXE 路径。" : "Output EXE path is required.");
    }

    private string CreateInstallScript()
    {
        return InstallScriptTemplate
            .Replace("__ID_SERVER__", Ps(idServer.Text))
            .Replace("__RELAY_SERVER__", Ps(relayServer.Text))
            .Replace("__API_SERVER__", Ps(apiServer.Text))
            .Replace("__KEY__", Ps(key.Text))
            .Replace("__PASSWORD__", Ps(password.Text));
    }

    private static string Ps(string value)
    {
        return value.Replace("'", "''");
    }

    private void Log(string message)
    {
        status.Text = message.Length > 54 ? message.Substring(0, 54) + "..." : message;
        log.AppendText(DateTime.Now.ToString("HH:mm:ss ") + message + Environment.NewLine);
    }

    private const string BootstrapperSource = @"
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

internal static class RustDeskCustomSetup
{
    private static int Main()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), ""RustDesk-Custom-"" + Guid.NewGuid().ToString(""N""));
        Directory.CreateDirectory(tempDir);
        Assembly assembly = Assembly.GetExecutingAssembly();
        foreach (string name in assembly.GetManifestResourceNames())
        {
            string target = Path.Combine(tempDir, name);
            using (Stream input = assembly.GetManifestResourceStream(name))
            using (FileStream output = File.Create(target))
            {
                if (input != null) input.CopyTo(output);
            }
        }
        string script = Path.Combine(tempDir, ""install-rustdesk-custom.ps1"");
        ProcessStartInfo info = new ProcessStartInfo();
        info.FileName = ""powershell.exe"";
        info.Arguments = ""-NoProfile -ExecutionPolicy Bypass -File \"""" + script + ""\"""";
        info.UseShellExecute = true;
        info.Verb = ""runas"";
        Process process = Process.Start(info);
        if (process == null) return 1;
        process.WaitForExit();
        return process.ExitCode;
    }
}
";

    private const string InstallScriptTemplate = @"
$ErrorActionPreference = 'Stop'
$IdServer = '__ID_SERVER__'
$RelayServer = '__RELAY_SERVER__'
$ApiServer = '__API_SERVER__'
$Key = '__KEY__'
$FixedPassword = '__PASSWORD__'
$LogDir = Join-Path $env:ProgramData 'RustDesk-Custom'
$LogFile = Join-Path $LogDir 'install.log'
New-Item -ItemType Directory -Force -Path $LogDir | Out-Null

function Write-Log([string]$Message) {
    $line = '{0} {1}' -f (Get-Date -Format 'yyyy-MM-dd HH:mm:ss'), $Message
    Write-Host $line
    Add-Content -Path $LogFile -Value $line -Encoding UTF8
}

function Find-Installer {
    $base = Split-Path -Parent $PSCommandPath
    $file = Get-ChildItem -Path $base -Filter 'rustdesk*.exe' -File | Sort-Object Length -Descending | Select-Object -First 1
    if (-not $file) { throw 'Embedded RustDesk installer was not found.' }
    return $file.FullName
}

function Wait-RustDeskExe {
    $paths = @(
        (Join-Path $env:ProgramFiles 'RustDesk\rustdesk.exe'),
        (Join-Path ${env:ProgramFiles(x86)} 'RustDesk\rustdesk.exe')
    )
    for ($i = 0; $i -lt 120; $i++) {
        foreach ($path in $paths) {
            if ($path -and (Test-Path $path)) { return $path }
        }
        Start-Sleep -Seconds 1
    }
    throw 'rustdesk.exe was not found after installation.'
}

function Stop-RustDesk {
    Stop-Service -Name 'Rustdesk' -Force -ErrorAction SilentlyContinue
    Get-Process -Name 'rustdesk' -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
}

function Write-Config([string]$Root) {
    if ([string]::IsNullOrWhiteSpace($Root)) { return }
    $dir = Join-Path $Root 'RustDesk\config'
    New-Item -ItemType Directory -Force -Path $dir | Out-Null
    $file = Join-Path $dir 'RustDesk2.toml'
    $content = @'
rendezvous_server = ""__ID_SERVER__""
nat_type = 1
serial = 0

[options]
custom-rendezvous-server = ""__ID_SERVER__""
relay-server = ""__RELAY_SERVER__""
api-server = ""__API_SERVER__""
key = ""__KEY__""
direct-server = ""Y""
direct-access-port = ""21118""
verification-method = ""use-permanent-password""
approve-mode = ""password""
'@
    Set-Content -LiteralPath $file -Value $content -Encoding UTF8 -Force
    Write-Log ""Wrote config: $file""
}

function Enable-RustDeskStartup([string]$RustDeskExe) {
    Write-Log 'Enabling startup.'
    & sc.exe config Rustdesk start= auto | Out-Null
    New-Item -Path 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Run' -Force | Out-Null
    Set-ItemProperty -Path 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Run' -Name 'RustDesk' -Value ""`""$RustDeskExe`"" --tray""
    New-Item -Path 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Run' -Force | Out-Null
    Set-ItemProperty -Path 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Run' -Name 'RustDesk' -Value ""`""$RustDeskExe`"" --tray""
}

function Start-RustDeskApp([string]$RustDeskExe) {
    Write-Log 'Starting RustDesk.'
    Start-Process -FilePath $RustDeskExe -ErrorAction SilentlyContinue
}

Write-Log 'Installing RustDesk.'
$installerPath = Find-Installer
$p = Start-Process -FilePath $installerPath -ArgumentList '--silent-install' -PassThru
$rustdesk = Wait-RustDeskExe
if ($p -and -not $p.HasExited) { Wait-Process -Id $p.Id -Timeout 20 -ErrorAction SilentlyContinue }

Stop-RustDesk
Write-Config $env:APPDATA
Write-Config (Join-Path $env:USERPROFILE 'AppData\Roaming')
Write-Config (Join-Path $env:WINDIR 'ServiceProfiles\LocalService\AppData\Roaming')
Write-Config $env:ProgramData

Write-Log 'Applying RustDesk command-line options.'
& $rustdesk --option custom-rendezvous-server $IdServer
& $rustdesk --option relay-server $RelayServer
& $rustdesk --option api-server $ApiServer
& $rustdesk --option key $Key
& $rustdesk --option direct-server Y
& $rustdesk --option direct-access-port 21118
& $rustdesk --password $FixedPassword
& $rustdesk --install-service
Start-Service -Name 'Rustdesk' -ErrorAction SilentlyContinue
Enable-RustDeskStartup $rustdesk
Start-RustDeskApp $rustdesk
Write-Log 'Completed.'
";
}
