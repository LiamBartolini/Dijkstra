using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace grafo.Models
{
    public class Grafo
    {
        public List<Nodo> Nodi;

        public Nodo PrimoNodo { get => Nodi[0]; }
        public Nodo UltimoNodo { get => Nodi[Nodi.Count - 1]; }

        public List<Ramo> Rami;

        public List<string> NodiStr;

        public int[,] MatriceDistanze { get; private set; } = new int[0, 0];

        public Grafo()
        {
            Nodi = new List<Nodo>();
            Rami = new List<Ramo>();
            NodiStr = new List<string>();
        }

        public (List<Nodo>, int costo) Dijkstra(Nodo partenza, Nodo arrivo)
        {
            /*
                        ~PSEUDOCODIFICA~
            Controllo tutti i nodi adiacenti al nodo corrente
            Guardo quale delle distanze da ogni nodo è la minore, 
            Salvo la distanza
            Aggiungo il nodo alla lista di nodi passati
            Il nodo corrente diventerà quel nodo li

            ripeto finché non raggiungo il nodo 'fine'
            */

            List<Nodo> toRet = new();
            int costoToRet = 0;
            Nodo corrente = partenza;
            List<int> distanzeTmp = new();
            int indiceNodoConDistanzaMinima = 0;

            toRet.Add(partenza);
            do
            {
                // Se dentro è tra i nodi adiacenti allora faccio i calcoli e lo ritorno
                if (RicercaNellaListaPerNome(corrente.NodiAdiacenti, arrivo.Nome) != null)
                {
                    Nodo nodoTmp = RicercaNellaListaPerNome(corrente.NodiAdiacenti, arrivo.Nome);

                    int i = Convert.ToInt32(corrente.Nome), j = Convert.ToInt32(arrivo.Nome);
                    costoToRet += MatriceDistanze[i + 1, j + 1];
                    toRet.Add(nodoTmp);
                    break;
                }

                foreach (Nodo nodoAdiacente in corrente.NodiAdiacenti)
                {
                    string nomeNodoAdiacente = nodoAdiacente.Nome;
                    int i = Convert.ToInt32(corrente.Nome), j = Convert.ToInt32(nodoAdiacente.Nome);
                    int distanza = (int)MatriceDistanze[i + 1, j + 1];
                    distanzeTmp.Add(distanza);

                    // Mi prendo l'indice del nodo con distanza minima
                    indiceNodoConDistanzaMinima = distanzeTmp.IndexOf(distanzeTmp.Min());
                }

                // Mi salvo l'indice del nodo con distanza minima
                Nodo nodoConDistanzaMinima = RicercaNellaListaPerNome(Nodi, corrente.NodiAdiacenti[indiceNodoConDistanzaMinima].Nome);
                // Salvo il nodo con distanza minima nella lista
                toRet.Add(nodoConDistanzaMinima);

                // Aggiorno il costo del percorso
                costoToRet += distanzeTmp[indiceNodoConDistanzaMinima];

                // Pulisco la lista
                distanzeTmp.Clear();

                // Il nodo corrente diventerà il nodo appena aggiunto
                corrente = nodoConDistanzaMinima;


                // Ripeto finché corrente.Nome != fine.Nome
            } while (corrente.Nome != arrivo.Nome);

            return (toRet, costoToRet);
        }

        public void CaricaGrafo(string fileName)
        {
            using (FileStream fin = new FileStream(fileName, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fin))
                {
                    while (!sr.EndOfStream)
                    {
                        string[] colonne = sr.ReadLine().Split(',');

                        // Se il primo carattere è un # allora è un commento e lo salto
                        if (colonne[0].StartsWith('#')) continue;

                        Nodo partenza = new Nodo(colonne[0]);
                        Nodo arrivo = new Nodo(colonne[1]);
                        Ramo ramo = new Ramo(partenza, arrivo, Convert.ToInt32(colonne[2]));

                        if (!NodiStr.Contains(colonne[0]))
                        {
                            Nodi.Add(partenza);
                            NodiStr.Add(colonne[0]);
                        }
                        else if (!NodiStr.Contains(colonne[1]))
                        {
                            Nodi.Add(arrivo);
                            NodiStr.Add(colonne[1]);
                        }

                        Rami.Add(ramo);
                    }
                }
            }
        }

        public int[,] CalcolaMatriceDistanze()
        {
            // Calcolo e salvo dentro un file la matrice            
            int[,] matriceDistanza = new int[Nodi.Count + 1, Nodi.Count + 1];
            InizializzaMatrice(ref matriceDistanza);

            // Crea il 'bordo' della tabella con i nomi dei nodi
            for (int i = 0; i < matriceDistanza.GetLength(0); matriceDistanza[0, i] = i - 1, i++) ;
            for (int j = 0; j < matriceDistanza.GetLength(1); matriceDistanza[j, 0] = j - 1, j++) ;

            foreach (Ramo ramo in Rami)
            {
                string partenzaString = ramo.Partenza.Nome;
                string arrivoString = ramo.Arrivo.Nome;

                int costo = ramo.Costo;
                int i = Convert.ToInt32(partenzaString), j = Convert.ToInt32(arrivoString);

                matriceDistanza[i + 1, j + 1] = costo;
            }

            MatriceDistanze = matriceDistanza;
            InserisciNodiAdiacenti(MatriceDistanze);
            SalvaMatriceDistanze("matriceDistanze.txt", FileMode.Open);
            return matriceDistanza;
        }

        public void SalvaMatriceDistanze(string fileName, FileMode fileMode = FileMode.Create)
        {
            using (FileStream fin = new FileStream(fileName, fileMode))
            {
                using (StreamWriter sw = new StreamWriter(fin))
                {
                    sw.WriteLine($"Matrice distanze {DateTime.Today:dddd dd/MMMM} {DateTime.Now:HH:mm}");
                    for (int i = 0; i < MatriceDistanze.GetLength(0); i++)
                    {
                        for (int j = 0; j < MatriceDistanze.GetLength(1); j++)
                            if (MatriceDistanze[i, j] == -1)
                                sw.Write($"∞".PadRight(3));
                            else 
                                sw.Write($"{ MatriceDistanze[i, j] }".PadRight(3));

                        sw.WriteLine("");
                    }
                    sw.Flush();
                }
            }
        }

        private void InserisciNodiAdiacenti(int[,] matrice)
        {
            List<Nodo> nodiAdiacenti = new();

            /*
            Controlla ogni riga della matrice delle distanze, appena è diversa da '-1'
            aggiunge il nodo ai nodi adiacenti del nodo corrente
            */
            for (int i = 1; i < matrice.GetLength(0); i++)
            {
                for (int j = 1; j < matrice.GetLength(1); j++)
                    if (matrice[i, j] != -1)
                        nodiAdiacenti.Add(this[(j - 1).ToString()]);

                Nodo nodo = RicercaNellaListaPerNome(Nodi, $"{ i - 1 }");
                nodo.AggiungiNodiAdiacenti(nodiAdiacenti);
                nodiAdiacenti.Clear();
            }
        }

        private void InizializzaMatrice(ref int[,] matrice)
        {
            for (int i = 0; i < matrice.GetLength(0); i++)
                for (int j = 0; j < matrice.GetLength(1); j++)
                    matrice[i, j] = -1;
        }

        public string StampaMatrice(int[,] matrice)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < matrice.GetLength(0); i++)
            {
                for (int j = 0; j < matrice.GetLength(1); j++)
                {
                    if (matrice[i, j] == -1)
                        sb.Append($"∞".PadRight(3));
                    else
                        sb.Append($"{ matrice[i, j] }".PadRight(3));
                }

                sb.AppendLine();
            }
            return sb.ToString();
        }

        private Nodo RicercaNellaListaPerNome(List<Nodo> lista, string daCercare)
        {
            foreach (Nodo nodo in lista)
                if (nodo.Nome == daCercare) return nodo;
            return null;
        }


        /// <summery>
        /// Permette di cercare un determinato nodo 
        /// all'interno del grafo usando il suo nome
        /// </summery>
        /// <returns>
        /// Il nodo se viene trovato, altrimenti ritorna null
        /// </returns>
        public Nodo this[string nomeNodo]
        {
            get
            {
                foreach (Nodo nodo in Nodi)
                    if (nodo.Nome == nomeNodo) return nodo;
                throw new Exception("Nodo non trovato");
            }
        }
    }
}