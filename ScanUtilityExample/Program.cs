using CommonLib.Clients;
using CommonLib.Clients.Object;
using CommonLib.Extensions.Property;
using CommonLib.Function;
using CommonLib.Helpers;
using ScanUtilityLibrary.Core.SICK;
using ScanUtilityLibrary.Core.SICK.Scanner;
using ScanUtilityLibrary.Core.Tianhe;
using ScanUtilityLibrary.Core.TripleIN;
using ScanUtilityLibrary.Model;
//using ScanUtilityLibrary.Core.SICK.Dx;
using ScanUtilityLibrary.Model.SICK.Dx;
using ScanUtilityLibrary.Model.Tianhe;
using ScanUtilityLibrary.Model.TripleIN;
using ScanUtilityLibraryVer2.LivoxSdk2;
using ScanUtilityLibraryVer2.LivoxSdk2.Core;
using ScanUtilityLibraryVer2.LivoxSdk2.Include;
using ScanUtilityLibraryVer2.LivoxSdk2.Model;
using ScanUtilityLibraryVer2.LivoxSdk2.Samples;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ScanUtilityLibraryVer2.LivoxSdk2.Include.LivoxLidarDef;

namespace ScanUtilityExample
{
    static class Program
    {
        private static void TcpClient_OnNoneReceived(object sender, CommonLib.Events.NoneReceivedEventArgs args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            #region test
            #region 天河数据测试1
            //string dataOutput;
            //#region 天河数据1
            //dataOutput = "AD 49 6E 00 00 5B 02 00 33 00 00 00 05 00 D7 F6 BA 08 00 00 01 00 00 00 AC FD B6 61 0E 00 00 00 00 00 00 30 20 00 38 04 14 00 06 00 00 00 38 50 FF FF E8 6E 03 00 FA 00 0B 00 00 01 00 00 08 00 AC FD B6 61 52 40 04 00 70 08 08 00 07 00 08 00 08 00 09 00 07 00 09 00 08 00 07 00 08 00 08 00 09 00 08 00 09 00 08 00 09 00 09 00 09 00 07 00 07 00 08 00 08 00 08 00 08 00 06 00 07 00 09 00 08 00 08 00 07 00 08 00 08 00 09 00 08 00 08 00 08 00 08 00 08 00 08 00 07 00 08 00 07 00 07 00 08 00 08 00 08 00 07 00 07 00 08 00 08 00 07 00 07 00 07 00 07 00 07 00 07 00 07 00 06 00 07 00 07 00 07 00 07 00 08 00 08 00 07 00 08 00 07 00 07 00 07 00 07 00 06 00 07 00 08 00 07 00 07 00 07 00 07 00 08 00 08 00 06 00 07 00 07 00 07 00 07 00 07 00 07 00 07 00 07 00 06 00 06 00 07 00 07 00 07 00 07 00 07 00 07 00 07 00 06 00 08 00 08 00 07 00 07 00 07 00 07 00 07 00 06 00 06 00 07 00 08 00 06 00 07 00 07 00 08 00 07 00 07 00 08 00 08 00 07 00 08 00 08 00 08 00 07 00 08 00 07 00 07 00 07 00 07 00 07 00 08 00 08 00 07 00 08 00 08 00 08 00 07 00 08 00 07 00 07 00 08 00 08 00 07 00 07 00 07 00 07 00 08 00 08 00 07 00 07 00 08 00 07 00 08 00 07 00 08 00 06 00 06 00 07 00 07 00 07 00 07 00 07 00 07 00 07 00 06 00 07 00 07 00 06 00 06 00 07 00 06 00 07 00 07 00 06 00 06 00 06 00 07 00 07 00 06 00 07 00 06 00 06 00 07 00 07 00 C8 06 CA 06 B9 0B 08 00 07 00 0A 00 06 00 06 00 07 00 B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B 78 07 77 07 76 07 70 07 B9 0B 46 06 4F 06 4F 06 50 06 53 06 54 06 55 06 57 06 56 06 57 06 58 06 59 06 5B 06 5B 06 5D 06 5E 06 62 06 5A 06 5C 06 63 06 65 06 6A 06 66 06 68 06 6D 06 70 06 71 06 74 06 74 06 76 06 76 06 7B 06 7E 06 7D 06 81 06 84 06 B9 0B 02 07 F3 07 D4 07 80 07 7B 07 B9 0B 5C 07 45 07 2E 07 ED 06 EC 06 E6 06 B9 0B B8 06 A6 06 97 06 87 06 88 06 8B 06 8D 06 91 06 90 06 04 06 04 06 05 06 F9 05 F4 05 F3 05 F6 05 FA 05 FB 05 F8 05 B9 0B B9 0B 69 05 62 05 00 04 02 04 02 04 01 04 02 04 01 04 01 04 01 04 02 04 01 04 01 04 01 04 00 04 FE 03 0F 04 13 04 14 04 69 03 1B 04 1D 04 21 04 23 04 7D 03 8A 03 AB 03 74 03 76 03 79 03 7B 03 95 03 2F 04 2B 04 0D 04 FE 03 01 04 07 04 07 04 B9 03 B8 03 BE 03 EC 03 E7 03 E2 03 DE 03 DE 03 D4 03 CF 03 C0 03 B6 03 CB 03 C6 03 C1 03 BC 03 B9 03 B2 03 AF 03 A9 03 A6 03 A1 03 9A 03 97 03 93 03 8C 03 8A 03 84 03 7F 03 7C 03 77 03 72 03 70 03 6B 03 69 03 64 03 5E 03 5C 03 59 03 54 03 52 03 4E 03 49 03 46 03 44 03 41 03 3E 03 38 03 35 03 33 03 2F 03 2B 03 29 03 23 03 1B 03 17 03 13 03 11 03 0D 03 0B 03 07 03 06 03 03 03 00 03 FE 02 FA 02 F8 02 F4 02 F2 02 F0 02 EE 02 EB 02 EA 02 04 01 04 01 03 01 01 01 01 01 02 01 02 01 02 01 04 01 07 01 07 01 08 01 0A 01 0C 01 0F 01 11 01 12 01 14 01 16 01 18 01 1A 01 1C 01 1E 01 21 01 23 01 25 01 26 01 24 01 25 01 25 01 23 01 21 01 21 01 21 01 20 01 20 01 1F 01 1E 01 1C 01 1A 01 19 01 18 01 17 01 16 01 16 01 14 01 11 01 0E 01 0F 01 0E 01 0D 01 10 01 10 01 0E 01 10 01 0B 01 0B 01 0A 01 05 01 05 01 02 01 03 01 02 01 00 01 FD 00 FB 00 F7 00 F7 00 F4 00 F4 00 F4 00 F3 00 F1 00 F1 00 F1 00 F1 00 F0 00 F0 00 EE 00 EE 00 EC 00 EB 00 EA 00 E9 00 E8 00 E7 00 E6 00 E5 00 E4 00 E2 00 DF 00 DF 00 DE 00 DF 00 DE 00 DE 00 DC 00 DA 00 DA 00 D9 00 D9 00 D8 00 D9 00 D8 00 D7 00 D7 00 D6 00 D8 00 D8 00 D6 00 D7 00 D5 00 D5 00 D5 00 D5 00 D5 00 D5 00 D6 00 D6 00 D5 00 D4 00 D3 00 D4 00 D2 00 D2 00 D2 00 D1 00 D2 00 D2 00 D1 00 D1 00 D2 00 D2 00 D3 00 D2 00 D2 00 D3 00 D3 00 D2 00 D3 00 D2 00 D1 00 D1 00 D1 00 D2 00 D1 00 D2 00 D2 00 D1 00 D2 00 D0 00 D1 00 D2 00 D2 00 D1 00 D1 00 D1 00 D2 00 D1 00 D1 00 D2 00 D1 00 D1 00 D1 00 D1 00 D2 00 D1 00 D1 00 D2 00 D3 00 D3 00 D2 00 D3 00 D3 00 D4 00 D3 00 D3 00 D3 00 D3 00 D4 00 D4 00 D4 00 D5 00 D6 00 D5 00 D4 00 D5 00 D6 00 D6 00 D7 00 D9 00 D8 00 D8 00 D8 00 D9 00 D9 00 DA 00 DB 00 DD 00 DD 00 DD 00 DC 00 DD 00 DD 00 DE 00 E0 00 E2 00 E3 00 E2 00 E5 00 E4 00 E7 00 E9 00 EA 00 EA 00 EC 00 EF 00 F1 00 F0 00 F1 00 F1 00 F3 00 F2 00 F3 00 F3 00 F5 00 F5 00 F6 00 F7 00 F7 00 F8 00 F9 00 FC 00 FF 00 01 01 05 01 0A 01 08 01 0B 01 09 01 0A 01 0D 01 0D 01 0F 01 14 01 12 01 14 01 15 01 16 01 1B 01 19 01 1D 01 1C 01 1D 01 1D 01 1D 01 1D 01 1E 01 21 01 21 01 22 01 23 01 24 01 25 01 26 01 24 01 26 01 25 01 25 01 28 01 26 01 23 01 23 01 1E 01 1D 01 1B 01 1A 01 17 01 16 01 13 01 12 01 0F 01 0D 01 0B 01 09 01 08 01 07 01 05 01 04 01 03 01 04 01 06 01 07 01 07 01 08 01 25 03 21 03 1C 03 18 03 14 03 0F 03 0A 03 06 03 03 03 06 03 08 03 0C 03 11 03 18 03 1D 03 1F 03 22 03 27 03 2A 03 2D 03 30 03 32 03 35 03 39 03 3B 03 42 03 42 03 47 03 4A 03 4C 03 50 03 55 03 5A 03 5D 03 60 03 63 03 68 03 6B 03 6F 03 71 03 76 03 79 03 80 03 84 03 87 03 8D 03 90 03 94 03 9B 03 9D 03 A4 03 A7 03 AC 03 B3 03 B6 03 BD 03 C2 03 C4 03 CB 03 D0 03 D4 03 D9 03 DF 03 E5 03 EC 03 F0 03 F7 03 FA 03 FD 03 FE 03 FB 03 F8 03 F5 03 F1 03 EF 03 EB 03 E9 03 E7 03 E3 03 E1 03 DE 03 DC 03 D9 03 D6 03 D4 03 D1 03 CB 03 C5 03 BD 03 B6 03 AF 03 A5 03 A7 03 A9 03 AC 03 AF 03 7E 04 7A 04 77 04 76 04 73 04 70 04 6E 04 6C 04 69 04 67 04 65 04 63 04 60 04 5E 04 5C 04 5A 04 56 04 BB 05 C8 05 D4 05 E3 05 4A 04 04 06 14 06 24 06 38 06 4B 06 5F 06 73 06 85 06 98 06 AC 06 BF 06 CE 06 DE 06 F7 06 0F 07 25 07 3A 07 50 07 D7 06 82 07 90 07 8E 07 94 07 C4 06 D7 07 F6 07 4C 08 72 08 96 08 BA 08 E1 08 0A 09 2E 09 53 09 B5 08 AE 09 D8 09 FB 09 AE 08 60 0A 95 0A A5 08 0C 0B 4E 0B 91 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B B9 0B 0E 00 07 00 07 00 07 00 07 00 08 00 06 00 07 00 06 00 07 00 06 00 06 00 06 00 07 00 06 00 08 00 06 00 07 00 07 00 08 00 08 00 06 00 08 00 06 00 06 00 08 00 06 00 07 00 07 00 07 00 07 00 07 00 08 00 09 00 07 00 08 00 07 00 08 00 07 00 08 00 08 00 07 00 08 00 0A 00 08 00 08 00 08 00 08 00 08 00 08 00 08 00 09 00 09 00 08 00 08 00 09 00 09 00 09 00 09 00 09 00 09 00 0A 00 09 00 08 00 0A 00 08 00 08 00 09 00 09 00 09 00 0A 00 09 00 09 00 09 00 09 00 0A 00 08 00 09 00 08 00 09 00 08 00 08 00 08 00 07 00 08 00 08 00 07 00 08 00 07 00 08 00 08 00 08 00 07 00 08 00 09 00 07 00 08 00 08 00 09 00 07 00 07 00 07 00 07 00 07 00 08 00 07 00 08 00 08 00 07 00 06 00 06 00 07 00 07 00 07 00 07 00 08 00 07 00 08 00 07 00 07 00 07 00 08 00 08 00 07 00 08 00 07 00 07 00 07 00 08 00 08 00 07 00 07 00 06 00 07 00 07 00 07 00 07 00 07 00 07 00 08 00 08 00 08 00 07 00 07 00 08 00 08 00 07 00 08 00 08 00 08 00 08 00 08 00 07 00 07 00 09 00 08 00 08 00 08 00 09 00 09 00 09 00 08 00 08 00 08 00 08 00 08 00 08 00 07 00 08 00 09 00 08 00 09 00 08 00 08 00 09 00 08 00 08 00 09 00 08 00 08 00 08 00 09 00 08 00 08 00 08 00";
            //#endregion
            //TipBodyDataOutput tipBodyDataOutput = new TipBodyDataOutput();
            //tipBodyDataOutput.Resolve(dataOutput);
            //return;
            #endregion

            #region triple-in 数据测试
            //var udpClient = new StreamUdpClient("127.0.0.1", 1024);
            ////udpClient.Connect();
            //udpClient.RecDataStream();

            //var str = "45 52 52 00 00 00 00 04 FF FF F8 24 3B 5D 2F A7";
            //var code = CommandBase.ResolveFuncCode(str);
            //var err_command = new ERRCommand(str);
            //return;

            //var gscn_command = new GSCNCommand();
            //var hex = gscn_command.ComposeHexString();
            ////hex = File.ReadAllText("D:\\SCANNING\\Triple-IN\\PSxxx-270\\数据\\data_in_PSDemoProgram\\GSCN\\GSCN#1.txt");
            //hex = File.ReadAllText("D:\\SCANNING\\Triple-IN\\PSxxx-270\\数据\\GSCN#1.txt");
            //gscn_command.Resolve(hex);
            //var scan = gscn_command.CurrScan;
            //DateTime time = DateTimeHelper.GetUtcTimeByTimeStampSec(scan.Parameters.PARAMETER_TIME_STAMP.ToString());
            //return;
            #endregion

            #region ScanUtilityLibrary.Core.SICK.Scanner.StreamTcpClient 超时未接收测试
            //ScanUtilityLibrary.Core.SICK.Scanner.StreamTcpClient tcpClient = new ScanUtilityLibrary.Core.SICK.Scanner.StreamTcpClient();
            //tcpClient.OnNoneReceived += new CommonLib.Events.NoneReceivedEventHandler(TcpClient_OnNoneReceived);
            //tcpClient.Connect("127.0.0.1", 8023);
            //while (true)
            //{
            //    Thread.Sleep(1000);
            //}
            #endregion

            #region livox sdk2 测试
            List<LivoxLidarCartesianHighRawPoint> points = new List<LivoxLidarCartesianHighRawPoint>()
            {
                new LivoxLidarCartesianHighRawPoint() { x = 1000, y = 0, z = 1000, reflectivity = 4 },
            };
            //横滚顺时针90度，俯仰向下45度，回转向右90度
            CoordTransParamSet transformer = new CoordTransParamSet(90, 45, -90);
            var newList = points.TransformPoints(transformer);

            ColorSmoother colorSmoother = new ColorSmoother(byte.MinValue, byte.MaxValue);
            PlyFileClient plyFile = new PlyFileClient(true) { Path = "D:\\", FileName = "snapshot" };
            plyFile.RegisterCustomProperty("reflectivity", typeof(byte));
            DllLoader.ConfigureDllPath();
            LivoxLidarQuickStart.FrameTime = 500;
            //转换到现有码头坐标系：走行增大的方向为前方，X轴正向前，Y轴正向左，Z轴正向上
            CoordTransParamSet coordTransParamSet = new CoordTransParamSet(-90, 45, -90);
            LivoxLidarQuickStart.Start("hap_config.json", coordTransParamSet);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds <= 300000)
            {
                Thread.Sleep(10000);
                var rawPointsCopy = LivoxLidarQuickStart.CartesianHighRawPoints.ToList();
                //List<PlyDotObject> plyDots = rawPointsCopy.Where(rawPoint => !(rawPoint.x == 0 && rawPoint.y == 0 && rawPoint.z == 0)).Select(rawPoint => new PlyDotObject(rawPoint.x, rawPoint.y, rawPoint.z, colorSmoother.GetColor(rawPoint.reflectivity)) { CustomProperties = new List<object>() { rawPoint.reflectivity } }).ToList();
                List<PlyDotObject> plyDots = rawPointsCopy
                    .Where(rawPoint => rawPoint.x != 0 || rawPoint.y != 0 || rawPoint.z != 0)
                    .Select(rawPoint => new PlyDotObject(rawPoint.x, rawPoint.y, rawPoint.z, colorSmoother.GetColor(rawPoint.reflectivity))
                    {
                        CustomProperties = new List<object> { rawPoint.reflectivity }
                    })
                    .ToList();
                plyFile.SaveVertexes(plyDots);
            }
            //Thread.Sleep(300000); // 保持程序运行
            LivoxLidarQuickStart.Stop();
            return;
            #endregion

