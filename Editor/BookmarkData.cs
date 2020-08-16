using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Kogane.Internal
{
	/// <summary>
	/// ブックマークの項目を管理するクラス
	/// </summary>
	[Serializable]
	internal sealed class BookmarkData : TreeViewItem
	{
		//==============================================================================
		// 変数(SerializeField)
		//==============================================================================
		[SerializeField] private string m_guid = "";

		//==============================================================================
		// 変数
		//==============================================================================
		private Object m_asset;

		//==============================================================================
		// プロパティ
		//==============================================================================
		public string Guid    => m_guid;
		public bool   IsValid => !string.IsNullOrWhiteSpace( m_guid );
		public string Name    => IsValid ? Asset.name : string.Empty;

		public Object Asset
		{
			get
			{
				if ( !IsValid ) return null;

				if ( m_asset == null )
				{
					var path = AssetDatabase.GUIDToAssetPath( m_guid );

					m_asset = AssetDatabase.LoadAssetAtPath<Object>( path );
				}

				return m_asset;
			}
		}

		//==============================================================================
		// 関数
		//==============================================================================
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public BookmarkData( int id, string guid ) : base( id )
		{
			m_guid = guid;
		}
	}
}