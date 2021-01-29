using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dma.BaseGame
{
    public class DatabaseManager : SingletoManagern<DatabaseManager>
    {
        public List<BaseDatabase> Databases;

        private Dictionary<Type, BaseDatabase> m_DatabaseDictionary;

        public T GetDatabaseByType<T>()
            where T : BaseDatabase
        {
            var type = typeof(T);

            if (m_DatabaseDictionary == null)
            {
                PopulateDictionary();
            }

            T database = null;

            if (m_DatabaseDictionary.TryGetValue(type, out BaseDatabase baseDatabase))
            {
                database = baseDatabase as T;
            }

            if (database == null)
            {
                Debug.LogError($"Cannot find Database {type} in DatabaseManager");
            }

            return database;
        }

        private void PopulateDictionary()
        {
            m_DatabaseDictionary = new Dictionary<Type, BaseDatabase>();

            if (Databases != null)
            {
                foreach (var database in Databases)
                {
                    m_DatabaseDictionary.Add(database.DatabaseType, database);
                }
            }
        }
    }
}