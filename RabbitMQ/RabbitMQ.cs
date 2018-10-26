using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ
{
    public class RabbitMQ : IDisposable
    {
        //Default username and password are guest for RabbitMQ.
        private const string ConnectionString = "host=localhost;username;guest;password=guest;";
        private string _message = string.Empty;
        private IConnection _connection = null;
        private IModel _channel = null;

        ~RabbitMQ()
        {
            this.CloseConnection();
        }

        public void Dispose()
        {
            this.CloseConnection();
        }

        private IModel CreateConnection()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            this._connection = factory.CreateConnection();
            this._channel = _connection.CreateModel();

            return _channel; 
        }

        private void CloseConnection()
        {
            this._channel.Close();
            this._connection.Close();
        }

        /// <summary>
        /// Declaring a queue is idempotent - it will only be created if it doesn't exist already. 
        /// </summary>
        /// <param name="channel"></param>
        public void DeclareQueue(IModel channel)
        {
            channel.QueueDeclare(queue: RabbitConstants.Queue,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
        }

        public void MessageBrokerPublish(string message)
        {
            Producer Rp = new Producer();
            this.CreateConnection();
            DeclareQueue(_channel);
            Rp.PublishMessage(_channel, message);
        }

        public void MessageBrokerConsume()
        {
            Consumer Rc = new Consumer();
            this.CreateConnection();
            DeclareQueue(_channel);
            Rc.ConsumeMessage(_channel);
        }
    }
}
