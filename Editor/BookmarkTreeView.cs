using System;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Kogane.Internal
{
	/// <summary>
	/// ブックマークウィンドウのツリービューを管理するクラス
	/// </summary>
	internal sealed class BookmarkTreeView : TreeView
	{
		//==============================================================================
		// 列挙型
		//==============================================================================
		private enum ColumnType
		{
			NAME,
			PING,
			OPEN,
			REMOVE,
		}

		// 要素が存在しない場合、 TreeView は例外を発生する
		// そのため、要素が存在しない場合は表示しないダミーデータを追加する
		private BookmarkData[] List =>
			BookmarkSaveData.List.List
				.DefaultIfEmpty( new BookmarkData( 0, null ) )
				.ToArray();
		
		//==============================================================================
		// イベント
		//==============================================================================
		public Action OnReload;

		//==============================================================================
		// 関数
		//==============================================================================
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public BookmarkTreeView
		(
			TreeViewState     state,
			MultiColumnHeader header
		) : base( state, header )
		{
			rowHeight                     =  16;
			showAlternatingRowBackgrounds =  true;
			header.sortingChanged         += SortItems;

			header.ResizeToFit();
			Reload();
			header.SetSorting( 0, true );
		}

		/// <summary>
		/// ツリーを作成します
		/// </summary>
		protected override TreeViewItem BuildRoot()
		{
			var root = new TreeViewItem { depth = -1 };

			foreach ( var n in List )
			{
				root.AddChild( n );
			}

			return root;
		}

		/// <summary>
		/// 各行の GUI を描画します
		/// </summary>
		protected override void RowGUI( RowGUIArgs args )
		{
			if ( !( args.item is BookmarkData item ) || !item.IsValid ) return;

			var asset   = item.Asset;
			var path    = AssetDatabase.GetAssetPath( asset );
			var columns = args.GetNumVisibleColumns();

			for ( var i = 0; i < columns; i++ )
			{
				var rect        = args.GetCellRect( i );
				var columnIndex = ( ColumnType ) args.GetColumn( i );

				switch ( columnIndex )
				{
					case ColumnType.NAME:
						EditorGUIUtility.SetIconSize( new Vector2( 16, 16 ) );
						var label = EditorGUIUtility.ObjectContent( asset, null );
						EditorGUI.LabelField( rect, label );
						break;

					case ColumnType.PING:
						if ( GUI.Button( rect, "ping", EditorStyles.miniButton ) )
						{
							EditorGUIUtility.PingObject( asset );
						}

						break;

					case ColumnType.OPEN:
						var isFolder = AssetDatabase.IsValidFolder( path );
						using ( new EditorGUI.DisabledScope( isFolder ) )
						{
							if ( GUI.Button( rect, "open", EditorStyles.miniButton ) )
							{
								AssetDatabase.OpenAsset( asset );
							}
						}

						break;

					case ColumnType.REMOVE:
						if ( GUI.Button( rect, "x", EditorStyles.miniButton ) )
						{
							BookmarkSaveData.List.Remove( item );
							BookmarkSaveData.Save();
							OnReload?.Invoke();
						}

						break;
				}
			}
		}

		/// <summary>
		/// 検索される時に呼び出されます
		/// </summary>
		protected override bool DoesItemMatchSearch( TreeViewItem treeViewItem, string search )
		{
			if ( !( treeViewItem is BookmarkData item ) ) return false;

			var name = item.Name;

			return name.IndexOf( search, StringComparison.OrdinalIgnoreCase ) != -1;
		}

		/// <summary>
		/// ソートされる時に呼び出されます
		/// </summary>
		private void SortItems( MultiColumnHeader header )
		{
			var ascending = header.IsSortedAscending( header.sortedColumnIndex );

			var ordered = List
					.OrderBy( c => c.Name )
					.ThenBy( c => c.id )
				;

			var items = ordered.AsEnumerable();

			if ( !ascending )
			{
				items = items.Reverse();
			}

			rootItem.children = items
					.Cast<TreeViewItem>()
					.ToList()
				;

			BuildRows( rootItem );
		}
	}
}