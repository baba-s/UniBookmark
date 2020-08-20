using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Kogane.Internal
{
	/// <summary>
	/// すべてのブックマークの情報を管理するクラス
	/// </summary>
	[Serializable]
	internal sealed class BookmarkList
	{
		//==============================================================================
		// 変数(SerializeField)
		//==============================================================================
		[SerializeField] private List<BookmarkData> m_list = new List<BookmarkData>();

		//==============================================================================
		// プロパティ
		//==============================================================================
		[NotNull] public IReadOnlyList<BookmarkData> List => m_list;

		//==============================================================================
		// 関数
		//==============================================================================
		/// <summary>
		/// 既に登録されているブックマークの場合 true を返します
		/// </summary>
		public bool Contains( string guid )
		{
			return m_list.Any( x => x.Guid == guid );
		}

		/// <summary>
		/// ブックマークを追加します
		/// </summary>
		public void Add( BookmarkData data )
		{
			m_list.Add( data );
		}

		/// <summary>
		/// ブックマークを削除します
		/// </summary>
		public void Remove( BookmarkData data )
		{
			var index = m_list.FindIndex( x => x.Guid == data.Guid );
			m_list.RemoveAt( index );
		}
	}
}