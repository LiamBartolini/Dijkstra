using System;
using System.Text;
using grafo.Models;
using System.Collections.Generic;

namespace grafo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // per visualizzare il segno dell'infinito
            Console.WriteLine("Bartolini Liam - grafo - Dijkstra");
            Grafo grafo = new();
            grafo.CaricaGrafo("grafoDaCaricare.csv");

            Console.WriteLine(grafo.StampaMatrice(grafo.CalcolaMatriceDistanze()));
            (List<Nodo>, int) dijkstra = grafo.Dijkstra(grafo["1"], grafo["4"]);
            
            StringBuilder elencoDeiNodi = new();
            dijkstra.Item1.ForEach(x => elencoDeiNodi.AppendLine($"\t{ x }"));

            Console.WriteLine($"Numero di nodi: { dijkstra.Item1.Count }\nNodi:\n{elencoDeiNodi}Costo: {dijkstra.Item2}");

            try { grafo.SalvaMatriceDistanze("matriceDistanze.txt"); }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }
    }
}