            //var time = DateTimeHelper.GetUtcTimeByTimeStampMillisec(1407535921);
            //var runningTime = new TimeSpan(0, 0, 0, 0, 1407535921);
            //var format = runningTime.ToString("c");
            //return;

            //var command = new SCANCommand();
            //string hex = command.ComposeHexString();
            //string rcvr = "53 43 41 4e 00 00 00 04 00 00 00 01 9a 35 13 20";
            //command.Resolve(rcvr);
            //return;

            //GVERCommand command = new GVERCommand();
            //string hex = command.ComposeHexString();
            //string gver = "47 56 45 52 00 00 00 9C 50 53 32 35 30 2D 32 37 30 0D 0A 5B 50 53 46 69 72 6D 57 61 72 65 3B 20 30 33 2E 30 33 2E 32 30 2E 30 39 3B 20 32 30 31 37 2D 30 32 2D 30 38 3B 20 28 63 29 20 54 72 69 70 6C 65 2D 49 4E 20 47 6D 62 48 20 32 30 31 37 5D 0D 0A 42 75 69 6C 64 3A 20 46 65 62 20 20 39 20 32 30 31 37 20 30 37 3A 35 36 3A 34 33 0D 0A 49 6E 66 6F 3A 20 20 20 24 44 61 74 65 3A 20 32 30 31 37 2F 30 32 2F 30 38 20 31 35 3A 30 33 3A 34 34 20 24 3B 20 50 52 4F 54 4F 54 59 50 45 0D 0A 00 00 00 73 D6 86 EC";
            //command.Resolve(gver);
            //return;

