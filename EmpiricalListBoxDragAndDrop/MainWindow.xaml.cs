using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EmpiricalListBoxDragAndDrop
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public class User
		{
			public int Rank { get; set; }
			public string Name { get; set; }
		}

		public ObservableCollection<User> LeftList;
		public ObservableCollection<User> RightList;

		public MainWindow()
		{
			InitializeComponent();

			LeftList = new ObservableCollection<User>
			{
				new User{ Rank=1, Name="佐藤" },
				new User{ Rank=2, Name="鈴木" },
				new User{ Rank=3, Name="高橋" },
				new User{ Rank=4, Name="田中" },
				new User{ Rank=5, Name="伊藤" },
				new User{ Rank=6, Name="渡辺" },
				new User{ Rank=7, Name="山本" },
				new User{ Rank=8, Name="中村" },
				new User{ Rank=9, Name="小林" },
				new User{ Rank=10, Name="加藤" },
			};
			RightList = new ObservableCollection<User>();

			LeftListView.ItemsSource = LeftList;
			RightListView.ItemsSource = RightList;
		}

		private DragDropEffects listView_DoDragDrop(object sender, MouseEventArgs e)
		{
			ListView listView = (ListView)sender;
			foreach (User list in listView.SelectedItems)
			{
				// マウス座標上のアイテムが選択状態かを調べる。
				ListViewItem item = (ListViewItem)listView.ItemContainerGenerator.ContainerFromItem(list);

				// ListViewItemインスタンスが未生成のものもSelectedItemsに含まれる
				// 場合があるので、その場合は処理をスキップする
				if (item == null) continue;

				System.Windows.Point pt = e.GetPosition(item);
				IInputElement target = item.InputHitTest(pt);
				if (target != null && target.IsMouseOver)
				{
					// 選択状態ならドラッグ開始
					return DragDrop.DoDragDrop(listView, listView.SelectedItems, DragDropEffects.Copy);
				}
			}

			return DragDropEffects.None;
		}

		//クリック開始位置
		Point mousePointOrigin;

		private void LeftListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if(listView_DoDragDrop(sender, e) == DragDropEffects.None)
			{
				// ドラッグ＆ドロップが発動しなければ、クリック位置を保存しておく
				mousePointOrigin = e.GetPosition(this);
			}
		}

		private void LeftListView_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				// クリック開始位置よりも一定以上の長さをドラッグしたとき、ドラッグアンドドロップを開始
				if (Point.Subtract(mousePointOrigin, e.GetPosition(this)).Length > 5)
				{
					// マウス左クリックの各イベント([Preview]Mouse[Left]ButtonDown)の完了後に
					// 呼び出されるので、ListView上の項目が選択状態になっている
					listView_DoDragDrop(sender, e);
				}
			}
		}

		private void RightListView_Drop(object sender, DragEventArgs e)
		{
			// EventArgsからListView.SelectedItemCollectionを取得するために使用する文字列。
			const string _selectedItemCollection = "System.Windows.Controls.SelectedItemCollection";

			var items = e.Data.GetData(_selectedItemCollection) as System.Collections.IList;
			if (items == null) return;

			foreach(User u in items)
			{
				RightList.Add(u);
			}
		}
	}
}
