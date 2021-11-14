using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Reflection
{
	class SQL
	{
		public static List<T> selectList<T>(string query)
		{
			// 전달받은 제네릭 타입의 리스트 생성
			List<T> _list = new List<T>();

			// 전달 받은 객체의 변수 목록을 받아옴.
			FieldInfo[] infoArr = Activator.CreateInstance(typeof(T)).GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

			// 받은 쿼리문을 실행하여 데이터 추출
			DataTable dt = ExecuteAdapter(query);
			if(dt != null)
            {
				for (int row = 0; row < dt.Rows.Count; row++)
				{
					// 전달받은 객체의 타입으로 생성.
					object _obj = Activator.CreateInstance(typeof(T));

					// 객체의 변수 목록만큼 반복.
					for (int i = 0; i < infoArr.Length; i++)
					{
						// 데이터 테이블 컬럼의 수만큼 반복하며 객체의 변수명과 동일한 컬럼을 찾음.
						for (int col = 0; col < dt.Columns.Count; col++)
						{
							// <변수명>k__BackingField 이므로 텍스트를 잘라 변수명만 체크함.
							if (infoArr[i].Name.ToUpper().Substring(1).Split('>')[0] == dt.Columns[col].ColumnName.ToUpper())
							{
								// 발견한 데이터를 입력.
								string value = dt.Rows[row][col].ToString();
								FieldInfo info = infoArr[i];
								ConvertObjType(info, _obj, value);
								break;
							}
						}
					}
					// 배열에 추가.
					_list.Add((T)_obj);
				}
            }
			return _list;
		}

		public static T selectOne<T>(string query)
		{
			// 전달받은 객체의 타입으로 생성.
			object obj = Activator.CreateInstance(typeof(T));

			// 전달 받은 객체의 변수 목록을 받아옴.
			FieldInfo[] infoArr = Activator.CreateInstance(typeof(T)).GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

			// 받은 쿼리문을 실행하여 데이터 추출
			DataTable dt = ExecuteAdapter(query);

			if (dt != null && dt.Rows.Count > 0)
			{
				// 객체의 변수 목록만큼 반복.
				for (int i = 0; i < infoArr.Length; i++)
				{				
					// 데이터 테이블 컬럼의 수만큼 반복하며 객체의 변수명과 동일한 컬럼을 찾음.
					for (int col = 0; col < dt.Columns.Count; col++)
					{
						// <변수명>k__BackingField 이므로 텍스트를 잘라 변수명만 체크함.
						if (infoArr[i].Name.ToUpper().Substring(1).Split('>')[0] == dt.Columns[col].ColumnName.ToUpper())
						{
							// 발견한 데이터를 입력.
							object value = dt.Rows[0][col];
							FieldInfo info = infoArr[i];
							ConvertObjType(info, obj, value);
							break;
						}
					}
				}
			}
			return (T)obj;
		}

		public static int insert<T>(T obj, string query)
        {
			string _query = string.Empty;
			_query = ConvertText(obj, query);
			return ExecuteScalar(_query);
		}

		public static int insert<T>(List<T> list, string query)
		{
			string _query = string.Empty;
			foreach (T obj in list)
			{
				_query += ConvertText(obj, query);
				if (_query.Substring(_query.Length - 1) != ";") _query += ";";
			}
			return ExecuteScalar(_query);
		}

		public static int Update<T>(T obj, string query)
		{
			string _query = string.Empty;
			_query = ConvertText(obj, query);
			return ExecuteNonQuery(_query);
		}

		public static int Update<T>(List<T> list, string query)
		{
			string _query = string.Empty;
			foreach (T obj in list)
			{
				_query += ConvertText(obj, query);
				if (_query.Substring(_query.Length - 1) != ";") _query += ";";
			}
			return ExecuteNonQuery(_query);
		}

		public static int Delete(string query)
		{
			return ExecuteNonQuery(query);
		}

		public static int Delete<T>(T obj, string query)
		{
			string _query = string.Empty;
			_query = ConvertText(obj, query);
			return ExecuteNonQuery(_query);
		}

		public static int Delete<T>(List<T> list, string query)
		{
			string _query = string.Empty;
			foreach (T obj in list)
			{
				_query += ConvertText(obj, query);
				if (_query.Substring(_query.Length - 1) != ";") _query += ";";
			}
			return ExecuteNonQuery(_query);
		}

		private static void ConvertObjType(FieldInfo info, object obj, object value)
		{
			// 해당하는 형태의 변수로 변환해서 삽입.
			if (info.FieldType == typeof(System.Int32))
			{
				int result;
				Int32.TryParse(value.ToString(), out result);
				info.SetValue(obj, result);
			}
			else if (info.FieldType == typeof(System.Double))
			{
				double result;
				double.TryParse(value.ToString(), out result);
				info.SetValue(obj, result);
			}
			else if (info.FieldType == typeof(System.String))
			{
				info.SetValue(obj, Convert.ToString(value));
			}
			else if (info.FieldType == typeof(System.DateTime))
			{
				info.SetValue(obj, Convert.ToDateTime(value));
			}
		}

		private static string ConvertText<T>(T obj, string query)
        {
			while (true)
			{
				// 템플릿 문자열 체크
				if (query.IndexOf("$[") == -1) break;
				else
				{
					int startNumber = query.IndexOf("$[");
					int endNumber = query.IndexOf("]", startNumber) + 1;
					string getText = query.Substring(startNumber, endNumber - startNumber);

					// 전달받은 객체의 변수 목록 확인
					FieldInfo[] infoArr = obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

					string f = query.Substring(0, startNumber);
					string b = query.Substring(endNumber);

					for (int i = 0; i < infoArr.Length; i++)
					{
						// 객체의 값이 있는경우만 진행
						if (infoArr[i] != null)
						{
							if (infoArr[i].Name.ToUpper().Substring(1).Split('>')[0] == getText.ToUpper().Substring(2, getText.Length - 3))
							{
								string v = "";

								if (infoArr[i].FieldType == typeof(System.DateTime))
								{
									v = ((DateTime)infoArr[i].GetValue(obj)).ToString("yyyy-MM-dd HH:mm:ss");
								} 
								else
								{
									v = infoArr[i].GetValue(obj).ToString(); ;
								}

								if (infoArr[i].FieldType == typeof(System.String))
								{
									query = $"{f}N'{v}'{b}";
								}
								else
								{
									query = $"{f}N'{v}'{b}";
								}
								break;
							}
						}

						if (i == infoArr.Length - 1)
						{
							string v = infoArr[i].GetValue(obj).ToString();
							query = $"{f}''{b}";
						}
					}
				}
			}
			return query;
		}
		
		public class Conn
		{
			public string ip { get; set; }
			public string port { get; set; }		
			public string database { get; set; }
			public string id { get; set; }
			public string pw { get; set; }

		}

		private static SqlConnection connection;

		public static void setConnection(Conn conn)
		{
			string connStr = string.Format("SERVER = {0}; DATABASE = {2}; UID = {3}; PASSWORD = {4}", conn.ip, conn.port, conn.database, conn.id, conn.pw);
			connection = new SqlConnection(connStr);
		}

		private static int ExecuteScalar(string query)
		{
			try
			{
				if (connection == null) throw new Exception("데이터 베이스가 연결되지 않았습니다");

				using (SqlCommand command = new SqlCommand($"{query}; SELECT CAST(SCOPE_IDENTITY() AS INT)".ToUpper(), query))
				{
					connection.Open();
					int seqID = (Int32)command.ExecuteScalar();
					connection.Close();
					return seqID;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return -1;
			}
		}

		private static int ExecuteNonQuery(string query)
		{
			try
			{
				if (connection == null) throw new Exception("데이터 베이스가 연결되지 않았습니다");

				using (SqlCommand command = new SqlCommand(query.ToUpper(), connection))
				{
					connection.Open();
					int seqID = (Int32)command.ExecuteNonQuery();
					connection.Close();
					return seqID;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return -1;
			}
		}

		private static DataTable ExecuteAdapter(string query)
		{
			try
			{
				if (connection == null) throw new Exception("데이터 베이스가 연결되지 않았습니다");

				using (SqlCommand command = new SqlCommand(query.ToUpper(), connection))
				{
					SqlDataAdapter sqladapter = new SqlDataAdapter();
					sqladapter.SelectCommand = command;
					DataTable memDT = new DataTable();
					sqladapter.Fill(memDT);

					return memDT;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
		}
	}
}
