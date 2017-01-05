using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Specialized;
using System.Collections.ObjectModel;


namespace WpfApplication2
{
    [Serializable]
    class village : INotifyPropertyChanged
    {
        public village() { }
        private string image_smithy;
        public string Image_Smithy
        {
            get
            {
                return image_smithy;
            }
            set
            {
                image_smithy = value;
                NotifyPropertyChanged();
            }
        }
        private string image_mill;
        public string Image_Mill
        {
            get
            {
                return image_mill;
            }
            set
            {
                
                image_mill = value;
                NotifyPropertyChanged();
            }
        }
        private string image_sawmill;
        public string Image_Sawmill
        {
            get
            {
                return image_sawmill;
            }
            set
            {
                
                image_sawmill = value;
                NotifyPropertyChanged();
            }
        }
        private string image;
        public string Image
        {
            get
            {
                return image;
            }
            set
            {
                
                image = value;
                NotifyPropertyChanged();
            }
        }
    [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public string Name { get; set; }
        public village(string name)
        {
            Smithy = false;
            Sawmill = false;
            Mill = false;
            Name = name;
            Robbery = false;
            Status = 5;
            tax = 5;
            Image = "sourse/Village_scene.png";
            Image_Mill = "sourse/construction.png";
            Image_Sawmill = "sourse/construction.png";
            Image_Smithy = "sourse/construction.png";

        }
        public bool Smithy { get; set; }
        public bool Sawmill { get; set; }
        public bool Mill { get; set; }
        public int Status { get; set; }
        public bool Robbery { get; set; }
        private int tax;
        public int Tax
        {
            get
            {
                if (Robbery) { return 0; }
                return tax;
            }
            set
            {
                tax = value;
                NotifyPropertyChanged();
            }
        }
    }
    [Serializable]
    class Lands : IEnumerable, INotifyPropertyChanged, INotifyCollectionChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;   
        protected virtual void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        [field: NonSerialized]
        public event NotifyCollectionChangedEventHandler CollectionChanged = delegate { };
        public void OnCollectionChanged(NotifyCollectionChangedAction action)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action));
            }
        }
        public int count()
        {
            return vill.Count;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public Villenum GetEnumerator()
        {
            return new Villenum(vill);
        }
        public int Col_Day { get; set; }
        private int _army;
        public int Army
        {
            get {
                return _army;
            }
            set
            {           
                _army = value;
                NotifyPropertyChanged();
            }
        }
        private int _gold;
        public int Gold
        {
            get { return _gold; }
            set
            {
                _gold = value;
                NotifyPropertyChanged();
                if (_gold <= 30)
                {
                    ICollection<string> c = vill.Keys;
                    foreach (string i in c)
                    {
                        if (!vill[i].Smithy)
                        {
                            vill[i].Image_Smithy = "sourse/no.png";
                        }
                        if (!vill[i].Sawmill)
                        {
                            vill[i].Image_Sawmill = "sourse/no.png";
                        }
                        if (!vill[i].Mill)
                        {
                            vill[i].Image_Mill = "sourse/no.png";
                        }
                    }
                }
                else 
                {
                    ICollection<string> c = vill.Keys;
                    foreach (string i in c)
                    {
                        if (!vill[i].Smithy)
                        {
                            vill[i].Image_Smithy = "sourse/construction.png";
                        }
                        if (!vill[i].Sawmill)
                        {
                            vill[i].Image_Sawmill = "sourse/construction.png";
                        }
                        if (!vill[i].Mill)
                        {
                            vill[i].Image_Mill = "sourse/construction.png";
                        }
                    }
                }
            }
        }
        public Dictionary<string, village> vill = new Dictionary<string, village> { };
        public Lands()
        {
            vill.Add("Залесье", new village("Залесье"));
            vill.Add("Дубки", new village("Дубки"));
            vill.Add("Бобрик", new village("Бобрик"));
            Gold = 50;
            Army = 0;
        }
        public bool Add(string name, bool build)
        {
            if (build)
            {
                if (Gold >= 100)
                {
                    if (!vill.ContainsKey(name))
                    {
                        vill.Add(name, new village(name));
                        OnCollectionChanged(NotifyCollectionChangedAction.Reset);
                        Gold -= 100;
                        return true;
                    }
                    else return false;
                }
                else return false;
            }
            else
            {
                if (!(vill.ContainsKey(name)))
                {

                    if (Army == 0) { return false; }
                    else if (Army <= 5)
                    {
                        Army = 0;
                        return false;
                    }
                    else
                    {
                        Random rnd = new Random();
                        int k = rnd.Next(0, Army-1);
                        Army -= k;
                        vill.Add(name, new village(name));
                        OnCollectionChanged(NotifyCollectionChangedAction.Reset);
                        return true;
                    }
                }
                else return false;
            }
        }
        public string Remove(string name)
        {
            vill.Remove(name);
            Gold += 75;
            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
            return "Деревня разорена, общая сумма награбленного 75 золотых";
        }
        public bool Protect(string name)
        {
            if (Army > 5)
            {
                Random rnd = new Random();
                int k = rnd.Next(0, Army/2);
                Army -= k;
                if (Army < 0)
                {
                    Army = 0;
                    return false;
                }
                else
                {
                    (vill[name]).Status = 5;
                    (vill[name]).Robbery = false;
                    (vill[name]).Tax = 5;
                    if ((vill[name]).Sawmill) (vill[name]).Tax += 5;
                    if ((vill[name]).Mill) (vill[name]).Tax += 5;
                    if ((vill[name]).Smithy) (vill[name]).Tax += 5;
                    (vill[name]).Image = "sourse/Village_scene.png";
                    return true;
                }
            }
            else {
                Army = 0;
                return false;
                 }
        }

        public string Day()
        {
            string k = "";
            string s = "";
            ICollection<string> c = vill.Keys;
            List<string> I = new List<string> { };
            foreach (string i in c)
            {
                Gold += (vill[i]).Tax;
                if (((village)vill[i]).Robbery)
                {
                    ((village)vill[i]).Status -= 1;
                    if (((village)vill[i]).Status == 0)
                    {
                        I.Add(i);
                    }
                }
            }
            foreach (string i in I)
            {
                vill.Remove(i);
                OnCollectionChanged(NotifyCollectionChangedAction.Reset);
                k += i + ": Деревня уничтожена ";
            }
            I.Clear();
            Col_Day++;
            Random rnd = new Random();
            int n = rnd.Next(0, 5);
            if ((n == 3) && (vill.Count != 0))
            {
                n = rnd.Next(0, vill.Count - 1);
                (vill[c.ElementAt(n)].Robbery) = true;
                vill[c.ElementAt(n)].Tax = 0;
                vill[c.ElementAt(n)].Image = "sourse/Damaged_village.png";
                s = " На деревню напали разбойники ";
            }
            return k + s;
        }
        public string mill(string name)
        {
            if (!vill[name].Mill)
            {
                if (Gold >= 30)
                {
                    (vill[name]).Mill = true;
                    Gold -= 30;
                    (vill[name]).Tax += 5;
                    (vill[name]).Image_Mill = "sourse/ok.png";
                    return "Постройка успешно выполнена";
                }
                else return "Недостаточно денег";
            }
            else return "уже построено";
        }
        public string sawmill(string name)
        {
            if (!vill[name].Sawmill)
            {
                if (Gold >= 30)
                {
                    (vill[name]).Sawmill = true;
                    Gold -= 30;
                    (vill[name]).Tax += 5;
                    (vill[name]).Image_Sawmill = "sourse/ok.png";
                    return "Постройка успешно выполнена";
                }
                else return "Недостаточно денег";
            } return "уже построено";
        }
        public string smithy(string name)
        {
            if (!vill[name].Smithy)
            {
                if (Gold >= 30)
                {
                    (vill[name]).Smithy = true;
                    Gold -= 30;
                    (vill[name]).Tax += 5;
                    (vill[name]).Image_Smithy = "sourse/ok.png";
                    return "Постройка успешно выполнена";
                }
                else return "Недостаточно денег";
            }
            else return "уже построено";
        }
        public bool army(int col)
        {
            if (Gold >= (col * 5))
            {
                Army += col;
                Gold -= col * 5;
                return true;
            }
            else return false;
        }
        public village get(string name)
        {
            return (vill[name]);
        }
    }
    [Serializable]
    class Villenum : IEnumerator
    {
        public Dictionary<string, village> vill;

        int position = -1;

        public Villenum(Dictionary<string, village> list)
        {
            vill = list;
        }

        public bool MoveNext()
        {
            position++;
            return (position < vill.Count);
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public village Current
        {
            get
            {
                try
                {
                    ICollection<string> c = vill.Keys;
                    return vill[c.ElementAt(position)];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}


