using Xunit;
using System.Collections;
using YamlDotNet.Serialization;

namespace Netnr.Test
{
    public class NetnrUAParser
    {
        [Fact]
        public void YmlToXml()
        {
            var projectDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent;
            var regexesDir = Path.Combine(projectDir.FullName, "regexes");
            if (Directory.Exists(regexesDir))
            {
                var deserializer = new Deserializer();

                var result = new UAParser.Entitys();

                #region Client

                var listPathClient = new List<string>
                {
                    "browsers.yml","feed_readers.yml","libraries.yml","mediaplayers.yml","mobile_apps.yml","pim.yml"
                };
                listPathClient.ForEach(clientFile =>
                {
                    var pathClient = Path.Combine(regexesDir, "client", clientFile);
                    var yamlClient = deserializer.Deserialize<List<Hashtable>>(new StreamReader(pathClient));
                    if (yamlClient != null)
                    {
                        yamlClient.ForEach(item =>
                        {
                            var modelClient = new UAParser.Entitys.ClientEntity
                            {
                                Type = string.Join(" ", clientFile.Split('.')[0].Split('_')),
                                Regex = item["regex"].ToString(),
                                Name = item["name"].ToString()
                            };
                            if (item.ContainsKey("version"))
                            {
                                modelClient.Version = item["version"]?.ToString() ?? "";
                            }
                            if (item.ContainsKey("engine"))
                            {
                                var engineObj = (Dictionary<object, object>)item["engine"];
                                if (engineObj.ContainsKey("default"))
                                {
                                    modelClient.Engine = engineObj["default"].ToString();
                                }
                            }

                            result.ListClient.Add(modelClient);
                        });
                    }
                });

                #endregion

                #region Device

                var listPathDevice = new List<string>
                {
                    "cameras.yml","car_browsers.yml","consoles.yml","mobiles.yml","notebooks.yml","portable_media_player.yml","shell_tv.yml","televisions.yml"
                };
                listPathDevice.ForEach(deviceFile =>
                {
                    var pathDevice = Path.Combine(regexesDir, "device", deviceFile);
                    var yamlDevice = deserializer.Deserialize<Dictionary<string, Hashtable>>(new StreamReader(pathDevice));
                    if (yamlDevice != null)
                    {
                        foreach (var brand in yamlDevice.Keys)
                        {
                            var item = yamlDevice[brand];

                            var modelDevice = new UAParser.Entitys.DeviceEntity
                            {
                                Type = string.Join(" ", deviceFile.Split('.')[0].Split('_')),
                                Regex = item["regex"].ToString(),
                                Brand = brand
                            };
                            if (modelDevice.Regex.Contains(@"\-\"))
                            {
                                modelDevice.Regex = modelDevice.Regex.Replace(@"\-\", @"\\-\\");
                            }
                            if (modelDevice.Regex.Contains(@" \_"))
                            {
                                modelDevice.Regex = modelDevice.Regex.Replace(@" \_", @" \\_");
                            }
                            if (item.ContainsKey("device"))
                            {
                                modelDevice.Device = item["device"]?.ToString() ?? "";
                            }
                            if (item.ContainsKey("model"))
                            {
                                modelDevice.Model = item["model"]?.ToString() ?? "";
                            }

                            result.ListDevice.Add(modelDevice);
                        }
                    }
                });

                #endregion

                #region Bot

                var pathBot = Path.Combine(regexesDir, "bots.yml");
                var yamlBot = deserializer.Deserialize<List<Hashtable>>(new StreamReader(pathBot));
                if (yamlBot != null)
                {
                    yamlBot.ForEach(item =>
                    {
                        var modelBot = new UAParser.Entitys.BotEntity
                        {
                            Regex = item["regex"].ToString(),
                            Name = item["name"].ToString()
                        };
                        if (item.ContainsKey("category"))
                        {
                            modelBot.Category = item["category"].ToString();
                        }
                        if (item.ContainsKey("producer"))
                        {
                            modelBot.Producer = ((Dictionary<object, object>)item["producer"]).Values.ToList().First().ToString();
                        }

                        result.ListBot.Add(modelBot);
                    });
                }

                #endregion

                #region OS

                var pathOs = Path.Combine(regexesDir, "oss.yml");
                var yamlOs = deserializer.Deserialize<List<Hashtable>>(new StreamReader(pathOs));
                if (yamlOs != null)
                {
                    yamlOs.ForEach(item =>
                    {
                        var modelOS = new UAParser.Entitys.OSEntity
                        {
                            Regex = item["regex"].ToString(),
                            Name = item["name"].ToString()
                        };
                        if (item.ContainsKey("version"))
                        {
                            modelOS.Version = item["version"].ToString();
                        }

                        result.ListOS.Add(modelOS);
                    });
                }

                #endregion

                var xmlContent = UAParser.Parsers.ToXml(result);
                xmlContent = UAParser.Parsers.NodeConvert(xmlContent, true);
                File.WriteAllText(Path.Combine(regexesDir, "regexes.xml"), xmlContent);
            }
        }

        [Fact]
        public void Parsers()
        {
            var listUserAgent = new List<string>
            {
                "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:62.0) Gecko/20100101 Firefox/62.0",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:101.0) Gecko/20100101 Firefox/101.0",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.102 Safari/537.36",
                "mozilla/5.0 (linux; u; android 4.1.2; zh-cn; mi-one plus build/jzo54k) applewebkit/534.30 (khtml, like gecko) version/4.0 mobile safari/534.30 micromessenger/5.0.1.352",
                "Mozilla/5.0 (Linux; Android 6.0.1; Nexus 5X Build/MMB29P) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.4951.64 Mobile Safari/537.36 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)"
            };

            var listMsg = new List<string>();
            var st = new SharedTimingVM();

            listUserAgent.ForEach(userAgent =>
            {
                var uap = new UAParser.Parsers(userAgent);

                var clientEntity = uap.GetClient();
                var deviceEntity = uap.GetDevice();
                if (deviceEntity == null && userAgent.Contains("Windows"))
                {
                    Console.WriteLine("device type: desktop");
                }
                var osEntity = uap.GetOS();
                var botEntity = uap.GetBot();

                listMsg.Add($"UAP: {st.PartTimeFormat()}");
                Debug.WriteLine(clientEntity.ToJson());
                Debug.WriteLine(deviceEntity.ToJson());
                Debug.WriteLine(osEntity.ToJson());
                Debug.WriteLine(botEntity.ToJson());
            });
            for (int i = 0; i < 2; i++)
            {
                listUserAgent.ForEach(userAgent =>
                {
                    var uap = new UAParser.Parsers(userAgent);

                    var clientEntity = uap.GetClient();
                    var deviceEntity = uap.GetDevice();
                    var osEntity = uap.GetOS();
                    var botEntity = uap.GetBot();
                });
            }
            listMsg.Add($"UAP: {st.PartTimeFormat()}");
        }
    }
}
