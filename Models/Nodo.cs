using System.Collections.Generic;

namespace grafo.Models
{
    public class Nodo
    {
        string _nome;

        List<Nodo> _nodiAdiacenti = null;        

        public List<Nodo> NodiAdiacenti { get => _nodiAdiacenti; }
        
        public string Nome { get => _nome; }

        public Nodo(string nome)
        {
            _nodiAdiacenti = new List<Nodo>();
            _nome = nome;
        }
        
        public void AggiungiNodiAdiacenti(List<Nodo> nodiAdiacenti) => _nodiAdiacenti.AddRange(nodiAdiacenti);

        public override string ToString() => _nome;
    }
}