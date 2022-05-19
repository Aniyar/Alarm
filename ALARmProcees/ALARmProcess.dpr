program ALARmProcess;

{$APPTYPE CONSOLE}
{$R *.res}

uses
  System.SysUtils,
  Params in 'Params.pas',
  DataModule in 'DataModule.pas' {MainDataModule: TDataModule},
  RWUn in 'RWUn.pas',
  FuncsProcs in 'FuncsProcs.pas',
  ForOTS in 'ForOTS.pas',
  REPORTS in 'REPORTS.pas',
  URemDistance in 'URemDistance.pas';

var
  p: integer;
  FileName, tripDate, dirId: string;
  f: TextFile;
  fs: TFormatSettings;

begin
  try
    { TODO -oUser -cConsole Main : Insert code here }
    // Flag_Sablog := true;
    FileName := ParamStr(1);
    if FileName = '' then
       //FileName := 'g:\work_shifrovka\km_709_4816.svgpdat';
        FileName := 'g:\work_shifrovka\km_717_4820.svgpdat'  ;
    writeln(FileName);

    MainDataModule := DataModule.TMainDataModule.Create(nil);
    MainDataModule.pgsConnection.Close();
    MainDataModule.pgsConnection.Params.LoadFromFile
      ('C:\sntfi\ALARm5\ALARmProcees\pgsparam.txt');
    MainDataModule.pgsConnection.Open();
    // flag_sablog := true;

    if not(FileExists(FileName)) then
    begin
      writeln('Не найден файл: ' + FileName);
      exit;
    end;
    if FileExists('DConfig.txt') then
    begin
      AssignFile(f, 'DConfig.txt');
      reset(f);
      readln(f);
      readln(f, k_mashrih);
      readln(f, k_mashrihpasp);
      readln(f, uklkoef);

      readln(f, k_nusk);
      CloseFile(f);
    end;
    AssignFile(km_shifrovka_file, FileName);
    reset(km_shifrovka_file);
    readln(km_shifrovka_file, GTripId);
    readln(km_shifrovka_file, Glb_PutList_GNapr);
    // ,  START_ST_F);    //начальная станция
    readln(km_shifrovka_file, Glb_GNapr);
    readln(km_shifrovka_file, CHIEF_F);
    readln(km_shifrovka_file, GlbKmTrue);
    readln(km_shifrovka_file, GlbNumPut);
    readln(km_shifrovka_file, GlbTrackId);
    readln(km_shifrovka_file, DIRECTION_F);
    readln(km_shifrovka_file, tripDate);
    readln(km_shifrovka_file, NUMBERCAR_F); // номер вагона
    readln(km_shifrovka_file);
    // Glb_GNapr := StrToInt(dirId);

    NAPR_DBIJ := 1;
    // IF (DIRECTION_F = 'Обратный') THEN
    // begin
    // NAPR_DBIJ := -1;
    // end;
    fs := TFormatSettings.Create;
    fs.DateSeparator := '.';
    fs.ShortDateFormat := 'dd.MM.yyyy';
    fs.TimeSeparator := ':';
    fs.ShortTimeFormat := 'HH:mm';
    GlbTripDate := strtodatetime(tripDate, fs);

    ReadPassport(GlbTrackId, GlbTripDate, GlbKmTrue);

    RWTB_PRK(FileName);
    writeln('Sapa success processed');

    try
      MainDataModule.pgsConnection.Close;
    finally

    end;

    exit;

  except
    on E: Exception do
      writeln(E.ClassName, ': ', E.Message);
  end;

end.