            //ScanUtilityLibrary.Core.Tianhe.StreamTcpClient tcpClient = new StreamTcpClient();
            //var scanData = tcpClient.GetScanDataString();
            //return;

            ////tcpClient.Connect("127.0.0.1", 8889);
            //tcpClient.Connect("127.0.0.1", 8887);
            //tcpClient.RecDataStream();
            ////while (true)
            ////{
            ////    Thread.Sleep(1);
            ////}

            //ScanPoint[] ScanPoints = new ScanPoint[25200];
            //for (int i = 0; i < ScanPoints.Length; i++)
            //{
            //    ScanPoints[i].Distance = 0;
            //    ScanPoints[i].EchoValue = 0;
            //}
            //return;

            //TipBodyHeartbeat bodyHeart = new TipBodyHeartbeat();
            //string hex = bodyHeart.ComposeHexString();
            //TipCode code = TipHeadBase.ResolveTipCode(hex);
            //if (code == TipCode.Heartbeat)
            //    bodyHeart = new TipBodyHeartbeat(hex);
            //return;

            //string hex = "AD 49 02 00 00 5B 00 00 11 00 00 00 05 00 D5 FF 28 00 00 00 00 00 00 00 40 88 CB 5E 0E 00 00 00 5B 00 A5 00 00 00 00 00";
            //TipCode code = TipHeadBase.ResolveTipCode(hex);
            //TipBodyHeartbeatReply bodyHeart;
            //if (code == TipCode.HeartbeatReply)
            //    bodyHeart = new TipBodyHeartbeatReply(hex);
            //return;

