using System;
using System.Collections.Generic;
using System.Threading;

namespace RailPatternsWacuumCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            Expression isDust = getDustExpression();
            Expression isGrease = getGreaseExpression();
            Expression isFilth = getFilthExpression();

            Console.WriteLine("Выберите вид загрязнения, из представленных ниже.");
            Console.WriteLine("В случае несовпадения с режимом пылесоса будет запущена  режим комплексной очистки всех загрязнений.");
            Console.WriteLine("Пыль     /    Грязь     /    Жирное пятно     /    Сгусток пыли     /    Маслянное пятно     /    Грязь с улицы \n\n");
            string cleanigMode = Console.ReadLine();

            if (isDust.interpret(cleanigMode))
                Console.WriteLine("Выбран режим очистки от пыли.");
            else if (isGrease.interpret(cleanigMode))
                Console.WriteLine("Выбран режим очистки от жирных пятен.");
            else if (isFilth.interpret(cleanigMode))
                Console.WriteLine("Выбран режим очистки от грязи.");
            else
                Console.WriteLine("Выбран режим комплексной очистки от всех загрязнений.");

           Console.WriteLine();

            Vision vision = new Vision();
            Turbine turbine = new Turbine();
            VoiceAssistant voiceAssistant = new VoiceAssistant();
            WheelEngine wheelEngine = new WheelEngine();

            Facade facade = new Facade(vision, turbine, voiceAssistant, wheelEngine);

            Client.ClientOperation(facade);
        }

        // INTERPRETATOR
        public static Expression getDustExpression()
        {
            Expression Dust = new TerminalExpression("Пыль");
            Expression ClotOfDust = new TerminalExpression("Сгусток пыли");

            return new OrExpression(Dust, ClotOfDust);
        }

        public static Expression getGreaseExpression()
        {
            Expression Grease = new TerminalExpression("Жирное пятно");
            Expression oil = new TerminalExpression("Маслянное пятно");

            return new OrExpression(Grease, oil);
        }

        public static Expression getFilthExpression()
        {
            Expression Filth = new TerminalExpression("Грязь");
            Expression Dirt = new TerminalExpression("Грязь с улицы");

            return new OrExpression(Filth, Dirt);
        }
    }

    /* FACADE */
    public class Facade
    {
        protected Vision vision;
        protected Turbine turbine;
        protected VoiceAssistant assistant;
        protected WheelEngine engine;

        public Facade(Vision vision, Turbine turbine, VoiceAssistant voiceAssistant, WheelEngine wheelEngine)
        {
            this.vision = vision;
            this.turbine = turbine;
            this.assistant = voiceAssistant;
            this.engine = wheelEngine;
        }

        // Facade code
        public void Operation()
        {
            vision.Operation();

            assistant.Operation();

            engine.Operation();

            Console.WriteLine();

            var wheelEngine = new WheelEngine();
            var voiceAssistant = new VoiceAssistant();

            turbine.Attach(wheelEngine);
            turbine.Attach(voiceAssistant);
            turbine.SomeBusinessLogic();
            turbine.Detach(voiceAssistant);
            turbine.SomeBusinessLogic();
        }
    }

    class Client
    {
        public static void ClientOperation(Facade facade)
        {
            facade.Operation();
        }
    }

    public class Vision
    {
        public void Operation()
        {
            Console.WriteLine("Камера: Фокусировка. Поиск загрязнений.");
        }
    }

    /*OBSERVER*/

    public interface IObserver
    {
        void Update(ISubject subject);
    }

    public interface ISubject
    {
        void Attach(IObserver observer);
        void Detach(IObserver observer);

        void Notify();
    }

    public class Turbine : ISubject
    {
        public int energy { get; set; } = -0;

        private List<IObserver> _observers = new List<IObserver>();

        public void Attach(IObserver observer)
        {
            Console.WriteLine("Турбина: Наблюдатель добавлен.");
            this._observers.Add(observer);
        }
        public void Detach(IObserver observer)
        {
            this._observers.Remove(observer);
            Console.WriteLine("Турбина: Наблюдатель удален.");
        }

        public void Notify()
        {
            Console.WriteLine("Турбина: Предупреждение наблюдателей...");

            foreach (var observer in _observers)
            {
                observer.Update(this);
            }
        }

        public void SomeBusinessLogic()
        {
            Console.WriteLine("\nТурбина: Работа в штатном режиме.");
            this.energy = new Random().Next(0, 15);

            // Decorator code
            CleaningMode cleaningMode1 = new DefaultClean();
            cleaningMode1 = new TurbineSuctionCleaningMode(cleaningMode1);
            Console.WriteLine("Режим: {0}", cleaningMode1.mode);
            Console.WriteLine("Потребление энергии: {0}", cleaningMode1.GetPowerConsumption());

            Console.WriteLine("");

            CleaningMode cleaningMode2 = new IntensiveClean();
            cleaningMode2 = new BrushingCleaningMode(cleaningMode2);
            cleaningMode2 = new TurbineSuctionCleaningMode(cleaningMode2);
            Console.WriteLine("Режим: {0}", cleaningMode2.mode);
            Console.WriteLine("Потребление энергии: {0}", cleaningMode2.GetPowerConsumption());
            Thread.Sleep(15);
            Console.WriteLine("Турбина: Осталось энергии " + this.energy + "%.");
            // Observer code
            this.Notify();
        }
    }

    public class WheelEngine : IObserver
    {
        public void Update(ISubject subject)
        {
            if ((subject as Turbine).energy < 10)
            {
                Console.WriteLine("Движок колес: Принято уведомление. Низкий заряд батареи.");
                Console.WriteLine("Движок колес: Расчет маршрута до рабочей станции.");
                Console.WriteLine("Движок колес: Отправка на подзарядку.");
            }
        }

        public void Operation()
        {
            Console.WriteLine("Движок колес: Начало движения.");
        }
    }

    public class VoiceAssistant : IObserver
    {
        public void Update(ISubject subject)
        {
            if ((subject as Turbine).energy < 5)
            {
                Console.WriteLine("Интерфейс: Принято уведомление. Критический низкий заряд.");
                Console.WriteLine("Интерфейс: Предупреждение о низкой энергии пользователя.");
            }
        }

        public void Operation()
        {
            Console.WriteLine("Интерфейс: Мелодия старта. Сообщение о состоянии заряда.");
        }
    }


    /* DECORATOR */
    abstract class CleaningMode
    {
        public CleaningMode(string s)
        {
            this.mode = s;
        }
        public string mode { get; protected set; }
        public abstract int GetPowerConsumption();
    }

    class DefaultClean : CleaningMode
    {
        public DefaultClean() : base("Обычная чистка") { }
        public override int GetPowerConsumption()
        {
            return 10;
        }
    }

    class IntensiveClean : CleaningMode
    {
        public IntensiveClean() : base("Интенсивная чистка") { }
        public override int GetPowerConsumption()
        {
            return 20;
        }
    }

    abstract class CleaningModeDecorator : CleaningMode
    {
        protected CleaningMode cleaningMode;
        public CleaningModeDecorator(string s, CleaningMode cleaningMode) : base(s)
        {
            this.cleaningMode = cleaningMode;
        }
    }

    class TurbineSuctionCleaningMode : CleaningModeDecorator
    {
        public TurbineSuctionCleaningMode(CleaningMode cleaning)
            : base(cleaning.mode + " с использованием турбины", cleaning) { }

        public override int GetPowerConsumption()
        {
            return cleaningMode.GetPowerConsumption() + 10;
        }
    }

    class BrushingCleaningMode : CleaningModeDecorator
    {
        public BrushingCleaningMode(CleaningMode cleaning)
            : base(cleaning.mode + " щетками и дезинфекцией", cleaning) { }

        public override int GetPowerConsumption()
        {
            return cleaningMode.GetPowerConsumption() + 20;
        }
    }

    /* INTERPRETATOR CODE */

    public interface Expression
    {
        public bool interpret(string context);
    }

    public class AndExpression : Expression
    {
        private Expression expression1;
        private Expression expression2;

        public AndExpression(Expression exp1, Expression exp2)
        {
            expression1 = exp1;
            expression2 = exp2;
        }

        public bool interpret(string context)
        {
            return expression1.interpret(context) && expression2.interpret(context);
        }
    }

    public class OrExpression : Expression
    {
        private Expression expression1;
        private Expression expression2;

        public OrExpression(Expression exp1, Expression exp2)
        {
            expression1 = exp1;
            expression2 = exp2;
        }

        public bool interpret(string context)
        {
            return expression1.interpret(context) || expression2.interpret(context);
        }
    }

    public class TerminalExpression : Expression
    {
        private string data;

        public TerminalExpression(string d)
        {
            data = d;
        }

        public bool interpret(string context)
        {
            if (context.Contains(data))
                return true;

            return false;
        }
    }
}

