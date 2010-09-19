#region License
//The contents of this file are subject to the Mozilla Public License
//Version 1.1 (the "License"); you may not use this file except in
//compliance with the License. You may obtain a copy of the License at
//http://www.mozilla.org/MPL/
//Software distributed under the License is distributed on an "AS IS"
//basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
//License for the specific language governing rights and limitations
//under the License.
#endregion

using System;
using System.Data;
using System.Configuration;
using Migrator.Framework;
using Migrator.Providers.SQLite;
using NUnit.Framework;

namespace Migrator.Tests.Providers
{
     [TestFixture, Category("SQLite")]
     public class SQLiteTransformationProviderTest : TransformationProviderBase
     {
         SQLiteTransformationProvider _sqlite_provider;

         [SetUp]
         public void SetUp()
         {
             string constr = ConfigurationManager.AppSettings["SQLiteConnectionString"];
             if (constr == null)
                 throw new ArgumentNullException("SQLiteConnectionString", "No config file");

             _provider = new SQLiteTransformationProvider(new SQLiteDialect(), constr);
             _provider.BeginTransaction();
             _sqlite_provider = (SQLiteTransformationProvider) _provider;
            
             AddDefaultTable();
         }

         [Test]
         public void CanParseSqlDefinitions() 
         {
             const string testSql = "CREATE TABLE bar ( id INTEGER PRIMARY KEY AUTOINCREMENT, bar TEXT, baz INTEGER NOT NULL )";
             string[] columns = _sqlite_provider.ParseSqlColumnDefs(testSql);
             Assert.IsNotNull(columns);
             Assert.AreEqual(3, columns.Length);
             Assert.AreEqual("id INTEGER PRIMARY KEY AUTOINCREMENT", columns[0]);
             Assert.AreEqual("bar TEXT", columns[1]);
             Assert.AreEqual("baz INTEGER NOT NULL", columns[2]);
         }
         
         [Test]
         public void CanParseSqlDefinitionsForColumnNames() 
         {
             const string testSql = "CREATE TABLE bar ( id INTEGER PRIMARY KEY AUTOINCREMENT, bar TEXT, baz INTEGER NOT NULL )";
             string[] columns = _sqlite_provider.ParseSqlForColumnNames(testSql);
             Assert.IsNotNull(columns);
             Assert.AreEqual(3, columns.Length);
             Assert.AreEqual("id", columns[0]);
             Assert.AreEqual("bar", columns[1]);
             Assert.AreEqual("baz", columns[2]);
         }

         [Test]
         public void CanParseColumnDefForNotNull()
         {
             const string nullString    = "bar TEXT";
             const string notNullString = "baz INTEGER NOT NULL";
             Assert.AreEqual(ColumnProperty.Null,    _sqlite_provider.GetColumnFromDef(nullString).ColumnProperty);
             Assert.AreEqual(ColumnProperty.NotNull, _sqlite_provider.GetColumnFromDef(notNullString).ColumnProperty);
         }

         [Test]
         public void CanParseColumnDefForName()
         {
             const string nullString    = "bar TEXT";
             const string notNullString = "baz INTEGER NOT NULL";
             Assert.AreEqual("bar", _sqlite_provider.GetColumnFromDef(nullString).Name);
             Assert.AreEqual("baz", _sqlite_provider.GetColumnFromDef(notNullString).Name);
         }

         [Test]
         public void CanParseColumnDefForStringType()
         {
             Assert.AreEqual(DbType.String, _sqlite_provider.GetColumnFromDef("bar TEXT").Type);
         }

         // TODO select a different DbType based on the column length (?)
         [Test]
         public void CanParseColumnDefForIntegerType()
         {
             Assert.AreEqual(DbType.Int32, _sqlite_provider.GetColumnFromDef("baz INTEGER NOT NULL").Type);
         }

         [Test]
         public void CanParseColumnDefForDecimalType()
         {
             Assert.AreEqual(DbType.Decimal, _sqlite_provider.GetColumnFromDef("payment_amount NUMERIC").Type);
         }

         [Test]
         public void CanParseColumnDefForDateTimeType()
         {
             Assert.AreEqual(DbType.DateTime, _sqlite_provider.GetColumnFromDef("created_at DATETIME").Type);
         }

         [Test]
         public void CanParseColumnDefForGuidType()
         {
             Assert.AreEqual(DbType.Guid, _sqlite_provider.GetColumnFromDef("guid UNIQUEIDENTIFIER").Type);
         }

         [Test]
         public void CanParseColumnDefForBinaryType()
         {
             Assert.AreEqual(DbType.Binary, _sqlite_provider.GetColumnFromDef("uploaded_image BLOB").Type);
         }
     }
}