            //TipBodyLogin bodyLogin = new TipBodyLogin();
            //bodyLogin.RestoreDefUserInfos();
            //string hex = bodyLogin.ComposeHexString();
            //TipCode code = TipHeadBase.ResolveTipCode(hex);
            //if (code == TipCode.Login)
            //    bodyLogin = new TipBodyLogin(hex);
            //return;

            //string hex = "AD 49 EB 0B 00 5B 02 00 0E 00 00 00 05 00 EC F3 28 00 00 00 01 00 00 00 2E B1 DD 5E 0F 00 00 00 00 00 00 00 00 00 00 00";
            //TipCode code = TipHeadBase.ResolveTipCode(hex);
            //TipBodyLoginReply bodyLogin;
            //if (code == TipCode.LoginReply)
            //    bodyLogin = new TipBodyLoginReply(hex);
            //return;

            //string userNameHex = "67 6A 64 74 31 64 00 00 00";
            //string userName = Encoding.ASCII.GetString(HexHelper.HexString2Bytes(userNameHex)).TrimEnd('\0');
            //int len = userName.Length;
            ////List<byte> bytes = new List<byte>() { 0 };
            ////bytes.AddRange(null);
            //return;

            //short s = 19;
            //byte[] bytes = BitConverter.GetBytes(s);
            ////string timeStamp = DateTimeHelper.GetTimeStampBySeconds();
            //return;

