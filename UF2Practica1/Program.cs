using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.IO;

namespace UF2Practica1
{
	class MainClass
	{
		//Valors constants
		#region Constants
		const int nCaixeres = 3;

		#endregion
		/* Cua concurrent
		 	Dos mètodes bàsics: 
		 		Cua.Enqueue per afegir a la cua
		 		bool success = Cua.TryDequeue(out clientActual) per extreure de la cua i posar a clientActual
		*/

		public static ConcurrentQueue<Client> cua = new ConcurrentQueue<Client>();

		public static void Main(string[] args)
		{
            var clock = new Stopwatch();
			var tasks = new List<Task>();
            var caixeres = 3;

            try
			{
				using (StreamReader sr = new StreamReader("clients.csv"))
				{

                    //Llegim la primera línia que conté les capçaleres
                    var header = sr.ReadLine();
                    while (sr.Peek() != -1)
    					{
                        
       						string line = sr.ReadLine();
        					var values = line.Split(',');
                    				var tmp = new Client() { nom = values[0], carretCompra = Int32.Parse(values[1]) };
                    				cua.Enqueue(tmp);


                    }
				}
			}
			catch (Exception)
			{
				Console.WriteLine("Error accedint a l'arxiu");
				Console.ReadKey();
				Environment.Exit(0);
			}

            clock.Start();

            for (int i = 1; i <= caixeres; i++)
            {
                var caixera = new Caixera() { idCaixera = i };
                tasks.Add(Task.Run(() => caixera.ProcessarCua(cua)));
            }
            Task.WaitAll(tasks.ToArray());

            Console.WriteLine($"** Final Processant cues **");

            clock.Stop();
			double temps = clock.ElapsedMilliseconds / 1000;
			// Console.Clear();
			Console.WriteLine("Temps total Task: " + temps + " segons");
			Console.ReadKey();
		}
	}
	#region ClassCaixera
	public class Caixera
	{
		public int idCaixera
		{
			get;
			set;
		}


        public void ProcessarCua(ConcurrentQueue<Client> cua)
		{
            Client actualClient;
            bool dequeued = cua.TryDequeue(out actualClient);
            while (dequeued)
            {
                ProcesarCompra(actualClient);
                dequeued = cua.TryDequeue(out actualClient);
            }
              
        }


		private void ProcesarCompra(Client client)
		{
            Console.WriteLine("La caixera "+idCaixera+" comenca amb el client "+client.nom+" que te "+client.carretCompra+" productes");
            for (int i = 0; i <= client.carretCompra; i++)
            {
                ProcessaProducte();
            }
            Console.WriteLine(">>>> La caixera " + idCaixera + " ha acabat amb el client " + client.nom);
        }



        private void ProcessaProducte()
		{
            Thread.Sleep(TimeSpan.FromSeconds(1));

		}
	}


	#endregion

	#region ClassClient

	public class Client
	{
		public string nom
		{
			get;
			set;
		}


		public int carretCompra
		{
			get;
			set;
		}


	}

	#endregion
}
