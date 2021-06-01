using System;
using System.Text;
using grafo.Models;
using System.Collections.Generic;

namespace grafo
{
    class Program
    {
        /*
        1. Matrice delle distanza - in un file
        2. Classe grafo ((Nodo[], int costo) PercorsoMinimo(Nodo in, Nodo out))
        */
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Bartolini Liam - grafo - Dijkstra");
            Grafo grafo = new Grafo();
            grafo.CaricaGrafo("grafoDaCaricare.csv");

            Console.WriteLine(grafo.StampaMatrice(grafo.CalcolaMatriceDistanze()));
            (List<Nodo>, int) ret = grafo.Dijkstra(grafo["1"], grafo["5"]);
            
            StringBuilder sb = new();
            ret.Item1.ForEach(x => sb.AppendLine($"\t{ x }"));

            Console.WriteLine($"Numero di nodi: { ret.Item1.Count }\nNodi:\n{sb}Costo: {ret.Item2}");
        }
    }
}