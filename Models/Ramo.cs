namespace grafo.Models
{
    public class Ramo
    {
        Nodo _partenza;
        Nodo _arrivo;
        int _costo;

        public Nodo Partenza { get => _partenza; }
        public Nodo Arrivo { get => _arrivo; }
        public int Costo { get => _costo; }

        public Ramo(Nodo partenza, Nodo arrivo, int costo)
        {
            _partenza = partenza;
            _arrivo = arrivo;
            _costo = costo;
        }

        public override string ToString() => $"{_partenza} ---> {_arrivo} (costo {_costo})";
    }
}