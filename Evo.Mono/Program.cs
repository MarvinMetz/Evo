using System;
using System.IO;

var fileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
if (fileInfo.Exists)
    log4net.Config.XmlConfigurator.Configure(fileInfo);
else
    throw new InvalidOperationException("No log config file found under " + AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
using var game = new Evo.Mono.Game1();
game.Run();