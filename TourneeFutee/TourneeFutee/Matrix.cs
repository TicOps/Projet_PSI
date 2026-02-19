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
            // TODO : implémenter
        }

        // Supprime la ligne à l'indice `i`. Décale les lignes suivantes vers le haut.
        // Lève une ArgumentOutOfRangeException si `i` est en dehors des indices valides
        public void RemoveRow(int i)
        {
            // TODO : implémenter
        }

        // Supprime la colonne à l'indice `j`. Décale les colonnes suivantes vers la gauche.
        // Lève une ArgumentOutOfRangeException si `j` est en dehors des indices valides
        public void RemoveColumn(int j)
        {
            // TODO : implémenter
        }

        // Renvoie la valeur à la ligne `i` et colonne `j`
        // Lève une ArgumentOutOfRangeException si `i` ou `j` est en dehors des indices valides
        public float GetValue(int i, int j)
        {
            // TODO : implémenter
            return 0.0f;
        }

        // Affecte la valeur à la ligne `i` et colonne `j` à `v`
        // Lève une ArgumentOutOfRangeException si `i` ou `j` est en dehors des indices valides
        public void SetValue(int i, int j, float v)
        {
            // TODO : implémenter
        }

        // Affiche la matrice
        public void Print()
        {
            // TODO : implémenter
        }


        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }


}