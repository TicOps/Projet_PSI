using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace TourneeFutee
{
    public class ServicePersistance
    {
        private readonly string _connectionString;

        public ServicePersistance(string serverIp, string dbname, string user, string pwd)
        {
            _connectionString =
                $"server={serverIp};database={dbname};uid={user};pwd={pwd};";

            // Test connexion immédiat
            using (var conn = OpenConnection())
            {
                conn.Close();
            }
        }

        // SAUVEGARDE GRAPHE
        public uint SaveGraph(Graph g)
        {
            using (var conn = OpenConnection())
            {
                // 1) INSERT Graphe
                string sqlGraph =
                    "INSERT INTO Graphe(est_oriente, nb_sommets) " +
                    "VALUES(@o,@n); SELECT LAST_INSERT_ID();";

                var cmdGraph = new MySqlCommand(sqlGraph, conn);
                cmdGraph.Parameters.AddWithValue("@o", g.IsOriented ? 1 : 0);
                cmdGraph.Parameters.AddWithValue("@n", g.VertexCount);

                uint graphId = Convert.ToUInt32(cmdGraph.ExecuteScalar());

                // 2) Sommets
                Dictionary<int, uint> ids = new Dictionary<int, uint>();

                for (int i = 0; i < g.VertexCount; i++)
                {
                    string sqlSommet =
                        "INSERT INTO Sommet(graphe_id, nom, valeur, indice) " +
                        "VALUES(@gid,@nom,@val,@ind); SELECT LAST_INSERT_ID();";

                    var cmdSommet = new MySqlCommand(sqlSommet, conn);
                    cmdSommet.Parameters.AddWithValue("@gid", graphId);
                    cmdSommet.Parameters.AddWithValue("@nom", "S" + i);
                    cmdSommet.Parameters.AddWithValue("@val", 0);
                    cmdSommet.Parameters.AddWithValue("@ind", i);

                    uint sommetId = Convert.ToUInt32(cmdSommet.ExecuteScalar());
                    ids[i] = sommetId;
                }

                // 3) Arcs
                for (int i = 0; i < g.VertexCount; i++)
                {
                    for (int j = 0; j < g.VertexCount; j++)
                    {
                        float poids = g.Matrix.GetValue(i, j);

                        if (poids != float.PositiveInfinity && i != j)
                        {
                            string sqlArc =
                                "INSERT INTO Arc(graphe_id,sommet_source,sommet_dest,poids) " +
                                "VALUES(@gid,@s,@d,@p)";

                            var cmdArc = new MySqlCommand(sqlArc, conn);
                            cmdArc.Parameters.AddWithValue("@gid", graphId);
                            cmdArc.Parameters.AddWithValue("@s", ids[i]);
                            cmdArc.Parameters.AddWithValue("@d", ids[j]);
                            cmdArc.Parameters.AddWithValue("@p", poids);

                            cmdArc.ExecuteNonQuery();
                        }
                    }
                }

                return graphId;
            }
        }

        // CHARGER GRAPHE
        public Graph LoadGraph(uint id)
        {
            using (var conn = OpenConnection())
            {
                bool oriented = false;
                int nb = 0;

                string sql1 = "SELECT est_oriente, nb_sommets FROM Graphe WHERE id=@id";
                var cmd1 = new MySqlCommand(sql1, conn);
                cmd1.Parameters.AddWithValue("@id", id);

                using (var reader = cmd1.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        oriented = Convert.ToBoolean(reader["est_oriente"]);
                        nb = Convert.ToInt32(reader["nb_sommets"]);
                    }
                }

                Graph g = new Graph(nb, oriented);

                Dictionary<uint, int> map = new Dictionary<uint, int>();

                string sql2 =
                    "SELECT id, indice FROM Sommet " +
                    "WHERE graphe_id=@id ORDER BY indice";

                var cmd2 = new MySqlCommand(sql2, conn);
                cmd2.Parameters.AddWithValue("@id", id);

                using (var reader = cmd2.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        uint sid = Convert.ToUInt32(reader["id"]);
                        int indice = Convert.ToInt32(reader["indice"]);
                        map[sid] = indice;
                    }
                }

                string sql3 =
                    "SELECT sommet_source, sommet_dest, poids " +
                    "FROM Arc WHERE graphe_id=@id";

                var cmd3 = new MySqlCommand(sql3, conn);
                cmd3.Parameters.AddWithValue("@id", id);

                using (var reader = cmd3.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        uint s = Convert.ToUInt32(reader["sommet_source"]);
                        uint d = Convert.ToUInt32(reader["sommet_dest"]);
                        float p = Convert.ToSingle(reader["poids"]);

                        g.Matrix.SetValue(map[s], map[d], p);
                    }
                }

                return g;
            }
        }

        // SAUVEGARDE TOURNEE
        public uint SaveTour(uint graphId, Tour t)
        {
            using (var conn = OpenConnection())
            {
                string sqlTour =
                    "INSERT INTO Tournee(graphe_id, cout_total) " +
                    "VALUES(@g,@c); SELECT LAST_INSERT_ID();";

                var cmdTour = new MySqlCommand(sqlTour, conn);
                cmdTour.Parameters.AddWithValue("@g", graphId);
                cmdTour.Parameters.AddWithValue("@c", t.TotalCost);

                uint tourId = Convert.ToUInt32(cmdTour.ExecuteScalar());

                for (int i = 0; i < t.Path.Count; i++)
                {
                    string sql =
                        "INSERT INTO EtapeTournee(tournee_id, numero_ordre, sommet_id) " +
                        "SELECT @tid,@ord,id FROM Sommet " +
                        "WHERE graphe_id=@gid AND indice=@ind";

                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@tid", tourId);
                    cmd.Parameters.AddWithValue("@ord", i);
                    cmd.Parameters.AddWithValue("@gid", graphId);
                    cmd.Parameters.AddWithValue("@ind", t.Path[i]);

                    cmd.ExecuteNonQuery();
                }

                return tourId;
            }
        }

        // CHARGER TOURNEE
        public Tour LoadTour(uint id)
        {
            using (var conn = OpenConnection())
            {
                float cout = 0;

                string sql1 =
                    "SELECT cout_total FROM Tournee WHERE id=@id";

                var cmd1 = new MySqlCommand(sql1, conn);
                cmd1.Parameters.AddWithValue("@id", id);

                using (var reader = cmd1.ExecuteReader())
                {
                    if (reader.Read())
                        cout = Convert.ToSingle(reader["cout_total"]);
                }

                List<int> chemin = new List<int>();

                string sql2 =
                    "SELECT s.indice " +
                    "FROM EtapeTournee e " +
                    "JOIN Sommet s ON e.sommet_id = s.id " +
                    "WHERE e.tournee_id=@id " +
                    "ORDER BY e.numero_ordre";

                var cmd2 = new MySqlCommand(sql2, conn);
                cmd2.Parameters.AddWithValue("@id", id);

                using (var reader = cmd2.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        chemin.Add(Convert.ToInt32(reader["indice"]));
                    }
                }

                Tour t = new Tour();
                t.Path = chemin;
                t.TotalCost = cout;

                return t;
            }
        }

        // CONNEXION
        private MySqlConnection OpenConnection()
        {
            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}