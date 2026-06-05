# RustDesk Deploy Packager / RustDesk 私有化部署封装器

> Build a single Windows setup EXE that installs RustDesk and writes your self-hosted server configuration automatically.
>
> 生成一个 Windows 单文件安装 EXE，自动安装 RustDesk 并写入你的私有化服务器配置。

## English

RustDesk Deploy Packager is a lightweight Windows tool for teams that deploy RustDesk with self-hosted infrastructure.

You enter your server settings, select or download the official RustDesk Windows installer, and the tool builds a single distributable setup EXE. The generated EXE installs RustDesk, applies the server configuration, sets the fixed password, enables direct IP access, configures startup, and launches RustDesk after installation.

### Features

- Bilingual UI: English and Simplified Chinese.
- Builds a single self-contained setup EXE.
- Supports self-hosted RustDesk server fields:
  - ID server
  - Relay server
  - API server
  - Public key
  - Fixed password
- Downloads the latest official Windows x64 RustDesk installer from GitHub.
- Also supports manually selecting a local RustDesk installer.
- Applies configuration after installation.
- Enables direct IP access on port `21118`.
- Installs and starts the RustDesk service.
- Enables Windows startup entries.
- Launches RustDesk automatically after setup.
- No private default values are embedded.

### Usage

1. Open `RustDeskConfigGenerator.exe`.
2. Fill in your RustDesk server configuration.
3. Click `Latest` to download the latest official RustDesk installer, or click `Browse` to select a local installer.
4. Choose an output path.
5. Click `Build setup EXE`.
6. Distribute the generated EXE to target Windows machines.

The generated setup EXE requests administrator permission because RustDesk installation, service setup, and machine startup configuration require elevated privileges.

### Build From Source

Requirements:

- Windows
- .NET Framework 4.x compiler, usually available at:
  - `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe`

Build the icon:

```powershell
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /nologo /target:exe /platform:anycpu /reference:System.Drawing.dll /out:MakeGeneratorIcon.exe MakeGeneratorIcon.cs
.\MakeGeneratorIcon.exe
```

Build the generator:

```powershell
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /nologo /codepage:65001 /target:winexe /platform:anycpu /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /win32icon:RustDeskConfigGenerator.ico /out:RustDeskConfigGenerator.exe RustDeskConfigGenerator.cs
```

## 中文

RustDesk 私有化部署封装器是一个轻量级 Windows 工具，适合需要批量部署 RustDesk 自建服务器配置的运维团队。

你只需要填写服务器配置，选择或下载官方 RustDesk Windows 安装包，工具会生成一个可分发的单文件安装 EXE。生成的 EXE 会自动安装 RustDesk、写入服务器配置、设置固定密码、开启 IP 直接访问、配置开机启动，并在安装完成后自动打开 RustDesk。

### 功能

- 中英文界面切换。
- 生成单文件安装 EXE。
- 支持 RustDesk 私有化服务器配置：
  - ID 服务器
  - 中继服务器
  - API 服务器
  - 公钥 Key
  - 固定密码
- 可从 GitHub 自动下载官方最新版 Windows x64 RustDesk 安装包。
- 也支持手动选择本地 RustDesk 安装包。
- 安装完成后自动写入配置。
- 自动开启 `21118` 端口的 IP 直接访问。
- 自动安装并启动 RustDesk 服务。
- 自动配置 Windows 开机启动。
- 安装完成后自动打开 RustDesk。
- 不内置任何私有默认配置，适合公开开源。

### 使用方法

1. 打开 `RustDeskConfigGenerator.exe`。
2. 填写你的 RustDesk 服务器配置。
3. 点击 `Latest` 下载官方最新版 RustDesk 安装包，或点击 `Browse` 选择本地安装包。
4. 选择输出路径。
5. 点击 `Build setup EXE`。
6. 将生成的 EXE 分发到目标 Windows 电脑运行。

生成的安装 EXE 会请求管理员权限，因为 RustDesk 安装、服务安装和机器级开机启动配置都需要管理员权限。

### 从源码构建

要求：

- Windows
- .NET Framework 4.x 编译器，通常位于：
  - `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe`

生成图标：

```powershell
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /nologo /target:exe /platform:anycpu /reference:System.Drawing.dll /out:MakeGeneratorIcon.exe MakeGeneratorIcon.cs
.\MakeGeneratorIcon.exe
```

编译生成器：

```powershell
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /nologo /codepage:65001 /target:winexe /platform:anycpu /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /win32icon:RustDeskConfigGenerator.ico /out:RustDeskConfigGenerator.exe RustDeskConfigGenerator.cs
```

## Release

Current stable release: `v0.1.0`

## License

MIT
