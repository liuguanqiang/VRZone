﻿using Prism.Logging;
using Scratch.Log;

namespace VRZone.Log
{
    /// <summary>
    /// 提供给Prism框架使用的日志,对Log4net日志组件进行包装
    /// </summary>
    public class LoggerWarp : ILoggerFacade
    {
        private ILog logger;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LoggerWarp()
        {
            logger = LogConfig.Logger;
        }

        /// <summary>
        /// 提供对Prism日志接口的实现
        /// </summary>
        /// <param name="message"></param>
        /// <param name="category"></param>
        /// <param name="priority"></param>
        public void Log(string message, Category category, Priority priority)
        {
            switch (category)
            {
                case Category.Debug:
                    {
                        logger.Debug(message);
                        break;
                    }

                case Category.Exception:
                    {
                        logger.Error(message);
                        break;
                    }

                case Category.Info:
                    {
                        logger.Info(message);
                        break;
                    }

                case Category.Warn:
                    {
                        logger.Error(message);
                        break;
                    }

                default: break;
            }
        }
    }
}