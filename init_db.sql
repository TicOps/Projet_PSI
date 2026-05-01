-- =============================================================================
-- PSI 2025-2026 – Objectif 3 : Base de données
-- Script d'initialisation de la base de données TourneeFutee
-- =============================================================================
CREATE DATABASE IF NOT EXISTS tourneefutee;
USE tourneefutee;
DROP TABLE IF EXISTS EtapeTournee;
DROP TABLE IF EXISTS Tournee;
DROP TABLE IF EXISTS Arc;
DROP TABLE IF EXISTS Sommet;
DROP TABLE IF EXISTS Graphe;

-- =============================================================================
-- Table : Graphe
-- =============================================================================
CREATE TABLE Graphe (
    id           INT UNSIGNED    NOT NULL AUTO_INCREMENT,
    nom          VARCHAR(100)    NULL,
    est_oriente  TINYINT(1)      NOT NULL DEFAULT 0,
    nb_sommets   INT UNSIGNED    NOT NULL DEFAULT 0,

    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


-- =============================================================================
-- Table : Sommet
-- =============================================================================
CREATE TABLE Sommet (
    id          INT UNSIGNED    NOT NULL AUTO_INCREMENT,
    graphe_id   INT UNSIGNED    NOT NULL,
    nom         VARCHAR(50)     NOT NULL,
    valeur      FLOAT           NULL,
    indice      INT UNSIGNED    NOT NULL,

    PRIMARY KEY (id),

    UNIQUE (graphe_id, indice),

    FOREIGN KEY (graphe_id)
        REFERENCES Graphe(id)
        ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


-- =============================================================================
-- Table : Arc
-- =============================================================================
CREATE TABLE Arc (
    id              INT UNSIGNED    NOT NULL AUTO_INCREMENT,
    graphe_id       INT UNSIGNED    NOT NULL,
    sommet_source   INT UNSIGNED    NOT NULL,
    sommet_dest     INT UNSIGNED    NOT NULL,
    poids           FLOAT           NOT NULL,

    PRIMARY KEY (id),

    UNIQUE (graphe_id, sommet_source, sommet_dest),

    FOREIGN KEY (graphe_id)
        REFERENCES Graphe(id)
        ON DELETE CASCADE,

    FOREIGN KEY (sommet_source)
        REFERENCES Sommet(id)
        ON DELETE CASCADE,

    FOREIGN KEY (sommet_dest)
        REFERENCES Sommet(id)
        ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


-- =============================================================================
-- Table : Tournee
-- =============================================================================
CREATE TABLE Tournee (
    id           INT UNSIGNED    NOT NULL AUTO_INCREMENT,
    graphe_id    INT UNSIGNED    NOT NULL,
    cout_total   FLOAT           NOT NULL,
    date_calcule TIMESTAMP       DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (id),

    FOREIGN KEY (graphe_id)
        REFERENCES Graphe(id)
        ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


-- =============================================================================
-- Table : EtapeTournee
-- =============================================================================
CREATE TABLE EtapeTournee (
    tournee_id      INT UNSIGNED    NOT NULL,
    numero_ordre    INT UNSIGNED    NOT NULL,
    sommet_id       INT UNSIGNED    NOT NULL,

    PRIMARY KEY (tournee_id, numero_ordre),

    FOREIGN KEY (tournee_id)
        REFERENCES Tournee(id)
        ON DELETE CASCADE,

    FOREIGN KEY (sommet_id)
        REFERENCES Sommet(id)
        ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


SHOW TABLES;