using System;
using System.Collections.Generic;

namespace TourneeFutee
{
    // Modélise une tournée simple pour le problème du voyageur de commerce
    public class Tour
    {
        // Liste des segments (source -> destination)
        private readonly List<(string source, string destination)> _segments;

        // Constructeur par défaut
        public Tour()
        {
            _segments = new List<(string source, string destination)>();
            Cost = 0.0f;
        }

        // Coût total de la tournée (lecture/écriture simple)
        public float Cost { get; set; }

        // Nombre de trajets dans la tournée
        public int NbSegments
        {
            get { return _segments.Count; }
        }

        // Ajoute un segment à la tournée
        public void AddSegment((string source, string destination) segment)
        {
            if (segment.source == null) throw new ArgumentNullException(nameof(segment.source));
            if (segment.destination == null) throw new ArgumentNullException(nameof(segment.destination));
            _segments.Add(segment);
        }

        // Renvoie vrai si la tournée contient le trajet `source`->`destination`
        public bool ContainsSegment((string source, string destination) segment)
        {
            return _segments.Contains(segment);
        }

        // Affiche les informations sur la tournée : coût total et trajets
        public void Print()
        {
            Console.WriteLine($"Coût total : {Cost}");
            Console.WriteLine("Trajets :");
            for (int i = 0; i < _segments.Count; i++)
            {
                var s = _segments[i];
                Console.WriteLine($"  {i + 1}: {s.source} -> {s.destination}");
            }
        }

        // Fournit une copie des segments (utile pour inspections/tests)
        public List<(string source, string destination)> GetSegments()
        {
            return new List<(string source, string destination)>(_segments);
        }
    }
}
