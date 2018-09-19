using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;

namespace RabbitMQEventBus
{
    /// <summary>
    /// RabbitMQ service class.
    /// </summary>
    public class MqService
    {
        private static string _body;
        private static object _jsonDeser;
        private readonly ILogger _logger;
        private readonly IConfigurationRoot _config;
        /// <summary>
        /// Creates MqService instance.
        /// </summary>
        public MqService()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _logger = new LoggerFactory().CreateLogger("MqService");
        }
        /// <summary>
        /// Publishes object to queue.
        /// </summary>
        /// <param name="qName">Queue name.</param>
        /// <param name="data">Data object to be placed into the queue.</param>
        public virtual void Publish(string qName, object data)
        {
            var connectionFactory = new ConnectionFactory();
            _config.GetSection("RabbitMqConnection").Bind(connectionFactory);

            _logger.Log(LogLevel.Information, "b4 connection");
            using (var connection = connectionFactory.CreateConnection())
            {
                _logger.LogInformation("in connection");
                using (var model = connection.CreateModel())
                {
                    _logger.LogInformation("in model");
                    model.QueueDeclare(qName, true, false, false, null);
                    var basicProperties = model.CreateBasicProperties();
                    basicProperties.Persistent = true;
                    _logger.LogInformation("data b4 json", data);
                    var jsonObj = JsonConvert.SerializeObject(data);
                    var dataBuffer = Encoding.UTF8.GetBytes(jsonObj);
                    model.BasicPublish(string.Empty, qName, basicProperties, dataBuffer);
                }
            }
        }
        /// <summary>
        /// Consumes/Subscribes queue and get published data.
        /// </summary>
        /// <typeparam name="T">Type of consumed object.</typeparam>
        /// <param name="qName">Queue name.</param>
        /// <returns>Returns consumed object result.</returns>
        public virtual T Subscribe<T>(string qName)
        {

            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            using (var connection = connectionFactory.CreateConnection())
            {
                using (var model = connection.CreateModel())
                {
                    model.QueueDeclare(qName, true, false, false, null);
                    model.BasicQos(0, 1, false);
                    var eventingBasicConsumer = new EventingBasicConsumer(model);
                    eventingBasicConsumer.Received += (sender, ea) =>
                    {
                        var body = ea.Body;
                        _body = Encoding.UTF8.GetString(body);
                        Debug.WriteLine(_body);
                        _jsonDeser = JsonConvert.DeserializeObject<T>(_body);
                        Debug.WriteLine(_jsonDeser.GetType());
                        Debug.WriteLine(_jsonDeser);
                    };
                    model.BasicConsume(qName, true, eventingBasicConsumer);
                }
            }

            return (T)_jsonDeser;
        }
    }
}
