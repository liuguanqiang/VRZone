using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using log4net;

namespace VRZone.Log
{
    internal class Log4Wrap
    {
        private const string LOG_FOLDER = "logs";
        private const string LOG_CONFIG_FOLDER = "log4net_config";
        private const string LOGGER_NAME = "Log";
        private const string MaxSizeRollBackups = "10";
        private const string MaxFileSize = "500KB";

        private static log4net.ILog logger = null;
        private static string configPath = string.Empty;
        private static string logfilename = string.Empty;

        /// <summary> 用户配置文件名 </summary>
        private static string SoftwareName { get { return "VRZone"; } }

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static Log4Wrap()
        {
            try
            {
                string userConfigFloder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), SoftwareName);
                if (!Directory.Exists(userConfigFloder))
                {
                    Directory.CreateDirectory(userConfigFloder);
                }
                string moduleName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                string moduleFolder = userConfigFloder;
                string filename = moduleName.Remove(0, moduleName.LastIndexOf("\\") + 1);
                string logConfigFolder = moduleFolder + "\\" + LOG_CONFIG_FOLDER;

                moduleName = moduleName.Remove(0, moduleName.LastIndexOf("\\") + 1);
                string logFolderFullPath = Path.Combine(userConfigFloder, LOG_FOLDER);
                logfilename = LOG_FOLDER + "\\" + moduleName + "_" + DateTime.Now.ToString("o") + ".log";
                logfilename = logfilename.Replace(":", ".");
                logfilename = Path.Combine(userConfigFloder, logfilename);

                if (!Directory.Exists(logConfigFolder))
                {
                    Directory.CreateDirectory(logConfigFolder);
                }

                if (!Directory.Exists(logFolderFullPath))
                {
                    Directory.CreateDirectory(logFolderFullPath);
                }

                configPath = logConfigFolder + "\\" + filename + ".xml";

                if (!File.Exists(configPath))
                {
                    CreateDefaultConfigFile(configPath);
                }
                else
                {
                    try
                    {
                        XElement root = XElement.Load(configPath);
                        root.Descendants("appender").First(n => n.Attribute("name").Value == "RollingFileAppender").Element("file").Attribute("value").Value = logfilename;
                        root.Save(configPath);
                    }
                    catch { }
                }

                log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(configPath));
                logger = LogManager.GetLogger(LOGGER_NAME);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// 公开Logger
        /// </summary>
        public static log4net.ILog Logger
        {
            get
            {
                return logger;
            }
        }

        /// <summary>
        /// 日志配置文件路径
        /// </summary>
        public static string ConfigPath
        {
            get
            {
                return configPath;
            }
        }

        private static void CreateDefaultConfigFile(string filename)
        {
            string configString = string.Empty;
            configString =
@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
    <configSections>
        <section name=""log4net"" type=""log4net.Config.Log4NetConfigurationSectionHandler,log4net-net-1.2"" />
    </configSections>

    <log4net>
        <logger name=""" + LOGGER_NAME + @""">
            <level value=""INFO"" />
            <appender-ref ref=""RollingFileAppender"" />
            <appender-ref ref=""ConsoleAppender"" />
        </logger>

        <appender name=""ConsoleAppender""  type=""log4net.Appender.ConsoleAppender"" >
            <layout type=""log4net.Layout.PatternLayout"">
                <param name=""ConversionPattern""  value=""%date [%-5level] [Thrd:%thread] %l - %message%newline""/>
            </layout>
        </appender>

        <appender name=""RollingFileAppender"" type=""log4net.Appender.RollingFileAppender"">
            <file value=""" + logfilename + @""" />
            <appendToFile value=""true"" />
            <rollingStyle value=""Size"" />
            <maxSizeRollBackups value=""" + MaxSizeRollBackups + @""" />
            <maximumFileSize value=""" + MaxFileSize + @""" />
            <staticLogFileName value=""true"" />
            <layout type=""log4net.Layout.PatternLayout"">
                <conversionPattern value=""%date [%-5level] [Thrd:%thread] %l - %message%newline"" />
            </layout>
        </appender>
    </log4net>
</configuration>";
            try
            {
                StreamWriter sw = File.CreateText(filename);
                sw.Write(configString);
                sw.Close();
                sw.Dispose();
            }
            catch (Exception ex)
            {
                string info = ex.Message;
            }
        }
    }
}