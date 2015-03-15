@echo off
setlocal
set thrift=.\packages\Thrift.0.9.1.3\tools\thrift-0.9.1.exe
set input=.\MetaThrift.thrift

set output_csharp=.\MetaThrift\gen-csharp
echo generating MetaThrift (C#)...
%thrift% --gen csharp -out %output_csharp% %input%

pause
endlocal