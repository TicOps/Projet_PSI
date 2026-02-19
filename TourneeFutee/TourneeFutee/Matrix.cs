using System;
using System.Collections.Generic;

namespace TourneeFutee
{
    public class Matrix
    {
        // Attributs
        private readonly float _defaultValue;               // valeur par défaut pour nouvelles cellules
        private List<List<float>> _cells;                    // stockage dynamique par lignes
        private int _nbRows;                                 // nombre actuel de lignes
        private int _nbColumns;                              // nombre actuel de colonnes

        /* Crée une matrice de dimensions `nbRows` x `nbColums`.
         * Toutes les cases de cette matrice sont remplies avec `defaultValue`.
         * Lève une ArgumentOutOfRangeException si une des dimensions est négative
         */
        public Matrix(int nbRows = 0, int nbColumns = 0, float defaultValue = 0)
        {
            if (nbRows < 0) throw new ArgumentOutOfRangeException(nameof(nbRows), "nbRows ne peut pas être négatif");
            if (nbColumns < 0) throw new ArgumentOutOfRangeException(nameof(nbColumns), "nbColumns ne peut pas être négatif");

            _defaultValue = defaultValue;
            _nbRows = nbRows;
            _nbColumns = nbColumns;

            _cells = new List<List<float>>(nbRows);
            for (int i = 0; i < nbRows; i++)
            {
                var row = new List<float>(nbColumns);
                for (int j = 0; j < nbColumns; j++)
                {
                    row.Add(defaultValue);
                }
                _cells.Add(row);
            }
        }

        // Propriété : valeur par défaut utilisée pour remplir les nouvelles cases
        // Lecture seule
        public float DefaultValue
        {
            get { return _defaultValue; }
        }

        // Propriété : nombre de lignes
        // Lecture seule
        public int NbRows
        {
            get { return _nbRows; }
        }

        // Propriété : nombre de colonnes
        // Lecture seule
        public int NbColumns
        {
            get { return _nbColumns; }
        }

        /* Insère une ligne à l'indice `i`. Décale les lignes suivantes vers le bas.
         * Toutes les cases de la nouvelle ligne contiennent DefaultValue.
         * Si `i` = NbRows, insère une ligne en fin de matrice
         * Lève une ArgumentOutOfRangeException si `i` est en dehors des indices valides
         */
        public void AddRow(int i)
        {
            // Vérifier l'indice : il est valide si 0 <= i <= _nbRows (insertion en fin autorisée)
            if (i < 0 || i > _nbRows)
            {
                throw new ArgumentOutOfRangeException(nameof(i), "i est en dehors des indices valides");
            }

            // Créer la nouvelle ligne remplie avec la valeur par défaut
            var newRow = new List<float>(_nbColumns);
            for (int col = 0; col < _nbColumns; col++)
            {
                newRow.Add(_defaultValue);
            }

            // Insérer la ligne à la position demandée
            _cells.Insert(i, newRow);

            // Mettre à jour le nombre de lignes
            _nbRows++;
        }

        /* Insère une colonne à l'indice `j`. Décale les colonnes suivantes vers la droite.
         * Toutes les cases de la nouvelle ligne contiennent DefaultValue.
         * Si `j` = NbColums, insère une colonne en fin de matrice
         * Lève une ArgumentOutOfRangeException si `j` est en dehors des indices valides
         */
        public void AddColumn(int j)
        {
            // Indice valide : 0 <= j <= _nbColumns (insertion en fin autorisée)
            if (j < 0 || j > _nbColumns)
            {
                throw new ArgumentOutOfRangeException(nameof(j), "j est en dehors des indices valides");
            }

            // Si pas de lignes, on met à jour seulement le compteur de colonnes
            if (_nbRows == 0)
            {
                _nbColumns++;
                return;
            }

            // Insérer la valeur par défaut dans chaque ligne à la position j
            foreach (var row in _cells)
            {
                // row.Count devrait être égal à _nbColumns, mais on protège contre les incohérences
                if (j >= 0 && j <= row.Count)
                {
                    row.Insert(j, _defaultValue);
                }
                else
                {
                    // Cas improbable : normaliser la ligne pour garantir la bonne taille puis insérer
                    while (row.Count < _nbColumns)
                        row.Add(_defaultValue);
                    row.Insert(j, _defaultValue);
                }
            }

            _nbColumns++;
        }

        // Supprime la ligne à l'indice `i`. Décale les lignes suivantes vers le haut.
        // Lève une ArgumentOutOfRangeException si `i` est en dehors des indices valides
        public void RemoveRow(int i)
        {
            if (i < 0 || i >= _nbRows)
            {
                throw new ArgumentOutOfRangeException(nameof(i), "i est en dehors des indices valides");
            }

            _cells.RemoveAt(i);
            _nbRows--;
        }

        // Supprime la colonne à l'indice `j`. Décale les colonnes suivantes vers la gauche.
        // Lève une ArgumentOutOfRangeException si `j` est en dehors des indices valides
        public void RemoveColumn(int j)
        {
            if (j < 0 || j >= _nbColumns)
            {
                throw new ArgumentOutOfRangeException(nameof(j), "j est en dehors des indices valides");
            }

            // Supprime la colonne pour chaque ligne
            foreach (var row in _cells)
            {
                // Protection : s'assurer que la ligne a suffisamment de colonnes
                if (j >= 0 && j < row.Count)
                {
                    row.RemoveAt(j);
                }
                else
                {
                    // Cas improbable d'incohérence entre _nbColumns et row.Count :
                    // on normalise la ligne pour éviter exceptions silencieuses
                    while (row.Count < _nbColumns)
                        row.Add(_defaultValue);
                    row.RemoveAt(j);
                }
            }

            _nbColumns--;
        }

        // Renvoie la valeur à la ligne `i` et colonne `j`
        // Lève une ArgumentOutOfRangeException si `i` ou `j` est en dehors des indices valides
        public float GetValue(int i, int j)
        {
            if (i < 0 || i >= _nbRows)
            {
                throw new ArgumentOutOfRangeException(nameof(i), "i est en dehors des indices valides");
            }
            if (j < 0 || j >= _nbColumns)
            {
                throw new ArgumentOutOfRangeException(nameof(j), "j est en dehors des indices valides");
            }

            return _cells[i][j];
        }

        // Affecte la valeur à la ligne `i` et colonne `j` à `v`
        // Lève une ArgumentOutOfRangeException si `i` ou `j` est en dehors des indices valides
        public void SetValue(int i, int j, float v)
        {
            if (i < 0 || i >= _nbRows)
            {
                throw new ArgumentOutOfRangeException(nameof(i), "i est en dehors des indices valides");
            }
            if (j < 0 || j >= _nbColumns)
            {
                throw new ArgumentOutOfRangeException(nameof(j), "j est en dehors des indices valides");
            }

            _cells[i][j] = v;
        }

        // Affiche la matrice
        public void Print()
        {
            if (_nbRows == 0)
            {
                Console.WriteLine("<matrice vide>");
                return;
            }

            for (int i = 0; i < _nbRows; i++)
            {
                var row = _cells[i];
                // S'assurer que la ligne contient le bon nombre de colonnes pour l'affichage
                while (row.Count < _nbColumns)
                    row.Add(_defaultValue);

                Console.WriteLine(string.Join(" ", row));
            }
        }


        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }


}