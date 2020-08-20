using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Kogane.Internal
{
	/// <summary>
	/// アセットをブックマークできるウィンドウ
	/// </summary>
	internal sealed class BookmarkWindow : EditorWindow
	{
		//==============================================================================
		// 定数(const)
		//==============================================================================
		private const string SEARCH_STRING_STATE_KEY = "BookmarkWindow_SearchString";

		//================================================================================
		// 変数
		//================================================================================
		private SearchField      m_searchField;
		private BookmarkHeader   m_header;
		private BookmarkTreeView m_treeView;

		//================================================================================
		// 関数
		//================================================================================
		/// <summary>
		/// 有効になった時に呼び出されます
		/// </summary>
		private void OnEnable()
		{
			BookmarkSaveData.Load();

			var state = new TreeViewState();

			m_header = new BookmarkHeader( null );

			m_treeView = new BookmarkTreeView( state, m_header )
			{
				searchString = SessionState.GetString( SEARCH_STRING_STATE_KEY, string.Empty )
			};

			m_treeView.OnReload += ReloadTreeView;

			m_searchField                         =  new SearchField();
			m_searchField.downOrUpArrowKeyPressed += m_treeView.SetFocusAndEnsureSelectedItem;
		}

		/// <summary>
		/// GUI を描画する時に呼び出されます
		/// </summary>
		private void OnGUI()
		{
			using ( new EditorGUILayout.HorizontalScope() )
			{
				DrawAddAssetButton();
				DrawSearchField();
			}

			DrawTreeView();
			DragTo();
		}

		/// <summary>
		/// アセット追加ボタンを描画します
		/// </summary>
		private void DrawAddAssetButton()
		{
			var singleLineHeight = EditorGUIUtility.singleLineHeight;

			var rect = new Rect
			{
				x      = 0,
				y      = 0,
				width  = position.width / 3,
				height = singleLineHeight - 1
			};

			var current   = Event.current;
			var controlId = GUIUtility.GetControlID( FocusType.Passive );

			if ( GUILayout.Button( "Add Asset", EditorStyles.toolbarButton, GUILayout.Width( rect.width ) ) )
			{
				var filePath  = EditorUtility.OpenFilePanel( "UniBookmark", "", "" );
				var assetPath = FileUtil.GetProjectRelativePath( filePath );

				AddBookmark( assetPath );

				ReloadTreeView();
			}

			switch ( current.type )
			{
				case EventType.DragUpdated:
				case EventType.DragPerform:

					if ( !rect.Contains( current.mousePosition ) ) break;

					DragAndDrop.visualMode      = DragAndDropVisualMode.Copy;
					DragAndDrop.activeControlID = controlId;

					if ( current.type == EventType.DragPerform )
					{
						DragAndDrop.AcceptDrag();

						foreach ( var draggedObject in DragAndDrop.objectReferences )
						{
							var path = AssetDatabase.GetAssetPath( draggedObject );
							AddBookmark( path );
						}

						BookmarkSaveData.Save();
						ReloadTreeView();

						DragAndDrop.activeControlID = 0;
					}

					current.Use();
					break;
			}
		}

		/// <summary>
		/// ブックマークを追加します
		/// </summary>
		private void AddBookmark( string assetPath )
		{
			var guid = AssetDatabase.AssetPathToGUID( assetPath );

			if ( string.IsNullOrWhiteSpace( guid ) ) return;
			if ( BookmarkSaveData.List.Contains( guid ) ) return;

			var list = BookmarkSaveData.List.List;
			var id   = list.Count <= 0 ? 1 : BookmarkSaveData.List.List.Max( x => x.id ) + 1;
			var item = new BookmarkData( id, guid );

			BookmarkSaveData.List.Add( item );
		}

		/// <summary>
		/// TreeView をリロードします
		/// </summary>
		private void ReloadTreeView()
		{
			m_treeView.Reload();

			var index = m_header.sortedColumnIndex;
			m_header.SetSorting( index, !m_header.IsSortedAscending( index ) );
			m_header.SetSorting( index, !m_header.IsSortedAscending( index ) );
		}

		/// <summary>
		/// 検索欄を描画します
		/// </summary>
		private void DrawSearchField()
		{
			using ( var checkScope = new EditorGUI.ChangeCheckScope() )
			{
				if ( m_treeView == null ) return;

				var searchString = m_searchField.OnToolbarGUI( m_treeView.searchString );

				if ( !checkScope.changed ) return;

				SessionState.SetString( SEARCH_STRING_STATE_KEY, searchString );
				m_treeView.searchString = searchString;
			}
		}

		/// <summary>
		/// ツリービューを描画します
		/// </summary>
		private void DrawTreeView()
		{
			var singleLineHeight = EditorGUIUtility.singleLineHeight;

			var rect = new Rect
			{
				x      = 0,
				y      = singleLineHeight + 1,
				width  = position.width,
				height = position.height - singleLineHeight - 1
			};

			m_treeView.OnGUI( rect );
		}

		/// <summary>
		/// アセットを Hierarchy や Project ビューにドラッグする時に呼び出します
		/// </summary>
		private void DragTo()
		{
			if ( Event.current.type != EventType.MouseDrag ) return;

			var selectedPrefabs = m_treeView
					.GetSelection()
					.Select( x => BookmarkSaveData.List.List.FirstOrDefault( y => y.id == x ) )
					.Where( x => x != null )
					.Where( x => x.IsValid )
					.Select( x => x.Asset )
					.ToArray()
				;

			if ( selectedPrefabs.Length <= 0 ) return;

			DragAndDrop.PrepareStartDrag();
			DragAndDrop.objectReferences = selectedPrefabs;
			DragAndDrop.StartDrag( "Dragging" );

			Event.current.Use();
		}

		//================================================================================
		// 関数(static)
		//================================================================================
		/// <summary>
		/// 開きます
		/// </summary>
		[MenuItem( "Window/UniBookmark" )]
		private static void Open()
		{
			GetWindow<BookmarkWindow>( "UniBookmark" );
		}
	}
}