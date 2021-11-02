using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Linq;

namespace PortalsDateBaseCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime startTime = new DateTime();
            try
            {
                List<string> rL = new List<string>();
                List<string> lines = File.ReadAllLines(Console.ReadLine(), Encoding.Default).ToList();
                startTime = DateTime.Now;
                List<Line> eL = new List<Line>();
                List<Line> unChecked = new List<Line>();
                List<Line> unFined = new List<Line>();
                List<List<string>> arg = lines.Select(x => x.Split(new string[1] { "||" }, StringSplitOptions.None).Select(c => c.Trim()).ToList()).ToList();
                for (int i = 0; i < lines.Count; i++)
                {
                    if (arg[i].Count == 11)
                    {
                        List<string> fA = arg[i][0].Split(' ').ToList();
                        List<string> fA1 = arg[i][1].Split(' ').ToList();
                        Line line = new Line();
                        if (fA[0] == "??")
                            line.Coord = new PointC() { x = int.Parse(fA1[0]) * 8, y = -1, z = int.Parse(fA1[1]) * 8 };
                        else if (fA.Count == 3)
                            line.Coord = new PointC() { x = int.Parse(fA[0]), y = int.Parse(fA[1]), z = int.Parse(fA[2]) };
                        else if (fA.Count == 2)
                            line.Coord = new PointC() { x = int.Parse(fA[0]), y = -1, z = int.Parse(fA[1]) };
                        line.NCoord = new PointC() { x = line.Coord.x / 8, y = -1, z = line.Coord.z / 8 };
                        line.Date = arg[i][2];
                        if (arg[i][3] == "??")
                        {
                            unFined.Add(line);
                            continue;
                        }
                        line.Break = arg[i][3].ToLower() == "yes" ? true : false;
                        line.Portal = arg[i][4];
                        line.Live = arg[i][5].ToLower() == "yes" ? true : false;
                        line.Pvp = arg[i][6].ToLower() == "yes" ? true : false;
                        line.Team = arg[i][7];
                        if (arg[i][8] != "??")
                            line.Spawner = int.Parse(arg[i][8]);
                        else line.Spawner = -1;
                        line.Secret = arg[i][9].ToLower() == "yes" ? true : false;
                        line.Gen = arg[i][10].Split(' ')[0].ToLower() == "com" ? true : false;
                        line.Comment = arg[i][10].Length > 4 ? arg[i][10].Remove(0, 3) : "";
                        eL.Add(line);
                    }
                    else
                    {
                        List<string> fA = arg[i][0].Split(' ').ToList();
                        if (fA.Count == 3)
                            unChecked.Add(new Line() { Coord = new PointC() { x = int.Parse(fA[0]), y = int.Parse(fA[1]), z = int.Parse(fA[2]) }, NCoord = new PointC() { x = int.Parse(fA[0]) / 8, y = -1, z = int.Parse(fA[2]) / 8 }, Date = null, Break = false, Comment = null, Gen = false, Live = false, Portal = null, Pvp = false, Secret = false, Spawner = -1, Team = null });
                        else if (fA.Count == 2)
                            unChecked.Add(new Line() { Coord = new PointC() { x = int.Parse(fA[0]), y = -1, z = int.Parse(fA[1]) }, NCoord = new PointC() { x = int.Parse(fA[0]) / 8, y = -1, z = int.Parse(fA[1]) / 8 }, Date = null, Break = false, Comment = null, Gen = false, Live = false, Portal = null, Pvp = false, Secret = false, Spawner = -1, Team = null });
                    }
                }
                List<Line> Team = eL.Where(x => x.Team != "No" && x.Break == false).ToList(),
                    Secret = eL.Where(x => x.Team == "No" && x.Secret == true && x.Gen == true).ToList(),
                    NoBreak = eL.Where(x => x.Team == "No" && x.Secret == false && x.Break == false && x.Gen == true).ToList(),
                    Break = eL.Where(x => x.Secret == false && x.Break == true && x.Gen == true).ToList(),
                    Def = eL.Where(x => x.Gen == false).ToList();
                Console.WriteLine(Def.Count);
                                
                Delta(new Point(0, 0), ref Team);
                Delta(new Point(0, 0), ref Secret);
                Delta(new Point(0, 0), ref NoBreak);
                Delta(new Point(0, 0), ref unChecked);
                Delta(new Point(0, 0), ref Break);
                Delta(new Point(0, 0), ref Def);
                Delta(new Point(0, 0), ref unFined);

                rL.AddRange(Team.Select(x => Pad(18, x.Coord.x + " " + (x.Coord.y != -1 ? (x.Coord.y + " ") : "") + x.Coord.z) + "|| " + Pad(12, x.NCoord.x + " " + x.NCoord.z) + "|| " + Pad(8, x.Date) + "|| " + Pad(4, x.Break == true ? "Yes" : "No") + "|| " + Pad(7, x.Portal) + "|| " + Pad(5, x.Live == true ? "Yes" : "No") + "|| " + Pad(4, x.Pvp == true ? "Yes" : "No") + "|| " + Pad(5, x.Team) + "|| " + Pad(3,(x.Spawner == -1 ? "??" : x.Spawner.ToString())) + "|| " + Pad(4, x.Secret == true ? "Yes" : "No") + "|| " + (x.Gen == true ? "Com" : "Def") + x.Comment));
                rL.AddRange(Secret.Select(x => Pad(18, x.Coord.x + " " + (x.Coord.y != -1 ? (x.Coord.y + " ") : "") + x.Coord.z) + "|| " + Pad(12, x.NCoord.x + " " + x.NCoord.z) + "|| " + Pad(8, x.Date) + "|| " + Pad(4, x.Break == true ? "Yes" : "No") + "|| " + Pad(7, x.Portal) + "|| " + Pad(5, x.Live == true ? "Yes" : "No") + "|| " + Pad(4, x.Pvp == true ? "Yes" : "No") + "|| " + Pad(5, x.Team) + "|| " + Pad(3, (x.Spawner == -1 ? "??" : x.Spawner.ToString())) + "|| " + Pad(4, x.Secret == true ? "Yes" : "No") + "|| " + (x.Gen == true ? "Com" : "Def") + x.Comment));
                rL.AddRange(NoBreak.Select(x => Pad(18, x.Coord.x + " " + (x.Coord.y != -1 ? (x.Coord.y + " ") : "") + x.Coord.z) + "|| " + Pad(12, x.NCoord.x + " " + x.NCoord.z) + "|| " + Pad(8, x.Date) + "|| " + Pad(4, x.Break == true ? "Yes" : "No") + "|| " + Pad(7, x.Portal) + "|| " + Pad(5, x.Live == true ? "Yes" : "No") + "|| " + Pad(4, x.Pvp == true ? "Yes" : "No") + "|| " + Pad(5, x.Team) + "|| " + Pad(3, (x.Spawner == -1 ? "??" : x.Spawner.ToString())) + "|| " + Pad(4, x.Secret == true ? "Yes" : "No") + "|| " + (x.Gen == true ? "Com" : "Def") + x.Comment));
                rL.AddRange(unChecked.Select(x => Pad(18, x.Coord.x + " " + x.Coord.z) + "|| " + Pad(12, x.NCoord.x + " " + x.NCoord.z) + "||"));
                rL.AddRange(Break.Select(x => Pad(18, x.Coord.x + " " + x.Coord.y + " " + x.Coord.z) + "|| " + Pad(12, x.NCoord.x + " " + x.NCoord.z) + "|| " + Pad(8, x.Date) + "|| " + Pad(4, x.Break == true ? "Yes" : "No") + "|| " + Pad(7, x.Portal) + "|| " + Pad(5, x.Live == true ? "Yes" : "No") + "|| " + Pad(4, x.Pvp == true ? "Yes" : "No") + "|| " + Pad(5, x.Team) + "|| " + Pad(3, (x.Spawner == -1 ? "??" : x.Spawner.ToString())) + "|| " + Pad(4, x.Secret == true ? "Yes" : "No") + "|| " + (x.Gen == true ? "Com" : "Def") + x.Comment));
                rL.AddRange(Def.Select(x => Pad(18, x.Coord.x + " " + x.Coord.y + " " + x.Coord.z) + "|| " + Pad(12, x.NCoord.x + " " + x.NCoord.z) + "|| " + Pad(8, x.Date) + "|| " + Pad(4, x.Break == true ? "Yes" : "No") + "|| " + Pad(7, x.Portal) + "|| " + Pad(5, x.Live == true ? "Yes" : "No") + "|| " + Pad(4, x.Pvp == true ? "Yes" : "No") + "|| " + Pad(5, x.Team) + "|| " + Pad(3, (x.Spawner == -1 ? "??" : x.Spawner.ToString())) + "|| " + Pad(4, x.Secret == true ? "Yes" : "No") + "|| " + (x.Gen == true ? "Com" : "Def") + x.Comment));
                rL.AddRange(unFined.Select(x => Pad(18, x.Coord.x + " " + (x.Coord.y != -1 ? (x.Coord.y + " ") : "") + x.Coord.z) + "|| " + Pad(12, x.NCoord.x + " " + x.NCoord.z)));

                Bitmap bitmap = new Bitmap(2000, 2000);
                Graphics g = Graphics.FromImage(bitmap);
                g.DrawPolygon(new Pen(Color.FromArgb(62, 62, 62), 1), new PointF[4] { new PointF(0, 0), new PointF(0, 2000), new PointF(2000, 2000), new PointF(2000, 0) });
                for (int x = 0; x < 125; x++)
                {
                    g.DrawLine(new Pen(Color.Yellow, 1), new Point(x * 16, 0), new Point(x * 16, 1999));
                    g.DrawLine(new Pen(Color.Yellow, 1), new Point(0, x * 16), new Point(1999, x * 16));
                }

                DrawingLines(g, Color.FromArgb(255, 255, 255), Team);
                DrawingLines(g, Color.FromArgb(220, 220, 220), Secret);
                DrawingLines(g, Color.FromArgb(180, 180, 180), NoBreak);
                DrawingLines(g, Color.FromArgb(140, 140, 140), unChecked);
                DrawingLines(g, Color.FromArgb(100, 100, 100), Break);
                DrawingLines(g, Color.FromArgb(60, 60, 60), Def);
                DrawingLines(g, Color.FromArgb(20, 20, 20), unFined);

                string path = @"C:\\Portals.txt";
                if (File.Exists(path))
                    File.Delete(path);
                File.Create(path).Dispose();
                File.WriteAllLines(path, rL, Encoding.Unicode);

                bitmap.Save(@"C:\\map.png");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine(DateTime.Now - startTime);
            Console.ReadLine();
        }
        private struct Line
        {
            public PointC Coord, NCoord;
            public string Date, Portal, Team, Comment;
            public bool Break, Live, Pvp, Secret, Gen;
            public int Spawner;
        }

        private struct PointC
        {
            public int x, y, z;
        }

        private static string Pad(int length, string word) => word.PadRight(length, ' ');

        private static void Delta(Point startIndex, ref List<Line> coordinates)
        {
            List<Line> newCoordinates = new List<Line>();
            int counter = coordinates.Count;
            int tempI = coordinates.Count;
            for (int i = 0; i < tempI; i++)
            {
                List<int> tempDelta = new List<int>();
                for (int o = 0; o < counter; o++)
                    tempDelta.Add(Math.Abs(startIndex.X - coordinates[o].Coord.x) + Math.Abs(startIndex.Y - coordinates[o].Coord.z));
                counter--;
                int tempInt = tempDelta.IndexOf(tempDelta.Min());
                newCoordinates.Add(coordinates[tempInt]);
                coordinates.RemoveAt(tempInt);
                startIndex = new Point(newCoordinates[newCoordinates.Count - 1].Coord.x, newCoordinates[newCoordinates.Count - 1].Coord.z); 
            }
            coordinates = newCoordinates;
        }

        private static void DrawingLines(Graphics g, Color color, List<Line> lines)
        {
            List<Point> refCoordinateslines = lines.Select(x => new Point(x.Coord.x >= 0 ? (x.Coord.x / 16 + 1000) : (Math.Abs(x.Coord.x / 16)), x.Coord.z >= 0 ? (x.Coord.z / 16 + 1000) : (x.Coord.z / 16))).ToList();
            for (int i = 1; i < refCoordinateslines.Count; i++)
                g.DrawLine(new Pen(color, 1), refCoordinateslines[i - 1], refCoordinateslines[i]);
        }
    }
}