            //string tipHead = "AD 49 6E 00 00 5B 02 00 33 00 00 00 05 00 D7 F6 BA 08 00 00 01 00 00 00 AC FD B6 61 0E 00 00 00 00 00 00 30 20 00 38 04";
            ////tipHead = "AD 49 6E 00 00 5B 02 00 33 00 00 00 05 00 0F FB 82 04 00 00 01 00 00 00 68 8D 1E 5F 0F 00 00 00 00 00 00 E2 20 00 1C 02";
            ////tipHead = "AD 49 6E 00 00 5B 02 00 33 00 00 00 05 00 17 FD 7A 02 00 00 01 00 00 00 22 07 DF 5E 0F 00 00 00 00 00 00 DD 20 00 18 01";
            //var head = new TipHeadDataOutput(tipHead);
            //return;

            #region DX
            //ScanUtilityLibrary.Core.SICK.Dx.StreamTcpClient client = new ScanUtilityLibrary.Core.SICK.Dx.StreamTcpClient("192.168.0.66", "2112");
            ////StreamTcpClient client = new StreamTcpClient("127.0.0.1", "2112");
            //client.Connect();
            //var sender = client.CommandSender;
            //string getFilter = "sRN filterSelection";
            //string getFilterResult = sender.SendCommand(getFilter);
            //string message = sender.Login(UserLevel.Service);
            //message = sender.SetContinualOutputProtocol(ScanUtilityLibrary.Core.SICK.Dx.ContinualOutputProtocol.STXETX);
            //message = sender.SetContinualOutputRate(100);
            //message = sender.SetContinualOutputType(ScanUtilityLibrary.Core.SICK.Dx.ContinualOutputType.DistVel);
            //message = sender.SetDistanceOffset(0);
            //message = sender.SaveSettingToDevice();
            //message = sender.Logout();
            //bool result = sender.RequireDistance(out double dist);
            //result = sender.RequireVelocity(out double vel);
            //result = sender.RequireSignalLevel(out int level);
            //result = sender.RequireDeviceTemperature(out byte temp);
            //result = sender.RequireDeviceOpHours(out uint hours);
            //DeviceStatusWord word = new DeviceStatusWord();
            //result = sender.RequireStatusWord(ref word);
            //client.RequiredDataTypes = new List<ScanUtilityLibrary.Core.SICK.Dx.RequiredDataType>() { ScanUtilityLibrary.Core.SICK.Dx.RequiredDataType.Distance, ScanUtilityLibrary.Core.SICK.Dx.RequiredDataType.Velocity, ScanUtilityLibrary.Core.SICK.Dx.RequiredDataType.SignalLevel, ScanUtilityLibrary.Core.SICK.Dx.RequiredDataType.DeviceTemperature, ScanUtilityLibrary.Core.SICK.Dx.RequiredDataType.OperatingHours, ScanUtilityLibrary.Core.SICK.Dx.RequiredDataType.DeviceStatusWord };
            //while (true)
            //{
            //    Thread.Sleep(100);
            //}
            #endregion
            #region LMS5XX
            //ScanUtilityLibrary.Core.SICK.Scanner.StreamTcpClient client = new ScanUtilityLibrary.Core.SICK.Scanner.StreamTcpClient("192.168.0.66", "2112");
            ////StreamTcpClient client = new StreamTcpClient("127.0.0.1", "2112");
            //client.Connect();
            //var sender = client.CommandSender;
            //string getFilter = "sRN filterSelection";
            //string getFilterResult = sender.SendCommand(getFilter);
            //string message = sender.Login(UserLevel.Service);
            //message = sender.SetContinualOutputProtocol(ContinualOutputProtocol.STXETX);
            //message = sender.SetContinualOutputRate(100);
            //message = sender.SetContinualOutputType(ContinualOutputType.DistVel);
            //message = sender.SetDistanceOffset(0);
            //message = sender.SaveSettingToDevice();
            //message = sender.Logout();
            //bool result = sender.RequireDistance(out double dist);
            //result = sender.RequireVelocity(out double vel);
            //result = sender.RequireSignalLevel(out int level);
            //result = sender.RequireDeviceTemperature(out byte temp);
            //result = sender.RequireDeviceOpHours(out uint hours);
            //DeviceStatusWord word = new DeviceStatusWord();
            //result = sender.RequireStatusWord(ref word);
            //client.RequiredDataTypes = new List<RequiredDataType>() { RequiredDataType.Distance, RequiredDataType.Velocity, RequiredDataType.SignalLevel, RequiredDataType.DeviceTemperature, RequiredDataType.OperatingHours, RequiredDataType.DeviceStatusWord };
            //while (true)
            //{
            //    Thread.Sleep(100);
            //}
            #endregion
            #endregion

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}
