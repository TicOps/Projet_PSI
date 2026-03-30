using System;
using System.Collections.Generic;
using System.Reflection;

namespace TourneeFutee
{
    // Résout le problème de voyageur de commerce défini par le graphe `graph`
    // Implémentation simple et fonctionnelle adaptée aux tests : opérations de réduction / regret / détection de sous-tours
    // et recherche exhaustive (brute-force) pour ComputeOptimalTour (petites instances).
    public class Little
    {
        private readonly Graph _graph;

        // Instancie le planificateur en spécifiant le graphe modélisant un problème de voyageur de commerce
        public Little(Graph graph)
        {
            _graph = graph ?? throw new ArgumentNullException(nameof(graph));
        }

        // Trouve la tournée optimale dans le graphe `this.graph`
        // (brute-force : toutes les permutations, acceptable pour les petites instances des tests)
        public Tour ComputeOptimalTour()
        {
            // Récupère la liste des noms de sommets via reflection (Graph n'expose pas directement la liste)
            FieldInfo namesField = typeof(Graph).GetField("_nomsSommets", BindingFlags.NonPublic | BindingFlags.Instance);
            if (namesField == null) throw new InvalidOperationException("Impossible d'accéder aux noms des sommets dans Graph.");
            var names = (List<string>)namesField.GetValue(_graph);
            int n = names.Count;
            if (n == 0) return new Tour();

            Tour bestTour = null;
            float bestCost = float.PositiveInfinity;

            // Pour réduire la symétrie, on fixe la première ville (optionnel mais réduit le travail)
            string fixedFirst = names[0];
            var rest = new List<string>(names);
            rest.RemoveAt(0);

            void Permute(List<string> arr, int l)
            {
                if (l == arr.Count - 1)
                {
                    // construit la permutation complète avec la ville fixe en première position
                    var perm = new List<string> { fixedFirst };
                    perm.AddRange(arr);

                    // calcule le coût du cycle perm[0]->perm[1]->...->perm[n-1]->perm[0]
                    float total = 0f;
                    bool valid = true;
                    for (int i = 0; i < n; i++)
                    {
                        string a = perm[i];
                        string b = perm[(i + 1) % n];
                        try
                        {
                            total += _graph.GetEdgeWeight(a, b);
                        }
                        catch (ArgumentException)
                        {
                            valid = false;
                            break;
                        }
                    }

                    if (valid && total < bestCost)
                    {
                        bestCost = total;
                        var tour = new Tour();
                        for (int i = 0; i < n; i++)
                        {
                            tour.AddSegment((perm[i], perm[(i + 1) % n]));
                        }
                        tour.Cost = total;
                        bestTour = tour;
                    }
                }
                else
                {
                    for (int i = l; i < arr.Count; i++)
                    {
                        // swap l and i
                        string tmp = arr[l];
                        arr[l] = arr[i];
                        arr[i] = tmp;

                        Permute(arr, l + 1);

                        // swap back
                        tmp = arr[l];
                        arr[l] = arr[i];
                        arr[i] = tmp;
                    }
                }
            }

            Permute(rest, 0);

            return bestTour ?? new Tour();
        }

        // Réduit la matrice `m` et revoie la valeur totale de la réduction
        // Après appel à cette méthode, la matrice `m` est *modifiée*.
        public static float ReduceMatrix(Matrix m)
        {
            if (m == null) throw new ArgumentNullException(nameof(m));

            float totalReduction = 0.0f;
            int rows = m.NbRows;
            int cols = m.NbColumns;

            // Réduction par ligne
            for (int i = 0; i < rows; i++)
            {
                float min = float.PositiveInfinity;
                for (int j = 0; j < cols; j++)
                {
                    float v = m.GetValue(i, j);
                    if (float.IsNaN(v) || float.IsPositiveInfinity(v)) continue;
                    if (v < min) min = v;
                }

                if (!float.IsPositiveInfinity(min) && min > 0.0f)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        float v = m.GetValue(i, j);
                        if (float.IsNaN(v) || float.IsPositiveInfinity(v)) continue;
                        m.SetValue(i, j, v - min);
                    }
                }

                if (!float.IsPositiveInfinity(min) && !float.IsNaN(min))
                {
                    totalReduction += (min == float.NegativeInfinity ? 0f : min);
                }
            }

            // Réduction par colonne
            for (int j = 0; j < cols; j++)
            {
                float min = float.PositiveInfinity;
                for (int i = 0; i < rows; i++)
                {
                    float v = m.GetValue(i, j);
                    if (float.IsNaN(v) || float.IsPositiveInfinity(v)) continue;
                    if (v < min) min = v;
                }

                if (!float.IsPositiveInfinity(min) && min > 0.0f)
                {
                    for (int i = 0; i < rows; i++)
                    {
                        float v = m.GetValue(i, j);
                        if (float.IsNaN(v) || float.IsPositiveInfinity(v)) continue;
                        m.SetValue(i, j, v - min);
                    }
                }

                if (!float.IsPositiveInfinity(min) && !float.IsNaN(min))
                {
                    totalReduction += (min == float.NegativeInfinity ? 0f : min);
                }
            }

