# Projet_PSI
Projet Problème Scientifique A2 - Membres : Heinz Ruben, Dormeux Enzo, Haime Ahsir

# Commandes github
pour envoyer sur le serveur distant : 
sauvegarder les fichiers modifiers: ctrl+s
``` bash
git add . 
```
git commit -m"ajout du fichier gitignore"
``` bash
git push
```
commande écraser en récupérant les fichiers du git:
``` bash
git fetch origin 
git reset --hard origin/main
```
``` bash
git pull
```

# Identification des collections
- Tableau 2D classique (tab[,]) : Très simple mais de taille fixe.
- Tableau de tableaux (tab[][]) : Permet une certaine flexibilité, mais peut être plus complexe à gérer au niveau de l'accès mémoire.
- Liste de listes (List<List<T>>) : Offre une grande flexibilité et des fonctionnalités supplémentaires, mais peut être plus complexe à implémenter.

# Complexité des opérations (m = lignes, n = colonnes)

| Opération | Tableau 2D `T[,]` | Tableau de tableaux `T[][]` | `List<List<T>>` |
|---------|------------------|-----------------------------|-----------------|
| Accès `[i, j]` | O(1) | O(1) | O(1) |
| Ajout ligne (fin) | O(m × n) | O(m + n) | O(n) |
| Ajout ligne (début / milieu) | O(m × n) | O(m + n) | O(m + n) |
| Ajout colonne (fin) | O(m × n) | O(m × n) | O(m) *(amorti)* |
| Suppression ligne | O(m × n) | O(m + n) | O(m) |
| Suppression colonne (fin) | O(m × n) | O(m × n) | O(m) *(amorti)* |
| Suppression colonne (début / milieu) | O(m × n) | O(m × n) | O(m × n) |


# Points clés de complexité
- L’accès aux éléments est en **O(1)** pour toutes les structures.
- `List<List<T>>` est la structure la plus flexible pour des données dynamiques, en particulier pour l’ajout de lignes et de colonnes.
