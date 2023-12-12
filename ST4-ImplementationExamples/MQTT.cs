using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace ST4_ImplementationExamples
{
    public class MQTT
    {
        public MQTT()
        { }
        MqttFactory factory;
        MqttClient mqttClient;
        IMqttClientOptions mqttClientOptions;
        MqttClientOptionsBuilder mqttClientOptionsBuilder;
        /// used to connection
        private async Task Connect()
        {
            string clientId = Guid.NewGuid().ToString();
            mqttClient = (MqttClient) new MqttFactory().CreateMqttClient();
            mqttClientOptions = new MqttClientOptionsBuilder()
                .WithCredentials("Jakub","1111")
                .WithClientId(clientId)
                .WithTcpServer("localhost", 1883)
                .WithCleanSession(true)
                .WithRequestResponseInformation(true)
                .Build();
            
            //the handlers of MQTTnet are very useful when working with an event-based communication
            //on established connection
            mqttClient.UseConnectedHandler(e =>
            {
                if (mqttClient.IsConnected)
                {
                    Console.WriteLine("Connected successfully with MQTT Brokers.");
                }
            });
            //on lost connection
            mqttClient.UseDisconnectedHandler(e =>
            {
                try
                {
                      if (mqttClient.IsConnected == false) 
                          Console.WriteLine("Disconnected from MQTT Brokers.");
                      Task.Delay(TimeSpan.FromSeconds(5)); 
                                    
                      mqttClient.ConnectAsync(mqttClientOptions); 
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            });
           
            // receive message on subscribed topic
            mqttClient.UseApplicationMessageReceivedHandler(  e =>
            { Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
               Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
               Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
               Console.WriteLine();
            });
            //connect
            await mqttClient.ConnectAsync(mqttClientOptions);
        }
        // idle state 0 
        public async void Idle()
        {
            mqttClientOptionsBuilder = new MqttClientOptionsBuilder();
            mqttClientOptionsBuilder.WithCommunicationTimeout(TimeSpan.FromSeconds(10));
            await SubscribeToTopic("emulator/status");
            await SubscribeToTopic("emulator/response");
            try
            {  
                Thread.Sleep(1000);
                var b = UnsubscribeToTopic("emulator/status").Wait(TimeSpan.FromSeconds(9));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
                        
        }
        //  execution state 1
        public async Task Execution()
        {
            await OperationRun();
            //on receive message on subscribed topic
            Console.WriteLine("it is in execution state:"); 
            await SubscribeToTopic("emulator/response");
            await SubscribeToTopic("emulator/checkhealth");
            await SubscribeToTopic("emulator/status");
            try
            {  
                Thread.Sleep(8000);
                var b = UnsubscribeToTopic("emulator/status").Wait(TimeSpan.FromSeconds(9));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

      
        
        //Subscribe messages from   
        private async Task SubscribeToTopic(string input,int qos = 1)
        {
            Console.WriteLine("Subscribing to : " + input);
            //define topics
            var topic = new MqttTopicFilterBuilder()
                .WithTopic(input)
                .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)qos)
                .Build();
            //subscribe topics
            await mqttClient.SubscribeAsync(topic);
        }
        
        //unSubscribe messages from 
        public async Task UnsubscribeToTopic(string input)
        {
            var topic1 = new MqttTopicFilterBuilder()
                .WithTopic(input)
                .Build();
           await mqttClient.UnsubscribeAsync(topic1.Topic);
        }
        
        
        //Publish messages to 
        public async Task PublishOnTopic(String msg, string topic, int qos = 1)
        
        {
            await mqttClient.PublishAsync(msg,topic);
        }
        
        //runner
        public async Task RunExample()
        { //connect and subscribe
            await Connect();
        }

        private async Task OperationRun()
        {
            //json serializable object
            var msg = new MqttMessage();
            msg.ProcessID =9;
            //run publish
            if (msg.ProcessID!=9999)
            {   // Operation topic is used to execute program
                await PublishOnTopic("emulator/operation", JsonConvert.SerializeObject(msg));
            }
           
        }
    }
    //class to serialize json objects
    public class MqttMessage
    {
        public int ProcessID { get; set; } 
    }
}

