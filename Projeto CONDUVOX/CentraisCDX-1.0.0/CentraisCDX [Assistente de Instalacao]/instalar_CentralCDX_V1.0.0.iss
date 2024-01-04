; INFORMA��ES DO ARQUIVO
; ---------------------------------------------------------------------------------
; - Utilidade do arquivo : C�digo fonte do setup Central-CDX
; - Vers�o do arquivo    : 01
; - Data cria��o         : 26/05/2014
; - Data altera��o       : 26/05/2014
; - Desenvolvido por     : Ricardo Fernando
; - Alterado por         : Ricardo Fernando
; - Hist�rico da vers�o  : Desenvolvendo
; ---------------------------------------------------------------------------------
;
; FUNCIONALIDADES
; ---------------------------------------------------------------------------------
; Assistente que orienta passo a passo o usu�rio instalar o aplicativo no PC.
; ---------------------------------------------------------------------------------
;
; OBSERVA��ES
; ---------------------------------------------------------------------------------
; Script gerado pelo Inno Setup Script Wizard (gratuito).
; ---------------------------------------------------------------------------------
;
; PENDENCIAS
; ---------------------------------------------------------------------------------
; N�o existe
; ---------------------------------------------------------------------------------

#define appPasta "M:\Projeto CONDUVOX\CentraisCDX-1.0.0\CentraisCDX [Assistente de Instalacao]"
#define appComent "Software de programa��o das Centrais CDX"
#define appNome "Central-CDX"
#define appVersao "1.0.0"
#define appEmpresa "Conduvox Telem�tica"
#define nomeInstal "instalar_CentralCDX_V1.0.0-Beta"

