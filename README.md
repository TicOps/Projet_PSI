# Projet_PSI
Projet Problème Scientifique A2 - Membres : Heinz Ruben, Dormeux Enzo, Haime Ahsir

# commandes github
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
- Liste de listes (List<T>) : Offre une grande flexibilité et des fonctionnalités supplémentaires, mais peut être plus complexe à implémenter.

# Complexité des opérations
OpérationTableau 2D (T[,])Tableau de tableaux (T[][])Liste de listes (List<List<T>>)Accès [i, j]$O(1)$$O(1)$$O(1)$Ajout ligne (fin)$O(m \times n)$$O(m)$$O(1)$ (amorti)Ajout ligne (début/milieu)$O(m \times n)$$O(m)$$O(m)$Ajout colonne (fin)$O(m \times n)$$O(m \times n)$$O(m)$Suppression ligne$O(m \times n)$$O(m)$$O(m)$Suppression colonne$O(m \times n)$$O(m \times n)$$O(m \times n)$

# Justification du choix de la structure de données
Accès aux valeurs : Les trois structures offrent une complexité de $O(1)$.
Optimisation des opérations : La structure List<List<T>> minimise la complexité des ajouts de lignes (passage de $O(m \times n)$ à $O(1)$ ou $O(m)$) et facilite l'ajout de colonnes par rapport aux tableaux fixes.