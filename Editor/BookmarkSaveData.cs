using System.IO;
using UnityEngine;

namespace Kogane.Internal
{
	/// <summary>
	/// ブックマークのセーブデータを管理するクラス
	/// </summary>
	internal static class BookmarkSaveData
	{
		//==============================================================================
		// 定数
		//==============================================================================
		private const string PATH = "UniBookmark/settings.json";

		//==============================================================================
		// 変数(static)
		//==============================================================================
		private static BookmarkList m_list;

		//==============================================================================
		// プロパティ(static)
		//==============================================================================
		public static BookmarkList List => m_list;

		//==============================================================================
		// 関数(static)
		//==============================================================================
		/// <summary>
		/// セーブデータを読み込みます
		/// </summary>
		public static void Load()
		{
			if ( File.Exists( PATH ) )
			{
				var json = File.ReadAllText( PATH );

				m_list = string.IsNullOrWhiteSpace( json )
						? new BookmarkList()
						: JsonUtility.FromJson<BookmarkList>( json ) ?? new BookmarkList()
					;
			}
			else
			{
				m_list = new BookmarkList();
			}

			// ID を割り振りし直し
			// ID を割り振らないとすべての項目の ID が 0 になり、
			// 項目を選択した時にすべての項目が選択された状態になってしまう
			for ( var i = 0; i < m_list.List.Count; i++ )
			{
				m_list.List[ i ].id = i;
			}
		}

		/// <summary>
		/// セーブデータを保存します
		/// </summary>
		public static void Save()
		{
			var json = JsonUtility.ToJson( m_list, true );
			var dir  = Path.GetDirectoryName( PATH );

			if ( !Directory.Exists( dir ) )
			{
				Directory.CreateDirectory( dir );
			}

			File.WriteAllText( PATH, json );
		}
	}
}