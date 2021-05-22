using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace LinqXML
{
    class Program
    {
        static void Main(string[] args)
        {
            Task7();

        }

        static void Task7()
        {
            ///Даны имена существующего текстового файла и создаваемого XML-документа. 
            ///Каждая строка текстового файла содержит несколько (одно или более) целых чисел, 
            ///разделенных ровно одним пробелом. Создать XML-документ с корневым элементом root, 
            ///элементами первого уровня line и элементами второго уровня sum-positive и number-negative. 
            ///Элементы line соответствуют строкам исходного файла и не содержат дочерних текстовых узлов, 
            ///элемент sum-positive является первым дочерним элементом каждого элемента line и содержит сумму 
            ///всех положительных чисел из соответствующей строки, элементы number-negative 
            ///со-держат по одному отрицательному числу из соответствую-щей строки.

            XElement root = new XElement("root");
            XDocument xdoc = new XDocument(root);

            string path = @"C:\Users\Lenovo\Desktop\linqxml\linqXML7.txt";
            int i = 1;
            string[] lines = File.ReadAllLines(path);
            //using (StreamReader reader = File.OpenText(path))
            //{
                //while (!reader.EndOfStream)
                //{
                //var lineRead = reader.ReadLine();
                //int posSum = lineRead.Split(' ').Select(Int32.Parse).Where(x => x > 0).Sum(x => x);
                //var negNum = lineRead.Split(' ').Select(x => x).Where(x => x.StartsWith('-')).ToList();

                //XElement line = new XElement("line", new XAttribute("order", $"{i}"), lineRead,
                //    new XElement("sum-positive", posSum));
                //if (negNum.Count != 0)
                //    line.Add(new XElement("number-negative", negNum));
                //xdoc.Root.Add(line);
                //i++;
                //}
                
                
                xdoc.Root.Add((from all_lines in lines select all_lines).Select(e =>
                {
                    return new
                    {
                        posSum = e.Split(' ').Select(Int32.Parse).Where(x => x > 0).Sum(x => x),
                        negNum = e.Split(' ').Select(x => x).Where(x => x.StartsWith('-')).ToList()
                    };
                }).Select(e =>
                e.negNum.Count() > 0 ?
                (new XElement("line", new XElement("sum-positive", e.posSum), new XElement("number-negative", e.negNum))) :
                (new XElement("line", new XElement("sum-positive", e.posSum)))));
            

            xdoc.Save("task7.xml");
            Console.WriteLine(xdoc);
        }
        static void Task17()
        {
            ///Дан XML-документ, содержащий хотя бы один текстовый узел. 
            ///Найти все различные имена элементов, имеющих дочерний текстовый узел, и вывести эти имена, 
            ///а также значения всех связанных с ними дочерних текстовых узлов. 
            ///Порядок имен должен соответствовать порядку их первого вхождения в документ; 
            ///текстовые значения, связан-ные с каждым именем, выводить в алфавитном порядке.

            XDocument xdoc = XDocument.Load("plant_catalog.xml");
            //List<XElement> elem = (from el in xdoc.Root.Elements("PLANT").Elements()
            //                       select el).OrderBy(x => x.Value).GroupBy(x => x.Name).Select(x => x.First()).ToList();

            List<XElement> elem = (from el in xdoc.Root.Elements().Elements()
                                   select el).OrderBy(x => x.Value).GroupBy(x => x.Name).Select(x => x.First()).ToList();
           
            foreach (var element in elem)
            {
                Console.Write($"имя узла:{element.Name} текстовый узел:{element.Value} " +
                    $"атрибут:");
                List<string> atr = (from el in element.Attributes() select el).Select(x => x.ToString()).ToList();
                foreach (var attribute in atr)
                {
                    Console.Write(attribute + ", ");
                }
                Console.WriteLine();
                Console.WriteLine("-----");
            }

        }
        static void Task27()
        {
            ///Дан XML-документ. Для всех элементов второго уровня удалить все их дочерние узлы, кроме последнего.

            XDocument xdoc = XDocument.Load("plant_catalog2.xml");
            (from el in xdoc.Root.Elements().Elements().Elements() select el)
                .Select(x => x)
                .Where(x => x.NextNode != null)
                .Remove();

            Console.WriteLine(xdoc);
        }
        static void Task37()
        {
            ///Дан XML-документ. Для каждого элемента второ-го уровня, имеющего потомков, 
            ///добавить к его текстовому содержимому текстовое содержимое всех элементов-потомков, 
            ///после чего удалить все его узлы-потомки, кроме дочернего текстового узла. 

            XDocument xdoc = XDocument.Load("plant_catalog2.xml");

            List<XElement> listOfElem = (from el in xdoc.Root.Elements().Elements()
                                         where !el.IsEmpty
                                         select el).ToList();
            foreach (var el in listOfElem)
                el.Value = el.Value;
            Console.WriteLine(xdoc);
        }
        static void Task47()
        {
            //Дан XML-документ. Для каждого элемента, имеющего хотя бы один дочерний элемент,
            //добавить дочер-ний элемент с именем has-comments и логическим значени-ем, равным true,
            //если данный элемент содержит в числе сво-их узлов-потомков один или более комментариев,
            //и false в противном случае. Новый элемент добавить после первого имеющегося дочернего элемента.

            XDocument xdoc = XDocument.Load("plant_catalog3.xml");
            XElement root = xdoc.Root;
            XElement com_node = new XElement("has-comment");

            foreach (XElement element in xdoc.Root.Descendants())
            {
                bool add = element.Elements().Count() >= 1 ? true:false;
                if (add)
                {
                    element.Elements().ElementAt(0).AddAfterSelf(com_node);

                    int a = (from el in element.DescendantNodes() where el.NodeType == XmlNodeType.Comment select el).ToList().Count();
                    if (a != 0)
                        element.Element("has-comment").SetAttributeValue("bool", true);
                    else
                        element.Element("has-comment").SetAttributeValue("bool", false);
                }

            }

            Console.WriteLine(xdoc);
            //foreach (XNode node in root.Nodes().OfType<XComment>())
            //    Console.WriteLine(node);

            ////el.NodeType == XmlNodeType.Comment
            //XElement true_com = new XElement("has-comment", new XAttribute("bool", true));
            //XElement false_com = new XElement("has-comment", new XAttribute("bool", false));
            //foreach (var el in listOfElem)
            //{
            //    if (el.NodeType == XmlNodeType.Comment)
            //    {
            //        el.AddAfterSelf(true_com);
            //    }
            //    else
            //    {
            //        el.AddAfterSelf(false_com);
            //    }
            //}

            //Console.WriteLine(xdoc);
            //foreach (var el in xdoc.Elements())
            //{
            //    Console.WriteLine(el);
            //}


        }
        static void Task57()
        {
            //Дан XML-документ и строки S1 и S2, содержащие различные пространства имен.
            //Определить в корневом эле-менте два префикса пространств имен:
            //префикс x, связанный с S1, и префикс y, связанный с S2.
            //Снабдить префиксом x все элементы первого уровня,
            //а префиксом y — все элементы второго и последующих уровней.

            string s1 = "CATALOG";
            string s2 = "PLANT";
            XDocument d = XDocument.Load("plant_catalog3.xml");
            d.Root.Add(new XAttribute(XNamespace.Xmlns + "x", s1),
            new XAttribute(XNamespace.Xmlns + "y", s2));
            XNamespace ns1 = (XNamespace)s1;
            XNamespace ns2 = (XNamespace)s2;
            foreach (var e in d.Descendants())
            {
                XNamespace ns = e.Ancestors().Count() <= 1 ? ns1 : ns2;
                e.Name = ns + e.Name.LocalName;
            }
            d.Descendants().Attributes("xmlns").Remove();
            Console.WriteLine(d);

        }
        static void Task67()
        {
            //<client id="10" time="PT5H13M"> <year>2000</year> <month>5</month> </client>

            //Преобразовать документ, сгруппировав данные по годам и месяцам и
            //изменив элементы первого уровня следующим об-разом:
            //<y2000>
            //  <m1 total-time="0" client-count="0" />
            //  ...
            //  <m5 total-time="956" client-count="3" />
            //  ...
            //</y2000>
            XDocument xdoc = XDocument.Load("Task67.xml");

            XNamespace ns = xdoc.Root.Name.Namespace;
            var client = xdoc.Root.Elements().Select(e =>
            {
                int? sum_time = 0;
                try
                {
                    string[] time = e.Attribute("time").Value.Substring(2).Split('H', 'M');
                    sum_time = Int32.Parse(time[0]) * 60 + Int32.Parse(time[1]);
                }
                catch { }
                return new
                {
                    sum_time = sum_time,
                    year = e.Element("year").Value,
                    month = e.Element("month").Value
                };
            });
            var months = Enumerable.Range(1, 12);

            xdoc.Root.ReplaceNodes(client.OrderBy(e => e.year)
                .GroupBy(e => e.year, (year_number, grouped_years) => new XElement(ns + ("y" + year_number),
                months.GroupJoin(grouped_years, e => e.ToString(), e2 => e2.month,
                (month_number, tt_sum) => new XElement(ns + ("m" + month_number),
                new XAttribute("client-count", tt_sum.Count()),
                new XAttribute("total-time", tt_sum.Sum(e => e.sum_time.GetValueOrDefault())))))));

            Console.WriteLine(xdoc);

            #region(lots of tries)
            //XElement elem = xdoc.Root.Elements()
            //    .GroupJoin(xdoc.Root.Elements().Elements("year"), xdoc.Root.Elements().Elements("month"));

            //    XElement Years = new XElement("Root",
            //from year in xdoc.Root.Elements().Elements("year")
            //join month in xdoc.Root.Elements().Elements("month") on year equals xdoc.Root.Elements().Elements("year") into gj
            //select new XElement("y" + year.Value.ToString(), from submonth in gj select new XElement("m", submonth.Value)));


            //Console.WriteLine(years);
            //var elem = xdoc.Root.Elements().Elements("year").GroupBy(year => year.Value).Select(x => new XElement("y"+x.Key.ToString(),x.Count()));
            //foreach (var r in elem)
            //{
            //    Console.WriteLine(r);
            //}

            //XNamespace ns = xdoc.Root.Name.Namespace;
            //xdoc.Root.ReplaceNodes(xdoc.Root.Elements().Select(e => new XElement(ns + "time",new XAttribute("id", e.Element(ns + "id").Value),new XAttribute("year",((DateTime)e.Element(ns + "date")).Year),new XAttribute("month",((DateTime)e.Element(ns + "date")).Month),e.Element(ns + "time").Value)));

            //Console.WriteLine(xdoc);



            //foreach (var cl in client)
            //{
            //    Console.WriteLine($"{cl.year} {cl.month} {cl.sum_time}");
            //}
            //Console.WriteLine("---------------");
            //client.GroupBy(e => e.year, (k, ee) => new XElement(ns + ("y" + k)));

            //client.OrderBy(e => e.year).GroupBy(e => e.year, (year_number, some) => new XElement(ns + ("y" + year_number)));


            //           xdoc.Root.ReplaceNodes(
            //from e in client
            //orderby e.year
            //group e by e.month
            //into ee
            //select new XElement(ns + ("house" + ee.Key),
            //from e1 in months
            //join e2 in ee
            //on e1 equals e2.month
            //into ee2 
            //select new XElement(ns + ("floor" + e1),
            //new XAttribute("count", ee2.Count()),
            //new XAttribute("total-debt",(ee2.Sum(e => e.sum_time))));


            //xdoc.Root.ReplaceNodes(client.OrderBy(e => e.year)
            //.GroupBy(e => e.year, (k, ee) => new XElement(ns + ("y" + k),
            //months.GroupJoin(ee, e1 => e1, e2 => e2.month,
            //(e1, ee2) => new XElement(ns + ("m" + e1),
            //new XAttribute("client-count", ee2.Count()),
            //new XAttribute("total-time", ee2.Sum(e => e.sum_time)))))));


            //IEnumerable<XElement> result2 = xdoc.Root.Elements().Elements("year")
            //                .GroupJoin(xdoc.Root.Elements().Elements("month"), // второй набор
            //            y => y.Value, // свойство-селектор объекта из первого набора
            //            m => m.Value, // свойство-селектор объекта из второго набора
            //            (year, month) => 
            //            new XElement ("y"+year.Value.ToString()));  // результирующий объект
            //foreach (var r in result2)
            //{ 
            //    Console.WriteLine(r); 


            //} 
            //xdoc.Root.Add(new XElement("y", 
            //    new XElement("m", 
            //    new XAttribute("total-time", "sum-time"), 
            //    new XAttribute("client-count", "sum-client"))));
            #endregion
        }
        static void Task77()
        {
            //<debt house="12" flat="129" name="Сергеев Т.М.">1833.32</debt>
            //Преобразовать документ, изменив элементы первого уровня следующим образом:
            //< address12 - 129 name = "Сергеев Т.М." debt = "1833.32" />

                 XDocument xdoc = XDocument.Load("Task77.xml");
            XNamespace ns = xdoc.Root.Name.Namespace;
            xdoc.Root.ReplaceNodes(xdoc.Root.Elements()
            .OrderBy(e => e.Attribute("house").Value).OrderBy(ex => ex.Attribute("flat").Value)
            .Select(e => new XElement(ns + "adress" + e.Attribute("house").Value + "-" + e.Attribute("flat").Value,
            new XAttribute("name", e.Attribute(ns + "name").Value),
            new XAttribute("debt", e.Value))));

            Console.WriteLine(xdoc);
        }
        static void Task87()
        {
            //<Физика> <class9> <mark-count>4</mark-count> <avr-mark>4.1</avr-mark> </class9> ... </Физика>

            XDocument xdoc = XDocument.Load("Task87.xml");
            XNamespace ns = xdoc.Root.Name.Namespace;

            var students = xdoc.Root.Elements().Elements().Select(e =>
            {
                int? s_mark = 0;
                return new
                {
                    s_class = e.Parent.Attribute("class").Value,
                    s_subject = e.Attribute("subject").Value,
                    s_mark = Int32.Parse(e.Value)
                };
            });

            var sub = students.OrderBy(s => s.s_class).Select(s => s.s_class).Distinct();

            xdoc.Root.ReplaceNodes(students
              .OrderBy(x => x.s_subject)
              .GroupBy(x => x.s_subject, (k, grouped_subj) =>
              new XElement(ns + k, sub.GroupJoin(grouped_subj, e => e.ToString(), e2 => e2.s_class, (class_number, avr) =>
            avr.Count() > 0 ? new XElement(ns + ("class" + class_number), new XElement("mark-count", avr.Count()),
              new XElement("avr-mark", Math.Round((10 * avr.Sum(e => e.s_mark) / avr.Count() * 0.1),2))) :
               null))));

            Console.WriteLine(xdoc);
        }
    }
}
