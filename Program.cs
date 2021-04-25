﻿using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
    public class Itog
    {
        public string Name { get; set; }
    }
    public class Itogset
    {
        public List<Itog> ItogList { get; set; }
    }
    public class StaticItog
    {
        private static Itogset Itogset = null;

        public static Itogset GetItogSetting()
        {
            Itogset Itogset = new Itogset();
            Itogset.ItogList = new List<Itog>();
            return Itogset;
        }
        public static void LoadStaticItog()
        {
            Itogset = StaticItog.GetItogSetting();
        }
        public static void AddStaticItog(Itog itog)
        {
            Itogset = StaticItog.GetItogset();
            Itogset.ItogList.Add(itog);
        }
        public static Itogset GetItogset()
        {
            return Itogset;
        }
        public static void SetItogsetNull()
        {
            Itogset = null;
        }
    }

    class Car
    {
        public delegate void Start();
        public delegate void Message();
        public event Message Display;
        public string Name;
        public float Speed;
        public int Prc;

        public virtual string getValues()
        {
            return this.Name + ", ск: " + this.Speed + "км/ч, в/пр: " + this.Prc + "%";
        }
        public Car(string Name, float Speed, int Prc)
        {
            this.Name = Name;
            this.Speed = Speed;
            this.Prc = Prc;
        }
        public float RandomSpeed(Random rnd, float speed)
        {
            return speed - rnd.Next((int)Math.Round(speed * 0.3));
        }

        public void MovingText(string stroka)
        {
            Console.Clear();
            Console.WriteLine($"{stroka} ");
        }

        static bool Ve(int percent, Random rnd = null)
        {
            return rnd.Next(100 - percent) == 0;
        }
        public void Drive()
        {
            int nprokol = 0;
            for (int i = 0; i <= Int32.Parse(StaticCache.GetAppset().SettingDict.FirstOrDefault().Value); i += 1)
            {
                Random rnd = new Random();
                bool prokol = Ve(Prc, rnd);
                string textProkol = "";
                int sleepProkol = 0;
                if (prokol)
                {
                    sleepProkol = 500;
                    textProkol += " Прокол!!!";
                    nprokol++;
                }
                Thread.Sleep(350 + sleepProkol - (int)Math.Round(RandomSpeed(new Random(), Speed)));
                Display += () => MovingText(Name + " прошел расстояние " + i.ToString() + "км" + textProkol);
                if (i == Int32.Parse(StaticCache.GetAppset().SettingDict.FirstOrDefault().Value))
                {
                    StaticItog.AddStaticItog(new Itog { Name = Name + (nprokol > 0 ? " (проколов: " + nprokol.ToString() + ")" : "") });
                }
                Display();
            }
        }
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
            return base.getValues() + ", гр: " + Carry + "тн";
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
            return base.getValues() + ", мест: " + Passangers;
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
            return base.getValues() + ", коляск" + (Sidecar ? "а есть" : "и нет");
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
                int dop = !String.IsNullOrEmpty(subs[2]) ? Int32.Parse(subs[2]) : 0;
                switch (s.Key.Substring(0, 4))
                {
                    case "груз":
                        Cars.Add(new Truck(s.Key, speed, prc, dop));
                        break;
                    case "легк":
                        Cars.Add(new Sedan(s.Key, speed, prc, dop));
                        break;
                    case "мото":
                        Cars.Add(new Moto(s.Key, speed, prc, dop == 1 ? true : false));
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

            ConsoleKey response;
            Console.WriteLine("Стартуем? [y/n] ");
            response = Console.ReadKey(true).Key;   
            while (response == ConsoleKey.Y)
            {
                StaticItog.SetItogsetNull();
                StaticItog.LoadStaticItog();
                List<Thread> threads = new List<Thread>();
                foreach (Car car in Cars)
                {
                    threads.Add(new Thread(car.Drive));
                }
                foreach (Thread list in threads)
                {
                    list.Start();
                }
                foreach (Thread list in threads)
                {
                    list.Join();
                }
                Console.Clear();
                Console.WriteLine("Таблица первенства: ");
                string[] ItogList=StaticItog.GetItogset().ItogList.Select(s=>s.Name).ToArray();
                int j = 1;
                foreach (string itog in ItogList)
                {
                    Console.WriteLine($"Место: {j}  {itog} ");
                    j++;
                }
                Console.WriteLine("еще круг? [y/n] ");
                response = Console.ReadKey(true).Key;
            }
        }
    }
}
