using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETPath.Generators.Interfaces
{
	public static class DatabaseLoader
	{
		public static readonly ConcurrentDictionary<string, IDatabase> LoadedModules = new ConcurrentDictionary<string, IDatabase>();

		public static void LoadModules()
		{
			//Build the module path and and determine if it is available on this system.
			string asmfp = System.IO.Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetCallingAssembly().CodeBase).LocalPath);
			if (asmfp == null) return;

			var gl = System.IO.Directory.EnumerateFiles(asmfp, "NETPath.Generators.Database.*.dll");

			foreach (var g in gl)
			{
				var moduleDLL = System.Reflection.Assembly.LoadFile(g);

				var instances = from t in moduleDLL.GetExportedTypes()
								where t.GetInterfaces().Contains(typeof(IDatabase)) && t.GetConstructor(Type.EmptyTypes) != null
								select Activator.CreateInstance(t) as IDatabase;

				foreach (var instance in instances)
				{
					LoadedModules.TryAdd(instance.Name, instance);
				}
			}
		}

		public static Task LoadModulesAsync()
		{
			return Task.Run(() => LoadModules());
		}
	}

	public interface IDatabase
	{
		//Entity Methods
		void GenerateConvertToNetworkType(StringBuilder code, IEntityType database, IEnumerable<IEntityElement> elements);
		void GenerateConvertToDatabaseType(StringBuilder code, IEntityType database, IEnumerable<IEntityElement> elements);
		void GenerateInsert(StringBuilder code, IEntityType database, IEnumerable<IEntityElement> elements);
		void GenerateUpdate(StringBuilder code, IEntityType database, IEnumerable<IEntityElement> elements);
		void GenerateDelete(StringBuilder code, IEntityType database, IEnumerable<IEntityElement> elements);

		//Sql Methods
		void GenerateConvertToNetworkType(StringBuilder code, ISqlType database, IEnumerable<ISqlElement> elements);
		void GenerateConvertToDatabaseType(StringBuilder code, ISqlType database, IEnumerable<ISqlElement> elements);
		void GenerateInsert(StringBuilder code, ISqlType database, IEnumerable<ISqlElement> elements);
		void GenerateUpdate(StringBuilder code, ISqlType database, IEnumerable<ISqlElement> elements);
		void GenerateDelete(StringBuilder code, ISqlType database, IEnumerable<ISqlElement> elements);
		void GenerateMerge(StringBuilder code, ISqlType database, IEnumerable<ISqlElement> elements);

		string Name { get; }

		ObservableCollection<string> SqlTypes { get; }
	}

	public interface IEntityType
	{
		bool HasEntity { get; set; }
		string EntityType { get; set; }
		string EntityContext { get; set; }
		bool CanInsert { get; set; }
		bool CanUpdate { get; set; }
		bool CanDelete { get; set; }
	}

	public interface ISqlType
	{
		bool HasSql { get; set; }
		string SchemaName { get; set; }
		string TableName { get; set; }
		bool CanInsert { get; set; }
		IsolationLevel? InsertTransactionLevel { get; set; }
		bool CanUpdate { get; set; }
		IsolationLevel? UpdateTransactionLevel { get; set; }
		bool CanDelete { get; set; }
		IsolationLevel? DeleteTransactionLevel { get; set; }
		bool CanMerge { get; set; }
		IsolationLevel? MergeTransactionLevel { get; set; }
	}

	public interface IEntityElement
	{
		bool HasEntity { get; set; }
		string EntityName { get; set; }
		string ElementName { get; }
		string ElementType { get; }
	}

	public interface ISqlElement
	{
		bool HasSql { get; set; }
		string SqlName { get; set; }
		string SqlType { get; set; }
		bool IsSqlPrimaryKey { get; set; }
		bool IsSqlPrimaryKeyIdentity { get; set; }
		bool IsSqlComputed { get; set; }
		string ElementName { get; }
		string ElementType { get; }
	}
}