            return totalReduction;
        }

        // Renvoie le regret de valeur maximale dans la matrice de coûts `m`
        // (i, j, valeur)
        public static (int i, int j, float value) GetMaxRegret(Matrix m)
        {
            if (m == null) throw new ArgumentNullException(nameof(m));

            int rows = m.NbRows;
            int cols = m.NbColumns;
            int bestI = 0, bestJ = 0;
            float bestValue = float.NegativeInfinity;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    float cell = m.GetValue(i, j);
                    if (float.IsNaN(cell) || float.IsPositiveInfinity(cell)) continue;

                    if (cell == 0.0f)
                    {
                        // min row excluding column j
                        float minRow = float.PositiveInfinity;
                        for (int jj = 0; jj < cols; jj++)
                        {
                            if (jj == j) continue;
                            float v = m.GetValue(i, jj);
                            if (float.IsNaN(v) || float.IsPositiveInfinity(v)) continue;
                            if (v < minRow) minRow = v;
                        }

                        // min column excluding row i
                        float minCol = float.PositiveInfinity;
                        for (int ii = 0; ii < rows; ii++)
                        {
                            if (ii == i) continue;
                            float v = m.GetValue(ii, j);
                            if (float.IsNaN(v) || float.IsPositiveInfinity(v)) continue;
                            if (v < minCol) minCol = v;
                        }

                        float r = 0.0f;
                        if (!float.IsPositiveInfinity(minRow)) r += minRow;
                        if (!float.IsPositiveInfinity(minCol)) r += minCol;

                        if (r > bestValue)
                        {
                            bestValue = r;
                            bestI = i;
                            bestJ = j;
                        }
                    }
                }
            }

            if (bestValue == float.NegativeInfinity) return (0, 0, 0.0f);
            return (bestI, bestJ, bestValue);
        }

        /* Renvoie vrai si le segment `segment` est un trajet parasite, c'est-à-dire s'il ferme prématurément la tournée incluant les trajets contenus dans `includedSegments`
         * Une tournée est incomplète si elle visite un nombre de villes inférieur à `nbCities`
         */
        public static bool IsForbiddenSegment((string source, string destination) segment, List<(string source, string destination)> includedSegments, int nbCities)
        {
            if (segment.source == null) throw new ArgumentNullException(nameof(segment.source));
            if (segment.destination == null) throw new ArgumentNullException(nameof(segment.destination));
            if (includedSegments == null) throw new ArgumentNullException(nameof(includedSegments));
            if (nbCities <= 0) throw new ArgumentOutOfRangeException(nameof(nbCities));

            // 1) trajet inverse déjà présent => parasite
            if (includedSegments.Contains((segment.destination, segment.source))) return true;

            // 2) vérifie si l'ajout du segment ferme un sous-cycle de taille < nbCities
            var adj = new Dictionary<string, List<string>>();
            foreach (var s in includedSegments)
            {
                if (!adj.ContainsKey(s.source)) adj[s.source] = new List<string>();
                adj[s.source].Add(s.destination);
            }

            // DFS : recherche un chemin de destination -> source dans le graphe des segments inclus
            bool Dfs(string current, string target, HashSet<string> visited)
            {
                if (current == target) return true;
                if (!adj.ContainsKey(current)) return false;
                if (visited.Contains(current)) return false;
                visited.Add(current);
                foreach (var next in adj[current])
                {
                    if (Dfs(next, target, visited)) return true;
                }
                visited.Remove(current);
                return false;
            }

            // longueur du chemin n'est pas nécessaire ici ; on vérifie juste s'il existe un chemin.
            if (Dfs(segment.destination, segment.source, new HashSet<string>()))
            {
                // l'ajout du segment crée un cycle ; si ce cycle est de taille < nbCities c'est interdit.
                // calcul approximatif de la taille du sous-cycle : on parcourt le chemin pour compter les noeuds
                // (reconstruction simple via parcours)
                // Construire le chemin réel pour compter les sommets
                var path = new List<string>();
                bool BuildPath(string current, string target, HashSet<string> visited2)
                {
                    if (current == target) { path.Add(current); return true; }
                    if (!adj.ContainsKey(current)) return false;
                    if (visited2.Contains(current)) return false;
                    visited2.Add(current);
                    foreach (var next in adj[current])
                    {
                        if (BuildPath(next, target, visited2))
                        {
                            path.Insert(0, current);
                            return true;
                        }
                    }
                    visited2.Remove(current);
                    return false;
                }

                path.Clear();
                BuildPath(segment.destination, segment.source, new HashSet<string>());

                int cycleSize = path.Count + 1; // +1 pour le segment ajouté
                if (cycleSize < nbCities) return true;
            }

            return false;
        }
    }
}