[Setup]
OutputDir={#appPasta}
OutputBaseFilename={#nomeInstal}

AppId={{855AD791-AE8F-423B-8EF1-F5DCF114DA77}
AppName={#appNome}
AppVerName={#appNome}
AppVersion={#appVersao}
AppPublisher={#appEmpresa}
AppComments={#appComent}
VersionInfoVersion={#appVersao}
VersionInfoDescription={#appComent}

DefaultDirName={sd}\{#appEmpresa}\{#appNome}
SetupIconFile={#appPasta}\Imagens_Utilizadas\icone_wizard.ico
WizardImageFile={#appPasta}\Imagens_Utilizadas\imgAssistente_Lateral.bmp
WizardSmallImageFile={#appPasta}\Imagens_Utilizadas\imgAssistente_Topo.bmp
DisableProgramGroupPage=true
;DefaultGroupName=TerminalC-VOZ
;DisableReadyPage=yes
;Uninstallable=no
;PrivilegesRequired=none
Compression=lzma
SolidCompression=true
Uninstallable=false

[Languages]
Name: brazilianportuguese; MessagesFile: {#appPasta}\Linguagem\BrazilianPortuguese.isl

[Files]
Source: Conteudo_do_Assistente\isxdl.dll; DestDir: {tmp}; Flags: deleteafterinstall; DestName: isxdl.dll
Source: Conteudo_do_Assistente\CentraisCDX.exe; DestDir: {app}; DestName: CentraisCDX.exe; Flags: ignoreversion
Source: Conteudo_do_Assistente\itextsharp.dll; DestDir: {app}; DestName: itextsharp.dll; Attribs: hidden; Flags: ignoreversion

[Dirs]
Name: {app}\Arquivos
Name: {app}\Relatorio
Name: {app}\Log

[Icons]
Name: {commondesktop}\Atalho para CentraisCDX; Filename: {app}\CentraisCDX.exe; Tasks: desktopicon; IconIndex: 0

[Tasks]
Name: desktopicon; Description: {cm:CreateDesktopIcon}; GroupDescription: {cm:AdditionalIcons}

[Run]
Filename: {app}\CentraisCDX.exe; Description: {cm:LaunchProgram,TerminalC-VOZ}; Flags: nowait postinstall skipifsilent



[Code]
var
	dotnetRedistPath: string;
	downloadNeeded: boolean;
	dotNetNeeded: boolean;
	memoDependenciesNeeded: string;

procedure isxdl_AddFile(URL, Filename: PChar);
external 'isxdl_AddFile@files:isxdl.dll stdcall';
function isxdl_DownloadFiles(hWnd: Integer): Integer;
external 'isxdl_DownloadFiles@files:isxdl.dll stdcall';
function isxdl_SetOption(Option, Value: PChar): Integer;
external 'isxdl_SetOption@files:isxdl.dll stdcall';

//*********************************************************************************
// Link para download do .NET FramaWork 2.0
//*********************************************************************************
const
	//dotnetRedistURL = 'http://www.heresysoft.com/downloads/LambdaEditorSetup.exe';
	dotnetRedistURL = 'http://download.microsoft.com/download/5/6/7/567758a3-759e-473e-bf8f-52154438565a/dotnetfx.exe';

//*********************************************************************************
// Aqui � onde tudo come�a
//*********************************************************************************
function InitializeSetup(): Boolean;

begin

	Result := true;
	dotNetNeeded := false;

	//*********************************************************************************
	// Verificar se existe o .NET 2.0 instalado antes de instalar o aplicativo
	//*********************************************************************************
	if (not RegKeyExists(HKLM, 'Software\Microsoft\.NETFramework\policy\v2.0')) then
		begin
			dotNetNeeded := true;
			memoDependenciesNeeded := '      .NET Framework 2.0' #13;

			// Atribui o caminho do .NET na pasta (usado no caso do CD de instala��o)
			dotnetRedistPath := ExpandConstant('{src}\Suporte\dotnetfx.exe');

			// Se n�o existir o arquivo na pasta habilita o download do .NET 2.0
			if not FileExists(dotnetRedistPath) then
				begin
					dotnetRedistPath := ExpandConstant('{tmp}\dotnetfx.exe');
					if not FileExists(dotnetRedistPath) then
						begin
							isxdl_AddFile(dotnetRedistURL, dotnetRedistPath);
							downloadNeeded := true;
							end
					end
					SetIniString('install', 'dotnetRedist', dotnetRedistPath, ExpandConstant('{tmp}\dep.ini'));
		end;

end;

function NextButtonClick(CurPage: Integer): Boolean;

var
	hWnd: Integer;
	ResultCode: Integer;

begin

	Result := true;

	//*********************************************************************************
	// Somente executar este na pagina "Pronto para instalar" wizard page
	//*********************************************************************************
	if CurPage = wpReady then
		begin

		hWnd := StrToInt(ExpandConstant('{wizardhwnd}'));

		if downloadNeeded and (dotNetNeeded = true) then
			begin
				isxdl_SetOption('label', 'Download Microsoft .NET Framework 2.0');
				isxdl_SetOption('description', 'O aplicativo precisa instalar o Microsoft .NET Framework 2.0. Por favor, aguarde enquanto o Assistente de Instala��o baixa e instala os arquivos necess�rios para o seu computador.');
				if isxdl_DownloadFiles(hWnd) = 0 then Result := false;
			end;

			//*********************************************************************************
			// Execute o arquivo de instala��o do. NET Framework 2.0
			//*********************************************************************************
        	if (dotNetNeeded = true) then
				begin
					if Exec(ExpandConstant(dotnetRedistPath), '', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then
						begin
							if not (ResultCode = 0) then
								begin
									Result := false;
								end
						end
					else
						begin
							Result := false;
						end
			end;
	end;
end;

//*********************************************************************************
// Atualiza a caixa "Pronto para instalar" com as pendencias
//*********************************************************************************
function UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo, MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo: String): String;

var
	s: string;
	msg: string;

begin
	// Define a mensagem
	if downloadNeeded = true then
		msg := 'O assistente ir� iniciar o Download e a instala��o automaticamente:'
	else
		msg := 'O assistente ir� iniciar a instala��o automaticamente:'

	if memoDependenciesNeeded <> '' then s := s + msg + NewLine + memoDependenciesNeeded + NewLine;

	Result := s + MemoDirInfo + NewLine + NewLine;
end;
