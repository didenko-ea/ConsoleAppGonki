using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleAppGonki
{
    public class Appset
    {
        public Dictionary<string, string> SettingDict { get; set; }
    }
    public class StaticCache
    {
        private static Appset Appset = null;

        public static Appset GetAppSetting()
        {
            Appset Appset = new Appset();
            NameValueCollection SettingsAll;
            SettingsAll = ConfigurationManager.AppSettings;
            Appset.SettingDict = SettingsAll.AllKeys.ToDictionary(t => t, t => SettingsAll[t]);
            return Appset;
        }
        public static void LoadStaticCache()
        {
            Appset = StaticCache.GetAppSetting();
        }
        public static Appset GetAppset()
        {
            return Appset;
        }
        public static void SetAppsetNull()
        {
            Appset = null;
        }
    }
 
    class Car 
    {
        public string Name;
        public float Speed;
        public int Prc;

        public void setValues(string Name,float Speed, int Prc)
        {
            this.Name = Name;
            this.Speed = Speed;
            this.Prc = Prc;
        }
        public virtual string getValues() 
        {
            return this.Name + ", ск: " + this.Speed + ", в/пр: " + this.Prc;
        }
        public Car(string Name, float Speed, int Prc)
        {
            this.Name = Name;
            this.Speed = Speed;
            this.Prc = Prc;
        }
        public Car() { } 
    }

    class Truck : Car
    {
        public int Carry;
        public Truck(string Name, float Speed, int Prc, int Carry) : base(Name, Speed, Prc)
        {
            this.Carry = Carry;
        }
       public override string getValues()
        {
            return base.getValues()+ ", гр: " + Carry + "тн";
        }
    }
    class Sedan : Car
    {
        public int Passangers;
        public Sedan(string Name, float Speed, int Prc, int Passangers) : base(Name, Speed, Prc)
        {
            this.Passangers = Passangers;
        }
        public override string getValues()
        {
            return base.getValues()+ ", мест: " + Passangers;
        }
    }
    class Moto : Car
    {
        public bool Sidecar;
        public Moto(string Name, float Speed, int Prc, bool Sidecar) : base(Name, Speed, Prc)
        {
            this.Sidecar = Sidecar;
        }
        public override string getValues()
        {
            return base.getValues()+ ", коляск" + (Sidecar ? "а есть" : "и нет");
        }
    }
  
    class Program
    {
        static void Main(string[] args)
        {
            StaticCache.SetAppsetNull();
            StaticCache.LoadStaticCache();
            Console.WriteLine(StaticCache.GetAppset().SettingDict.FirstOrDefault().Key + " " + StaticCache.GetAppset().SettingDict.FirstOrDefault().Value + "км");
            Console.WriteLine("Участники:");
            List<Car> Cars = new List<Car>();
            int i = 1;
            foreach (KeyValuePair<string, string> s in StaticCache.GetAppset().SettingDict.Skip(1).ToList())
            {
                string stroka = s.Value.ToString();
                string[] subs = stroka.Split('/');
                float speed = float.Parse(subs[0]);
                int prc = Int32.Parse(subs[1]);
                int dop = !String.IsNullOrEmpty(subs[2])? Int32.Parse(subs[2]):0;
                switch (s.Key.Substring(0, 4))
                {
                    case "груз":
                        Cars.Add(new Truck(s.Key, speed, prc, dop));
                        break;
                    case "легк":
                        Cars.Add(new Sedan(s.Key, speed, prc, dop));
                        break;
                    case "мото":
                        Cars.Add(new Moto(s.Key, speed, prc, dop==1?true:false));
                        break;
                    default:
                        Cars.Add(new Car(s.Key, speed, prc));
                        break;
                }
                i++;
            }
            foreach (Car car in Cars)
            {
                Console.WriteLine(car.getValues());
            }
            Console.ReadLine();
        }
    }
}
