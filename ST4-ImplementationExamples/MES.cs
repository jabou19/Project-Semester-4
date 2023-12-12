using System;
using System.Threading;

namespace ST4_ImplementationExamples
{
    public class Program
    {
        private static REST _rest ;
        private static MQTT _mqtt;
        private static SOAP soap;

        public static void Main(string[] args)
        {
            _rest = new REST();
            _mqtt = new MQTT();
            _mqtt.RunExample();
            soap = new SOAP();

            while (true)
            {
                //Warehouse
                soap.PickAndInsertItem();
                
                _rest.CheckBattery();
                
                _rest.ChooseOperation(1);//MoveToStorageOperation
                _rest.execute();
                _rest.GetStatus();
                Thread.Sleep(8000);
                _rest.ChooseOperation(2);//PickWarehouseOperation
                _rest.execute();
                _rest.GetStatus();
                Thread.Sleep(8000);
                _rest.ChooseOperation(4);//MoveToAssemblyOperation
                _rest.execute();
                _rest.GetStatus();
                Thread.Sleep(8000);

                _mqtt.Idle();// idle state:
                Thread.Sleep(8000);
            
                _rest.ChooseOperation(5);//PutAssemblyOperation
                _rest.execute();
                _rest.GetStatus();
            
                Thread.Sleep(8000);
                
                //Assembly Line
                _mqtt.Execution();//execution state
                Thread.Sleep(9000);

                _rest.ChooseOperation(6);//PickAssemblyOperation
                _rest.execute();
                _rest.GetStatus();
                Thread.Sleep(8000);
                _rest.ChooseOperation(1);//MoveToStorageOperation
                _rest.execute();
                _rest.GetStatus();
                Thread.Sleep(8000);
                _rest.ChooseOperation(3);//PutWarehouseOperation
                _rest.execute();
                _rest.GetStatus();
                Thread.Sleep(8000);
                _rest.GetStatus();
                
                Thread.Sleep(200);
                Console.WriteLine("Press 1 to stop the production or 2 to continue the production");
                if (Console.ReadLine() == "1")
                {
                    Environment.Exit(0);
                }
            }
        }
    }
}