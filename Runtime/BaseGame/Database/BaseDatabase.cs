using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dma.BaseGame
{
    public abstract class BaseDatabase : ScriptableObject
    {
        public abstract List<string> Ids { get; }
        public abstract Type DatabaseType { get; }
    }

    public abstract class BaseDatabase<TDatabase, TDatabaseElememt> : BaseDatabase
        where TDatabase : BaseDatabase
        where TDatabaseElememt : BaseDatabaseElement
    {
        private static TDatabase m_Instance;
        public static TDatabase Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = DatabaseManager.Instance.GetDatabaseByType<TDatabase>();
                }

                return m_Instance;
            }
        }

        private static Type s_DatabaseType = typeof(TDatabase);

        public override List<string> Ids => DataDict.Keys.ToList();
        public override Type DatabaseType => s_DatabaseType;

        public List<TDatabaseElememt> Data;

        private Dictionary<string, TDatabaseElememt> m_DataDict;
        public Dictionary<string, TDatabaseElememt> DataDict
        {
            get
            {
                if (m_DataDict == null || m_DataDict.Count != Data.Count)
                {
                    m_DataDict = new Dictionary<string, TDatabaseElememt>();
                    foreach (var data in Data)
                    {
                        m_DataDict.Add(data.Id, data);
                    }
                }

                return m_DataDict;
            }
        }
    }
}
