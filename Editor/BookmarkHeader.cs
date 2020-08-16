using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Kogane.Internal
{
	/// <summary>
	/// ブックマークウィンドウのヘッダーを管理するクラス
	/// </summary>
	internal sealed class BookmarkHeader : MultiColumnHeader
	{
		//==============================================================================
		// 関数
		//==============================================================================
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public BookmarkHeader( MultiColumnHeaderState state ) : base( state )
		{
			const int buttonColumnWidth = 48;
			const int removeColumnWidth = 24;

			var columns = new[]
			{
				new MultiColumnHeaderState.Column
				{
					headerContent       = new GUIContent( "Name" ),
					headerTextAlignment = TextAlignment.Center,
				},
				new MultiColumnHeaderState.Column
				{
					width               = buttonColumnWidth,
					minWidth            = buttonColumnWidth,
					maxWidth            = buttonColumnWidth,
					headerContent       = new GUIContent( "" ),
					headerTextAlignment = TextAlignment.Center,
				},
				new MultiColumnHeaderState.Column
				{
					width               = buttonColumnWidth,
					minWidth            = buttonColumnWidth,
					maxWidth            = buttonColumnWidth,
					headerContent       = new GUIContent( "" ),
					headerTextAlignment = TextAlignment.Center,
				},
				new MultiColumnHeaderState.Column
				{
					width               = removeColumnWidth,
					minWidth            = removeColumnWidth,
					maxWidth            = removeColumnWidth,
					headerContent       = new GUIContent( "" ),
					headerTextAlignment = TextAlignment.Center,
				},
			};

			this.state = new MultiColumnHeaderState( columns );
		}
	}
}