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
            _connectionString = $"server={serverIp};database={dbname};uid={user};pwd={pwd};";
            using (var conn = OpenConnection()) conn.Close();
        }

        public uint SaveGraph(Graph g)
        {
            using (var conn = OpenConnection())
            {
                var cmdGraph = new MySqlCommand(
                    "INSERT INTO Graphe(est_oriente, nb_sommets) VALUES(@o,@n); SELECT LAST_INSERT_ID();", conn);
                cmdGraph.Parameters.AddWithValue("@o", g.IsOriented ? 1 : 0);
                cmdGraph.Parameters.AddWithValue("@n", g.VertexCount);
                uint graphId = Convert.ToUInt32(cmdGraph.ExecuteScalar());

                var ids = new Dictionary<int, uint>();
                for (int i = 0; i < g.VertexCount; i++)
                {
                    string nom = g.GetVertexName(i);
                    var cmd = new MySqlCommand(
                        "INSERT INTO Sommet(graphe_id, nom, valeur, indice) VALUES(@gid,@nom,@val,@ind); SELECT LAST_INSERT_ID();", conn);
                    cmd.Parameters.AddWithValue("@gid", graphId);
                    cmd.Parameters.AddWithValue("@nom", nom);
                    cmd.Parameters.AddWithValue("@val", g.GetVertexValue(nom));
                    cmd.Parameters.AddWithValue("@ind", i);
                    ids[i] = Convert.ToUInt32(cmd.ExecuteScalar());
                }

                for (int i = 0; i < g.VertexCount; i++)
                    for (int j = 0; j < g.VertexCount; j++)
                    {
                        float p = g.Matrix.GetValue(i, j);
                        if (!float.IsNaN(p))
                        {
                            var cmd = new MySqlCommand(
                                "INSERT INTO Arc(graphe_id,sommet_source,sommet_dest,poids) VALUES(@gid,@s,@d,@p)", conn);
                            cmd.Parameters.AddWithValue("@gid", graphId);
                            cmd.Parameters.AddWithValue("@s", ids[i]);
                            cmd.Parameters.AddWithValue("@d", ids[j]);
                            cmd.Parameters.AddWithValue("@p", p);
                            cmd.ExecuteNonQuery();
                        }
                    }

                return graphId;
            }
        }

        public Graph LoadGraph(uint id)
        {
            using (var conn = OpenConnection())
            {
                var cmd1 = new MySqlCommand("SELECT est_oriente, nb_sommets FROM Graphe WHERE id=@id", conn);
                cmd1.Parameters.AddWithValue("@id", id);
                bool oriented = false;
                using (var r = cmd1.ExecuteReader())
                    if (r.Read()) oriented = Convert.ToBoolean(r["est_oriente"]);

                var g = new Graph(oriented);
                var map = new Dictionary<uint, int>();

                var cmd2 = new MySqlCommand("SELECT id, nom, valeur, indice FROM Sommet WHERE graphe_id=@id ORDER BY indice", conn);
                cmd2.Parameters.AddWithValue("@id", id);
                using (var r = cmd2.ExecuteReader())
                    while (r.Read())
                    {
                        uint sid = Convert.ToUInt32(r["id"]);
                        map[sid] = Convert.ToInt32(r["indice"]);
                        g.AddVertex(r["nom"].ToString(), Convert.ToSingle(r["valeur"]));
                    }

                var cmd3 = new MySqlCommand("SELECT sommet_source, sommet_dest, poids FROM Arc WHERE graphe_id=@id", conn);
                cmd3.Parameters.AddWithValue("@id", id);
                using (var r = cmd3.ExecuteReader())
                    while (r.Read())
                        g.Matrix.SetValue(map[Convert.ToUInt32(r["sommet_source"])], map[Convert.ToUInt32(r["sommet_dest"])], Convert.ToSingle(r["poids"]));

                return g;
            }
        }

        public uint SaveTour(uint graphId, Tour t)
        {
            using (var conn = OpenConnection())
            {
                var cmdTour = new MySqlCommand(
                    "INSERT INTO Tournee(graphe_id, cout_total) VALUES(@g,@c); SELECT LAST_INSERT_ID();", conn);
                cmdTour.Parameters.AddWithValue("@g", graphId);
                cmdTour.Parameters.AddWithValue("@c", t.TotalCost);
                uint tourId = Convert.ToUInt32(cmdTour.ExecuteScalar());

                List<int> indices = t.Path != null && t.Path.Count > 0
                    ? t.Path
                    : ResolveNamesToIndices(t.Vertices, graphId, conn);

                for (int i = 0; i < indices.Count; i++)
                {
                    var cmd = new MySqlCommand(
                        "INSERT INTO EtapeTournee(tournee_id, numero_ordre, sommet_id) SELECT @tid,@ord,id FROM Sommet WHERE graphe_id=@gid AND indice=@ind", conn);
                    cmd.Parameters.AddWithValue("@tid", tourId);
                    cmd.Parameters.AddWithValue("@ord", i);
                    cmd.Parameters.AddWithValue("@gid", graphId);
                    cmd.Parameters.AddWithValue("@ind", indices[i]);
                    cmd.ExecuteNonQuery();
                }

                return tourId;
            }
        }

        public Tour LoadTour(uint id)
        {
            using (var conn = OpenConnection())
            {
                float cout = 0;
                uint graphId = 0;
                var cmd1 = new MySqlCommand("SELECT graphe_id, cout_total FROM Tournee WHERE id=@id", conn);
                cmd1.Parameters.AddWithValue("@id", id);
                using (var r = cmd1.ExecuteReader())
                    if (r.Read())
                    {
                        cout = Convert.ToSingle(r["cout_total"]);
                        graphId = Convert.ToUInt32(r["graphe_id"]);
                    }

                var indexToName = new Dictionary<int, string>();
                var cmdNoms = new MySqlCommand("SELECT indice, nom FROM Sommet WHERE graphe_id=@gid", conn);
                cmdNoms.Parameters.AddWithValue("@gid", graphId);
                using (var r = cmdNoms.ExecuteReader())
                    while (r.Read())
                        indexToName[Convert.ToInt32(r["indice"])] = r["nom"].ToString();

                var chemin = new List<int>();
                var cmd2 = new MySqlCommand(
                    "SELECT s.indice FROM EtapeTournee e JOIN Sommet s ON e.sommet_id = s.id WHERE e.tournee_id=@id ORDER BY e.numero_ordre", conn);
                cmd2.Parameters.AddWithValue("@id", id);
                using (var r = cmd2.ExecuteReader())
                    while (r.Read())
                        chemin.Add(Convert.ToInt32(r["indice"]));

                var vertices = new List<string>();
                foreach (int i in chemin)
                    if (indexToName.ContainsKey(i)) vertices.Add(indexToName[i]);

                return new Tour { Path = chemin, TotalCost = cout, Vertices = vertices };
            }
        }

        private List<int> ResolveNamesToIndices(IList<string> names, uint graphId, MySqlConnection conn)
        {
            var nameToIndex = new Dictionary<string, int>();
            var cmd = new MySqlCommand("SELECT nom, indice FROM Sommet WHERE graphe_id=@gid", conn);
            cmd.Parameters.AddWithValue("@gid", graphId);
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                    nameToIndex[r["nom"].ToString()] = Convert.ToInt32(r["indice"]);

            var indices = new List<int>();
            foreach (string nom in names)
                if (nameToIndex.ContainsKey(nom)) indices.Add(nameToIndex[nom]);
            return indices;
        }

        private MySqlConnection OpenConnection()
        {
            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
