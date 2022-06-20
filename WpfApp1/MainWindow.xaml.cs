using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string FilePath { get; set; } = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        public bool OpenFileDialog()
        {
            var openFileDialog = new OpenFileDialog { Filter = "XML (*.xml)|*.xml", DefaultExt = ".xml" };

            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                return true;
            }

            return false;
        }

        public bool SaveFileDialog()
        {
            var saveFileDialog = new SaveFileDialog { Filter = "XML (*.xml)|*.xml", DefaultExt = ".xml" };

            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                return true;
            }

            return false;
        }

        private string _currentNode = "";
        private string _currentChapter = "";

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            if (!OpenFileDialog())
            {
                return;
            }

            var xDoc = new XmlDocument();
            xDoc.Load(FilePath);
            XmlElement? xRoot = xDoc.DocumentElement;

            if (xRoot != null)
            {
                var nodeRoot = new TreeViewItem();
                tree.Items.Add(nodeRoot);
                ReadNodes(xRoot, nodeRoot);
            }
        }

        private void ReadNodes(XmlElement x, TreeViewItem t)
        {
            foreach (XmlElement xnode in x)
            {
                if (xnode.Name == "cadastral_block")
                {
                    t.Header = xnode["cadastral_number"]?.FirstChild;
                    var parcel = new TreeViewItem { Name = "Parcel", Header = "Parcel" };
                    t.Items.Add(parcel);
                    var objectRealty = new TreeViewItem { Name = "ObjectRealty", Header = "ObjectRealty" };
                    t.Items.Add(objectRealty);
                    var spatialData = new TreeViewItem { Name = "SpatialData", Header = "SpatialData" };
                    t.Items.Add(spatialData);
                    var bound = new TreeViewItem { Name = "Bound", Header = "Bound" };
                    t.Items.Add(bound);
                    var zone = new TreeViewItem { Name = "Zone", Header = "Zone" };
                    t.Items.Add(zone);
                }

                if (xnode.Name == "land_record")
                {
                    var node = new TreeViewItem { Header = xnode["object"]?["common_data"]?["cad_number"]?.FirstChild };
                    node.Selected += NodeParcel_Selected;
                    (t.Items[0] as TreeViewItem)?.Items.Add(node);
                }

                if (xnode.Name == "build_record" || xnode.Name == "construction_records")
                {
                    var node = new TreeViewItem { Header = xnode["object"]?["common_data"]?["cad_number"]?.FirstChild };
                    node.Selected += NodeObjectRealty_Selected;
                    (t.Items[1] as TreeViewItem)?.Items.Add(node);
                }

                if (xnode.Name == "entity_spatial" && xnode.ParentNode?.Name == "spatial_data")
                {
                    var node = new TreeViewItem { Header = xnode["sk_id"]?.FirstChild };
                    node.Selected += NodeSpatialData_Selected;
                    (t.Items[2] as TreeViewItem)?.Items.Add(node);
                }

                if (xnode.Name == "municipal_boundary_record")
                {
                    var node = new TreeViewItem
                    {
                        Header = xnode["b_object_municipal_boundary"]?["b_object"]?["reg_numb_border"]?.FirstChild
                    };
                    node.Selected += NodeBound_Selected;
                    (t.Items[3] as TreeViewItem)?.Items.Add(node);
                }

                if (xnode.Name == "zones_and_territories_record")
                {
                    var node = new TreeViewItem
                    {
                        Header = xnode["b_object_zones_and_territories"]?["b_object"]?["reg_numb_border"]?.FirstChild
                    };
                    node.Selected += NodeZone_Selected;
                    (t.Items[4] as TreeViewItem)?.Items.Add(node);
                }

                if ((xnode?.HasChildNodes ?? false) && (xnode.FirstChild is XmlElement))
                {
                    ReadNodes(xnode, t);
                }
            }
        }

        private void NodeParcel_Selected(object sender, RoutedEventArgs e)
        {
            var xDoc = new XmlDocument();
            xDoc.Load("test.xml");
            XmlElement? xRoot = xDoc.DocumentElement;
            _currentChapter = "Parcel";
            var sectionName = "land_record";
            var id = ((XmlText)((TreeViewItem)sender).Header).Value!;

            if (xRoot != null)
            {
                SearchNode(sectionName, id, xRoot);
            }
        }

        private void NodeObjectRealty_Selected(object sender, RoutedEventArgs e)
        {
            var xDoc = new XmlDocument();
            xDoc.Load("test.xml");
            XmlElement? xRoot = xDoc.DocumentElement;
            _currentChapter = "ObjectRealty";
            var sectionName1 = "build_record";
            var sectionName2 = "construction_records";
            var id = ((XmlText)((TreeViewItem)sender).Header).Value!;

            if (xRoot != null)
            {
                SearchNode(sectionName1, id, xRoot);
                SearchNode(sectionName2, id, xRoot);
            }
        }

        private void NodeSpatialData_Selected(object sender, RoutedEventArgs e)
        {
            var xDoc = new XmlDocument();
            xDoc.Load("test.xml");
            XmlElement? xRoot = xDoc.DocumentElement;
            _currentChapter = "SpatialData";
            var sectionName = "spatial_data";
            var id = ((XmlText)((TreeViewItem)sender).Header).Value!;

            if (xRoot != null)
            {
                SearchNode(sectionName, id, xRoot);
            }
        }

        private void NodeBound_Selected(object sender, RoutedEventArgs e)
        {
            var xDoc = new XmlDocument();
            xDoc.Load("test.xml");
            XmlElement? xRoot = xDoc.DocumentElement;
            _currentChapter = "Bound";
            var sectionName = "municipal_boundary_record";
            var id = ((XmlText)((TreeViewItem)sender).Header).Value!;

            if (xRoot != null)
            {
                SearchNode(sectionName, id, xRoot);
            }
        }

        private void NodeZone_Selected(object sender, RoutedEventArgs e)
        {
            var xDoc = new XmlDocument();
            xDoc.Load("test.xml");
            XmlElement? xRoot = xDoc.DocumentElement;
            _currentChapter = "Zone";
            var sectionName = "zones_and_territories_record";
            var id = ((XmlText)((TreeViewItem)sender).Header).Value!;

            if (xRoot != null)
            {
                SearchNode(sectionName, id, xRoot);
            }
        }

        private void SearchNode(string sectionName, string id, XmlElement x)
        {
            foreach (XmlElement xnode in x)
            {
                if (xnode.Name == sectionName)
                {
                    if (((xnode["object"]?["common_data"]?["cad_number"]?.FirstChild as XmlText)?.Value ?? "") == id
                        || ((xnode["entity_spatial"]?["sk_id"]?.FirstChild as XmlText)?.Value ?? "") == id
                        || ((xnode["b_object_municipal_boundary"]?["b_object"]?["reg_numb_border"]?.FirstChild as XmlText)?.Value ?? "") == id
                        || ((xnode["b_object_zones_and_territories"]?["b_object"]?["reg_numb_border"]?.FirstChild as XmlText)?.Value ?? "") == id)
                    {
                        _currentNode = xnode.InnerXml.Replace(">", ">\n").Replace("</", "\n</").Replace("\n\n", "\n");
                        data.Text = _currentNode;
                        return;
                    }
                }

                if ((xnode?.HasChildNodes ?? false) && (xnode.FirstChild is XmlElement))
                {
                    SearchNode(sectionName, id, xnode);
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!SaveFileDialog())
            {
                return;
            }

            _currentNode = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<" + _currentChapter + ">"
                + _currentNode 
                + "</" + _currentChapter + ">";

            using var writer = new StreamWriter(FilePath);
            writer.WriteLineAsync(_currentNode);
            MessageBox.Show("Текущий узел сохранён в файл");
        }
    }
}
