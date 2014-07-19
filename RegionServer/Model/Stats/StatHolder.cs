using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ExitGames.Logging;
using RegionServer.Calculators;
using RegionServer.Model.Interfaces;

namespace RegionServer.Model.Stats
{
    public class StatHolder : IStatHolder
    {
        private Dictionary<Type, IStat> _stats;
        protected Dictionary<IStat, Calculator> Calculators = new Dictionary<IStat, Calculator>(); 
        //protected ILogger Log = LogManager.GetCurrentClassLogger();

        public StatHolder(IEnumerable<IStat> stats)
        {
            _stats = new Dictionary<Type, IStat>();

            foreach (var stat in stats)
            {
                Calculators.Add(stat, new Calculator());
                _stats.Add(stat.GetType(), stat);

                var derived = stat as IDerivedStat;

                if (derived != null)
                {
                    Calculators[stat].AddFunction(derived.Functions);
                }
            }
        }

        public float CalcStat(IStat stat)
        {
            return CalcStat(stat, null);
        }

        public float CalcStat(IStat stat, ICharacter target)
        {
            float returnValue = stat.BaseValue;

            var calculator = Calculators[stat];
            var env = new Calculators.Environment()
            {
                Value = returnValue,
                Character = Character,
                Target = target
            };

            calculator.Calculate(env);

            if (env.Value <= 0 && stat.IsNonZero)
            {
                return 1;
            }

            return env.Value;
        }

        [Serializable]
        public class SerializedStat
        {
            public string StatType { get; set; }
            public float StatValue { get; set; }
        }

        #region Implementation of IStatHolder

        public ICharacter Character { get; set; }
        public Dictionary<Type, IStat> Stats { get { return _stats; } }

        public float GetStat<T>() where T : class, IStat
        {
            IStat result;
            _stats.TryGetValue(typeof (T), out result);

            if (result != null)
            {
                return CalcStat(result);
            }

            return 0;
        }

        public float GetStat<T>(T stat) where T : class, IStat
        {
            IStat result;
            _stats.TryGetValue(typeof (T), out result);

            if (result == null)
            {
                _stats.TryGetValue(((dynamic) stat).GetType(), out result);
            }

            if (result != null)
            {
                return CalcStat(result);
            }

            return 0;
        }

        public void SetStat<T>(float value) where T : class, IStat
        {
            IStat result;
            _stats.TryGetValue(typeof (T), out result);

            if (result != null)
            {
                result.BaseValue = value;
            }
        }

        public string SerializedStats()
        {
            List<SerializedStat> StatValues = new List<SerializedStat>();

            foreach (var stat in Stats.Values.ToList())
            {
                StatValues.Add(new SerializedStat()
                {
                    StatType = stat.Name,
                    StatValue = stat.BaseValue
                });
            }

            XmlSerializer mySerializer = new XmlSerializer(typeof(List<SerializedStat>));
            StringWriter writer = new StringWriter();
            mySerializer.Serialize(writer, StatValues.ToList());

            return writer.ToString();
        }

        public void DeserializeStats(string stats)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(List<SerializedStat>));
            StringReader reader = new StringReader(stats);

            foreach (var stat in (List<SerializedStat>)mySerializer.Deserialize(reader))
            {
                var result = _stats.Values.FirstOrDefault(s => s.Name == stat.StatType);

                if (result != null)
                {
                    result.BaseValue = stat.StatValue;
                }
            }
        }

        #endregion
    }
}