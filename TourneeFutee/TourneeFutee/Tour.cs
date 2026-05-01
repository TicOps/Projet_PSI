using System;
using System.Collections.Generic;

namespace TourneeFutee
{
    // Modélise une tournée
    public class Tour
    {
        // segments texte (compatibilité ancien code)
        private readonly List<(string source, string destination)> _segments;

        // chemin par indices (utile BDD / algo Little)
        public List<int> Path { get; set; }

        // Constructeur vide
        public Tour()
        {
            _segments = new List<(string source, string destination)>();
            Path = new List<int>();
            Cost = 0.0f;
        }

        // Constructeur pratique : liste des sommets + coût
        public Tour(List<int> path, float cost)
        {
            _segments = new List<(string source, string destination)>();
            Path = path;
            Cost = cost;
        }

        // compatibilité si on passe List<string>
        public Tour(List<string> list, float v)
        {
            _segments = new List<(string source, string destination)>();
            Path = new List<int>();
            Cost = v;

            Vertices = list;
        }

        // coût principal
        public float Cost { get; set; }

        // alias utilisé dans ServicePersistance
        public float TotalCost
        {
            get { return Cost; }
            set { Cost = value; }
        }

        // nombre de segments texte
        public int NbSegments
        {
            get { return _segments.Count; }
        }

        // ancienne propriété conservée
        public IList<string> Vertices { get; set; }

        // ajoute sommet au chemin
        public void AddVertex(int index)
        {
            Path.Add(index);
        }

        // ajoute un segment texte
        public void AddSegment((string source, string destination) segment)
        {
            if (segment.source == null)
                throw new ArgumentNullException(nameof(segment.source));

            if (segment.destination == null)
                throw new ArgumentNullException(nameof(segment.destination));

            _segments.Add(segment);
        }

        // vérifie présence segment
        public bool ContainsSegment((string source, string destination) segment)
        {
            return _segments.Contains(segment);
        }

        // affichage simple
        public void Print()
        {
            Console.WriteLine("Coût total : " + Cost);

            // affichage Path si utilisé
            if (Path.Count > 0)
            {
                Console.WriteLine("Chemin :");

                for (int i = 0; i < Path.Count; i++)
                {
                    Console.Write(Path[i]);

                    if (i < Path.Count - 1)
                        Console.Write(" -> ");
                }

                Console.WriteLine();
            }

            // affichage segments texte si utilisés
            if (_segments.Count > 0)
            {
                Console.WriteLine("Trajets :");

                for (int i = 0; i < _segments.Count; i++)
                {
                    var s = _segments[i];
                    Console.WriteLine($"{i + 1}: {s.source} -> {s.destination}");
                }
            }
        }

        public List<(string source, string destination)> GetSegments()
        {
            return new List<(string source, string destination)>(_segments);
        }
    }
}