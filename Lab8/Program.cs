using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Lab8
{
    class Program
    {
        private const int MAX_NODES = 10;
        private const string BIN_FILE_NAME = "test.dat";
        private const string XML_FILE_NAME = "xml_test.xml";

        static void Main(string[] args)
        {
            Graph graph = null;
            int menuItem = -1;
            try
            {
                while (menuItem != 0)
                {
                    printMenu();
                    menuItem = Int32.Parse(Console.ReadLine());
                    switch (menuItem)
                    {
                        case 1:
                            graph = new Graph();
                            initGraph(graph);
                            break;
                        case 2:
                            graph = BinaryDeserializeFrom(BIN_FILE_NAME);
                            break;
                        case 3:
                            BinarySeriliazeTo(BIN_FILE_NAME, graph);
                            break;
                        case 4:
                            graph = XmlDeserializeFrom(XML_FILE_NAME);
                            break;
                        case 5:
                            XmlSerializeTo(XML_FILE_NAME, graph);
                            break;
                        case 6:
                            graph.printAdjacencyList();
                            break;
                        case 7:
                            graph.Clear();
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }


            //DeserializeGraphFrom("binarySerialize.txt");
            /*initGraph(graph);
            graph.printAdjacencyList();

            BinarySeriliazeTo("binarySerialize.txt", graph);*/

            //testGraph(graph);

            //Console.ReadLine();
        }

        private static void printMenu()
        {
            Console.WriteLine("Меню");
            Console.WriteLine("0.Выход");
            Console.WriteLine("1.Создать новый граф");
            Console.WriteLine("2.Двоичная десерилизация из файла");
            Console.WriteLine("3.Двоичная серилизация в файл");
            Console.WriteLine("4.Xml десерилизация из файла");
            Console.WriteLine("5.Xml серилизация в файл");
            Console.WriteLine("6.Вывод графа");
            Console.WriteLine("7.Очистить граф");

            Console.WriteLine("Введите пункт меню:");
        }

        private static void initGraph(Graph graph)
        {
            Random random = new Random();
            for (int i = 0; i < 25; i++)
            {
                int node1 = random.Next(MAX_NODES);
                int node2 = node1;
                while (node2 == node1)
                {
                    node2 = random.Next(MAX_NODES);
                }
                graph.addEdge(node1, node2);
            }
        }

        private static void testGraph(Graph graph)
        {
            Random random = new Random();
            for (int i = 0; i < 25; i++)
            {
                double probability = random.NextDouble();
                if (i < 13)
                {
                    probability += 0.6;
                }
                else if (i > 20)
                {
                    if (graph.IsEmpty())
                    {
                        return;
                    }
                    if (random.NextDouble() > 0.5)
                    {
                        probability = 0.5;
                    }
                }
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine(i + ".Вероятность = " + probability);
                Console.WriteLine("------------------------------------------------------------");
                int node1 = random.Next(MAX_NODES);
                int node2 = node1;
                while (node2 == node1)
                {
                    node2 = random.Next(MAX_NODES);
                }
                if (probability < 0.4)
                {
                    NodeAdj nodeAdj = null;
                    bool isSuccess;
                    int count = 0;
                    do
                    {
                        node1 = random.Next(MAX_NODES);
                        isSuccess = graph.Nodes.TryGetValue(node1, out nodeAdj);
                        if (isSuccess)
                        {
                            count = nodeAdj.AdjacencyNodes.Count;
                        }
                    } while (!isSuccess && count == 0);


                    int index = random.Next(0, nodeAdj.AdjacencyNodes.Count - 1);
                    node2 = nodeAdj.AdjacencyNodes[index];
                    Console.WriteLine("Удаление ребра (" + node1 + ", " + node2 + ")");
                    graph.removeEdge(node1, node2);
                }
                else if (probability > 0.6)
                {
                    Console.WriteLine("Добавление ребра (" + node1 + ", " + node2 + ")");
                    graph.addEdge(node1, node2);
                }
                else
                {
                    NodeAdj nodeAdj = null;
                    while (!graph.Nodes.TryGetValue(node1, out nodeAdj))
                    {
                        node1 = random.Next(MAX_NODES);
                    }
                    Console.WriteLine("Удаление вершины " + node1);
                    graph.removeNode(node1);
                }
                Console.WriteLine();
                graph.printAdjacencyList();
                Console.WriteLine();
            }
        }

        private static void BinarySeriliazeTo(string fileName, Graph graph)
        {
            Stream st = new FileStream(fileName, FileMode.Create);
            IFormatter f = new BinaryFormatter();
            f.Serialize(st, graph);
            st.Close();
        }

        public static Graph BinaryDeserializeFrom(string fileName)
        {
            Stream st = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            IFormatter f = new BinaryFormatter();
            Graph car = (Graph) f.Deserialize(st);
            st.Close();
            return car;
        }

        public static void XmlSerializeTo(string fileName, Graph graph)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader reader = new StreamReader(memoryStream))
            {
                DataContractSerializer serializer = new DataContractSerializer(graph.GetType());
                serializer.WriteObject(memoryStream, graph);
                memoryStream.Position = 0;
                File.WriteAllText(fileName, reader.ReadToEnd());
            }


            /*var serializer = new DataContractSerializer(typeof(Graph));
           // string xmlString;
            using (var sw = new StringWriter())
            {
                using (var writer = new XmlTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented; // indent the Xml so it's human readable
                    serializer.WriteObject(writer, graph);
                    writer.Flush();
                    File.WriteAllText(fileName, sw.ToString());
                    //xmlString = sw.ToString();
                }
            }*/
        }

        public static Graph XmlDeserializeFrom(string fileName)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(fileName));
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(typeof(Graph));
                return (Graph) deserializer.ReadObject(stream);
            }
        }
    }


    [Serializable]
    [DataContract]
    public class Graph
    {
        /*key is value of node; value is list*/
        [DataMember]
        public SortedDictionary<int, NodeAdj> Nodes { get; set; }

  
        public Graph()
        {
            Nodes = new SortedDictionary<int, NodeAdj>();
        }

        public void addEdge(int nodeValue1, int nodeValue2)
        {
            addOrientedEdge(nodeValue1, nodeValue2);
            addOrientedEdge(nodeValue2, nodeValue1);
        }

        private void addOrientedEdge(int nodeValue1, int nodeValue2)
        {
            NodeAdj nodeAdj = null;
            if (!Nodes.TryGetValue(nodeValue1, out nodeAdj))
            {
                nodeAdj = new NodeAdj();
                Nodes.Add(nodeValue1, nodeAdj);
            }
            nodeAdj.addNode(nodeValue2);
        }

        public void removeEdge(int nodeValue1, int nodeValue2)
        {
            removeOrientedEdge(nodeValue1, nodeValue2);
            removeOrientedEdge(nodeValue2, nodeValue1);
        }

        private void removeOrientedEdge(int nodeValue1, int nodeValue2)
        {
            NodeAdj nodeAdj = null;
            if (!Nodes.TryGetValue(nodeValue1, out nodeAdj))
            {
                nodeAdj = new NodeAdj();
            }
            nodeAdj.removeNode(nodeValue2);
        }

        private void addNode(int value)
        {
            Nodes.Add(value, new NodeAdj());
        }

        public void removeNode(int value)
        {
            NodeAdj nodeAdj = null;
            if (Nodes.TryGetValue(value, out nodeAdj))
            {
                List<int> adjacencyNodes = nodeAdj.AdjacencyNodes;
                foreach (int adjacencyNode in adjacencyNodes)
                {
                    NodeAdj tempNode = null;
                    if (Nodes.TryGetValue(adjacencyNode, out tempNode))
                    {
                        tempNode.removeNode(value);
                    }
                }
            }
            Nodes.Remove(value);
        }

        public bool IsEmpty()
        {
            foreach (KeyValuePair<int, NodeAdj> keyValuePair in Nodes)
            {
                if (keyValuePair.Value.AdjacencyNodes.Count > 0)
                {
                    return false;
                }
            }
            return true;
        }

        public void Clear()
        {
            Nodes.Clear();
        }

        public void printAdjacencyList()
        {
            Console.WriteLine("Список смежности: ");
            foreach (KeyValuePair<int, NodeAdj> node in Nodes)
            {
                Console.WriteLine(node.Key + " : " + node.Value.ToString());
            }
        }
    }

    [Serializable]
    [DataContract]
    public class NodeAdj
    {
        [DataMember]
        public List<int> AdjacencyNodes { get; set; }

        
        public NodeAdj()
        {
            AdjacencyNodes = new List<int>();
        }

        public void addNode(int value)
        {
            if (!AdjacencyNodes.Contains(value))
            {
                AdjacencyNodes.Add(value);
            }
        }

        public void removeNode(int value)
        {
            AdjacencyNodes.Remove(value);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (int adjacencyNode in AdjacencyNodes)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(", ");
                }
                stringBuilder.Append(adjacencyNode);
            }
            return stringBuilder.ToString();
        }
    }
}