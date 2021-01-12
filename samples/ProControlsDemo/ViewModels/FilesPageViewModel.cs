﻿using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ProControlsDemo.Models;

namespace ProControlsDemo.ViewModels
{
    class FilesPageViewModel
    {
        private FileTreeNodeModel _root;
        private Bitmap _folderIcon;
        private Bitmap _fileIcon;

        public FilesPageViewModel()
        {
            var assetLoader = AvaloniaLocator.Current.GetService<IAssetLoader>();
            
            using (var s = assetLoader.Open(new Uri("avares://ProControlsDemo/Assets/file.png")))
                _fileIcon = new Bitmap(s);
            using (var s = assetLoader.Open(new Uri("avares://ProControlsDemo/Assets/folder.png")))
                _folderIcon = new Bitmap(s);

            _root = new FileTreeNodeModel(@"c:\", isDirectory: true, isRoot: true);

            Source = new HierarchicalTreeDataGridSource<FileTreeNodeModel>(_root)
            {
                Columns =
                        {
                            new TemplateColumn<FileTreeNodeModel>(
                                null,
                                new FuncDataTemplate<FileTreeNodeModel>(FileCheckTemplate)),
                            new HierarchicalExpanderColumn<FileTreeNodeModel>(
                                new TemplateColumn<FileTreeNodeModel>(
                                    "Name",
                                    new FuncDataTemplate<FileTreeNodeModel>(FileNameTemplate),
                                    new GridLength(1, GridUnitType.Star),
                                    new ColumnOptions<FileTreeNodeModel>
                                    {
                                        CompareAscending = FileTreeNodeModel.SortAscending(x => x.Name),
                                        CompareDescending = FileTreeNodeModel.SortDescending(x => x.Name),
                                    }),
                                x => x.Children,
                                x => x.IsDirectory),
                            new TextColumn<FileTreeNodeModel, long?>(
                                "Size",
                                x => x.Size,
                                options: new ColumnOptions<FileTreeNodeModel>
                                {
                                    CompareAscending = FileTreeNodeModel.SortAscending(x => x.Size),
                                    CompareDescending = FileTreeNodeModel.SortDescending(x => x.Size),
                                }),
                            new TextColumn<FileTreeNodeModel, DateTimeOffset?>(
                                "Size",
                                x => x.Modified,
                                options: new ColumnOptions<FileTreeNodeModel>
                                {
                                    CompareAscending = FileTreeNodeModel.SortAscending(x => x.Modified),
                                    CompareDescending = FileTreeNodeModel.SortDescending(x => x.Modified),
                                }),
                        }
            };
        }

        public HierarchicalTreeDataGridSource<FileTreeNodeModel> Source { get; }

        private IControl FileCheckTemplate(FileTreeNodeModel node, INameScope ns)
        {
            return new CheckBox
            {
                MinWidth = 0,
                [!CheckBox.IsCheckedProperty] = new Binding(nameof(FileTreeNodeModel.IsChecked)),
            };
        }

        private IControl FileNameTemplate(FileTreeNodeModel node, INameScope ns)
        {
            return new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                Children =
                {
                    new Image
                    {
                        Source = node.IsDirectory ? _folderIcon : _fileIcon,
                        Margin = new Thickness(0, 0, 4, 0),
                        VerticalAlignment = VerticalAlignment.Center,
                    },
                    new TextBlock
                    {
                        [!TextBlock.TextProperty] = new Binding(nameof(FileTreeNodeModel.Name)),
                        VerticalAlignment = VerticalAlignment.Center,
                    }
                }
            };
        }
    }
}