using System;
using System.Collections.Generic;

namespace TourneeFutee
{
    public class Graph
    {
        private bool _directed;
        private Matrix _adjacency;
        private List<string> _nomsSommets;
        private Dictionary<string, int> _indicesSommets;
        private Dictionary<string, float> _valeursSommets;

        public Graph(bool directed)
        {
            _directed = directed;
            _adjacency = new Matrix(0, 0, float.NaN);
            _nomsSommets = new List<string>();
            _indicesSommets = new Dictionary<string, int>();
            _valeursSommets = new Dictionary<string, float>();
        }

        public bool IsDirected
        {
            get { return _directed; }
        }

        public int Order
        {
            get { return _nomsSommets.Count; }
        }

        private int GetIndex(string nom)
        {
            if (nom == null) throw new ArgumentNullException(nameof(nom));
            if (!_indicesSommets.ContainsKey(nom)) throw new ArgumentException("Le sommet n'existe pas : " + nom);

            return _indicesSommets[nom];
        }

        public void AddVertex(string name, float value = 0)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (_indicesSommets.ContainsKey(name)) throw new ArgumentException("Un sommet avec ce nom existe déjà.");

            int nouvelIndice = _nomsSommets.Count;

            _nomsSommets.Add(name);
            _indicesSommets.Add(name, nouvelIndice);
            _valeursSommets.Add(name, value);

            _adjacency.AddRow(nouvelIndice);
            _adjacency.AddColumn(nouvelIndice);
        }

        public void RemoveVertex(string name)
        {
            int index = GetIndex(name);

            _adjacency.RemoveRow(index);
            _adjacency.RemoveColumn(index);

            _nomsSommets.RemoveAt(index);
            _indicesSommets.Remove(name);
            _valeursSommets.Remove(name);

            for (int i = index; i < _nomsSommets.Count; i++)
            {
                _indicesSommets[_nomsSommets[i]] = i;
            }
        }

        public float GetVertexValue(string name)
        {
            GetIndex(name);
            return _valeursSommets[name];
        }

        public void SetVertexValue(string name, float value)
        {
            GetIndex(name);
            _valeursSommets[name] = value;
        }

        public void AddEdge(string sourceName, string destinationName, float weight = 0)
        {
            int i = GetIndex(sourceName);
            int j = GetIndex(destinationName);

            if (!float.IsNaN(_adjacency.GetValue(i, j)))
            {
                throw new ArgumentException("Cet arc existe déjà.");
            }

            _adjacency.SetValue(i, j, weight);

            if (!_directed && i != j)
            {
                _adjacency.SetValue(j, i, weight);
            }
        }

        public void RemoveEdge(string sourceName, string destinationName)
        {
            int i = GetIndex(sourceName);
            int j = GetIndex(destinationName);

            if (float.IsNaN(_adjacency.GetValue(i, j)))
            {
                throw new ArgumentException("Cet arc n'existe pas.");
            }

            _adjacency.SetValue(i, j, float.NaN);

            if (!_directed && i != j)
            {
                _adjacency.SetValue(j, i, float.NaN);
            }
        }

        public float GetEdgeWeight(string sourceName, string destinationName)
        {
            int i = GetIndex(sourceName);
            int j = GetIndex(destinationName);

            float poids = _adjacency.GetValue(i, j);

            if (float.IsNaN(poids))
            {
                throw new ArgumentException("Cet arc n'existe pas.");
            }

            return poids;
        }

        public void SetEdgeWeight(string sourceName, string destinationName, float weight)
        {
            int i = GetIndex(sourceName);
            int j = GetIndex(destinationName);

            if (float.IsNaN(_adjacency.GetValue(i, j)))
            {
                throw new ArgumentException("Cet arc n'existe pas.");
            }

            _adjacency.SetValue(i, j, weight);

            if (!_directed && i != j)
            {
                _adjacency.SetValue(j, i, weight);
            }
        }

        public List<string> GetNeighbors(string vertexName)
        {
            int i = GetIndex(vertexName);
            List<string> voisins = new List<string>();

            for (int j = 0; j < _adjacency.NbColumns; j++)
            {
                if (!float.IsNaN(_adjacency.GetValue(i, j)))
                {
                    voisins.Add(_nomsSommets[j]);
                }
            }

            return voisins;
        }
    }
}
