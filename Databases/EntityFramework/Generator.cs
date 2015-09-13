using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NETPath.Generators.Interfaces;

namespace NETPath.Generators.Databases.EntityFramework
{
	public class Generator : NETPath.Generators.Interfaces.IDatabase
	{
		public void GenerateConvertToNetworkType(StringBuilder code, IEntityType database, IEnumerable<IEntityElement> elements)
		{
			throw new NotImplementedException();
		}

		public void GenerateConvertToDatabaseType(StringBuilder code, IEntityType database, IEnumerable<IEntityElement> elements)
		{
			throw new NotImplementedException();
		}

		public void GenerateInsert(StringBuilder code, IEntityType database, IEnumerable<IEntityElement> elements)
		{
			throw new NotImplementedException();
		}

		public void GenerateUpdate(StringBuilder code, IEntityType database, IEnumerable<IEntityElement> elements)
		{
			throw new NotImplementedException();
		}

		public void GenerateDelete(StringBuilder code, IEntityType database, IEnumerable<IEntityElement> elements)
		{
			throw new NotImplementedException();
		}

		public void GenerateConvertToNetworkType(StringBuilder code, ISqlType database, IEnumerable<ISqlElement> elements)
		{
		}

		public void GenerateConvertToDatabaseType(StringBuilder code, ISqlType database, IEnumerable<ISqlElement> elements)
		{
		}

		public void GenerateInsert(StringBuilder code, ISqlType database, IEnumerable<ISqlElement> elements)
		{
		}

		public void GenerateUpdate(StringBuilder code, ISqlType database, IEnumerable<ISqlElement> elements)
		{
		}

		public void GenerateDelete(StringBuilder code, ISqlType database, IEnumerable<ISqlElement> elements)
		{
		}

		public void GenerateMerge(StringBuilder code, ISqlType database, IEnumerable<ISqlElement> elements)
		{
		}

		public string Name { get { return "Entity Framework 6"; } }
		public ObservableCollection<string> SqlTypes { get; }
	}
}
